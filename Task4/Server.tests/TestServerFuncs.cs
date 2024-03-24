using System.Net;

namespace SimpleFTP.Tests;

public class WorkspaceTest
{
    static ((IPAddress, int), string[])[] workspaces =
{
        ((IPAddress.Loopback, 135), new string[] { }),
        ((IPAddress.Parse("150.50.100.15"), 135), new string[] { "address", "150.50.100.15" }),
        ((IPAddress.Loopback, 1000), new string[] { "port", "1000" }),
        (
            (IPAddress.Parse("150.50.100.15"), 1000),
            new string[] { "address", "150.50.100.15", "port", "1000" }
        ),
    };

    [TestCaseSource(nameof(workspaces))]
    public void TestWorkspace(((IPAddress, int) expected, string[] args) testCase)
    {
        var workspace = new Workspace(testCase.args);
        Assert.That(workspace.ServerAddress, Is.EqualTo(testCase.expected.Item1));
        Assert.That(workspace.ServerPort, Is.EqualTo(testCase.expected.Item2));
    }
}