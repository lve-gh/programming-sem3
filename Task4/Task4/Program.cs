namespace SimpleFTP.Client;

using System.Net.Sockets;
using System.Text.RegularExpressions;
using Utils;
/// <summary> Internal class Program.</summary>
internal class Program
{
    private const string message =
        "Commands:\n"
        + "list <path>\n"
        + "get <path>\n"
        + "quit\n";

    /// <summary> The entry point of the program. </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>Main task.</returns>
    public static async Task Main(string[] args)
    {
        Console.WriteLine(message);
        var workspace = new Workspace(args);
        var client = new TcpClient();
        await client.ConnectAsync(workspace.ServerAddress, workspace.ServerPort);
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        while (true)
        {
            switch (Option<string>.From(Console.ReadLine()).Map(Command.TryParse))
            {
                case Option<Option<Command>>.Some(Option<Command>.Some(Command.List(var path))):
                    {
                        await writer.WriteLineAsync(new Request.List(path).ToString());
                        await reader.ReadToEndAsync();
                        Console.WriteLine(path);
                        var response = Option<string>
                            .From(await reader.ReadLineAsync())
                            .AndThen(Response.List.TryParse);
                        if (response is Option<Response.List>.Some(var list))
                        {
                            Console.WriteLine(
                                string.Join(
                                    '\n',
                                    list.list.Select(
                                        file => $"{file.name} - {(file.isDir ? "directory" : "file")}"
                                    )
                                )
                            );
                        }
                        else
                        {
                            Console.WriteLine("Invalid response");
                        }

                        break;
                    }

                case Option<Option<Command>>.Some(Option<Command>.Some(Command.Get(var path))):
                    {
                        await writer.WriteLineAsync(new Request.Get(path).ToString());
                        if (await Response.Get.TryParseAsync(reader) is Option<Response.Get>.Some(var get))
                        {
                            await File.WriteAllTextAsync(
                                new Regex(@"([^/]+/)*(.+)").Match(path).Groups[2].Value,
                                get.data
                            );
                            Console.WriteLine("Saved");
                        }
                        else
                        {
                            Console.WriteLine("Invalid response");
                        }

                        break;
                    }

                case Option<Option<Command>>.Some(Option<Command>.Some(Command.Quit)):
                    return;
            }
        }
    }
}

public record Command
{
    /// <summary>
    /// The command to list the files and directories of a directory.
    /// </summary>
    public sealed record List(string path) : Command;

    /// <summary>
    /// The command to get the content of a file.
    /// </summary>
    public sealed record Get(string path) : Command;

    /// <summary>
    /// The command to quit the program.
    /// </summary>
    public sealed record Quit() : Command;

    /// <summary>
    /// Tries to parse a command from a string.
    /// </summary>
    /// <param name="str">The string to parse.</param>
    /// <returns>The parsed command or <see cref="Option{Command}.None"/> on error.</returns>
    public static Option<Command> TryParse(string str)
    {
        var match = new Regex(@"^(\w{1,4})( (\S+))?$").Match(str);

        var command = match.Groups[1].Value;

        switch (command)
        {
            case "list":
                return new List(match.Groups[3].Value);
            case "get":
                return new Get(match.Groups[3].Value);
            case "quit":
                return new Quit();
            default:
                return Option<Command>.None;
        }
    }

    private Command() { }
}
