using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Jellyfin.Plugin.RealtimeAmbilight;
using Xunit;

namespace Jellyfin.Plugin.RealtimeAmbilight.Tests.Hue;

public class HueChannelBrightnessSerializationTests
{
    [Fact]
    public void PluginConfigurationXmlRoundTripPreservesAreaAndChannelBrightness()
    {
        var original = new PluginConfiguration
        {
            HueChannelBrightnessEntries =
            [
                new() { EntertainmentConfigurationId = "area-a", ChannelId = 0, BrightnessPercent = 25 },
                new() { EntertainmentConfigurationId = "area-a", ChannelId = 1, BrightnessPercent = 75 },
                new() { EntertainmentConfigurationId = "area-b", ChannelId = 0, BrightnessPercent = 50 }
            ]
        };
        var serializer = new XmlSerializer(typeof(PluginConfiguration));
        using var writer = new StringWriter();
        serializer.Serialize(writer, original);
        using var reader = new StringReader(writer.ToString());
        using var xmlReader = XmlReader.Create(reader, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        });
        var restored = Assert.IsType<PluginConfiguration>(serializer.Deserialize(xmlReader));
        Assert.Equal(3, restored.HueChannelBrightnessEntries.Count);
        Assert.Equal(25, restored.HueChannelBrightnessEntries[0].BrightnessPercent);
        Assert.Equal(75, restored.HueChannelBrightnessEntries[1].BrightnessPercent);
        Assert.Equal("area-b", restored.HueChannelBrightnessEntries[2].EntertainmentConfigurationId);
        Assert.Equal(50, restored.HueChannelBrightnessEntries[2].BrightnessPercent);
    }

    [Fact]
    public void DefaultPluginConfigurationCanBeSerialized()
    {
        var serializer = new XmlSerializer(typeof(PluginConfiguration));
        using var writer = new StringWriter();
        serializer.Serialize(writer, new PluginConfiguration());
    }
}
