using System;

namespace Mjcheetham.PromptToolkit
{
    public static class ConsoleExtensions
    {
        public static void WriteLineSuccess(this IConsole console, string message, params object[] args)
        {
            WriteLineSymbol(console, message, "✓", ConsoleColor.Green, args);
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
            WriteLineSymbol(console, message, "⨯", ConsoleColor.Red, args);
        }

        private static void WriteLineSymbol(this IConsole console, string message, string symbol, ConsoleColor color, params object[] args)
        {
            using (console.SetColor(color))
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
