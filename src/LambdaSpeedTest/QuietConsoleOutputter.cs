using System.Globalization;

namespace LambdaSpeedTest;

public class QuietConsoleOutputter : IOutputter
{
    public virtual void StartProgress(string testName)
    {
        Console.WriteLine($"Running test \"{testName}\"");
    }

    public virtual void StepProgress()
    {
    }

    public virtual void StopProgress()
    {
    }

    public void OutputRecordSet(IList<TestDataAverageResult> results)
    {
        if (results is null)
        {
            throw new ArgumentNullException(nameof(results));
        }

        string[] headings = new[] { "Size", "Time (ticks)", "Time (ms)" };
        string integerNumberFormat = "n0";
        string decimalNumberFormat = "n1";
        int[] widths = headings.Select((h, i) => GetMaxWidth(results, GetPropertySelector(i), h.Length, integerNumberFormat)).ToArray();
        string hrule = "+" + string.Join("+", widths.Select(static (w) => new string('-', w + 2))) + "+";
        string lineFormat = $"| {{0,{widths[0]}:{integerNumberFormat}}} | {{1,{widths[1]}:{integerNumberFormat}}} | {{2,{widths[2]}:{decimalNumberFormat}}} |";
        Console.WriteLine(hrule);
        Console.WriteLine(string.Format(CultureInfo.CurrentCulture, lineFormat, headings[0], headings[1], headings[2]));
        Console.WriteLine(hrule);
        foreach (TestDataAverageResult record in results)
        {
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, lineFormat, record.Size, record.ElapsedTime, record.ElapsedTimeMs));
        }
        Console.WriteLine(hrule);
        Console.WriteLine("");
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
