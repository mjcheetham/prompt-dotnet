using System;

namespace Mjcheetham.PromptToolkit
{
    public class SystemConsole : IConsole
    {
        private const char ESC = (char) 27;

        public SystemConsole()
        {
            PlatformUtils.EnsureNotMono();
        }

        public (int left, int top) Cursor
        {
            get => (Console.CursorLeft, Console.CursorTop);
            set
            {
                Console.CursorLeft = value.left;
                Console.CursorTop = value.top;
            }
        }

        public int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public Cursor GetCursor() => new(CursorLeft, CursorTop);

        public void SetCursor(int left, int top) => Console.SetCursorPosition(left, top);

        public void SetCursor(Cursor cursor) => SetCursor(cursor.Left, cursor.Top);

        public void ResetColor() => Console.ResetColor();

        public void Write(object value) => Console.Write(value);

        public void WriteLine(object value) => Console.WriteLine(value);

        public void Write(string message, params object[] args) => Console.Write(message, args);

        public void WriteLine(string message, params object[] args) => Console.WriteLine(message, args);

        public void WriteLine() => Console.WriteLine();

        public string ReadLine() => Console.ReadLine();

        public void MoveCursorUp(int num = 1)
        {
            Console.SetCursorPosition(CursorLeft, Math.Max(0, CursorTop - num));
        }

        public void MoveCursorDown(int num = 1)
        {
            Console.SetCursorPosition(CursorLeft, Math.Min(Console.WindowHeight, CursorTop + num));
        }

        public void MoveCursorLeft(int num = 1)
        {
            Console.SetCursorPosition(Math.Max(0, CursorLeft - num), CursorTop);
        }

        public void MoveCursorRight(int num = 1)
        {
            Console.SetCursorPosition(Math.Min(Console.WindowWidth, CursorLeft + num), CursorTop);
        }

        public void EraseRight(int num = -1)
        {
            var c = GetCursor();

            int len = num < 0 ? Console.WindowWidth - c.Left : Math.Min(num, Console.WindowWidth - c.Left);

            Write(new string(' ', len));

            SetCursor(c);
        }

        public void EraseLeft(int num = -1)
        {
            var c = GetCursor();

            int len = num < 0 ? c.Left : Math.Max(num, c.Left);

            SetCursor(c.Left - len, c.Top);
            Write(new string(' ', len));

            SetCursor(c);
        }

        public void EraseLine()
        {
            var c = GetCursor();

            SetCursor(0, c.Top);
            Write(new string(' ', Console.WindowWidth));

            SetCursor(c);
        }

        public IDisposable SetColor(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
        {
            ConsoleColor? fg = null;
            ConsoleColor? bg = null;

            if (foregroundColor.HasValue)
            {
                fg = Console.ForegroundColor;
                Console.ForegroundColor = foregroundColor.Value;
            }

            if (backgroundColor.HasValue)
            {
                bg = Console.BackgroundColor;
                Console.BackgroundColor = backgroundColor.Value;
            }

            return new SystemConsoleColorCookie(fg, bg);
        }

        public IDisposable SetStyle(ConsoleStyle style)
        {
            if (PlatformUtils.IsVt100Enabled())
            {
                Console.Write("{0}[{1}m", ESC, (int) style);
            }

            return new SystemConsoleStyleCookie();
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Console.ReadKey(intercept);
        }

        public void HideCursor()
        {
            Console.CursorVisible = false;
        }

        public void ShowCursor()
        {
            Console.CursorVisible = true;
        }

        private class SystemConsoleColorCookie : IDisposable
        {
            private readonly ConsoleColor? _foreground;
            private readonly ConsoleColor? _background;

            public SystemConsoleColorCookie(ConsoleColor? foreground, ConsoleColor? background)
            {
                _foreground = foreground;
                _background = background;
            }

            public void Dispose()
            {
                if (_foreground.HasValue)
                {
                    Console.ForegroundColor = _foreground.Value;
                }

                if (_background.HasValue)
                {
                    Console.BackgroundColor = _background.Value;
                }
            }
        }

        private class SystemConsoleStyleCookie : IDisposable
        {
            public void Dispose()
            {
                if (PlatformUtils.IsVt100Enabled())
                {
                    Console.Write("{0}[0m", ESC);
                }
            }
        }
    }

}
