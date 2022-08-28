using System.Diagnostics;

namespace LambdaSpeedTest;

internal class TestRunner : IDisposable
{
    private static readonly Random _random = new();

    private List<int>? _testSourceData;
    private bool _disposed;

    internal List<int>? TestOutputData { get; private set; }

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
        if (_testSourceData is null || TestOutputData is null)
        {
            throw new InvalidOperationException();
        }

        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(VirtualInstanceMethod(val));
        }
    }

    protected virtual int VirtualInstanceMethod(int x)
        => x * 3 + 5;

    private void TestMethodWithForeachAndInstanceMethod()
    {
        if (_testSourceData is null || TestOutputData is null)
        {
            throw new InvalidOperationException();
        }

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
        if (_testSourceData is null || TestOutputData is null)
        {
            throw new InvalidOperationException();
        }

        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(StaticMethod(val));
        }
    }

    private static int StaticMethod(int x)
        => x * 3 + 5;

    private void TestMethodWithForeachAndInstanceLambda()
    {
        if (_testSourceData is null || TestOutputData is null)
        {
            throw new InvalidOperationException();
        }

        var lambda = (int x) => x * 3 + 5;
        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(lambda(val));
        }
    }

    private void TestMethodWithForeachAndStaticLambda()
    {
        if (_testSourceData is null || TestOutputData is null)
        {
            throw new InvalidOperationException();
        }

        var lambda = static (int x) => x * 3 + 5;
        foreach (int val in _testSourceData)
        {
            TestOutputData.Add(lambda(val));
        }
    }

    private void TestMethodWithSelectAndInstanceLambda()
    {
        if (_testSourceData is null || TestOutputData is null)
        {
            throw new InvalidOperationException();
        }

        foreach (int output in _testSourceData.Select(x => x * 3 + 5))
        {
            TestOutputData.Add(output);
        }
    }

    private void TestMethodWithSelectAndStaticLambda()
    {
        if (_testSourceData is null || TestOutputData is null)
        {
            throw new InvalidOperationException();
        }

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
        if (_testSourceData is null)
        {
            throw new InvalidOperationException();
        }

        for (int i = 0; i < size; ++i)
        {
#pragma warning disable CA5394 // Do not use insecure randomness
            _testSourceData.Add(_random.Next(int.MaxValue / 4));
#pragma warning restore CA5394 // Do not use insecure randomness
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _testSourceData = null;
            TestOutputData = null;
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
