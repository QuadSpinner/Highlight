using System.Windows.Media;
using Lighthouse2.Utilities;

namespace Lighthouse2
{
    public class HighlightTag
    {
        public HighlightTag(string criteria)
        {
            Criteria = criteria;
        }

        public string Criteria { get; set; }
        public Color Color { get; set; } = Helper.GetRandomColor();
        public TagShape Shape { get; set; } = TagShape.Tag;
        public BlurIntensity Blur { get; set; } = BlurIntensity.None;
        public bool IsActive { get; set; } = true;

        public bool IsUnder() => Shape is TagShape.LineUnder or TagShape.TagUnder;

        public bool IsLine() => Shape is TagShape.Line or TagShape.LineUnder;

        public override string ToString() => Criteria;
    }


}