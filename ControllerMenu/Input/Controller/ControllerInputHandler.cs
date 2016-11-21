using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerMenu.Input.Joystick
{
	public class ControllerInputHandler : IInputHandler
	{
	    [DllImport("xinput1_4.dll")]
	    private static extern void XInputEnable(bool enable);

	    [DllImport("xinput1_4.dll")]
	    private static extern int XInputGetState(int deviceIndex, ref XInputState state);

	    [DllImport("xinput1_4.dll")]
	    private static extern int XInputSetState(int deviceIndex, ref XInputVibration vibration);

	    [DllImport("xinput1_4.dll")]
	    private static extern int XInputGetCapabilities(int deviceIndex, int flags, ref XInputCapabilities capabilities);

	    [DllImport("xinput1_4.dll")]
	    private static extern int XInputGetBatteryInformation(int deviceIndex, byte deviceType, ref XInputBatteryInformation batteryInformation);

	    private CancellationTokenSource controllerWatchCancellationSource;

	    public event InputEventHandler InputDetected;

		public void Listen(Control parent)
		{
		    //todo add check to prevent multiple calls to this

		    this.controllerWatchCancellationSource = new CancellationTokenSource();
		    var token = this.controllerWatchCancellationSource.Token;
		    Task.Factory.StartNew(() => this.WatchControllerState(token), token);
		}

	    public void Dispose()
	    {
	        this.controllerWatchCancellationSource.Cancel();
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

	                // ReSharper disable once PossibleNullReferenceException - loop is skipped if there are no subscribers
	                this.InputDetected(this, inputType.Value);
	            }

	            if (cancellationToken.IsCancellationRequested)
	            {
	                break;
	            }

	            Thread.Sleep(100);
	        }
	    }

	    private static InputType? GetInputTypeFromStateChanges(XInputGamepad previousState, XInputGamepad currentState)
	    {
	        if (!previousState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_DPAD_UP) && currentState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_DPAD_UP))
	        {
	            return InputType.PreviousItem;
	        }

	        if (previousState.sThumbLY < 8000 && currentState.sThumbLY >= 8000)
	        {
	            return InputType.PreviousItem;
	        }

	        if (!previousState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_DPAD_DOWN) && currentState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_DPAD_DOWN))
	        {
	            return InputType.NextItem;
	        }

	        if (previousState.sThumbLY > -8000 && currentState.sThumbLY <= -8000)
	        {
	            return InputType.NextItem;
	        }

	        if (!previousState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_A) && currentState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_A))
	        {
	            return InputType.SelectItem;
	        }

	        if (!previousState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_B) && currentState.IsButtonPressed((int)GamepadButton.XINPUT_GAMEPAD_B))
	        {
	            return InputType.Back;
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
}