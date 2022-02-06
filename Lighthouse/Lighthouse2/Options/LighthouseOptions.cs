using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Lighthouse2.Highlights;
using Lighthouse2.Utilities;

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
        public const string settingsFile = "Lighthouse2.xml";
        public const string tagsFile = "CommonTags2.xml";

        public static readonly string localPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\QuadSpinner\\VSX\\";

        
        public LightHouseOptions()
        {
            Directory.CreateDirectory(localPath);
        }

        [Category("Highlighting")]
        [DisplayName("Highlights")]
        public HighlightTag[] ColorTags { get; set; } = null;


        [Category("Appearance")]
        [DisplayName("Allow Blur / Effects")]
        [Description("Allow blurring and other effects. Disable for performance, or if you are using lower-end hardware or VM.")]
        [DefaultValue(true)]
        public bool AllowBlurEffects { get; set; } = true;

        //public void SaveSettingsToStorage()
        //{
        //    Directory.CreateDirectory(localPath);
        //    Helper.SaveTagsToFile(localPath + tagsFile, ColorTags);
        //    Helper.SaveSettingsToFile(localPath + settingsFile, this);
        //}

        //public void LoadSettingsFromStorage()
        //{
        //    try
        //    {
        //        ColorTags = Helper.LoadTagsFromFile(localPath + tagsFile);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}