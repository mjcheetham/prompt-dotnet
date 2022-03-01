using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mjcheetham.PromptToolkit;

public interface IPrompt
{
    T Ask<T>(string question, IAnswerHandler<T> handler);

    T AskOption<T>(string question, IEnumerable<T> options, Func<T, string> displayNameFunc);
}

public class Prompt : IPrompt
{
    private readonly IConsole _console;

    public Prompt(IConsole console)
    {
        _console = console;
    }

    public T Ask<T>(string question, IAnswerHandler<T> handler)
    {
        using (_console.SetStyle(ConsoleColor.Green, ConsoleStyle.Bold))
        {
            _console.Write("? ");
        }

        using (_console.SetStyle(ConsoleStyle.Bold))
        {
            _console.Write("{0} ", question);
        }

        _console.SaveCursor();

        bool failed = false;
        while (true)
        {
            string str = _console.ReadLine();

            if (handler.TryParse(str, out T answer, out string errorMessage))
            {
                if (failed)
                {
                    using (_console.SetCursorVisible(false))
                    {
                        _console.RestoreCursor();
                        _console.EraseLine(EraseLineMode.LineEnd);
                    }
                    _console.WriteLine(str);
                }

                return answer;
            }

            failed = true;
            using (_console.SetCursorVisible(false))
            {
                _console.RestoreCursor();
                _console.EraseLine(EraseLineMode.LineEnd);
            }

            using (_console.SetStyle(ConsoleColor.Red))
            {
                _console.Write("({0}) ", errorMessage);
            }
        }
    }

    public T AskOption<T>(string question, IEnumerable<T> options, Func<T, string> displayNameFunc)
    {
        using (_console.SetStyle(ConsoleColor.Green, ConsoleStyle.Bold))
        {
            _console.Write("? ");
        }

        using (_console.SetStyle(ConsoleStyle.Bold))
        {
            _console.Write("{0} ", question);
        }

        _console.SaveCursor();

        using (_console.SetStyle(ConsoleColor.Cyan))
        {
            _console.WriteLine("[use arrow keys to move]");
        }

        _console.HideCursor();

        T[] menuOptions = options.ToArray();
        string[] menuNames = menuOptions.Select(displayNameFunc).ToArray();

        void PrintItem(int index, bool selected)
        {
            if (selected)
            {
                using (_console.SetStyle(ConsoleColor.Cyan))
                {
                    _console.Write("> {0}", menuNames[index]);
                }
            }
            else
            {
                _console.Write("  {0}", menuNames[index]);
            }
        }

        // Initial selection is the first item
        int selectedIndex = 0;

        // Render all menu items
        for (int i = 0; i < menuOptions.Length; i++)
        {
            PrintItem(i, i == selectedIndex);
            _console.WriteLine();
        }

        // Move cursor to initial selection
        _console.MoveCursorUp(menuOptions.Length - selectedIndex);

        void MoveUp()
        {
            int newIndex = Math.Max(0, selectedIndex - 1);
            if (newIndex != selectedIndex)
            {
                // Re-print the currently selected item
                _console.MoveCursorAbsoluteX(0);
                PrintItem(selectedIndex, false);

                // Update selection index
                selectedIndex = newIndex;

                // Move up and print the newly selected item
                _console.MoveCursorUp();
                _console.MoveCursorAbsoluteX(0);
                PrintItem(selectedIndex, true);
            }
        }

        void MoveDown()
        {
            int newIndex = Math.Min(menuOptions.Length - 1, selectedIndex + 1);
            if (newIndex != selectedIndex)
            {
                // Re-print the currently selected item
                _console.MoveCursorAbsoluteX(0);
                PrintItem(selectedIndex, false);

                // Update selection index
                selectedIndex = newIndex;

                // Move down and print selection symbol
                _console.MoveCursorDown();
                _console.MoveCursorAbsoluteX(0);
                PrintItem(selectedIndex, true);
            }
        }

        bool done = false;
        while (!done)
        {
            ConsoleKeyInfo key = _console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    MoveUp();
                    break;
                case ConsoleKey.DownArrow:
                    MoveDown();
                    break;
                case ConsoleKey.Enter:
                    done = true;
                    break;
                default:
                    continue;
            }
        }

        _console.MoveCursorDown(menuOptions.Length - selectedIndex - 1);

        for (int i = 0; i < selectedIndex - 1; i++)
        {
            _console.EraseLine(EraseLineMode.All);
            _console.MoveCursorUp();
        }

        _console.RestoreCursor();
        _console.EraseLine(EraseLineMode.LineEnd);
        _console.WriteLine(menuNames[selectedIndex]);

        _console.ShowCursor();

        return menuOptions[selectedIndex];
    }
}

public interface IAnswerHandler<T>
{
    bool TryParse(string str, out T value, out string errorMessage);
}

public class StringAnswerHandler : IAnswerHandler<string>
{
    public bool IsRequired { get; set; }

    public int? MinimumLength { get; set; }

    public int? MaximumLength { get; set; }

    public bool TryParse(string str, out string value, out string errorMessage)
    {
        errorMessage = null;
        value = str;

        if (IsRequired && string.IsNullOrWhiteSpace(str))
        {
            errorMessage = "required";
            return false;
        }

        if (MinimumLength.HasValue && str?.Length < MinimumLength)
        {
            errorMessage = $"min length {MinimumLength}";
            return false;
        }

        if (MaximumLength.HasValue && str?.Length > MaximumLength)
        {
            errorMessage = $"max length {MaximumLength}";
            return false;
        }

        return true;
    }
}

public class Int32AnswerHandler : IAnswerHandler<int>
{
    public bool IsRequired { get; set; }

    public int? Minimum { get; set; }

    public int? Maximum { get; set; }

    public bool TryParse(string str, out int value, out string errorMessage)
    {
        errorMessage = null;
        value = default;

        if (IsRequired && string.IsNullOrWhiteSpace(str))
        {
            errorMessage = "required";
            return false;
        }

        if (!int.TryParse(str, out value))
        {
            errorMessage = "not an integer";
            return false;
        }

        bool validMin = !Minimum.HasValue || value >= Minimum;
        bool validMax = !Maximum.HasValue || value <= Maximum;

        if (!validMin || !validMax)
        {
            if (Maximum.HasValue && !Minimum.HasValue)
            {
                errorMessage = $"must be less than {Maximum}";
            }
            else if (!Maximum.HasValue)
            {
                errorMessage = $"must be greater than {Minimum}";
            }
            else
            {
                errorMessage = $"must be between {Minimum} and {Maximum}";
            }

            return false;
        }

        return true;
    }
}

public static class PromptExtensions
{
    public static string AskString(this IPrompt prompt,
        string question, bool isRequired = false, int? minLength = null, int? maxLength = null)
    {
        return prompt.Ask(question,
            new StringAnswerHandler
            {
                IsRequired = isRequired,
                MinimumLength = minLength,
                MaximumLength = maxLength
            }
        );
    }

    public static int AskInt32(this IPrompt prompt,
        string question, bool isRequired = false, int? min = null, int? max = null)
    {
        return prompt.Ask(question,
            new Int32AnswerHandler
            {
                IsRequired = isRequired,
                Minimum = min,
                Maximum = max
            }
        );
    }

    public static T AskOption<T>(this IPrompt prompt,
        string question, IEnumerable<T> options)
    {
        return prompt.AskOption(question, options, x => x.ToString());
    }
}
