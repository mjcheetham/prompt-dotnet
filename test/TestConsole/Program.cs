using System;
using Mjcheetham.PromptToolkit;

namespace TestConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var stdin = Console.In;
            var stdout = Console.Out;
            IConsole con = new AnsiConsole(stdin, stdout);
            var prompt = new Prompt(con);

            string name = prompt.AskString("What is your name?", true, 5, 10);
            con.WriteLineInfo("Your name is {0}", name);

            con.WriteLineFailure("Failure");
            con.WriteLineSuccess("All good!");

            // SortingAlgorithm algo2 = prompt.AskOption<SortingAlgorithm>("Pick a sorting algorithm");
            //
            // for (int i = Console.CursorTop; i < Console.WindowHeight; i++)
            // {
            //     Console.WriteLine("line {2} [{0},{1}]", Console.CursorLeft, Console.CursorTop, i);
            // }
            //
            // Console.WriteLine("one past [{0},{1}]", Console.CursorLeft, Console.CursorTop);
            // Console.WriteLine("two past [{0},{1}]", Console.CursorLeft, Console.CursorTop);
            //
            // con.WriteLineAlert("This will test the console cursor APIs...");
            // con.Write("keepme");
            // con.WriteLine("eraseme");
            // con.WriteLine("eraseme");
            // con.WriteLine("eraseme");
            // con.Write("eraseme");
            //
            // var c1 = con.GetCursor();
            // con.EraseLeft();
            // con.MoveCursorUp();
            // con.EraseLine();
            // con.MoveCursorUp();
            // con.EraseLine();
            // con.MoveCursorUp();
            // con.CursorLeft = 6;
            // con.EraseLeft();
            // con.SetCursor(c1);
            //
            // con.WriteLine("keepme");
            // con.WriteLine("keepme");
            // con.WriteLineInfo("Bye!");
            //
            // con.WriteLineAlert("Please complete the survey...");
            //
            // string name = prompt.AskString("What is your name?");
            // con.WriteLineInfo("Hello, {0}!", name);
            //
            // if (prompt.AskBoolean("Do you like cheese?", true))
            // {
            //     con.WriteLineSuccess("You like cheese!");
            // }
            // else
            // {
            //     con.WriteLineFailure("What is wrong with cheese?!");
            // }
            //
            // int age = prompt.AskInteger("How old are you?");
            // con.WriteLineSuccess("You are {0} years old.", age);
            //
            // double height = prompt.AskDouble("How tall are you?");
            // string unit = prompt.AskOption("..and what unit was that?", new[] {"metres", "feet"});
            // con.WriteLineSuccess("You are {0} {1} tall.", height, unit);
            //
            // SortingAlgorithm algo = prompt.AskOption<SortingAlgorithm>("Pick a sorting algorithm");
            // con.WriteLineSuccess("You selected {0}.", algo);
            //
            // con.WriteLineInfo("Thank you for your time, {0}. Good bye!", name);
        }

        private enum SortingAlgorithm
        {
            QuickSort,
            MergeSort,
            BubbleSort,
            InsertionSort
        }
    }
}
