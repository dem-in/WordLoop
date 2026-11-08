using System.Reflection;

namespace WordLoop.Core.Tests;

public class CoreDependencyTests
{
    private static readonly string[] ForbiddenAssemblyPrefixes =
    [
        "Microsoft.AspNetCore",
        "Microsoft.JSInterop",
        "Microsoft.Extensions.Http",
    ];

    [Fact]
    public void CoreAssembly_DoesNotReferenceUiBrowserOrHttpAssemblies()
    {
        var core = Assembly.Load("WordLoop.Core");

        var forbidden = core.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(name => ForbiddenAssemblyPrefixes.Any(prefix =>
                name == prefix || name.StartsWith(prefix + ".", StringComparison.Ordinal)))
            .ToList();

        Assert.Empty(forbidden);
    }
}
