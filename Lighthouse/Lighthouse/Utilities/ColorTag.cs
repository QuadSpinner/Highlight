using System;
using System.ComponentModel;
using System.Windows.Media;
using PropertyTools.DataAnnotations;

namespace Lighthouse.Utilities
{
    [Serializable]
    [System.ComponentModel.DisplayName("Color Tag")]
    public class ColorTag
    {
        [Resettable]
        [HorizontalAlignment(HorizontalAlignment.Left)]
        public string Criteria { get; set; }

        [Resettable]
        [System.ComponentModel.DisplayName("Color")]
        public Color ColorSwatch { get; set; }

        [Resettable]
        [DefaultValue(false)]
        [System.ComponentModel.DisplayName("Line")]
        public bool isFullLine { get; set; }

        [System.ComponentModel.DisplayName("Active")]
        [Resettable]
        [DefaultValue(true)]
        public bool isActive { get; set; } = true;

        [Resettable]
        [DefaultValue(0)]
        public BlurIntensity Blur { get; set; } = BlurIntensity.None;
    }
}