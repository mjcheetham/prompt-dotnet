using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mjcheetham.PromptToolkit;

public class AnsiConsole : IConsole
{
    private const string ESC = "\x1b";
    private const string CSI = ESC + "[";

    private static readonly Regex DsrRegex = new Regex(@"\x1b\[(?'row'\d+);(?'col'\d+)R$", RegexOptions.Compiled);

    private readonly TextReader _stdin;
    private readonly TextWriter _stdout;

    private static readonly Lazy<bool> _isAppleTerminal = new(
        () => StringComparer.OrdinalIgnoreCase.Equals(Environment.GetEnvironmentVariable("TERM_PROGRAM"), "Apple_Terminal")
    );

    private static bool IsAppleTerminal()
    {
        return _isAppleTerminal.Value;
    }

    public AnsiConsole(TextReader stdin, TextWriter stdout)
    {
        _stdin = stdin;
        _stdout = stdout;
    }

    public void Write(object value)
    {
        _stdout.Write(value);
    }

    public void WriteLine(object value)
    {
        _stdout.WriteLine(value);
    }

    public void Write(string message, params object[] args)
    {
        _stdout.Write(message, args);
    }

    public void WriteLine(string message, params object[] args)
    {
        _stdout.WriteLine(message, args);
    }

    public void WriteLine()
    {
        _stdout.WriteLine();
    }

    public string ReadLine()
    {
        return _stdin.ReadLine();
    }

    public ConsoleKeyInfo ReadKey()
    {
        // TODO: implement in ANSI escape codes???
        return Console.ReadKey(true);
    }

    public void ShowCursor()
    {
        _stdout.Write(CSI + "?25h");
    }

    public void HideCursor()
    {
        _stdout.Write(CSI + "?25l");
    }

    public void ScrollUp(int num = 1)
    {
        _stdout.Write(CSI + $"{num}S");
    }

    public void ScrollDown(int num = 1)
    {
        _stdout.Write(CSI + $"{num}T");
    }

    public (int X, int Y) GetCursor()
    {
        // Write Device Status Report (DSR) escape sequence
        _stdout.Write(CSI + "6n");

        // Result is "ESC[n;mR" where n = row and m = column
        string sequence = _stdin.ReadToFirst('R');

        Match match = DsrRegex.Match(sequence);
        if (match.Success)
        {
            int col = int.Parse(match.Groups["col"].Value);
            int row = int.Parse(match.Groups["row"].Value);

            return (col, row);
        }

        throw new ConsoleException("Failed to read DSR response sequence");
    }

    public void SaveCursor()
    {
        string ctrl = IsAppleTerminal() ? ESC + '7' : CSI + 's';
        _stdout.Write(ctrl);
    }

    public void RestoreCursor()
    {
        string ctrl = IsAppleTerminal() ? ESC + '8' : CSI + 'u';
        _stdout.Write(ctrl);
    }

    public void NextLine(int num)
    {
        MoveCursorDown(num);
        MoveCursorAbsoluteX(0);
    }

    public void PreviousLine(int num)
    {
        MoveCursorUp(num);
        MoveCursorAbsoluteX(0);
    }

    public void MoveCursor(int x, int y)
    {
        _stdout.Write(CSI + $"{x};{y}f");
    }

    public void MoveCursorAbsoluteX(int position)
    {
        _stdout.Write(CSI + $"{position}G");
    }

    public void MoveCursorUp(int num = 1)
    {
        _stdout.Write(CSI + $"{num}A");
    }

    public void MoveCursorDown(int num = 1)
    {
        _stdout.Write(CSI + $"{num}B");
    }

    public void MoveCursorRight(int num = 1)
    {
        _stdout.Write(CSI + $"{num}C");
    }

    public void MoveCursorLeft(int num = 1)
    {
        _stdout.Write(CSI + $"{num}D");
    }

    public void EraseLine(EraseLineMode mode)
    {
        int i = (int)mode;
        _stdout.Write(CSI + $"{i}K");
    }

    public (int X, int Y) GetSize()
    {
        HideCursor();
        SaveCursor();

        try
        {
            MoveCursor(999, 999);

            return GetCursor();
        }
        finally
        {
            ShowCursor();
            RestoreCursor();
        }
    }

    public IDisposable SetStyle(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, ConsoleStyle? style)
    {
        var ctrl = new StringBuilder(CSI);

        if (style.HasValue)
        {
            ctrl.AppendFormat("{0};", GetAnsiCode(style.Value));
        }

        if (foregroundColor.HasValue)
        {
            ctrl.AppendFormat("{0};", GetAnsiCode(foregroundColor.Value));
        }

        if (backgroundColor.HasValue)
        {
            ctrl.AppendFormat("{0};", GetAnsiCode(backgroundColor.Value) + 10);
        }

        if (ctrl[ctrl.Length - 1] == ';')
        {
            ctrl.Remove(ctrl.Length - 1, 1);
        }

        ctrl.Append('m');

        _stdout.Write(ctrl.ToString());

        return new SgrResetDisposable(this);
    }

    private static int GetAnsiCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black   => 30,
            ConsoleColor.Red     => 31,
            ConsoleColor.Green   => 32,
            ConsoleColor.Yellow  => 33,
            ConsoleColor.Blue    => 34,
            ConsoleColor.Magenta => 35,
            ConsoleColor.Cyan    => 36,
            ConsoleColor.White   => 37,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, "Unsupported console color")
        };
    }

    private static int GetAnsiCode(ConsoleStyle style)
    {
        return style switch
        {
            ConsoleStyle.Bold => 1,
            ConsoleStyle.Underlined => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
    }

    public bool IsVt100Enabled()
    {
        return !PlatformUtils.IsWindows();
    }

    private class SgrResetDisposable : IDisposable
    {
        private readonly AnsiConsole _console;

        public SgrResetDisposable(AnsiConsole console)
        {
            _console = console;
        }

        public void Dispose()
        {
            _console._stdout.Write(CSI + "0m");
        }
    }
}

public enum EraseLineMode
{
    LineEnd = 0,
    LineStart = 1,
    All = 2
}

public class ConsoleException : Exception
{
    public ConsoleException(string message)
        : base(message) { }
}
