using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Media;

namespace Lighthouse.Utilities
{
    // --------------------------------------------------------------------------------------------------------------------
    // The MIT License (MIT)
    // 
    // Copyright (c) 2012 Oystein Bjorke
    // 
    // Permission is hereby granted, free of charge, to any person obtaining a copy of this software
    // and associated documentation files (the "Software"), to deal in the Software without
    // restriction, including without limitation the rights to use, copy, modify, merge, publish,
    // distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
    // Software is furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in all copies or
    // substantial portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
    // BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
    // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. --------------------------------------------------------------------------------------------------------------------

    public static class ColorHelper
    {
        static ColorHelper()
        {
            Automatic = Color.FromArgb(0, 0, 0, 1);
            UndefinedColor = Color.FromArgb(0, 0, 0, 0);
        }

        public static Color Automatic
        { get; private set; }

        public static Color UndefinedColor
        { get; }

        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
        public static Color ChangeAlpha(this Color c, byte alpha)
        {
            return Color.FromArgb(alpha, c.R, c.G, c.B);
        }

        public static Color CmykToColor(double c, double m, double y, double k)
        {
            double r = 1 - (c / 100) * (1 - (k / 100)) - (k / 100);
            double g = 1 - (m / 100) * (1 - (k / 100)) - (k / 100);
            double b = 1 - (y / 100) * (1 - (k / 100)) - (k / 100);
            return Color.FromRgb((byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
        }

        public static double ColorDifference(Color c1, Color c2)
        {
            // http: //en.wikipedia.org/wiki/Color_difference
            // http: //mathworld.wolfram.com/L2-Norm.html
            double dr = (c1.R - c2.R) / 255.0;
            double dg = (c1.G - c2.G) / 255.0;
            double db = (c1.B - c2.B) / 255.0;
            double da = (c1.A - c2.A) / 255.0;
            double e = dr * dr + dg * dg + db * db + da * da;
            return Math.Sqrt(e);
        }

        public static double[] ColorToCmyk(byte r, byte g, byte b)
        {
            if (r == 0 && g == 0 && b == 0)
            {
                return new[] { 0, 0, 0, 1.0 };
            }
            double computedC = 1 - (r / 255.0);
            double computedM = 1 - (g / 255.0);
            double computedY = 1 - (b / 255.0);
            var min = Math.Min(computedC, Math.Min(computedM, computedY));
            computedC = (computedC - min) / (1 - min);
            computedM = (computedM - min) / (1 - min);
            computedY = (computedY - min) / (1 - min);
            double computedK = min;
            return new[] { computedC, computedM, computedY, computedK };
        }

        public static string ColorToHex(this Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static double[] ColorToHsv(this Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            double h = 0;
            double s;
            double min = Math.Min(Math.Min(r, g), b);
            double v = Math.Max(Math.Max(r, g), b);
            double delta = v - min;
            if (v == 0.0)
            {
                s = 0;
            }
            else
            {
                s = delta / v;
            }
            if (s == 0)
            {
                h = 0.0;
            }
            else
            {
                if (r == v)
                {
                    h = (g - b) / delta;
                }
                else if (g == v)
                {
                    h = 2 + (b - r) / delta;
                }
                else if (b == v)
                {
                    h = 4 + (r - g) / delta;
                }
                h *= 60;
                if (h < 0.0)
                {
                    h = h + 360;
                }
            }
            var hsv = new double[3];
            hsv[0] = h / 360.0;
            hsv[1] = s;
            hsv[2] = v / 255.0;
            return hsv;
        }

        public static byte[] ColorToHsvBytes(this Color color)
        {
            double[] hsv1 = ColorToHsv(color);
            var hsv2 = new byte[3];
            hsv2[0] = (byte)(hsv1[0] * 255);
            hsv2[1] = (byte)(hsv1[1] * 255);
            hsv2[2] = (byte)(hsv1[2] * 255);
            return hsv2;
        }

        public static uint ColorToUint(this Color c)
        {
            uint u = (uint)c.A << 24;
            u += (uint)c.R << 16;
            u += (uint)c.G << 8;
            u += c.B;
            return u;
        }

        public static Color Complementary(this Color c)
        {
            // http: //en.wikipedia.org/wiki/Complementary_color
            double[] hsv = ColorToHsv(c);
            double newHue = hsv[0] - 0.5;

            // clamp to [0,1]
            if (newHue < 0)
            {
                newHue += 1.0;
            }
            return HsvToColor(newHue, hsv[1], hsv[2]);
        }

        public static Color[] GetSpectrumColors(int colorCount)
        {
            var spectrumColors = new Color[colorCount];
            for (int i = 0; i < colorCount; ++i)
            {
                double hue = (i * 1.0) / (colorCount - 1);
                spectrumColors[i] = HsvToColor(hue, 1.0, 1.0);
            }
            return spectrumColors;
        }

        public static List<Color> SetupColors(this List<Color> colors)
        {
            colors.Add(HexToColor("#e4603c"));
            colors.Add(HexToColor("#efa48f"));
            colors.Add(HexToColor("#e4823c"));
            colors.Add(HexToColor("#efb78f"));
            colors.Add(HexToColor("#e4943c"));
            colors.Add(HexToColor("#efc28f"));
            colors.Add(HexToColor("#e4a83c"));
            colors.Add(HexToColor("#efcd8f"));
            colors.Add(HexToColor("#e4be3c"));
            colors.Add(HexToColor("#efda8f"));
            colors.Add(HexToColor("#e4d43c"));
            colors.Add(HexToColor("#efe68f"));
            colors.Add(HexToColor("#dde43c"));
            colors.Add(HexToColor("#ebef8f"));
            colors.Add(HexToColor("#bde43d"));
            colors.Add(HexToColor("#d9ef8f"));
            colors.Add(HexToColor("#8ce43c"));
            colors.Add(HexToColor("#bdef8f"));
            colors.Add(HexToColor("#4ee43c"));
            colors.Add(HexToColor("#99f08f"));
            colors.Add(HexToColor("#3de4a5"));
            colors.Add(HexToColor("#8fefcb"));
            colors.Add(HexToColor("#3dbde3"));
            colors.Add(HexToColor("#8fd9ef"));
            colors.Add(HexToColor("#3ca1e4"));
            colors.Add(HexToColor("#8fc9f0"));
            colors.Add(HexToColor("#3d7be3"));
            colors.Add(HexToColor("#8fb3ef"));
            colors.Add(HexToColor("#3c50e4"));
            colors.Add(HexToColor("#8f9bef"));
            colors.Add(HexToColor("#6d3ce4"));
            colors.Add(HexToColor("#ab8ff0"));
            colors.Add(HexToColor("#b53de4"));
            colors.Add(HexToColor("#d58fef"));
            colors.Add(HexToColor("#e43cc6"));
            colors.Add(HexToColor("#ef8fde"));
            colors.Add(HexToColor("#e43c80"));
            colors.Add(HexToColor("#ef8fb6"));
            colors.Add(HexToColor("#e43d41"));
            colors.Add(HexToColor("#ef8f92"));

            // <!-- /// -->
            colors.Add(HexToColor("#00a9fa"));
            colors.Add(HexToColor("#00bda4"));
            colors.Add(HexToColor("#0f0f0f"));
            colors.Add(HexToColor("#1683b1"));
            colors.Add(HexToColor("#20ac9a"));
            colors.Add(HexToColor("#218a7c"));
            colors.Add(HexToColor("#2a7172"));
            colors.Add(HexToColor("#363636"));
            colors.Add(HexToColor("#376c99"));
            colors.Add(HexToColor("#4a5c9c"));
            colors.Add(HexToColor("#4d7e63"));
            colors.Add(HexToColor("#4d8f6c"));
            colors.Add(HexToColor("#52a779"));
            colors.Add(HexToColor("#544b59"));
            colors.Add(HexToColor("#575276"));
            colors.Add(HexToColor("#589ccc"));
            colors.Add(HexToColor("#635a96"));
            colors.Add(HexToColor("#6a6a6a"));
            colors.Add(HexToColor("#6db570"));
            colors.Add(HexToColor("#6e8a59"));
            colors.Add(HexToColor("#86b065"));
            colors.Add(HexToColor("#905986"));
            colors.Add(HexToColor("#9482a9"));
            colors.Add(HexToColor("#949494"));
            colors.Add(HexToColor("#958e60"));
            colors.Add(HexToColor("#977723"));
            colors.Add(HexToColor("#97d9ad"));
            colors.Add(HexToColor("#98a1a9"));
            colors.Add(HexToColor("#9b576d"));
            colors.Add(HexToColor("#9b6657"));
            colors.Add(HexToColor("#a5b1d9"));
            colors.Add(HexToColor("#ad9559"));
            colors.Add(HexToColor("#afafaf"));
            colors.Add(HexToColor("#b0d6e0"));
            colors.Add(HexToColor("#bc5e8d"));
            colors.Add(HexToColor("#c96395"));
            colors.Add(HexToColor("#cc9863"));
            colors.Add(HexToColor("#ccb4d6"));
            colors.Add(HexToColor("#cdaef1"));
            colors.Add(HexToColor("#cdc172"));
            colors.Add(HexToColor("#d6d6d6"));
            colors.Add(HexToColor("#dbba6a"));
            colors.Add(HexToColor("#df6475"));
            colors.Add(HexToColor("#e89e94"));
            colors.Add(HexToColor("#f1f1f1"));
            colors.Add(HexToColor("#f4d6a3"));
            colors.Add(HexToColor("#f6d973"));
            colors.Add(HexToColor("#fefefe"));

            return colors;
        }

        public static Color HexToColor(string value)
        {
            value = value.Trim('#');
            if (value.Length == 0)
            {
                return UndefinedColor;
            }
            if (value.Length <= 6)
            {
                value = "FF" + value.PadLeft(6, '0');
            }
            uint u;
            return uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out u) ? UIntToColor(u) : UndefinedColor;
        }

        public static Color HsvToColor(byte hue, byte saturation, byte value, byte alpha = 255)
        {
            double r, g, b;
            double h = hue * 360.0 / 255;
            double s = saturation / 255.0;
            double v = value / 255.0;
            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                if (h == 360)
                {
                    h = 0;
                }
                else
                {
                    h = h / 60;
                }
                var i = (int)Math.Truncate(h);
                double f = h - i;
                double p = v * (1.0 - s);
                double q = v * (1.0 - (s * f));
                double t = v * (1.0 - (s * (1.0 - f)));
                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }
            return Color.FromArgb(alpha, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        public static Color HsvToColor(double hue, double sat, double val, double alpha = 1.0)
        {
            double r = 0;
            double g = 0;
            double b = 0;
            if (sat == 0)
            {
                // Gray scale
                r = g = b = val;
            }
            else
            {
                if (hue == 1.0)
                {
                    hue = 0;
                }
                hue *= 6.0;
                var i = (int)Math.Floor(hue);
                double f = hue - i;
                double aa = val * (1 - sat);
                double bb = val * (1 - (sat * f));
                double cc = val * (1 - (sat * (1 - f)));
                switch (i)
                {
                    case 0:
                        r = val;
                        g = cc;
                        b = aa;
                        break;

                    case 1:
                        r = bb;
                        g = val;
                        b = aa;
                        break;

                    case 2:
                        r = aa;
                        g = val;
                        b = cc;
                        break;

                    case 3:
                        r = aa;
                        g = bb;
                        b = val;
                        break;

                    case 4:
                        r = cc;
                        g = aa;
                        b = val;
                        break;

                    case 5:
                        r = val;
                        g = aa;
                        b = bb;
                        break;
                }
            }
            return Color.FromArgb((byte)(alpha * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        public static double HueDifference(Color c1, Color c2)
        {
            double[] hsv1 = ColorToHsv(c1);
            double[] hsv2 = ColorToHsv(c2);
            double dh = hsv1[0] - hsv2[0];

            // clamp to [-0.5,0.5]
            if (dh > 0.5)
            {
                dh -= 1.0;
            }
            if (dh < -0.5)
            {
                dh += 1.0;
            }
            double e = dh * dh;
            return Math.Sqrt(e);
        }

        public static Color Interpolate(Color c0, Color c1, double x)
        {
            double r = c0.R * (1 - x) + c1.R * x;
            double g = c0.G * (1 - x) + c1.G * x;
            double b = c0.B * (1 - x) + c1.B * x;
            double a = c0.A * (1 - x) + c1.A * x;
            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

        public static Color UIntToColor(uint color)
        {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }
    }
}