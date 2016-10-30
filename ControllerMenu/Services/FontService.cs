using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using ControllerMenu.Properties;

namespace ControllerMenu.Services
{
	public class Fonts
	{
		public static string Menu = "Open Sans";
	}

	public class FontService
	{
		[DllImport("gdi32.dll")]
		private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

		private readonly PrivateFontCollection fonts = new PrivateFontCollection();

		public FontService()
		{
			var fontData = Resources.OpenSans_Regular;
			var fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
			Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
			uint dummy = 0;
			this.fonts.AddMemoryFont(fontPtr, Resources.OpenSans_Regular.Length);
			AddFontMemResourceEx(fontPtr, (uint) Resources.OpenSans_Regular.Length, IntPtr.Zero, ref dummy);
			Marshal.FreeCoTaskMem(fontPtr);
		}

		public Font GetFontByName(string name, float size)
		{
			var fontFamily = this.fonts.Families.First(x => String.Equals(name, x.Name));
			return new Font(fontFamily, size);
		}
	}
}
