namespace SimpleFTP.Client.Tests;

public class CommandTests
{
    static (Command, string)[] commands =
    {
        (new Command.List("path"), "list path"),
        (new Command.Get("file"), "get file"),
        (new Command.Quit(), "quit"),
    };

    [TestCaseSource(nameof(commands))]
    public void TestParse((Command command, string expected) testCase)
    {
        var command = Command.TryParse(testCase.expected);
        Assert.That(command, Is.TypeOf<Utils.Option<Command>.Some>());
        var cmd = command.Unwrap();
        Assert.That(cmd, Is.EqualTo(testCase.command));
    }
}