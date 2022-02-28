using System;

namespace Mjcheetham.PromptToolkit;

public interface IPrompt
{
    T Ask<T>(string question, IAnswerHandler<T> handler);
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
}
