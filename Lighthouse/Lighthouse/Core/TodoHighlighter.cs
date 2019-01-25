using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Lighthouse.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Lighthouse.Core
{
    [SuppressMessage("ReSharper", "ClassCanBeSealed.Local")]
    public class TodoHighlighter
    {
        //private Button bT = new Button();
        //private bool isOnCanvas;

        private IAdornmentLayer adornmentLayer;
        private BlurType isBlurred = BlurType.Selective;
        private IWpfTextView textView;

        public TodoHighlighter(IWpfTextView view)
        {
            //Init();
            if (Lighthouse.Options == null)
                Lighthouse.Init();

            textView = view;
            adornmentLayer = view.GetAdornmentLayer("LighthouseHighlighter");

            textView.LayoutChanged += OnLayoutChanged;

            if (!File.Exists(LighthouseOptions.localPath + LighthouseOptions.settingsFile))
            {
                Lighthouse.Init();
                Lighthouse.Options.SaveSettingsToStorage();
            }

            FileSystemWatcher fsw = new FileSystemWatcher(LighthouseOptions.localPath, LighthouseOptions.settingsFile);
            fsw.Changed += Fsw_Changed;
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            Lighthouse.Init();

            adornmentLayer.RemoveAllAdornments();

            foreach (ITextViewLine line in textView.TextViewLines.Where(x => x.VisibilityState == VisibilityState.FullyVisible))
            {
                CreateVisuals(line);
            }
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs args)
        {
            if (Lighthouse.Options == null)
            {
                Lighthouse.Init();
            }

            if (!Lighthouse.IsEnabled)
                return;

            try
            {
                foreach (ITextViewLine line in args.NewOrReformattedLines)
                {
                    CreateVisuals(line);
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        private void CreateVisuals(ITextViewLine line)
        {
            // Grab a reference to the lines in the current TextView
            IWpfTextViewLineCollection textViewLines = textView.TextViewLines;
            int start = line.Start;
            int end = line.End;
            List<Geometry> geometries = new List<Geometry>();

            var keywordFirstLetters = Lighthouse.keywordFormats.Keys
                                                    .Select(k => k[0])
                                                    .Distinct()
                                                    .ToArray();
            char[] escapes =
            {
                ' ',
                '!',
                '"',
                '@',
                '$',
                '(',
                ')',
                '[',
                ']',
                '*',
                '-',
                '.',
                '/',
                '>',
                '<',
                '"',
                ':',
                ';',
                ',',
                '?',
                '\'',
                '\n',
                '\r',
                '\t',
                '='
            };
            isBlurred = Lighthouse.Options.Blurred;

            // ~~ Main Loop
            for (int i = start; i < end; i++)
            {
                if (keywordFirstLetters.Contains(textView.TextSnapshot[i]))
                {
                    foreach (var kvp in Lighthouse.keywordFormats)
                    {
                        string keyword = kvp.Key.Trim();

                        if (textView.TextSnapshot[i] == keyword[0] &&
                            i <= end - keyword.Length &&
                            textView.TextSnapshot.GetText(i, keyword.Length) == keyword &&
                            escapes.Contains(Convert.ToChar(textView.TextSnapshot.GetText(i - 1, 1))) &&
                            escapes.Contains(Convert.ToChar(textView.TextSnapshot.GetText(i + keyword.Length, 1)))
                        )
                        {
                            SnapshotSpan span =
                                new SnapshotSpan(textView.TextSnapshot, Span.FromBounds(i, i + keyword.Length));

                            Thickness t = new Thickness(2, 0, 2, 0);

                            switch (isBlurred)
                            {
                                case BlurType.BlurAll:
                                    t = new Thickness(2, -3, 2, -3);
                                    
                                    break;

                                case BlurType.Selective:
                                    t = new Thickness(2,
                                                      kvp.Value.Blurred != BlurIntensity.None ? -3 : 0,
                                                      2,
                                                      kvp.Value.Blurred != BlurIntensity.None ? -3 : 0);
                                    break;
                            }

                            Geometry markerGeometry = textViewLines.GetMarkerGeometry(span, true, t);
                            if (markerGeometry != null)
                            {
                                if (!geometries.Any(g => g.FillContainsWithDetail(markerGeometry) >
                                                         IntersectionDetail.Empty))
                                {
                                    geometries.Add(markerGeometry);
                                    AddMarker(span, 
                                              markerGeometry, 
                                              kvp.Value, 
                                              kvp.Value.isFullLine, 
                                              isBlurred == BlurType.BlurAll 
                                                  ? Lighthouse.Options.Blur
                                                  : kvp.Value.Blurred);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddMarker(SnapshotSpan span,
                               Geometry markerGeometry,
                               FormatInfo formatInfo,
                               bool isFullLine = false,
                               BlurIntensity Blurred = BlurIntensity.None)
        {
            double cornerRadius;

            try
            {
                switch (Lighthouse.Options.HighlightCorner)
                {
                    case CornerStyle.Square:
                        cornerRadius = 0.0;
                        break;

                    case CornerStyle.RoundedCorner:
                        cornerRadius = 2.0;
                        break;

                    case CornerStyle.Soft:
                        cornerRadius = 6.0;
                        break;

                    default:
                        cornerRadius = 2.0;
                        break;
                }
            }
            catch (Exception)
            {
                cornerRadius = 2.0;
            }

            Rectangle r = new Rectangle
            {
                Fill = formatInfo.Background,
                RadiusX = cornerRadius,
                RadiusY = cornerRadius,
                Width = markerGeometry.Bounds.Width,
                Height = markerGeometry.Bounds.Height,
                Stroke = formatInfo.Outline
            };

            if (isFullLine)
            {
                r.Width = textView.ViewportWidth - markerGeometry.Bounds.Left;
            }

            if (isBlurred != BlurType.NoBlur)
            {
                if (isBlurred == BlurType.BlurAll)
                    Blurred = Lighthouse.Options.Blur;

                if (Blurred != BlurIntensity.None)
                {
                    r.Effect = new BlurEffect
                    {
                        KernelType = KernelType.Gaussian,
                        RenderingBias = RenderingBias.Performance
                    };

                    switch (Blurred)
                    {
                        case BlurIntensity.Low:
                            ((SolidColorBrush)r.Fill).Color.ChangeAlpha(80);
                            ((BlurEffect)r.Effect).Radius = 4.0;
                            break;

                        case BlurIntensity.Medium:
                            ((SolidColorBrush)r.Fill).Color.ChangeAlpha(120);
                            ((BlurEffect)r.Effect).Radius = 7.0;
                            break;

                        case BlurIntensity.High:
                            ((SolidColorBrush)r.Fill).Color.ChangeAlpha(170);
                            ((BlurEffect)r.Effect).Radius = 11.0;
                            break;

                        case BlurIntensity.Ultra:
                            ((SolidColorBrush)r.Fill).Color.ChangeAlpha(255);
                            ((BlurEffect)r.Effect).Radius = 20.0;
                            break;

                        case BlurIntensity.None:
                            r.Effect = null;
                            break;

                    }

                    r.Stroke = null;
                }
            }

            // Align the image with the top of the bounds of the text geometry
            Canvas.SetLeft(r, markerGeometry.Bounds.Left);
            Canvas.SetTop(r, markerGeometry.Bounds.Top);

            adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, r, null);
        }
    }
}