namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;
using Utils;
/// <summary> Internal class Program.</summary>
internal class Program
{
    /// <summary> The entry point of the program. </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>Main task.</returns>
    public static async Task Main(string[] args)
    {
        var workspace = new Workspace(args);
        var server = new TcpListener(workspace.ServerAddress, workspace.ServerPort);
        server.Start();
        while (true)
        {
            HandleClient(await server.AcceptTcpClientAsync());
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        using (client)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            while (client.Connected)
            {
                var response = await Option<string>
                    .From(await reader.ReadLineAsync())
                    .AndThen(Request.TryFrom)
                    .MapOr(GetAnswer, Response.UnknownRequest);

                Try<System.IO.IOException>.Call(() => writer.WriteLineAsync(response.ToString()));
            }
        }
    }

    private static async Task<Response> GetAnswer(Request request)
    {
        if (request is Request.List list)
        {
            var files = Try<System.IO.IOException>.Call(
                () => Directory.GetFiles(list.path).Select(x => (x, false))
            );
            if (!files.IsOk())
            {
                return new Response.Error(files.UnwrapErr().Message);
            }

            var directories = Try<System.IO.IOException>.Call(
                () => Directory.GetDirectories(list.path).Select(x => (x, true))
            );
            if (!directories.IsOk())
            {
                return new Response.Error(directories.UnwrapErr().Message);
            }

            return new Response.List(files.Unwrap().Concat(directories.Unwrap()).ToArray());
        }
        else if (request is Request.Get get)
        {
            var body = await Try<System.IO.IOException>.CallAsync(
                async () => await File.ReadAllTextAsync(get.path)
            );
            return body.Map(x => (Response)new Response.Get(x))
                .UnwrapOrElse(x => new Response.Error(x.Message));
        }
        else
        {
            return Response.UnknownRequest;
        }
    }
}

public abstract record Response
{
    /// <summary>
    /// The response to list the files and directories of a directory.
    /// </summary>
    /// <param name="list">The list of files and directories.</param>
    public sealed record List((string name, bool isDir)[] list) : Response
    {
        /// <summary>
        /// Tries to parse a response from a string.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The parsed response or <see cref="Option{List}.None"/> on error.</returns>
        public static Option<List> TryParse(string str) =>
            Option<(string name, bool isDir)>
                .Collect(
                    string.Join(string.Empty, str.ToArray().SkipWhile(x => x != ' ').Skip(1))
                        .Split(' ')
                        .Select((x, index) => (x, index))
                        .GroupBy(x => x.index / 2)
                        .Select(
                            entry =>
                                (
                                    name: entry.First().x,
                                    isDir: Try<System.InvalidOperationException>
                                        .Call(() => entry.Skip(1).First().x)
                                        .TryUnwrap()
                                        .AndThen(TryParseBool)
                                )
                        )
                        .Select(
                            ((string name, Option<bool> isDir) entry) =>
                                (Option<(string name, bool isDir)>)(
                                    entry.isDir.IsSome()
                                        ? (entry.name, entry.isDir.Unwrap())
                                        : Option<(string name, bool isDir)>.None
                                )
                        )
                        .ToArray()
                )
                .Map(x => new List(x.ToArray()));

        /// <inheritdoc cref="Response"/>
        public override string ToString() =>
            $"{this.list.Length} {string.Join(' ', this.list.Select(file => $"{file.name} {(file.isDir ? "true" : "false")}"))}";

        private static Option<bool> TryParseBool(string str) =>
            str == "true"
                ? true
                : str == "false"
                    ? false
                    : Option<bool>.None;
    }

    /// <summary>
    /// The response to get the content of a file.
    /// </summary>
    public sealed record Get(string data) : Response
    {
        /// <summary>
        /// Tries to parse a response from a stream.
        /// </summary>
        /// <param name="reader">The stream to parse.</param>
        /// <returns>The parsed response or <see cref="Option{Get}.None"/> on error.</returns>
        public static async Task<Option<Get>> TryParseAsync(StreamReader reader)
        {
            var length = await ParseIntAsync(reader);
            var buf = new char[length];

            var read = await reader.ReadBlockAsync(buf, 0, length);

            return read == length ? new Get(string.Join(string.Empty, buf)) : Option<Get>.None;
        }

        private static async Task<int> ParseIntAsync(StreamReader reader)
        {
            var buf = new char[1];
            var answer = 0;
            while (true)
            {
                await reader.ReadBlockAsync(buf, 0, 1);
                if (buf[0] < '0' || buf[0] > '9')
                {
                    break;
                }

                answer = (answer * 10) + buf[0] - '0';
            }

            return answer;
        }

        /// <inheritdoc cref="Response"/>
        public override string ToString() =>
            $"{this.data.Length} {string.Join(string.Empty, this.data.Select(x => (char)x))}";
    }

    /// <summary>
    /// The error response.
    /// </summary>
    public sealed record Error(string message) : Response
    {
        /// <inheritdoc cref="Response"/>
        public override string ToString() => $"Error: {this.message}";
    }

    /// <summary>
    /// The unknown request response. This is singleton class. Use <see cref="UnknownRequest"/>.
    /// </summary>
    public sealed record _UnknownRequest() : Response
    {
        /// <inheritdoc cref="Response"/>
        public override string ToString() => "Unknown request";
    }

    /// <summary>
    /// Gets the instance of <see cref="_UnknownRequest"/>.
    /// </summary>
    public static _UnknownRequest UnknownRequest { get; } = new _UnknownRequest();

    /// <summary>
    /// String representation of the response.
    /// </summary>
    /// <returns>The string representation of the request.</returns>
    public abstract override string ToString();

    private Response() { }
}

public abstract record Request
{
    /// <summary>
    /// The request to list the files and directories of a directory.
    /// </summary>
    public sealed record List(string path) : Request
    {
        /// <inheritdoc cref="Request"/>
        public override string ToString() => $"1 {this.path}";
    }

    /// <summary>
    /// The request to get the content of a file.
    /// </summary>
    public sealed record Get(string path) : Request
    {
        /// <inheritdoc cref="Request"/>
        public override string ToString() => $"2 {this.path}";
    }

    /// <summary>
    /// Tries to parse a request from a string.
    /// </summary>
    /// <param name="str">The string to parse.</param>
    /// <returns>The parsed request or <see cref="Option{Request}.None"/> on error.</returns>
    public static Option<Request> TryFrom(string str) =>
        str.Length > 2
            ? str[0] == '1'
                ? new List(str.Substring(2))
                : str[0] == '2'
                    ? new Get(str.Substring(2))
                    : Option<Request>.None
            : Option<Request>.None;

    /// <summary>
    /// String representation of the request.
    /// </summary>
    /// <returns>The string representation of the request.</returns>
    public abstract override string ToString();

    private Request() { }
}

/// <summary>
/// The workspace of the SimpleFTP server or client.
/// </summary>
public class Workspace
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Workspace"/> class from command line arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public Workspace(string[] args)
    {
        this.ServerAddress = IPAddress.Loopback;
        this.ServerPort = 135;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "address":
                    this.ServerAddress = IPAddress.Parse(args[++i]);
                    break;
                case "port":
                    this.ServerPort = int.Parse(args[++i]);
                    break;
            }
        }
    }

    /// <summary>
    /// Gets the address of the server.
    /// </summary>
    public IPAddress ServerAddress { get; }

    /// <summary>
    /// Gets the port of the server.
    /// </summary>
    public int ServerPort { get; }
}
