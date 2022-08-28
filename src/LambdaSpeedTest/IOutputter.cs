namespace LambdaSpeedTest;

internal interface IOutputter
{
    void StartProgress(string testName);

    void StepProgress();

    void StopProgress();

    void OutputRecordSet(IList<TestDataAverageResult> results);
}
