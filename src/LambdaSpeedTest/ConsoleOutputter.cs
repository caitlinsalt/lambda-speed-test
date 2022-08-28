namespace LambdaSpeedTest;

public class ConsoleOutputter : QuietConsoleOutputter
{
    private bool _isRunning;
    private int _step;

    private readonly char[] _batonChars = new[] { '/', '-', '\\', '|' };

    public override void StartProgress(string testName)
    {
        if (_isRunning)
        {
            StopProgress();
        }
        Console.WriteLine($"Running test \"{testName}\"");
        _step = 0;
        Console.Write(_batonChars[_step]);
    }

    public override void StopProgress()
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

    public override void StepProgress()
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
}
