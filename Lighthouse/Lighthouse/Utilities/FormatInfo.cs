using System.Windows.Media;

namespace Lighthouse.Utilities
{
    public class FormatInfo
    {
        public FormatInfo(Color backgroundColor, Color outlineColor, bool isFull = false, BlurIntensity blurred = BlurIntensity.None)
        {
            //if (backgroundColor.A > 0)
            //{
            Background = new SolidColorBrush(backgroundColor);
            Background.Freeze();

            //}
            //if (outlineColor.A > 0)
            //{
            Outline = new SolidColorBrush(outlineColor);
            Outline.Freeze();

            //}
            isFullLine = isFull;
            Blurred = blurred;
        }

        public Brush Background
        { get; }
        public Brush Outline
        { get; }
        public bool isFullLine
        { get; }
        public BlurIntensity Blurred
        { get; }
    }
}