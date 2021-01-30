using System;

namespace Mjcheetham.PromptToolkit
{
    public class Prompt
    {
        private readonly IConsole _console;

        public Prompt(IConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        private interface IAnswerHandler<T>
        {
            string GetDefaultHint(T defaultValue);
            bool? TryParse(string str, out T result);
            string GetDisplayValue(T value);
        }

        private T Ask<T>(string question, T? defaultValue, IAnswerHandler<T> handler) where T : struct
        {
            int lineLength = 0;
            using (_console.SetStyle(ConsoleStyle.Bold))
            {
                using (_console.SetColor(ConsoleColor.Green))
                {
                    _console.Write("? ");
                    lineLength += 2;
                }

                _console.Write("{0} ", question);
                lineLength += question.Length + 1;
            }

            _console.HideCursor();
            _console.WriteLine();
            _console.MoveCursorUp();
            _console.CursorLeft = lineLength;
            _console.ShowCursor();

            int fullLineLength = lineLength;

            if (defaultValue.HasValue)
            {
                string hint = handler.GetDefaultHint(defaultValue.Value);
                using (_console.SetColor(ConsoleColor.Gray))
                {
                    _console.Write("{0} ", hint);
                }
                fullLineLength += hint.Length + 1;
            }

            T answer;
            while (true)
            {
                _console.ShowCursor();
                string answerStr = _console.ReadLine();
                _console.HideCursor();
                bool? result = handler.TryParse(answerStr, out answer);
                if (result == true)
                {
                    break;
                }

                if (result is null && defaultValue.HasValue)
                {
                    answer = defaultValue.Value;
                    break;
                }

                _console.MoveCursorUp();
                _console.CursorLeft = fullLineLength;
                _console.EraseRight();
            }

            _console.MoveCursorUp();
            _console.CursorLeft = lineLength;
            _console.EraseRight();
            _console.ShowCursor();

            using (_console.SetColor(ConsoleColor.Cyan))
            {
                _console.WriteLine(handler.GetDisplayValue(answer));
            }

            return answer;
        }

        private T Ask<T>(string question, T defaultValue, IAnswerHandler<T> handler) where T : class
        {
            int lineLength = 0;
            using (_console.SetStyle(ConsoleStyle.Bold))
            {
                using (_console.SetColor(ConsoleColor.Green))
                {
                    _console.Write("? ");
                    lineLength += 2;
                }

                _console.Write("{0} ", question);
                lineLength += question.Length + 1;
            }

            _console.HideCursor();
            _console.WriteLine();
            _console.MoveCursorUp();
            _console.CursorLeft = lineLength;
            _console.ShowCursor();

            int fullLineLength = lineLength;

            if (!(defaultValue is null))
            {
                string hint = handler.GetDefaultHint(defaultValue);
                using (_console.SetColor(ConsoleColor.Gray))
                {
                    _console.Write("{0} ", hint);
                }

                fullLineLength += hint.Length + 1;
            }

            T answer;
            while (true)
            {
                _console.ShowCursor();
                string answerStr = _console.ReadLine();
                _console.HideCursor();
                bool? result = handler.TryParse(answerStr, out answer);
                if (result == true)
                {
                    break;
                }

                if (result == true || result is null && !(defaultValue is null))
                {
                    break;
                }

                _console.MoveCursorUp();
                _console.CursorLeft = fullLineLength;
                _console.EraseRight();
            }

            _console.MoveCursorUp();
            _console.CursorLeft = lineLength;
            _console.EraseRight();
            _console.ShowCursor();

            using (_console.SetColor(ConsoleColor.Cyan))
            {
                _console.WriteLine(handler.GetDisplayValue(answer));
            }

            return answer;
        }

        public TEnum AskOption<TEnum>(string question) where TEnum : Enum
        {
            var options = (TEnum[]) Enum.GetValues(typeof(TEnum));
            return AskOption(question, options);
        }

        public T AskOption<T>(string question, T[] options)
        {
            _console.HideCursor();

            using (_console.SetStyle(ConsoleStyle.Bold))
            {
                using (_console.SetColor(ConsoleColor.Green))
                {
                    _console.Write("? ");
                }

                _console.Write("{0} ", question);
            }

            using (_console.SetColor(ConsoleColor.Gray))
            {
                _console.WriteLine("(use arrow keys to select)");
            }

            for (var i = 0; i < options.Length; i++)
            {
                T option = options[i];
                if (i > 0)
                {
                    _console.WriteLine("   {0}", option.ToString());
                }
                else
                {
                    using (_console.SetStyle(ConsoleStyle.Bold))
                    using (_console.SetColor(ConsoleColor.Green))
                    {
                        _console.WriteLine(" > {0}", option.ToString());
                    }
                }
            }

            _console.MoveCursorUp(options.Length);
            _console.CursorLeft = 0;

            int index = 0;

            bool done = false;
            while (!done)
            {
                int newIndex;
                ConsoleKeyInfo keyInfo = _console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        _console.Write("   {0}", options[index]);
                        newIndex = Math.Max(0, index - 1);
                        _console.MoveCursorUp(index - newIndex);
                        _console.CursorLeft = 0;
                        using (_console.SetStyle(ConsoleStyle.Bold))
                        using (_console.SetColor(ConsoleColor.Green))
                        {
                            _console.Write(" > {0}", options[newIndex]);
                        }
                        _console.CursorLeft = 0;
                        index = newIndex;
                        break;

                    case ConsoleKey.DownArrow:
                        _console.Write("   {0}", options[index]);
                        newIndex = Math.Min(options.Length - 1, index + 1);
                        _console.MoveCursorDown(newIndex - index);
                        _console.CursorLeft = 0;
                        using (_console.SetStyle(ConsoleStyle.Bold))
                        using (_console.SetColor(ConsoleColor.Green))
                        {
                            _console.Write(" > {0}", options[newIndex]);
                        }
                        _console.CursorLeft = 0;
                        index = newIndex;
                        break;

                    case ConsoleKey.Enter:
                        done = true;
                        _console.MoveCursorDown(options.Length - index);
                        break;
                }
            }

            for (int i = 0; i < options.Length; i++)
            {
                _console.MoveCursorUp();
                _console.EraseLine();
            }

            _console.MoveCursorUp();
            _console.CursorLeft = question.Length + 3;
            _console.EraseRight();

            using (_console.SetColor(ConsoleColor.Cyan))
            {
                _console.WriteLine(options[index]);
            }

            _console.ShowCursor();

            return options[index];
        }

        public string AskString(string question, string defaultValue = null) =>
            Ask(question, defaultValue, StringHandler.Instance);

        public bool AskBoolean(string question, bool? defaultValue = null) =>
            Ask(question, defaultValue, BooleanYesNoHandler.Instance);

        public int AskInteger(string question, int? defaultValue = null) =>
            Ask(question, defaultValue, new NumberHandler<int>(int.TryParse));

        public double AskDouble(string question, double? defaultValue = null) =>
            Ask(question, defaultValue, new NumberHandler<double>(double.TryParse));

        private class NumberHandler<T> : IAnswerHandler<T>
        {
            public delegate bool TryParseFunc<TOut>(string input, out TOut output);

            private readonly TryParseFunc<T> _tryParse;

            public NumberHandler(TryParseFunc<T> tryParse) => _tryParse = tryParse;

            public string GetDefaultHint(T defaultValue) => $"(default: {defaultValue})";

            public bool? TryParse(string str, out T result)
            {
                if (_tryParse(str, out result))
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(str))
                {
                    return null;
                }

                return false;
            }

            public string GetDisplayValue(T value) => value.ToString();
        }

        private class BooleanYesNoHandler : IAnswerHandler<bool>
        {
            public static readonly BooleanYesNoHandler Instance = new BooleanYesNoHandler();
            private BooleanYesNoHandler() { }

            public string GetDefaultHint(bool defaultValue)
            {
                return defaultValue ? "(Y/n)" : "(y/N)";
            }

            public bool? TryParse(string str, out bool result)
            {
                result = false;

                switch (str.ToLowerInvariant())
                {
                    case "yes":
                    case "y":
                        result = true;
                        return true;

                    case "no":
                    case "n":
                        result = false;
                        return true;
                }

                if (string.IsNullOrWhiteSpace(str)) return null;

                return false;
            }

            public string GetDisplayValue(bool value)
            {
                return value ? "Yes" : "No";
            }
        }

        private class StringHandler : IAnswerHandler<string>
        {
            public static readonly StringHandler Instance = new StringHandler();
            private StringHandler() { }

            public string GetDefaultHint(string defaultValue)
            {
                return $"({defaultValue})";
            }

            public bool? TryParse(string str, out string result)
            {
                result = str;
                if (string.IsNullOrWhiteSpace(str)) return null;
                return true;
            }

            public string GetDisplayValue(string value)
            {
                return value;
            }
        }
    }
}
