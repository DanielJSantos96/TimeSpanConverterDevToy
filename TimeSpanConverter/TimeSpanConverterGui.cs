using DevToys.Api;
using System.ComponentModel.Composition;
using static DevToys.Api.GUI;

namespace TimeSpanConverter;

[Export(typeof(IGuiTool))]
[Name("TimeSpanConverter")]
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",
    IconGlyph = '\uED82',
    GroupName = PredefinedCommonToolGroupNames.Converters,
    ResourceManagerAssemblyIdentifier = nameof(TimeSpanConverterExtensionResourceAssemblyIdentifier),
    ResourceManagerBaseName = "TimeSpanConverter.TimeSpanConverter",
    ShortDisplayTitleResourceName = nameof(TimeSpanConverter.ShortDisplayTitle),
    LongDisplayTitleResourceName = nameof(TimeSpanConverter.LongDisplayTitle),
    DescriptionResourceName = nameof(TimeSpanConverter.Description),
    AccessibleNameResourceName = nameof(TimeSpanConverter.AccessibleName))]
internal sealed class TimeSpanConverterGui : IGuiTool
{
    public UIToolView View => new(
        Stack()
            .Vertical()
            .WithChildren(
                new TimeSpanInput(
                    TimeSpanFormat.Days,
                    TimeSpanFormat.Hours,
                    TimeSpanFormat.Minutes,
                    TimeSpanFormat.Seconds).AsUIElement(),
                new TimeSpanInput(
                    TimeSpanFormat.Hours, 
                    TimeSpanFormat.Minutes, 
                    TimeSpanFormat.Seconds).AsUIElement(),
                new TimeSpanInput(TimeSpanFormat.Hours).AsUIElement(),
                new TimeSpanInput(TimeSpanFormat.Minutes).AsUIElement(),
                new TimeSpanInput(TimeSpanFormat.Seconds).AsUIElement(),
                new TimeSpanInput(TimeSpanFormat.Miliseconds).AsUIElement(),
                new TimeSpanInput(TimeSpanFormat.Nanoseconds).AsUIElement(),
                new TimeSpanInput(TimeSpanFormat.Ticks).AsUIElement(),
                new TimeSpanInput(TimeSpanFormat.Unix).AsUIElement()

            )
    );

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
    }

}
