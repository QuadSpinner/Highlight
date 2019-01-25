using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Lighthouse.Utilities;
using PropertyTools.Wpf;
using ColorHelper = Lighthouse.Utilities.ColorHelper;

namespace Lighthouse
{
    public partial class LighthouseManagerControl
    {
        private ColorTag colorTag;

        public LighthouseManagerControl()
        {
            InitializeComponent();
            Loaded += Manage_Loaded;
            grid.ControlFactory = new DataGridControlFactory();

            grid.MouseLeftButtonUp += (sender, args) => UpdatePreview();
            grid.KeyUp += (sender, args) => UpdatePreview();
            grid.SourceUpdated += (sender, args) => UpdatePreview();
            SizeChanged += (sender, args) => UpdatePreview();

            btnSave.Click += BtnSave_Click;
            btnClose.Click += BtnClose_Click;

            colorTag = new ColorTag
                       {
                           Criteria = "// Highlight Preview",
                           ColorSwatch = ColorHelper.HexToColor("#FF58C5B6")
                       };
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            grid.ItemsSource = null;
            Lighthouse.Init();
            grid.ItemsSource = Lighthouse.Options.ColorTags;
        }

        private static void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reopen editor windows to see the updated visuals.", "Lighthouse Highlights Updated");
            Lighthouse.Options.SaveSettingsToStorage();
            Lighthouse.Init();
        }

        public bool OptionsVisible { get; set; } = true;

        private void Manage_Loaded(object sender, RoutedEventArgs e)
        {
            stackOptions.Visibility = OptionsVisible ? Visibility.Visible : Visibility.Collapsed;

            try
            {
                string s = ((int)Lighthouse.Options.HighlightCorner).ToString();

                foreach (RadioButton child in stackH.Children.Cast<RadioButton>()
                                                    .Where(child => child.Tag.ToString() == s))
                {
                    child.IsChecked = true;
                }

                s = ((int)Lighthouse.Options.Blur).ToString();
                foreach (RadioButton child in stackBlur
                                              .Children.Cast<RadioButton>().Where(child => child.Tag.ToString() == s))
                {
                    child.IsChecked = true;
                }

                s = ((int)Lighthouse.Options.Blurred).ToString();
                foreach (RadioButton child in stackBlurring
                                              .Children.Cast<RadioButton>().Where(child => child.Tag.ToString() == s))
                {
                    child.IsChecked = true;
                }
            }
            catch (Exception) { }

            grid.ItemsSource = Lighthouse.Options.ColorTags;
        }

        private void Blur_Checked(object sender, RoutedEventArgs e)
        {
            Lighthouse.Options.Blur = (BlurIntensity)int.Parse((sender as RadioButton).Tag.ToString());
            UpdatePreview();
        }

        internal void UpdatePreview()
        {
            try
            {
                //HighlightCorner = HighlightCorner;

                colorTag.Blur = Lighthouse.Options.Blurred == BlurType.NoBlur ? BlurIntensity.None : Lighthouse.Options.Blur;

                colorTag.isFullLine = Lighthouse.Options.ColorTags[grid.SelectionCell.Row].isFullLine;
                colorTag.ColorSwatch = Lighthouse.Options.ColorTags[grid.SelectionCell.Row].ColorSwatch;
                colorTag.Criteria = Lighthouse.Options.ColorTags[grid.SelectionCell.Row].Criteria;

                if (Lighthouse.Options.Blurred == BlurType.Selective)
                {
                    colorTag.Blur = Lighthouse.Options.ColorTags[grid.SelectionCell.Row].Blur;
                }

                CreatePreview();
            }
            catch (Exception) { }
        }

        private void Highlight_Checked(object sender, RoutedEventArgs e)
        {
            Lighthouse.Options.HighlightCorner = (CornerStyle)int.Parse((sender as RadioButton).Tag.ToString());
            UpdatePreview();
        }

        private void Blurring_Checked(object sender, RoutedEventArgs e)
        {
            Lighthouse.Options.Blurred = (BlurType)int.Parse((sender as RadioButton).Tag.ToString());
            UpdatePreview();
        }

        internal void CreatePreview()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render,
                                   new Action(() =>
                                   {
                                       try
                                       {
                                           if (colorTag == null)
                                               return;

                                           CornerStyle corner = Lighthouse.Options.HighlightCorner;

                                           t.Text = colorTag.Criteria;

                                           UpdateLayout();

                                           r.Fill = new SolidColorBrush(Color.FromArgb(60,
                                                                                       colorTag.ColorSwatch.R,
                                                                                       colorTag.ColorSwatch.G,
                                                                                       colorTag.ColorSwatch.B));
                                           r.Stroke = new SolidColorBrush(Color.FromArgb(100,
                                                                                         colorTag.ColorSwatch.R,
                                                                                         colorTag.ColorSwatch.G,
                                                                                         colorTag.ColorSwatch.B));

                                           double Vert = colorTag.Blur != BlurIntensity.None ? 3 : 2;
                                           const double Horz = 4; //colorTag.isFullLine ? 1920 : 4;

                                           r.Width = colorTag.isFullLine ? previewGrid.ActualWidth - 8 : t.ActualWidth;

                                           if (r.Width < t.ActualWidth)
                                               r.Width = t.ActualWidth + 2;

                                           t.Padding = new Thickness(Horz, Vert, Horz, Vert);

                                           r.Effect = null;

                                           colorTag.Blur = colorTag.Blur;

                                           if (colorTag.Blur != BlurIntensity.None)
                                           {
                                               r.Effect = new BlurEffect
                                               {
                                                   KernelType = KernelType.Gaussian,
                                                   RenderingBias = RenderingBias.Performance
                                               };

                                               switch (colorTag.Blur)
                                               {
                                                   case BlurIntensity.Low:
                                                       ColorHelper.ChangeAlpha(((SolidColorBrush)r.Fill).Color, 80);
                                                       ((BlurEffect)r.Effect).Radius = 4.0;
                                                       break;

                                                   case BlurIntensity.Medium:
                                                       ColorHelper.ChangeAlpha(((SolidColorBrush)r.Fill).Color, 120);
                                                       ((BlurEffect)r.Effect).Radius = 7.0;
                                                       break;

                                                   case BlurIntensity.High:
                                                       ColorHelper.ChangeAlpha(((SolidColorBrush)r.Fill).Color, 170);
                                                       ((BlurEffect)r.Effect).Radius = 11.0;
                                                       break;

                                                   case BlurIntensity.Ultra:
                                                       ColorHelper.ChangeAlpha(((SolidColorBrush)r.Fill).Color, 255);
                                                       ((BlurEffect)r.Effect).Radius = 20.0;
                                                       break;
                                               }

                                               r.Stroke = null;
                                           }

                                           double cornerRadius;

                                           try
                                           {
                                               switch (corner)
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

                                           r.RadiusX = cornerRadius;
                                           r.RadiusY = cornerRadius;
                                       }
                                       catch (Exception) { }
                                   }));
        }

        #region Plumbing

        //public BlurIntensity Blur { get; set; } = BlurIntensity.None;

        //public BlurType Blurred // Blurring
        //{
        //    get;
        //    set;
        //} = BlurType.Selective;

        //public CornerStyle HighlightCorner { get; set; } = CornerStyle.RoundedCorner;

        //public bool OverrideStyles { get; set; } = true;

        //public List<ColorTag> ColorTags { get; set; }

        #endregion Plumbing
    }
}