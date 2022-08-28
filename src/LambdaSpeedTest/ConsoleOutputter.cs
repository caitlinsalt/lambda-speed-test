using System.Globalization;

namespace LambdaSpeedTest;

public class ConsoleOutputter : IOutputter
{
    private bool _isRunning;
    private int _step;

    private readonly char[] _batonChars = new[] { '/', '-', '\\', '|' };

    public void StartProgress(string testName)
    {
        if (_isRunning)
        {
            StopProgress();
        }
        Console.WriteLine($"Running test \"{testName}\"");
        _step = 0;
        Console.Write(_batonChars[_step]);
    }

    public void StopProgress()
    {
        // Assume the last thing output was a baton character - overwrite it with a space then reset the position, 
        // unless the cursor is already on the left edge.
        var position = Console.GetCursorPosition();
        if (position.Left > 0)
        {
            Console.SetCursorPosition(position.Left - 1, position.Top);
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.Write(" ");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            Console.SetCursorPosition(position.Left - 1, position.Top);
        }
        _isRunning = false;
    }

    public void StepProgress()
    {
        _step = (_step + 1) % _batonChars.Length;
        // Assume the last thing output was a baton character, unless the cursor is already on the left edge.
        var position = Console.GetCursorPosition();
        if (position.Left > 0)
        {
            Console.SetCursorPosition(position.Left - 1, position.Top);
        }
        Console.Write(_batonChars[_step]);
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
