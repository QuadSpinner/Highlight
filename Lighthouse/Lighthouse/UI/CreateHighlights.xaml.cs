using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Lighthouse.Utilities;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Ookii.Dialogs.Wpf;

namespace Lighthouse.UI
{
    public partial class CreateHighlights
    {
        private const string solutionFile = @"\solution.lhtags";
        private bool allowColoring;
        private bool allowSelectionChange;
        private BlurIntensity Blur;
        private DTE dte;
        internal string selection = "Test Criteria";

        public CreateHighlights()
        {
            dte = (DTE)Package.GetGlobalService(typeof(DTE));
            InitializeComponent();

            Loaded += CreateHighlights_Loaded;
            MouseLeftButtonDown += (sender, e) => DragMove();

            btnCreate.Click += BtnCreate_Click;

            btnChange.Click += BtnChange_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClose.Click += (_, __) => Close();

            KeyUp += CreateHighlights_KeyUp;

            chkLine.Checked += (sender, args) => colorTag.isFullLine = chkLine.IsChecked == true;
            chkLine.Unchecked += (sender, args) => colorTag.isFullLine = chkLine.IsChecked == true;
            chkActive.Checked += (sender, args) => colorTag.isActive = chkActive.IsChecked == true;
            chkActive.Unchecked += (sender, args) => colorTag.isActive = chkActive.IsChecked == true;

            cboBlur.SelectionChanged += CboBlur_SelectionChanged;
        }

        public ColorTag colorTag { get; set; }

        private void CreateHighlights_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Enter:
                    if (allowColoring && !string.IsNullOrWhiteSpace(selection) && !string.IsNullOrEmpty(selection))
                    {
                        CreateTag(selection, ((SolidColorBrush)((ComboBoxItem)cboColors.SelectedItem).Foreground).Color);
                    }
                    break;
            }
        }

        private void CreateHighlights_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                selection = (dte.ActiveDocument.Selection as TextSelection).Text;
                colorTag = new ColorTag
                {
                    ColorSwatch = Colors.Purple,
                    Criteria = selection,
                    isFullLine = true
                };
                foreach (ComboBoxItem l in Lighthouse.colors.Select(color => new ComboBoxItem
                {
                    Foreground = new SolidColorBrush(color),
                    Background = new SolidColorBrush(color)
                }))
                {
                    cboColors.Items.Add(l);
                }

                allowColoring = true;
            }
            catch (Exception ex)
            {
                Lighthouse.output.OutputString(ex.Message + "\n\n" + ex.StackTrace);
            }

            MoveToMousePosition();
            ShowEditor();
        }

        private void cboColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cboColors.Foreground = ((ComboBoxItem)cboColors.SelectedItem).Foreground;

                if (allowSelectionChange)
                    colorTag.ColorSwatch = ((SolidColorBrush)((ComboBoxItem)cboColors.SelectedItem).Foreground).Color;
                CreatePreview();
            }
            catch (Exception) { }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.WindowTitle = "Delete Highlight Rule?";
                dialog.MainInstruction = $"Are you sure you want to delete the tag \"{selection}\"?";
                dialog.Content = "Deleting a rule is permament. You can also disable the rule until you need it again.";
                dialog.ButtonStyle = TaskDialogButtonStyle.CommandLinks;

                TaskDialogButton deleteButton = new TaskDialogButton
                {
                    Text = "Delete Rule"
                };

                TaskDialogButton deactivateButton = new TaskDialogButton
                {
                    Text = "Deactivate Rule",
                    CommandLinkNote = "Keep the rule, but deactivate it."
                };

                TaskDialogButton cancelButton = new TaskDialogButton(ButtonType.Cancel);

                dialog.Buttons.Add(deleteButton);
                dialog.Buttons.Add(cancelButton);
                dialog.Buttons.Add(deactivateButton);

                TaskDialogButton button = dialog.ShowDialog();

                if (button == deleteButton)
                {
                    DeleteHighlight();
                }

                if (button == deactivateButton)
                {
                    DeactivateHighlight();
                }
            }
        }

        private void CboBlur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Blur = (BlurIntensity)cboBlur.SelectedIndex;
            colorTag.Blur = Blur;
            CreatePreview();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (allowColoring && !string.IsNullOrWhiteSpace(selection) && !string.IsNullOrEmpty(selection))
            {
                CreateTag(selection, ((SolidColorBrush)((ComboBoxItem)cboColors.SelectedItem).Foreground).Color);
                RefreshDocument();
            }
        }

        private void RefreshDocument()
        {
            dte.ActiveDocument.ReplaceText(selection, selection, (int)vsFindOptions.vsFindOptionsMatchCase);
        }

        private void DeactivateHighlight()
        {
            if (Lighthouse.Options.ColorTags.Any(x => x.Criteria == selection))
            {
                Lighthouse.Options.ColorTags.Find(x => x.Criteria == selection).isActive = false;
                Lighthouse.Options.SaveSettingsToStorage();

                Lighthouse.LoadColorTags();
            }
        }

        private void DeleteHighlight()
        {
            if (Lighthouse.Options.ColorTags.Any(x => x.Criteria == selection))
            {
                Lighthouse.Options.ColorTags.Remove(Lighthouse.Options.ColorTags.First(x => x.Criteria == selection));
                Lighthouse.Options.SaveSettingsToStorage();
            }
            else
            {
                List<ColorTag> temp = HelperFunctions.LoadTagsFromFile(new FileInfo(dte.Solution.FullName).DirectoryName +
                                                                       solutionFile);

                temp.Remove(temp.First(x => x.Criteria == selection));
                HelperFunctions.SaveTagsToFile(new FileInfo(dte.Solution.FullName).DirectoryName + solutionFile, temp);
            }

            Lighthouse.LoadColorTags();
            RefreshDocument();
            Close();
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if (Lighthouse.Options.ColorTags.Any(x => x.Criteria == selection))
            {
                ColorTag ct = Lighthouse.Options.ColorTags.Find(x => x.Criteria == selection);
                ct.ColorSwatch = ((SolidColorBrush)((ComboBoxItem)cboColors.SelectedItem).Foreground).Color;
                ct.isFullLine = chkLine.IsChecked == true;
                ct.isUnderline = chkUnderline.IsChecked == true;
                ct.Blur = (BlurIntensity)cboBlur.SelectedIndex;
                ct.isActive = chkActive.IsChecked == true;

                Lighthouse.Options.SaveSettingsToStorage();

                Lighthouse.LoadColorTags();
            }
            RefreshDocument();
            Close();
        }

        internal void CreateTag(string criteria, Color c, bool isSolution = false)
        {
            if (!isSolution)
            {
                if (Lighthouse.Options.ColorTags.Any(x => x.Criteria == criteria))
                {
                    return;
                }

                Lighthouse.Options.ColorTags.Add(new ColorTag
                {
                    Criteria = criteria,
                    ColorSwatch = c,
                    isFullLine = chkLine.IsChecked == true,
                    isUnderline = chkUnderline.IsChecked == true,
                    Blur = (BlurIntensity)cboBlur.SelectedIndex,
                    isActive = chkActive.IsChecked == true,
                });

                Lighthouse.Options.SaveSettingsToStorage();
            }
            else
            {

                //! --- SOLUTION ------
                List<ColorTag> temp = HelperFunctions.LoadTagsFromFile(new FileInfo(dte.Solution.FullName).DirectoryName + solutionFile);

                if (temp.Any(x => x.Criteria == selection))
                {
                    return;
                }

                temp.Add(new ColorTag
                {
                    Criteria = selection,
                    ColorSwatch = c,
                    isFullLine = chkLine.IsChecked == true,
                    isUnderline = chkUnderline.IsChecked == true,
                    Blur = (BlurIntensity)cboBlur.SelectedIndex,
                    isActive = chkActive.IsChecked == true,
                });

                HelperFunctions.SaveTagsToFile(new FileInfo(dte.Solution.FullName).DirectoryName + solutionFile, temp);
            }

            Lighthouse.LoadColorTags();

            RefreshDocument();
            Close();
        }

        internal void ShowEditor()
        {
            // Change stacks
            selection = (dte.ActiveDocument.Selection as TextSelection).Text;
            if (!string.IsNullOrWhiteSpace(selection) && !string.IsNullOrEmpty(selection))
            {
                // Exists
                if (Lighthouse.Options.ColorTags.Any(x => x.Criteria == selection))
                {
                    stackExists.Visibility = btnChange.Visibility = Visibility.Visible;
                    btnCreate.Visibility = Visibility.Collapsed;
                    allowSelectionChange = false;
                    colorTag = Lighthouse.Options.ColorTags.First(x => x.Criteria == selection);

                    cboColors.Items.Cast<ComboBoxItem>()
                             .First(item => (item.Foreground as SolidColorBrush).Color == colorTag.ColorSwatch)
                             .IsSelected = true;

                    allowSelectionChange = true;
                    cboBlur.SelectedIndex = (int)colorTag.Blur;
                    chkLine.IsChecked = colorTag.isFullLine;
                    chkUnderline.IsChecked = colorTag.isUnderline;
                    chkActive.IsChecked = colorTag.isActive;

                    chkSolution.Visibility = Visibility.Collapsed;
                }
                else // NEW
                {
                    stackExists.Visibility = btnChange.Visibility = Visibility.Collapsed;
                    btnCreate.Visibility = Visibility.Visible;
                    chkSolution.Visibility = Visibility.Visible;
                    allowSelectionChange = true;

                    cboColors.SelectedIndex = new Random().Next(0, cboColors.Items.Count - 1);
                    colorTag = new ColorTag
                    {
                        Criteria = selection,
                        ColorSwatch = ((SolidColorBrush)((ComboBoxItem)cboColors.SelectedItem).Foreground).Color,
                        isFullLine = chkLine.IsChecked == true,
                        isUnderline = chkUnderline.IsChecked == true,
                        Blur = (BlurIntensity)cboBlur.SelectedIndex,
                        isActive = chkActive.IsChecked == true
                    };
                    cboBlur.SelectedIndex = (int)Lighthouse.Options.Blur;
                }

                chkLine.IsChecked = colorTag.isFullLine;
                chkActive.IsChecked = colorTag.isActive;

                CreatePreview();
            }
        }

        private void MoveToMousePosition()
        {
            var p = PointToScreen(Mouse.GetPosition(this));

            Left = p.X;
            Top = p.Y;
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

                                           UpdateLayout();
                                           CornerStyle HighlightCorner = Lighthouse.Options.HighlightCorner;

                                           t.Text = colorTag.Criteria;


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
                                           bool isLine = colorTag.isUnderline;

                                           if (isLine)
                                           {
                                               r.Height = 4;
                                               r.Margin = new Thickness(0, t.ActualHeight - 2, 0, 0);
                                           }
                                           else
                                           {
                                               r.Height = t.ActualHeight;
                                           }

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

                                           double cornerRadius;

                                           try
                                           {
                                               switch (HighlightCorner)
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
    }
}