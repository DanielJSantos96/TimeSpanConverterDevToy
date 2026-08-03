using DevToys.Api;
using System.ComponentModel.Composition;

namespace TimeSpanConverter;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(TimeSpanConverterExtensionResourceAssemblyIdentifier))]
internal sealed class TimeSpanConverterExtensionResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        return ValueTask.FromResult(Array.Empty<FontDefinition>());
    }
}