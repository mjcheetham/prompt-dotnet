using System;
using System.Runtime.InteropServices;

namespace Mjcheetham.PromptToolkit
{
    internal static class PlatformUtils
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out ConsoleModes lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [Flags]
        private enum ConsoleModes : uint
        {
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
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        public static bool IsVt100Enabled()
        {
            if (!IsWindows())
            {
                return true;
            }

            IntPtr consoleHandle = GetConsoleWindow();
            GetConsoleMode(consoleHandle, out ConsoleModes consoleMode);

            return (consoleMode & ConsoleModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0;
        }

        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static void EnsureNotMono()
        {
            if (IsMono()) throw new Exception("Not available on the Mono runtime!");
        }
    }
}
