using System.Diagnostics;

namespace LambdaSpeedTest;

internal class TestRunner
{
    private static readonly Random _random = new();

    private readonly List<int> _testSourceData;

    internal List<int> TestOutputData { get; init; }

    internal TestRunner(int size)
    {
        _testSourceData = new List<int>(size);
        TestOutputData = new List<int>(size);
        PopulateSourceData(size);
    }

    public long TestWithForeachAndInstanceLambda()
        => RunTestWithStopwatch(TestMethodWithForeachAndInstanceLambda);

    public long TestWithForeachAndStaticLambda()
        => RunTestWithStopwatch(TestMethodWithForeachAndStaticLambda);

    public long TestWithSelectAndInstanceLambda()
        => RunTestWithStopwatch(TestMethodWithSelectAndInstanceLambda);

    public long TestWithSelectAndStaticLambda()
        => RunTestWithStopwatch(TestMethodWithSelectAndStaticLambda);

    private void TestMethodWithForeachAndInstanceLambda()
    {
        var lambda = (int x) => x * 3 + 5;
        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(lambda(val));
        }
    }

    private void TestMethodWithForeachAndStaticLambda()
    {
        var lambda = static (int x) => x * 3 + 5;
        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(lambda(val));
        }
    }

    private void TestMethodWithSelectAndInstanceLambda()
    {
        foreach (int output in _testSourceData.Select(x => x * 3 + 5))
        {
            TestOutputData.Add(output);
        }
    }

    private void TestMethodWithSelectAndStaticLambda()
    {
        foreach (int output in _testSourceData.Select(static (x) => x * 3 + 5))
        {
            TestOutputData.Add(output);
        }
    }
        
    private static long RunTestWithStopwatch(Action testFunction)
    {
        Stopwatch watch = new();
        watch.Start();
        testFunction();
        watch.Stop();
        return watch.ElapsedMilliseconds;
    }

    private void PopulateSourceData(int size)
    {
        for (int i = 0; i < size; ++i)
        {
            _testSourceData.Add(_random.Next(int.MaxValue / 4));
        }
    }
}
