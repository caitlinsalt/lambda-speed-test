namespace LambdaSpeedTest;

public record TestDataResult(int Size, long ElapsedTime)
{
    public double ElapsedTimeMs => ElapsedTime / 10_000d;
}
