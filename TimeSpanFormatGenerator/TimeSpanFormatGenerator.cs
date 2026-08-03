using Domain;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace TimeSpanFormatGenerator;

[Generator]
public class TimeSpanFormatGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var formats = context.CompilationProvider.Select(static (comp, _) =>
            comp.GetTypeByMetadataName($"{nameof(Domain)}.{nameof(TimeSpanFormats)}")
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public
                && p.IsStatic
                && p.Type.Name == nameof(String))
            .Select(p => p.Name).ToImmutableArray()
        );
        context.RegisterSourceOutput(formats, (ctx, formats) =>
        {
       
            ctx.AddSource("TimeSpanFormat.g.cs", $@"
using {nameof(Domain)};
namespace TimeSpanConverter;

internal enum TimeSpanFormat 
{{
    {string.Join(",\n\t", formats)}
}}

internal static partial class TimeSpanFormatExtensions 
{{
    public static string LocalizeName(this TimeSpanFormat format) =>
        format switch
        {{
            {string.Join(",\n\t\t\t", formats.Select(f => $"TimeSpanFormat.{f} => {nameof(TimeSpanFormats)}.{f}"))}
        }};
}}
            ");
        });
    }
}
