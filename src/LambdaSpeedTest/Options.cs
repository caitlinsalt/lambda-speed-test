using CommandLine;

namespace LambdaSpeedTest;

public class Options
{
    [Option('c', "count", Default = 1, Required = false, HelpText = "Number of times to repeat each test and average the results over.")]
    public int Count { get; set; }
}
