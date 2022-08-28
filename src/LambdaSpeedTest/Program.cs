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
        IOutputter outputter = new ConsoleOutputter();
        RunTest(static (r) => r.TestWithForeachAndVirtualInstanceMethod, "Foreach loop, virtual instance method", options.Count, outputter);
        RunTest(static (r) => r.TestWithForeachAndInstanceMethod, "Foreach loop, instance method", options.Count, outputter);
        RunTest(static (r) => r.TestWithForeachAndStaticMethod, "Foreach loop, static method", options.Count, outputter);
        RunTest(static (r) => r.TestWithForeachAndInstanceLambda, "Foreach loop, instance lambda", options.Count, outputter);
        RunTest(static (r) => r.TestWithForeachAndStaticLambda, "Foreach loop, static lambda", options.Count, outputter);
        RunTest(static (r) => r.TestWithSelectAndInstanceLambda, "LINQ, instance lambda", options.Count, outputter);
        RunTest(static (r) => r.TestWithSelectAndStaticLambda, "LINQ, static lambda", options.Count, outputter);
    }

    private static void RunTest(Func<TestRunner, Func<long>> testSelector, string testName, int repeatCount, IOutputter outputter)
    {
        //writeLine($"Running test \"{testName}\"");
        outputter.StartProgress(testName);
        List<TestDataAverageResult> results = new();

        // The first invocation of the test method incurs overhead which we don't want to affect the results.
        using (TestRunner throwaway = new(1))
        {
            _ = testSelector(throwaway).Invoke();
        }

        for (int size = 1; size <= 1_000_000_000; size *= 10)
        {
            TestDataAverageResult collectedResults = new(size);
            for (int repeats = 0; repeats < repeatCount; repeats++)
            {
                using (TestRunner runner = new(size))
                {
                    long result = testSelector(runner).Invoke();
                    collectedResults.Add(new TestDataResult(size, result));
                    outputter.StepProgress();
                }

                // Aggressively get rid of the rubbish now to try to avoid it confusing our stats if it runs during a test
                GC.Collect(3, GCCollectionMode.Forced, true);
                GC.Collect(3, GCCollectionMode.Forced, true);
                GC.Collect(3, GCCollectionMode.Forced, true);
            }
            results.Add(collectedResults);
        }

        outputter.StopProgress();
        outputter.OutputRecordSet(results);
    }
}