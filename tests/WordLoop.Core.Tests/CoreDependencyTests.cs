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

    [Fact]
    public void CorePublicApi_DoesNotExposeSchedulingLibraryTypes()
    {
        var core = Assembly.Load("WordLoop.Core");
        const BindingFlags members = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        var exposed = core.GetExportedTypes()
            .SelectMany(type => new[] { type.BaseType }
                .Concat(type.GetInterfaces())
                .Concat(type.GetConstructors().SelectMany(c => c.GetParameters()).Select(p => p.ParameterType))
                .Concat(type.GetMethods(members).SelectMany(m => m.GetParameters().Select(p => p.ParameterType).Append(m.ReturnType)))
                .Concat(type.GetProperties(members).Select(p => p.PropertyType))
                .Concat(type.GetFields(members).Select(f => f.FieldType))
                .Where(t => t?.Assembly.GetName().Name == "FSRS.Core")
                .Select(t => $"{type.FullName} → {t!.FullName}"))
            .ToList();

        Assert.Empty(exposed);
    }
}
