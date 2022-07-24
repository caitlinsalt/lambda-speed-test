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

    public long TestWithForeachAndVirtualInstanceMethod()
        => RunTestWithStopwatch(TestMethodWithForeachAndVirtualInstanceMethod);

    public long TestWithForeachAndInstanceMethod()
        => RunTestWithStopwatch(TestMethodWithForeachAndInstanceMethod);

    public long TestWithForeachAndStaticMethod()
        => RunTestWithStopwatch(TestMethodWithForeachAndStaticMethod);

    public long TestWithForeachAndInstanceLambda()
        => RunTestWithStopwatch(TestMethodWithForeachAndInstanceLambda);

    public long TestWithForeachAndStaticLambda()
        => RunTestWithStopwatch(TestMethodWithForeachAndStaticLambda);

    public long TestWithSelectAndInstanceLambda()
        => RunTestWithStopwatch(TestMethodWithSelectAndInstanceLambda);

    public long TestWithSelectAndStaticLambda()
        => RunTestWithStopwatch(TestMethodWithSelectAndStaticLambda);

    private void TestMethodWithForeachAndVirtualInstanceMethod()
    {
        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(VirtualInstanceMethod(val));
        }
    }

    protected virtual int VirtualInstanceMethod(int x)
        => x * 3 + 5;

    private void TestMethodWithForeachAndInstanceMethod()
    {
        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(InstanceMethod(val));
        }
    }

#pragma warning disable CA1822 // Mark members as static

    private int InstanceMethod(int x)
        => x * 3 + 5;

#pragma warning restore CA1822 // Mark members as static

    private void TestMethodWithForeachAndStaticMethod()
    {
        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(StaticMethod(val));
        }
    }

    private static int StaticMethod(int x)
        => x * 3 + 5;

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
        return watch.ElapsedTicks;
    }

    private void PopulateSourceData(int size)
    {
        for (int i = 0; i < size; ++i)
        {
#pragma warning disable CA5394 // Do not use insecure randomness
            _testSourceData.Add(_random.Next(int.MaxValue / 4));
#pragma warning restore CA5394 // Do not use insecure randomness
        }
    }
}
