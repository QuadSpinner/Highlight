using System;
using System.Windows.Media;

namespace Lighthouse.Utilities
{
    [Serializable]
    public class ColorTag
    {
        public string Criteria { get; set; }

        public Color ColorSwatch { get; set; }

        public bool isFullLine { get; set; }

        public bool isUnderline { get; set; }

        public bool isActive { get; set; } = true;

        public BlurIntensity Blur { get; set; } = BlurIntensity.None;
    }
}