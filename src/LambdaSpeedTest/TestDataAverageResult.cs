using System.Collections.Immutable;

namespace LambdaSpeedTest;

internal record TestDataAverageResult(int Size)
{
    private readonly List<TestDataResult> _results = new();

    public IList<TestDataResult> Results => _results.ToImmutableArray();

    public double ElapsedTime => _results.Select(x => x.ElapsedTime).Average();

    public double ElapsedTimeMs => ElapsedTime / 10_000;

    public void Add(TestDataResult result)
    {
        _results.Add(result);
    }
}
