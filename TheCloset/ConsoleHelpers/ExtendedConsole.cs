using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace TheCloset.ConsoleHelpers {

	public static class ExtendedConsole {
		#region Fields

		private static readonly SafeFileHandle ConsoleInputHandle;

		private static readonly SafeFileHandle ConsoleOutputHandle;

		private static readonly IntPtr WindowHandle;

		#endregion Fields

		#region Constructors

		static ExtendedConsole() {
			WindowHandle = GetConsoleWindow();

			if (WindowHandle == IntPtr.Zero)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			ConsoleOutputHandle = CreateFile("CONOUT$", 0x80000000 | 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
			if (ConsoleOutputHandle.IsInvalid)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			ConsoleInputHandle = CreateFile("CONIN$", 0x80000000 | 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
			if (ConsoleInputHandle.IsInvalid)
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		#endregion Constructors

		#region Methods

		public static void ClearConsoleArea(short x, short y, short w, short h) {
			var r = Enumerable.Repeat(new CharInfo {
				Char = new CharUnion { UnicodeChar = ' ' },
				Attributes = (short)((short)Console.ForegroundColor | (short)((short)Console.BackgroundColor << 8))
			}, w * h).ToArray();
			var rect = new SmallRect {
				Top = y,
				Left = x,
				Bottom = (short)(y + (h - 1)),
				Right = (short)(x + (w - 1))
			};
			var size = new Coord(w, h);
			var pos = Coord.Zero;
			if (!UpdateRegion(r, pos, size, ref rect))
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		public static void ClearConsoleLine(short line) {
			var r = Enumerable.Repeat(new CharInfo {
				Char = new CharUnion { UnicodeChar = ' ' },
				Attributes = (short)((short)Console.ForegroundColor | (short)((short)Console.BackgroundColor << 8))
			}, Console.BufferWidth).ToArray();
			var rect = new SmallRect { Top = line, Bottom = line, Left = 0, Right = (short)(r.Length - 1) };
			var size = new Coord((short)r.Length, 1);
			var pos = Coord.Zero;
			if (!UpdateRegion(r, pos, size, ref rect))
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		public static void ClearCurrentConsoleLine() =>
			ClearConsoleLine((short)Console.CursorTop);

		public static void EnableMouseInput() {
			uint mode;
			if (!GetConsoleMode(ConsoleInputHandle, out mode))
				throw new Win32Exception(Marshal.GetLastWin32Error());
			mode |= (uint)ConsoleModes.ENABLE_MOUSE_INPUT;
			mode &= ~(uint)ConsoleModes.ENABLE_QUICK_EDIT_MODE;
			mode |= (uint)ConsoleModes.ENABLE_EXTENDED_FLAGS;

			if (!SetConsoleMode(ConsoleInputHandle, mode))
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		public static void FixConsoleSize() =>
			SetWindowLong(WindowHandle, -16, GetWindowLong(WindowHandle, -16) ^ 0x00050000);

		public static void GetConsoleInput(ref InputRecord[] buf) {
			uint r;
			if (!ReadConsoleInput(ConsoleInputHandle, buf, buf.Length, out r))
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		public static bool UpdateRegion(CharInfo[] buffer, Coord point, Coord size, ref SmallRect smallRect) =>
			WriteConsoleOutput(ConsoleOutputHandle, buffer, size, point, ref smallRect);

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern SafeFileHandle CreateFile(
			string fileName,
			[MarshalAs(UnmanagedType.U4)] uint fileAccess,
			[MarshalAs(UnmanagedType.U4)] uint fileShare,
			IntPtr securityAttributes,
			[MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
			[MarshalAs(UnmanagedType.U4)] int flags,
			IntPtr template);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetConsoleMode(SafeFileHandle hConsoleHandle, out uint lpMode);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
		private static extern bool ReadConsoleInput(
			SafeFileHandle hConsoleInput,
			[Out] InputRecord[] lpBuffer,
			int nLength,
			out uint lpNumberOfEventsRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetConsoleMode(SafeFileHandle hConsoleHandle, uint dwMode);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteConsoleOutput(
			SafeFileHandle hConsoleOutput,
			CharInfo[] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
			ref SmallRect lpWriteRegion);

		#endregion Methods

		#region Enums

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
			all = ENABLE_PROCESSED_INPUT | ENABLE_LINE_INPUT | ENABLE_ECHO_INPUT | ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT | ENABLE_INSERT_MODE | ENABLE_QUICK_EDIT_MODE | ENABLE_EXTENDED_FLAGS | ENABLE_AUTO_POSITION
		}

		#endregion Enums

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
		public struct Coord {
			public short X;
			public short Y;

			public Coord(short x, short y) {
				X = x;
				Y = y;
			}

			public static Coord Zero = new Coord(0, 0);
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct FocusEventRecord {
			public uint bSetFocus;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct InputRecord {

			[FieldOffset(0)]
			public ushort EventType;

			[FieldOffset(4)]
			public KeyEventRecord KeyEvent;

			[FieldOffset(4)]
			public MouseEventRecord MouseEvent;

			[FieldOffset(4)]
			public WindowBufferSizeRecord WindowBufferSizeEvent;

			[FieldOffset(4)]
			public MenuEventRecord MenuEvent;

			[FieldOffset(4)]
			public FocusEventRecord FocusEvent;
		};

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct KeyEventRecord {

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

		[StructLayout(LayoutKind.Sequential)]
		public struct MenuEventRecord {
			public uint dwCommandId;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct MouseEventRecord {

			[FieldOffset(0)]
			public Coord dwMousePosition;

			[FieldOffset(4)]
			public uint dwButtonState;

			[FieldOffset(8)]
			public uint dwControlKeyState;

			[FieldOffset(12)]
			public uint dwEventFlags;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SmallRect {
			#region Fields

			public short Left;
			public short Top;

			#endregion Fields

			public short Right;
			public short Bottom;
		}

		public struct WindowBufferSizeRecord {
			public Coord dwSize;
		}
	}
}