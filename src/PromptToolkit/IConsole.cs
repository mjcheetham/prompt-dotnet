using System;

namespace Mjcheetham.PromptToolkit
{
    public interface IConsole
    {
        int CursorLeft { get; set; }
        int CursorTop { get; set; }
        Cursor GetCursor();
        void SetCursor(Cursor cursor);
        void SetCursor(int left, int top);
        void ResetColor();
        void Write(object value);
        void WriteLine(object value);
        void Write(string message, params object[] args);
        void WriteLine(string message, params object[] args);
        void WriteLine();
        string ReadLine();
        void MoveCursorUp(int num);
        void MoveCursorDown(int num);
        void MoveCursorLeft(int num);
        void MoveCursorRight(int num);
        void EraseRight(int num);
        void EraseLeft(int num);
        void EraseLine();
        IDisposable SetColor(ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null);
        IDisposable SetStyle(ConsoleStyle style);
        ConsoleKeyInfo ReadKey(bool intercept);
        void EraseTo(Cursor cursor);
    }

    public readonly struct Cursor
    {
        public int Left { get; }
        public int Top { get; }

        public Cursor(int left, int top)
        {
            Left = left;
            Top = top;
        }
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
