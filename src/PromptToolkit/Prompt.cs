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
            IConsoleCursor resultCursor;

            using (_console.SetStyle(ConsoleStyle.Bold))
            {
                using (_console.SetColor(ConsoleColor.Green))
                {
                    _console.Write("? ");
                }

                _console.Write("{0} ", question);
                resultCursor = _console.SaveCursor();
            }

            if (defaultValue.HasValue)
            {
                string hint = handler.GetDefaultHint(defaultValue.Value);
                using (_console.SetColor(ConsoleColor.Gray))
                {
                    _console.Write("{0} ", hint);
                }
            }

            IConsoleCursor promptCursor = _console.SaveCursor();

            T answer;
            while (true)
            {
                string answerStr = _console.ReadLine();
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

                promptCursor.Reset(clear: true);
            }

            resultCursor.Reset(clear: true);

            using (_console.SetColor(ConsoleColor.Cyan))
            {
                _console.WriteLine(handler.GetDisplayValue(answer));
            }

            return answer;
        }

        private T Ask<T>(string question, T defaultValue, IAnswerHandler<T> handler) where T : class
        {
            IConsoleCursor resultCursor;

            using (_console.SetStyle(ConsoleStyle.Bold))
            {
                using (_console.SetColor(ConsoleColor.Green))
                {
                    _console.Write("? ");
                }

                _console.Write("{0} ", question);
                resultCursor = _console.SaveCursor();
            }

            if (!(defaultValue is null))
            {
                string hint = handler.GetDefaultHint(defaultValue);
                using (_console.SetColor(ConsoleColor.Gray))
                {
                    _console.Write("{0} ", hint);
                }
            }

            IConsoleCursor promptCursor = _console.SaveCursor();

            T answer;
            while (true)
            {
                string answerStr = _console.ReadLine();
                bool? result = handler.TryParse(answerStr, out answer);
                if (result == true || result is null && !(defaultValue is null))
                {
                    break;
                }

                promptCursor.Reset(clear: true);
            }

            resultCursor.Reset(clear: true);

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
            IConsoleCursor resultCursor;

            using (_console.SetStyle(ConsoleStyle.Bold))
            {
                using (_console.SetColor(ConsoleColor.Green))
                {
                    _console.Write("? ");
                }

                _console.Write("{0} ", question);
                resultCursor = _console.SaveCursor();
            }

            using (_console.SetColor(ConsoleColor.Gray))
            {
                _console.WriteLine("(use arrow keys to select)");
            }

            int optionOffset = _console.CursorTop;
            foreach (T option in options)
            {
                _console.WriteLine("   {0}", option.ToString());
            }

            IConsoleCursor promptCursor = _console.SaveCursor();
            int index = -1;

            void UpdateIndex(int newIndex)
            {
                if (newIndex == index)
                {
                    return;
                }

                // Erase old index
                if (index > -1)
                {
                    _console.SetCursor(1, optionOffset + index);
                    _console.Write(' ');
                }

                // Draw new index
                _console.SetCursor(1, optionOffset + newIndex);
                using (_console.SetColor(ConsoleColor.Green))
                using (_console.SetStyle(ConsoleStyle.Bold))
                {
                    _console.Write('>');
                }

                index = newIndex;

                promptCursor.Reset();
            }

            UpdateIndex(0);

            bool done = false;
            while (!done)
            {
                ConsoleKeyInfo keyInfo = _console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        UpdateIndex(Math.Max(0, index - 1));
                        break;

                    case ConsoleKey.DownArrow:
                        UpdateIndex(Math.Min(options.Length - 1, index + 1));
                        break;

                    case ConsoleKey.Enter:
                        done = true;
                        break;
                }
            }

            resultCursor.Reset(clear: true);

            using (_console.SetColor(ConsoleColor.Cyan))
            {
                _console.WriteLine(options[index]);
            }

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
