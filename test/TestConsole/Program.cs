using Mjcheetham.PromptToolkit;

namespace TestConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var console = new SystemConsole();
            var prompt = new Prompt(console);

            console.WriteLineAlert("This will test the console cursor APIs...");
            console.Write("keepme");
            var c1 = console.SaveCursor();
            console.WriteLine("eraseme");
            console.WriteLine("eraseme");
            console.WriteLine("eraseme");
            console.Write("eraseme");
            var c2 = console.SaveCursor();
            console.WriteLine("keepme");
            console.WriteLine("keepme");
            var c3 = console.SaveCursor();
            console.ReadKey(intercept: true);
            c2.Reset();
            console.ReadKey(intercept: true);
            c1.Reset(clear: true);
            console.ReadKey(intercept: true);
            c3.Reset();
            console.WriteLineInfo("Bye!");

            console.WriteLineAlert("Please complete the survey...");

            string name = prompt.AskString("What is your name?");
            console.WriteLineInfo("Hello, {0}!", name);

            if (prompt.AskBoolean("Do you like cheese?", true))
            {
                console.WriteLineSuccess("You like cheese!");
            }
            else
            {
                console.WriteLineFailure("What is wrong with cheese?!");
            }

            int age = prompt.AskInteger("How old are you?");
            console.WriteLineSuccess("You are {0} years old.", age);

            double height = prompt.AskDouble("How tall are you?");
            string unit = prompt.AskOption("..and what unit was that?", new[] {"metres", "feet"});
            console.WriteLineSuccess("You are {0} {1} tall.", height, unit);

            SortingAlgorithm algo = prompt.AskOption<SortingAlgorithm>("Pick a sorting algorithm");
            console.WriteLineSuccess("You selected {0}.", algo);

            console.WriteLineInfo("Thank you for your time, {0}. Good bye!", name);
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
