using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerMenu.Input.Models;

namespace ControllerMenu.Input.Controller
{
	public class ControllerInputHandler : IInputHandler
	{
	    [DllImport("xinput1_4.dll")]
	    private static extern void XInputEnable(bool enable);

	    [DllImport("xinput1_4.dll")]
	    private static extern int XInputGetState(int deviceIndex, ref XInputState state);

	    [DllImport("xinput1_4.dll")]
	    private static extern int XInputSetState(int deviceIndex, ref XInputVibration vibration);
//
//	    [DllImport("xinput1_4.dll")]
//	    private static extern int XInputGetKeystroke(int deviceIndex, int reserved, ref XInputKeystroke keystroke);

        [DllImport("xinput1_4.dll")]
	    private static extern int XInputGetCapabilities(int deviceIndex, int flags, ref XInputCapabilities capabilities);

	    [DllImport("xinput1_4.dll")]
	    private static extern int XInputGetBatteryInformation(int deviceIndex, byte deviceType, ref XInputBatteryInformation batteryInformation);

	    [DllImport("xinput1_4.dll")]
	    private static extern int XInputPowerOffController(int deviceIndex);

	    private static readonly Dictionary<InputType, Func<XInputGamepad, XInputGamepad, InputType?>> GamepadStateChangeDetectorMap = new Dictionary<InputType, Func<XInputGamepad, XInputGamepad, InputType?>>
	    {
	        { InputType.Menu, CheckForMenuInput },
	        { InputType.PreviousItem, CheckForPreviousItemInput },
	        { InputType.NextItem, CheckForNextItemInput },
	        { InputType.SelectItem, CheckForSelectItemInput },
	        { InputType.Back, CheckForBackInput },
	    };

	    private readonly List<InputType> activeInputTypes;
	    private CancellationTokenSource cancellationSource;

	    public ControllerInputHandler()
	    {
	        this.activeInputTypes = new List<InputType>();
	    }

	    public event InputEventHandler InputDetected;

	    public IList<InputType> ActiveInputs
	    {
	        set
	        {
	            this.activeInputTypes.Clear();
	            this.activeInputTypes.AddRange(value);
	        }
	    }

		public void Listen(Control parent, IList<InputType> initialInputTypes)
		{
		    this.ActiveInputs = initialInputTypes;

		    if (this.cancellationSource != null && !this.cancellationSource.IsCancellationRequested)
		    {
		        //we're already listening
		        return;
		    }

		    this.cancellationSource = new CancellationTokenSource();
		    var token = this.cancellationSource.Token;
		    Task.Factory.StartNew(() => this.WatchControllerState(token), token);
		}

	    public void Dispose()
	    {
	        this.cancellationSource.Cancel();
	    }

	    private void WatchControllerState(CancellationToken cancellationToken)
	    {
	        const int deviceCount = 4;
	        var currentDeviceStates = new XInputState[deviceCount];

	        while (!cancellationToken.IsCancellationRequested)
	        {
	            var isThereAnybodyOutThere = this.InputDetected != null;

	            for (var deviceIndex = 0; isThereAnybodyOutThere && deviceIndex < deviceCount; deviceIndex++)
	            {
	                var currentState = new XInputState();
	                var result = XInputGetState(deviceIndex, ref currentState);
	                if (result != 0)
	                {
	                    //controller is not connected
	                    continue;
	                }

	                var previousDeviceState = currentDeviceStates[deviceIndex];
                    if (previousDeviceState.PacketNumber == currentState.PacketNumber)
                    {
                        //nothing has changed
                        continue;
                    }

	                var inputType = GetInputTypeFromStateChanges(previousDeviceState.Gamepad, currentState.Gamepad);

	                currentDeviceStates[deviceIndex] = currentState;

	                if (!inputType.HasValue)
	                {
	                    continue;
	                }

	                //this.PulseVibration(deviceIndex, 200);
	                this.InputDetected(this, inputType.Value);
	            }

	            if (cancellationToken.IsCancellationRequested)
	            {
	                break;
	            }

	            Thread.Sleep(100);
	        }
	    }

	    private InputType? GetInputTypeFromStateChanges(XInputGamepad previousState, XInputGamepad currentState)
	    {
	        foreach (var activeInputType in this.activeInputTypes)
	        {
	            var stateChangeDetector = GamepadStateChangeDetectorMap[activeInputType];
	            var inputType = stateChangeDetector(previousState, currentState);

	            if (inputType.HasValue)
	            {
	                return inputType;
	            }
	        }

	        return null;
	    }

	    private void PulseVibration(int deviceIndex, int vibrationLength)
	    {
	        var vibration = new XInputVibration { LeftMotorSpeed = 65535 };
	        XInputSetState(deviceIndex, ref vibration);

	        Thread.Sleep(vibrationLength);

	        vibration.LeftMotorSpeed = 0;
	        XInputSetState(deviceIndex, ref vibration);
	    }

	    private static InputType? CheckForMenuInput(XInputGamepad previousState, XInputGamepad currentState)
	    {
	        const int buttonCombo =
	            (short) GamepadButton.XINPUT_GAMEPAD_LEFT_SHOULDER |
	            (short) GamepadButton.XINPUT_GAMEPAD_RIGHT_SHOULDER |
	            (short) GamepadButton.XINPUT_GAMEPAD_START;

	        var comboWasPressed = previousState.wButtons == buttonCombo;
	        var comboIsPressed = currentState.wButtons == buttonCombo;

	        return !comboWasPressed && comboIsPressed ? InputType.Menu : null as InputType?;
	    }

	    private static InputType? CheckForPreviousItemInput(XInputGamepad previousState, XInputGamepad currentState)
	    {
	        if (!previousState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_DPAD_UP) && currentState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_DPAD_UP))
	        {
	            return InputType.PreviousItem;
	        }

	        if (previousState.sThumbLY < 8000 && currentState.sThumbLY >= 8000)
	        {
	            return InputType.PreviousItem;
	        }

	        return null;
	    }

	    private static InputType? CheckForNextItemInput(XInputGamepad previousState, XInputGamepad currentState)
	    {
	        if (!previousState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_DPAD_DOWN) && currentState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_DPAD_DOWN))
	        {
	            return InputType.NextItem;
	        }

	        if (previousState.sThumbLY > -8000 && currentState.sThumbLY <= -8000)
	        {
	            return InputType.NextItem;
	        }

	        return null;
	    }

	    private static InputType? CheckForSelectItemInput(XInputGamepad previousState, XInputGamepad currentState)
	    {
	        if (!previousState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_A) && currentState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_A))
	        {
	            return InputType.SelectItem;
	        }

	        return null;
	    }

	    private static InputType? CheckForBackInput(XInputGamepad previousState, XInputGamepad currentState)
	    {
	        if (!previousState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_B) && currentState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_B))
	        {
	            return InputType.Back;
	        }

	        return null;
	    }
	}

    public enum GamepadButton
    {
        XINPUT_GAMEPAD_DPAD_UP = 0x0001,
        XINPUT_GAMEPAD_DPAD_DOWN = 0x0002,
        XINPUT_GAMEPAD_DPAD_LEFT = 0x0004,
        XINPUT_GAMEPAD_DPAD_RIGHT = 0x0008,
        XINPUT_GAMEPAD_START = 0x0010,
        XINPUT_GAMEPAD_BACK = 0x0020,
        XINPUT_GAMEPAD_LEFT_THUMB = 0x0040,
        XINPUT_GAMEPAD_RIGHT_THUMB = 0x0080,
        XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100,
        XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200,
        XINPUT_GAMEPAD_A = 0x1000,
        XINPUT_GAMEPAD_B = 0x2000,
        XINPUT_GAMEPAD_X = 0x4000,
        XINPUT_GAMEPAD_Y = 0x8000
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct  XInputGamepad
    {
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(0)]
        public short wButtons;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(2)]
        public byte bLeftTrigger;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(3)]
        public byte bRightTrigger;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(4)]
        public short sThumbLX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(6)]
        public short sThumbLY;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(8)]
        public short sThumbRX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(10)]
        public short sThumbRY;


        public bool IsButtonPressed(int buttonFlags)
        {
            return (wButtons & buttonFlags) == buttonFlags;
        }

        public bool IsButtonPresent(int buttonFlags)
        {
            return (wButtons & buttonFlags) == buttonFlags;
        }

        public void Copy(XInputGamepad source)
        {
            sThumbLX = source.sThumbLX;
            sThumbLY = source.sThumbLY;
            sThumbRX = source.sThumbRX;
            sThumbRY = source.sThumbRY;
            bLeftTrigger = source.bLeftTrigger;
            bRightTrigger = source.bRightTrigger;
            wButtons = source.wButtons;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is XInputGamepad))
                return false;
            XInputGamepad source = (XInputGamepad)obj;
            return ((sThumbLX == source.sThumbLX)
                    && (sThumbLY == source.sThumbLY)
                    && (sThumbRX == source.sThumbRX)
                    && (sThumbRY == source.sThumbRY)
                    && (bLeftTrigger == source.bLeftTrigger)
                    && (bRightTrigger == source.bRightTrigger)
                    && (wButtons == source.wButtons));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct  XInputVibration
    {
        [MarshalAs(UnmanagedType.I2)]
        public ushort LeftMotorSpeed;

        [MarshalAs(UnmanagedType.I2)]
        public ushort RightMotorSpeed;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct  XInputState
    {
        [FieldOffset(0)]
        public int PacketNumber;

        [FieldOffset(4)]
        public XInputGamepad Gamepad;

        public void Copy(XInputState source)
        {
            PacketNumber = source.PacketNumber;
            Gamepad.Copy(source.Gamepad);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (!(obj is XInputState)))
                return false;
            XInputState source = (XInputState)obj;

            return ((PacketNumber == source.PacketNumber)
                    && (Gamepad.Equals(source.Gamepad)));
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct  XInputCapabilities
    {
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        byte Type;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        public byte SubType;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(2)]
        public short Flags;


        [FieldOffset(4)]
        public XInputGamepad Gamepad;

        [FieldOffset(16)]
        public XInputVibration Vibration;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct  XInputBatteryInformation
    {
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        public byte BatteryType;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        public byte BatteryLevel;
    }

//    [StructLayout(LayoutKind.Explicit)]
//    public struct XInputKeystroke {
//        public short VirtualKey;
//        public char Unicode;
//        public short Flags;
//        public byte UserIndex;
//        public byte HidCode;
//    }
}