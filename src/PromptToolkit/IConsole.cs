using System;

namespace Mjcheetham.PromptToolkit
{
    public interface IConsole
    {
        int CursorLeft { get; }
        int CursorTop { get; }
        void ResetColor();
        void Write(object value);
        void WriteLine(object value);
        void Write(string message, params object[] args);
        void WriteLine(string message, params object[] args);
        void WriteLine();
        string ReadLine();
        IConsoleCursor SaveCursor();
        void SetCursor(int left, int top);
        IConsoleSnapshot SetColor(ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null);
        IConsoleSnapshot SetStyle(ConsoleStyle style);
        ConsoleKeyInfo ReadKey(bool intercept);
    }

    public interface IConsoleSnapshot : IDisposable
    {
        void Reset();
    }

    public interface IConsoleCursor
    {
        int Top { get; }
        int Left { get; }
        void Reset(bool clear = false);
    }

    public enum ConsoleStyle
    {
        Normal = 0,
        Bold = 1,
        Dim = 2,
        Italic = 3,
        Underlined = 4,
        Blinking = 5,
        Reverse = 6,
        Invisible = 7,
    }
}
