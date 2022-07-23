namespace LambdaSpeedTest;

internal class Program
{
    static void Main(string[] args)
    {
        RunTest(static (r) => r.TestWithForeachAndInstanceLambda, "Foreach loop, instance lambda", Console.WriteLine);
        RunTest(static (r) => r.TestWithForeachAndStaticLambda, "Foreach loop, static lambda", Console.WriteLine);
        RunTest(static (r) => r.TestWithSelectAndInstanceLambda, "LINQ, instance lambda", Console.WriteLine);
        RunTest(static (r) => r.TestWithSelectAndStaticLambda, "LINQ, static lambda", Console.WriteLine);
    }

    private static void RunTest(Func<TestRunner, Func<long>> testSelector, string testName, Action<string> writeLine)
    {
        writeLine($"Running test \"{testName}\"");
        List<TestDataResult> results = new();
        for (int i = 1; i <= 1_000_000_000; i *= 10)
        {
            TestRunner runner = new(i);
            long result = testSelector(runner).Invoke();
            results.Add(new TestDataResult(i, result));
        }
        OutputRecordSet(results, writeLine);
    }

    private static void OutputRecordSet(IList<TestDataResult> results, Action<string> writeLine)
    {
        int[] widths = new int[2];
        string[] headings = new[] { "Size", "Time (ms)" };
        widths[0] = GetMaxWidth(results, r => r.Size, headings[0].Length);
        widths[1] = GetMaxWidth(results, r => r.ElapsedTime, headings[1].Length);
        string hrule = "+" + string.Join("+", widths.Select(static (w) => new string('-', w + 2))) + "+";
        string lineFormat = $"| {{0,-{widths[0]}}} | {{1, -{widths[1]}}} |";
        writeLine(hrule);
        writeLine(string.Format(lineFormat, headings[0], headings[1]));
        writeLine(hrule);
        foreach (TestDataResult record in results)
        {
            writeLine(string.Format(lineFormat, record.Size, record.ElapsedTime));
        }
        writeLine(hrule);
        writeLine("");
    }

    private static int GetMaxWidth(IEnumerable<TestDataResult> data, Func<TestDataResult, object> selector, int min)
    {
        int calc = data.Select(x => selector(x)?.ToString()?.Length ?? 0).Max();
        return calc < min ? min : calc;
    }
}