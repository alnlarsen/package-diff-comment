using OpenTap.Cli;
using PackageDiff;

namespace PackageDiffComment;

public class DiffAction : ICliAction
{
    [CommandLineArgument("package")]
    public string Package { get; set; }
    [CommandLineArgument("token")]
    public string Token { get; set; }

    public int Execute(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Package = {Package}");
        Publisher.Token = Token;
        return new PackageDiffAction() { Package = Package }.Execute(cancellationToken);
    }
}