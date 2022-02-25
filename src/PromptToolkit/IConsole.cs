using System;

namespace Mjcheetham.PromptToolkit;

public interface IConsole
{
    void Write(object value);
    void WriteLine(object value);
    void Write(string message, params object[] args);
    void WriteLine(string message, params object[] args);
    void WriteLine();
    string ReadLine();

    void ShowCursor();
    void HideCursor();

    (int X, int Y) GetCursor();

    void SaveCursor();
    void RestoreCursor();

    void NextLine(int num);
    void PreviousLine(int num);

    void MoveCursor(int x, int y);
    void MoveCursorAbsoluteX(int position);
    void MoveCursorUp(int num = 1);
    void MoveCursorDown(int num = 1);
    void MoveCursorRight(int num = 1);
    void MoveCursorLeft(int num = 1);

    void EraseLine(EraseLineMode mode);

    void ScrollUp(int num = 1);
    void ScrollDown(int num = 1);

    (int X, int Y) GetSize();

    IDisposable SetStyle(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, ConsoleStyle? style);

    bool IsVt100Enabled();
}

public enum ConsoleStyle
{
    Normal,
    Bold,
    Underlined,
}

