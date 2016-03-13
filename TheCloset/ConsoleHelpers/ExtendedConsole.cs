using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace TheCloset.ConsoleHelpers {

	public static class ExtendedConsole {

		[Flags]
		private enum ConsoleModes : uint {
			ENABLE_PROCESSED_INPUT = 0x0001,
			ENABLE_LINE_INPUT = 0x0002,
			ENABLE_ECHO_INPUT = 0x0004,
			ENABLE_WINDOW_INPUT = 0x0008,
			ENABLE_MOUSE_INPUT = 0x0010,
			ENABLE_INSERT_MODE = 0x0020,
			ENABLE_QUICK_EDIT_MODE = 0x0040,
			ENABLE_EXTENDED_FLAGS = 0x0080,
			ENABLE_AUTO_POSITION = 0x0100,
			ENABLE_PROCESSED_OUTPUT = 0x0001,
			ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
			all = ENABLE_PROCESSED_INPUT|ENABLE_LINE_INPUT|ENABLE_ECHO_INPUT|ENABLE_WINDOW_INPUT|ENABLE_MOUSE_INPUT|ENABLE_INSERT_MODE|ENABLE_QUICK_EDIT_MODE|ENABLE_EXTENDED_FLAGS|ENABLE_AUTO_POSITION
		}


		#region Methods

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern SafeFileHandle CreateFile(
		string fileName,
		[MarshalAs(UnmanagedType.U4)] uint fileAccess,
		[MarshalAs(UnmanagedType.U4)] uint fileShare,
		IntPtr securityAttributes,
		[MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
		[MarshalAs(UnmanagedType.U4)] int flags,
		IntPtr template);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteConsoleOutput(
		  SafeFileHandle hConsoleOutput,
		  CharInfo[] lpBuffer,
		  COORD dwBufferSize,
		  COORD dwBufferCoord,
		  ref SmallRect lpWriteRegion);

		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
		[DllImport("user32.dll", SetLastError = true)]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetConsoleWindow();
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleMode(SafeFileHandle hConsoleHandle, out uint lpMode);
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleMode(SafeFileHandle hConsoleHandle, uint dwMode);

		[DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
		static extern bool ReadConsoleInput(
		 SafeFileHandle hConsoleInput,
		 [Out] INPUT_RECORD[] lpBuffer,
		 int nLength,
		 out uint lpNumberOfEventsRead);
		#endregion Methods


		#region Structs

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct KEY_EVENT_RECORD {
			[FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
			public bool bKeyDown;
			[FieldOffset(4), MarshalAs(UnmanagedType.U2)]
			public ushort wRepeatCount;
			[FieldOffset(6), MarshalAs(UnmanagedType.U2)]
			public ushort wVirtualKeyCode;
			[FieldOffset(8), MarshalAs(UnmanagedType.U2)]
			public ushort wVirtualScanCode;
			[FieldOffset(10)]
			public char UnicodeChar;
			[FieldOffset(12), MarshalAs(UnmanagedType.U4)]
			public uint dwControlKeyState;
		}
		[StructLayout(LayoutKind.Explicit)]
		public struct MOUSE_EVENT_RECORD {
			[FieldOffset(0)]
			public COORD dwMousePosition;
			[FieldOffset(4)]
			public uint dwButtonState;
			[FieldOffset(8)]
			public uint dwControlKeyState;
			[FieldOffset(12)]
			public uint dwEventFlags;
		}
		public struct WINDOW_BUFFER_SIZE_RECORD {
			public COORD dwSize;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct MENU_EVENT_RECORD {
			public uint dwCommandId;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct FOCUS_EVENT_RECORD {
			public uint bSetFocus;
		}
		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT_RECORD {
			[FieldOffset(0)]
			public ushort EventType;
			[FieldOffset(4)]
			public KEY_EVENT_RECORD KeyEvent;
			[FieldOffset(4)]
			public MOUSE_EVENT_RECORD MouseEvent;
			[FieldOffset(4)]
			public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
			[FieldOffset(4)]
			public MENU_EVENT_RECORD MenuEvent;
			[FieldOffset(4)]
			public FOCUS_EVENT_RECORD FocusEvent;
		};

		[StructLayout(LayoutKind.Explicit)]
		public struct CharInfo {
			[FieldOffset(0)]
			public CharUnion Char;
			[FieldOffset(2)]
			public short Attributes;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct CharUnion {
			[FieldOffset(0)]
			public char UnicodeChar;
			[FieldOffset(0)]
			public byte AsciiChar;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct COORD {
			public short X;
			public short Y;

			public COORD(short X, short Y) {
				this.X = X;
				this.Y = Y;
			}

			public static COORD Zero = new COORD(0,0);
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct SmallRect {
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}

		#endregion Structs

		#region Fields

		static SafeFileHandle _consoleOutputHandle;
		static SafeFileHandle _consoleInputHandle;
		static IntPtr _windowHandle;

		#endregion Fields

		#region Methods

		public static void FixConsoleSize() {
			SetWindowLong(_windowHandle, -16, GetWindowLong(_windowHandle, -16) ^ 0x00050000);
		}

		public static void ClearConsoleLine(int line) {
			Console.SetCursorPosition(0, line);
			Console.Write(new string(' ', Console.WindowWidth - 1));
		}

		public static void ClearCurrentConsoleLine() {
			ClearConsoleLine(Console.CursorTop);
		}

		public static void Init(bool mouseMode = true) {
			_windowHandle = GetConsoleWindow();
			_consoleOutputHandle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
			//if (_consoleOutputHandle)
			//	throw new Win32Exception(Marshal.GetLastWin32Error());
			_consoleInputHandle = CreateFile("CONIN$", 0x80000000 | 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
			//if(_consoleInputHandle.IsInvalid)
			//	throw new Win32Exception(Marshal.GetLastWin32Error());
			uint mode = 0;
			if (!GetConsoleMode(_consoleInputHandle, out mode))
				throw new Win32Exception(Marshal.GetLastWin32Error());
			mode |= (uint)ConsoleModes.ENABLE_MOUSE_INPUT;
			mode &= ~(uint)ConsoleModes.ENABLE_QUICK_EDIT_MODE;
			mode |= (uint)ConsoleModes.ENABLE_EXTENDED_FLAGS;
			if (!SetConsoleMode(_consoleInputHandle, mode))
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		public static bool UpdateRegion(CharInfo[] buffer, COORD point, COORD size, ref SmallRect smallRect) {
			return WriteConsoleOutput(_consoleOutputHandle, buffer, size, point, ref smallRect);
		}
		
		public static void GetConsoleInput(ref INPUT_RECORD[] buf) {
			uint r;
			if(!ReadConsoleInput(_consoleInputHandle, buf, buf.Length, out r))
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		#endregion Methods
	}
}