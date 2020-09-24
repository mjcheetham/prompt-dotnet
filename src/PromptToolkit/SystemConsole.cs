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

        public int CursorLeft => Console.CursorLeft;

        public int CursorTop => Console.CursorTop;

        public void ResetColor() => Console.ResetColor();

        public void Write(object value) => Console.Write(value);

        public void WriteLine(object value) => Console.WriteLine(value);

        public void Write(string message, params object[] args) => Console.Write(message, args);

        public void WriteLine(string message, params object[] args) => Console.WriteLine(message, args);

        public void WriteLine() => Console.WriteLine();

        public string ReadLine() => Console.ReadLine();

        public IConsoleCursor SaveCursor()
        {
            return new SystemConsoleCursorCookie(Console.CursorLeft, Console.CursorTop);
        }

        public void SetCursor(int left, int top)
        {
            Console.SetCursorPosition(left, top);
        }

        public IConsoleSnapshot SetColor(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
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

        public IConsoleSnapshot SetStyle(ConsoleStyle style)
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

        private class SystemConsoleColorCookie : SystemConsoleCookie
        {
            private readonly ConsoleColor? _foreground;
            private readonly ConsoleColor? _background;

            public SystemConsoleColorCookie(ConsoleColor? foreground, ConsoleColor? background)
            {
                _foreground = foreground;
                _background = background;
            }

            public override void Reset()
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

        private abstract class SystemConsoleCookie : IConsoleSnapshot
        {
            public void Dispose() => Reset();

            public abstract void Reset();
        }

        private class SystemConsoleStyleCookie : SystemConsoleCookie
        {
            public override void Reset()
            {
                if (PlatformUtils.IsVt100Enabled())
                {
                    Console.Write("{0}[0m", ESC);
                }
            }
        }

        private class SystemConsoleCursorCookie : IConsoleCursor
        {
            public SystemConsoleCursorCookie(int left, int top)
            {
                Left = left;
                Top = top;
            }

            public int Top { get; }

            public int Left { get; }

            public void Reset(bool clear)
            {
                int currentLeft = Console.CursorLeft;
                int currentTop = Console.CursorTop;

                if (currentLeft == Left && currentTop == Top)
                {
                    return;
                }

                if (clear)
                {
                    // Clear current line
                    Console.SetCursorPosition(0, currentTop);
                    Console.Write(new string(' ', currentLeft));

                    // Clear all full lines between
                    if (currentTop > Top)
                    {
                        var blankLine = new string(' ', Console.WindowWidth);
                        for (int i = currentTop - 1; i > Top; i--)
                        {
                            Console.SetCursorPosition(0, i);
                            Console.Write(blankLine);
                        }
                    }

                    // Clear first line to saved left
                    if (Left < Console.WindowWidth)
                    {
                        Console.SetCursorPosition(Left, Top);
                        Console.Write(new string(' ', Console.WindowWidth - Left));
                    }
                }

                Console.SetCursorPosition(Left, Top);
            }
        }
    }

}
