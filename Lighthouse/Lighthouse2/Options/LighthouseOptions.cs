using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Lighthouse2
{
    [ComVisible(true)]
    internal class OptionsProvider
    {
        [ComVisible(true)]
        public class LightHouseOptionsOptions : BaseOptionPage<LightHouseOptions>
        {
            
        }
    }

    [ComVisible(true)]
    public class LightHouseOptions : BaseOptionModel<LightHouseOptions>
    {
        [XmlIgnore] internal bool NeedsUpdate = false;

        [Category("Highlighting")]
        [DisplayName("Highlights")]
        public HighlightTag[] ColorTags { get; set; } = null;


        [Category("Appearance")]
        [DisplayName("Performance")]
        [Description("Choose the performance level.")]
        [DefaultValue(Performance.Normal)]
        [TypeConverter(typeof(EnumConverter))]
        public Performance Performance { get; set; } = Performance.Normal;

    }

    public enum Performance
    {
        Normal,
        Fast,
        NoEffects
    }
}