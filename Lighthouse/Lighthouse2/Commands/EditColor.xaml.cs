using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Lighthouse2.Core;
using MessageBox = System.Windows.MessageBox;

namespace Lighthouse2.Commands
{
    public partial class EditColor
    {
        public EditColor()
        {
            InitializeComponent();

            Loaded += EditColor_Loaded;

            btnModify.Click += BtnModify_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void EditColor_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (ListBoxItem l in Helper.colors.Select(color => new ListBoxItem
            {
                Foreground = new SolidColorBrush(color),
                Background = new SolidColorBrush(color),
                Tag = color,
                IsSelected = TagToEdit.Color.ColorToHex() == color.ColorToHex()
            }))
            {
                lstColors.Items.Add(l);
            }

            cboShape.SelectedIndex = (int)TagToEdit.Shape;
            cboBlur.SelectedIndex = (int)TagToEdit.Blur;
            //txtCriteria.Text = TagToEdit.Criteria;
            chkActive.IsChecked = TagToEdit.IsActive;

            //txtCriteria.TextChanged += (_, __) => CreatePreview();
            cboBlur.SelectionChanged += (_, __) => CreatePreview();
            cboShape.SelectionChanged += (_, __) => CreatePreview();
            lstColors.SelectionChanged += (_, __) => CreatePreview();
            
            CreatePreview();
        }

        public HighlightTag TagToEdit { get; set; }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            TagToEdit.Shape = (TagShape)cboShape.SelectedIndex;
            TagToEdit.Blur = (BlurIntensity)cboBlur.SelectedIndex;
            //TagToEdit.Criteria = txtCriteria.Text;
            TagToEdit.IsActive = chkActive.IsChecked == true;
            TagToEdit.Color = (Color)(lstColors.SelectedItem as ListBoxItem).Tag;
            DialogResult = true;
            Close();
        }


        internal void CreatePreview()
        {

            try
            {
                if (TagToEdit == null)
                    return;


                HighlightTag tag = new()
                {
                    Shape = (TagShape)cboShape.SelectedIndex,
                    Blur = (BlurIntensity)cboBlur.SelectedIndex,
                    Criteria = TagToEdit.Criteria,
                    IsActive = chkActive.IsChecked == true,
                    Color = (Color)(lstColors.SelectedItem as ListBoxItem).Tag
                };

                // UpdateLayout();


                bool isLine = tag.IsLine();

                r.Effect = null;

                if (tag.Blur != BlurIntensity.None)
                {
                    r.Effect = new BlurEffect
                    {
                        KernelType = KernelType.Gaussian,
                        RenderingBias = RenderingBias.Performance
                    };

                    switch (tag.Blur)
                    {
                        case BlurIntensity.Low:
                            ((SolidColorBrush)r.Fill).Color.ChangeAlpha(80);
                            ((BlurEffect)r.Effect).Radius = isLine ? 2 : 4.0;
                            break;

                        case BlurIntensity.Medium:
                            ((SolidColorBrush)r.Fill).Color.ChangeAlpha(120);
                            ((BlurEffect)r.Effect).Radius = isLine ? 4 : 7.0;
                            break;

                        case BlurIntensity.High:
                            ((SolidColorBrush)r.Fill).Color.ChangeAlpha(170);
                            ((BlurEffect)r.Effect).Radius = isLine ? 6 : 11.0;
                            break;

                        case BlurIntensity.Ultra:
                            ((SolidColorBrush)r.Fill).Color.ChangeAlpha(255);
                            ((BlurEffect)r.Effect).Radius = isLine ? 8 : 20.0;
                            break;
                    }

                    r.Stroke = null;
                }

                t.Text = tag.Criteria;

                r.Fill = new SolidColorBrush(Color.FromArgb(60, tag.Color.R, tag.Color.G, tag.Color.B));
                r.Stroke = new SolidColorBrush(Color.FromArgb(100, tag.Color.R, tag.Color.G, tag.Color.B));

                double Vert = tag.Blur != BlurIntensity.None ? 4 : 2;
                const double Horz = 2; //tag.isFullLine ? 1920 : 4;

                r.Width = isLine ? previewGrid.ActualWidth - 8 : t.ActualWidth + 2;
                t.Text = tag.Criteria;
                t.Padding = new Thickness(Horz, Vert, Horz, Vert);

                if (isLine)
                {
                    r.Height = 4;
                    r.Margin = new Thickness(0, t.ActualHeight - 3, 0, 0);
                }
                else
                {
                    r.Height = t.ActualHeight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

        }
    }
}