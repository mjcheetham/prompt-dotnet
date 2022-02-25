using System;

namespace Mjcheetham.PromptToolkit
{
    public static class ConsoleExtensions
    {
        public static IDisposable SetStyle(this IConsole console, ConsoleColor foregroundColor, ConsoleStyle style)
        {
            return console.SetStyle(foregroundColor, null, style);
        }

        public static IDisposable SetStyle(this IConsole console, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            return console.SetStyle(foregroundColor, backgroundColor, null);
        }

        public static IDisposable SetStyle(this IConsole console, ConsoleColor foregroundColor)
        {
            return console.SetStyle(foregroundColor, null, null);
        }

        public static IDisposable SetStyle(this IConsole console, ConsoleStyle style)
        {
            return console.SetStyle(null, null, style);
        }

        public static void WriteLineSuccess(this IConsole console, string message, params object[] args)
        {
            WriteLineSymbol(console, message, console.IsVt100Enabled() ? "✓" : "/", ConsoleColor.Green, args);
        }

        public static void WriteLineAlert(this IConsole console, string message, params object[] args)
        {
            WriteLineSymbol(console, message, "!", ConsoleColor.Yellow, args);
        }

        public static void WriteLineInfo(this IConsole console, string message, params object[] args)
        {
            WriteLineSymbol(console, message, "-", ConsoleColor.White, args);
        }

        public static void WriteLineFailure(this IConsole console, string message, params object[] args)
        {
            WriteLineSymbol(console, message, console.IsVt100Enabled() ? "⨯" : "x", ConsoleColor.Red, args);
        }

        private static void WriteLineSymbol(this IConsole console, string message, string symbol, ConsoleColor color, params object[] args)
        {
            using (console.SetStyle(color))
            {
                console.Write(symbol);
            }

            if (!string.IsNullOrEmpty(message))
            {
                console.Write(" {0}", string.Format(message, args));
            }

            console.WriteLine();
        }
    }
}
