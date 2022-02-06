using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Lighthouse2.Highlights;
using Lighthouse2.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Lighthouse2.Core
{
    /// <summary>
    ///     Adorner places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class Adorner
    {
        private const double cornerRadius = 2.0;

        /// <summary>
        ///     The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        ///     Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        private Thickness tBlur = new(2, -3, 2, -3);
        private Thickness tNone = new(2, 0, 2, 0);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Adorner" /> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public Adorner(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            Helper.InitDefaults();
            Helper.Load();

            layer = view.GetAdornmentLayer("LighthouseHighlighter");

            this.view = view;
            this.view.LayoutChanged += OnLayoutChanged;
        }

        /// <summary>
        ///     Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks>
        ///     <para>This event is raised whenever the rendered text displayed in the <see cref="ITextView" /> changes.</para>
        ///     <para>
        ///         It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is
        ///         called or in response to text or classification changes).
        ///     </para>
        ///     <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                CreateVisuals(line);
            }
        }

        private void CreateVisuals(ITextViewLine line)
        {
            // Grab a reference to the lines in the current TextView
            IWpfTextViewLineCollection textViewLines = view.TextViewLines;
            int start = line.Start;
            int end = line.End;
            List<Geometry> geometries = new();

            var firstChars = Helper.keywordFormats.Keys.Select(k => k[0]).Distinct().ToArray();

            // ~~ Main Loop
            for (int i = start; i < end; i++)
            {
                if (firstChars.Contains(view.TextSnapshot[i]))
                {
                    foreach (var kvp in Helper.keywordFormats)
                    {
                        string keyword = kvp.Key.Trim();

                        if (view.TextSnapshot[i] == keyword[0] &&
                            i <= end - keyword.Length &&
                            view.TextSnapshot.GetText(i, keyword.Length) == keyword &&
                            Helper.escapes.Contains(Convert.ToChar(view.TextSnapshot.GetText(Math.Max(0, i - 1), 1))) &&
                            Helper.escapes.Contains(Convert.ToChar(view.TextSnapshot.GetText(i + keyword.Length, 1))))
                        {
                            SnapshotSpan span = new(view.TextSnapshot, Span.FromBounds(i, i + keyword.Length));

                            Geometry markerGeometry = textViewLines.GetMarkerGeometry(span, true, kvp.Value.Blur == BlurIntensity.None ? tNone : tBlur);

                            if (markerGeometry != null)
                            {
                                if (!geometries.Any(g => g.FillContainsWithDetail(markerGeometry) >
                                                         IntersectionDetail.Empty))
                                {
                                    geometries.Add(markerGeometry);
                                    AddMarker(span, markerGeometry, kvp.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddMarker(SnapshotSpan span, Geometry markerGeometry, HighlightTag ct)
        {
            Rectangle r = new()
            {
                Fill = new SolidColorBrush(ct.Color.ChangeAlpha(60)),
                RadiusX = cornerRadius,
                RadiusY = cornerRadius,
                Width = markerGeometry.Bounds.Width,
                Height = markerGeometry.Bounds.Height,
                Stroke = new SolidColorBrush(ct.Color.ChangeAlpha(100))
            };

            bool isLine = ct.IsLine();

            if (ct.IsUnder())
            {
                r.Height = 4.0;
            }

            if (isLine)
            {
                r.Width = view.ViewportWidth - markerGeometry.Bounds.Left;
            }


            if (ct.Blur != BlurIntensity.None)
            {
                r.Effect = new BlurEffect
                {
                    KernelType = KernelType.Gaussian,
                    RenderingBias = RenderingBias.Performance
                };


                switch (ct.Blur)
                {
                    case BlurIntensity.Low:
                        ((SolidColorBrush) r.Fill).Color.ChangeAlpha(80);
                        ((BlurEffect) r.Effect).Radius = isLine ? 2 : 4.0;
                        break;

                    case BlurIntensity.Medium:
                        ((SolidColorBrush) r.Fill).Color.ChangeAlpha(120);
                        ((BlurEffect) r.Effect).Radius = isLine ? 4 : 7.0;
                        break;

                    case BlurIntensity.High:
                        ((SolidColorBrush) r.Fill).Color.ChangeAlpha(170);
                        ((BlurEffect) r.Effect).Radius = isLine ? 6 : 11.0;
                        break;

                    case BlurIntensity.Ultra:
                        ((SolidColorBrush) r.Fill).Color.ChangeAlpha(255);
                        ((BlurEffect) r.Effect).Radius = isLine ? 8 : 20.0;
                        break;

                    default:
                    case BlurIntensity.None:
                        r.Effect = null;
                        break;
                }

                r.Stroke = null;
            }

            // Align the image with the top of the bounds of the text geometry
            Canvas.SetLeft(r, markerGeometry.Bounds.Left);
            if (ct.IsUnder())
            {
                Canvas.SetTop(r, markerGeometry.Bounds.Top + markerGeometry.Bounds.Height - 2);
            }
            else
            {
                Canvas.SetTop(r, markerGeometry.Bounds.Top);
            }

            layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, r, null);
        }
    }
}