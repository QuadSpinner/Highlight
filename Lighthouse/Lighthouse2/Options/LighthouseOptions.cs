using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Lighthouse2
{
    [ComVisible(true)]
    internal class OptionsProvider
    {
        [ComVisible(true)]
        public class LighthouseOptions : BaseOptionPage<Options>
        {
        }
    }

    [ComVisible(true)]
    public class Options : BaseOptionModel<Options>
    {

        public Options()
        {
            Debug.WriteLine("LOADED!");
        }

        [Category("Tags")]
        [DisplayName("Global Rules")]
        [Description("These rules are applied across all projects.")]
        public HighlightTag[] ColorTags { get; set; } = null;  
        
        [Category("Tags")]
        [DisplayName("Solution Rules")]
        [Description("These rules are applied only to the current solution.")]
        public HighlightTag[] SolutionTags { get; set; } = null;


        [Category("Appearance")]
        [DisplayName("Performance")]
        [Description("Choose the performance level.")]
        [DefaultValue(Performance.Normal)]
        [TypeConverter(typeof(EnumConverter))]
        public Performance Performance { get; set; } = Performance.Normal;

    }
    
}