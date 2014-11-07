using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TukuiAddOnManagerEnhanced
{
	public enum DefaultMonitor
	{
		None = 0,
		Primary = 1,
		Nearest = 2,
	}

	static class NativeMethods
	{
		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4 )]
		public class MONITORINFOEX
		{
			public int     cbSize = Marshal.SizeOf( typeof( MONITORINFOEX ) );
			public RECT    rcMonitor = new RECT( );
			public RECT    rcWork = new RECT( );
			public int     dwFlags = 0;

			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
			public char[]  szDevice = new char[ 32 ];
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		[DllImport( "User32.dll", CharSet = CharSet.Auto )]
		public static extern bool GetMonitorInfo( HandleRef hmonitor, [In, Out]MONITORINFOEX info );

		[DllImport( "user32.dll" )]
		public static extern IntPtr MonitorFromWindow( IntPtr hwnd, uint dwFlags );

		public static WPFMonitor CurrentMonitor( this Window Window, DefaultMonitor flags = DefaultMonitor.Primary )
		{
			WindowInteropHelper handleHelper = new WindowInteropHelper( Window );
			IntPtr monitorPtr = MonitorFromWindow( handleHelper.Handle, (uint) flags );
			return new WPFMonitor( Window, monitorPtr );
		}

		[DllImport( "user32.dll", SetLastError = true )]
		internal static extern bool MoveWindow( IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint );

		public static void ForceWindowMove( this Window Window, int X, int Y, double Width = Double.NaN, double Height = Double.NaN, bool Repaint = true )
		{
			if ( Width == Double.NaN ) Width = Window.Width;
			if ( Height == Double.NaN ) Height = Window.Height;
			WindowInteropHelper handleHelper = new WindowInteropHelper( Window );
			bool success = MoveWindow( handleHelper.Handle, X, Y, (int) Math.Round( Width ), (int) Math.Round( Height ), Repaint );
		}

		public static void ForceWindowMove( this Window Window, Rect rect, bool Repaint = true )
		{
			ForceWindowMove( Window, (int) rect.X, (int) rect.Y, rect.Width, rect.Height, Repaint );
		}
	}

	public struct MonitorInfo
	{
		public Rect MonitorArea;
		public Rect WorkingArea;

		internal MonitorInfo( NativeMethods.MONITORINFOEX info )
		{
			MonitorArea = new Rect( info.rcMonitor.left, info.rcMonitor.top, info.rcMonitor.right - info.rcMonitor.left, info.rcMonitor.bottom - info.rcMonitor.top );
			WorkingArea = new Rect( info.rcWork.left, info.rcWork.top, info.rcWork.right - info.rcWork.left, info.rcWork.bottom - info.rcWork.top );
		}
	}

	public class WPFMonitor
	{
		internal Window window;
		internal IntPtr handle;

		internal WPFMonitor( Window window, IntPtr handle )
		{
			this.window = window;
			this.handle = handle;
		}

		public MonitorInfo GetMonitorInfo( )
		{
			NativeMethods.MONITORINFOEX info = new NativeMethods.MONITORINFOEX( );
			HandleRef refHandle = new HandleRef( window, handle );

			bool test = NativeMethods.GetMonitorInfo( refHandle, info );
			return new MonitorInfo( info );
		}
	}
}
