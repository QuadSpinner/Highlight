using System.Windows.Media;

namespace Lighthouse2.Highlights
{
    public class HighlightTag
    {
        public string Criteria { get; set; }
        public Color Color { get; set; }
        public TagShape Shape { get; set; } = TagShape.Tag;
        public BlurIntensity Blur { get; set; } = BlurIntensity.None;
        public bool IsActive { get; set; } = true;

        public bool IsUnder() => Shape is TagShape.LineUnder or TagShape.TagUnder;
        public bool IsLine() => Shape is TagShape.Line or TagShape.LineUnder;

        public override string ToString() => Criteria;
    }


    [Serializable]
    public enum TagShape
    {
        Tag,
        TagUnder,
        Line,
        LineUnder
    }


    [Serializable]
    public enum CornerStyle
    {
        Square = 0,
        RoundedCorner = 2,
        Soft = 6
    }

    [Serializable]
    public enum BlurType
    {
        NoBlur = 0,
        BlurAll = 1,
        Selective = 2
    }

    [Serializable]
    public enum BlurIntensity
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Ultra = 4
    }
}