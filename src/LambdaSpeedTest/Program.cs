using CommandLine;
using System.Globalization;

[assembly: CLSCompliant(true)]

namespace LambdaSpeedTest;

internal class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(opt => RunApp(opt));
    }

    private static void RunApp(Options options)
    {
        RunTest(static (r) => r.TestWithForeachAndVirtualInstanceMethod, "Foreach loop, virtual instance method", options.Count, Console.WriteLine);
        RunTest(static (r) => r.TestWithForeachAndInstanceMethod, "Foreach loop, instance method", options.Count, Console.WriteLine);
        RunTest(static (r) => r.TestWithForeachAndStaticMethod, "Foreach loop, static method", options.Count, Console.WriteLine);
        RunTest(static (r) => r.TestWithForeachAndInstanceLambda, "Foreach loop, instance lambda", options.Count, Console.WriteLine);
        RunTest(static (r) => r.TestWithForeachAndStaticLambda, "Foreach loop, static lambda", options.Count, Console.WriteLine);
        RunTest(static (r) => r.TestWithSelectAndInstanceLambda, "LINQ, instance lambda", options.Count, Console.WriteLine);
        RunTest(static (r) => r.TestWithSelectAndStaticLambda, "LINQ, static lambda", options.Count, Console.WriteLine);
    }

    private static void RunTest(Func<TestRunner, Func<long>> testSelector, string testName, int repeatCount, Action<string> writeLine)
    {
        writeLine($"Running test \"{testName}\"");
        List<TestDataAverageResult> results = new();

        // The first invocation of the test method incurs overhead which we don't want to affect the results.
        _ = testSelector(new TestRunner(1)).Invoke();

        for (int size = 1; size <= 1_000_000_000; size *= 10)
        {
            TestDataAverageResult collectedResults = new(size);
            for (int repeats = 0; repeats < repeatCount; repeats++)
            {
                using (TestRunner runner = new(size))
                {
                    long result = testSelector(runner).Invoke();
                    collectedResults.Add(new TestDataResult(size, result));
                }

                // Aggressively get rid of the rubbish now to try to avoid it confusing our stats if it runs during a test
                GC.Collect(3, GCCollectionMode.Forced, true);
                GC.Collect(3, GCCollectionMode.Forced, true);
                GC.Collect(3, GCCollectionMode.Forced, true);
            }
            results.Add(collectedResults);
        }
        
        OutputRecordSet(results, writeLine);
    }

    private static void OutputRecordSet(IList<TestDataAverageResult> results, Action<string> writeLine)
    {
        string[] headings = new[] { "Size", "Time (ticks)", "Time (ms)" };
        string integerNumberFormat = "n0";
        string decimalNumberFormat = "n1";
        int[] widths = headings.Select((h, i) => GetMaxWidth(results, GetPropertySelector(i), h.Length, integerNumberFormat)).ToArray();
        string hrule = "+" + string.Join("+", widths.Select(static (w) => new string('-', w + 2))) + "+";
        string lineFormat = $"| {{0,{widths[0]}:{integerNumberFormat}}} | {{1,{widths[1]}:{integerNumberFormat}}} | {{2,{widths[2]}:{decimalNumberFormat}}} |";
        writeLine(hrule);
        writeLine(string.Format(CultureInfo.CurrentCulture, lineFormat, headings[0], headings[1], headings[2]));
        writeLine(hrule);
        foreach (TestDataAverageResult record in results)
        {
            writeLine(string.Format(CultureInfo.CurrentCulture, lineFormat, record.Size, record.ElapsedTime, record.ElapsedTimeMs));
        }
        writeLine(hrule);
        writeLine("");
    }

    private static Func<TestDataAverageResult, IFormattable> GetPropertySelector(int columnIndex)
        => columnIndex switch
        {
            0 => r => r.Size,
            1 => r => r.ElapsedTime,
            2 => r => r.ElapsedTimeMs,
            _ => throw new NotSupportedException(),
        };

    private static int GetMaxWidth(IEnumerable<TestDataAverageResult> data, Func<TestDataAverageResult, IFormattable> selector, int min, string format)
    {
        int calc = data.Select(x => selector(x)?.ToString(format, CultureInfo.CurrentCulture)?.Length ?? 0).Max();
        return calc < min ? min : calc;
    }
}