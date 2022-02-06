using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Lighthouse2.Utilities;
using Ookii.Dialogs.Wpf;

namespace Lighthouse2.Commands
{
    [Command(PackageIds.CreateHighlight)]
    internal sealed class CreateHighlight : BaseCommand<CreateHighlight>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var dte = (DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));

            string selection = (dte.ActiveDocument.Selection as TextSelection).Text;

            if (string.IsNullOrEmpty(selection) || string.IsNullOrWhiteSpace(selection))
                return;

            var options = LightHouseOptions.GetLiveInstanceAsync().Result;
            options.ColorTags ??= Helper.GetFillerTags().ToArray();

            List<HighlightTag> tags = options.ColorTags.ToList();

            var found = tags.FirstOrDefault(x => x.Criteria == selection);

            if (found == null)
            {
                // CREATE NEW

                tags.Add(new HighlightTag(selection));
            }
            else
            {
                // EXISTS
                TaskDialog td = new()
                {
                    WindowTitle = "QuadSpinner Lighthouse",
                    MainInstruction = $"Modify color tag rule for \"{found.Criteria}\"",
                    Content = "Select a modification below to modify this tag.",
                    AllowDialogCancellation = true,
                    //ButtonStyle = TaskDialogButtonStyle.CommandLinksNoIcon,
                    RadioButtons =
                    {
                        new TaskDialogRadioButton() {Checked = found.Shape == TagShape.Tag, Text = "Tag"},
                        new TaskDialogRadioButton() {Checked = found.Shape == TagShape.TagUnder, Text = "Underlined Tag"},
                        new TaskDialogRadioButton() {Checked = found.Shape == TagShape.Line, Text="Full Line"},
                        new TaskDialogRadioButton() {Checked = found.Shape == TagShape.LineUnder, Text="Full Underline"},
                    },
                    CenterParent = true,
                    Footer = "For further customizations, go to Options -> QuadSpinner -> Lighthouse"
                };

                //const string shapeTag = "Change Shape --> Tag";
                //const string shapeTagUnderline = "Change Shape --> Underlined Tag";
                //const string shapeLine = "Change Shape --> Full Line";
                //const string shapeUnderline = "Change Shape --> Full Underline";

                const string changeColor = "Change Color";
                const string toggleBlur = "Change Blur";
                const string deactivate = "Toggle Rule";
                const string delete = "Delete";

                td.Buttons.Add(new TaskDialogButton(changeColor));
                td.Buttons.Add(new TaskDialogButton(toggleBlur));
                td.Buttons.Add(new TaskDialogButton(deactivate));
                td.Buttons.Add(new TaskDialogButton(delete));

                var response = td.ShowDialog();

                if (response.ButtonType == ButtonType.Cancel)
                    return;
                
                if (td.RadioButtons[0].Checked) found.Shape = TagShape.Tag;
                if (td.RadioButtons[1].Checked) found.Shape = TagShape.TagUnder;
                if (td.RadioButtons[2].Checked) found.Shape = TagShape.Line;
                if (td.RadioButtons[3].Checked) found.Shape = TagShape.LineUnder;

                switch (response.Text)
                {
                    case toggleBlur:
                        ChangeBlur(ref found);
                        //found.Blur = found.Blur == BlurIntensity.None ? BlurIntensity.Medium : BlurIntensity.None;
                        break;

                    case changeColor:
                        break;

                    case deactivate:
                        found.IsActive = !found.IsActive;
                        break;

                    case delete:
                        tags.Remove(found);
                        break;
                }
            }

            options.ColorTags = tags.ToArray();
            await options.SaveAsync();

            dte.ActiveDocument.ReplaceText(selection, selection, (int)vsFindOptions.vsFindOptionsMatchCase | (int)vsFindOptions.vsFindOptionsMatchWholeWord);
        }

        private static void ChangeBlur(ref HighlightTag found)
        {
            // EXISTS
            TaskDialog td = new()
            {
                WindowTitle = "QuadSpinner Lighthouse",
                MainInstruction = $"Modify blur value for \"{found.Criteria}\"",
                Content = "Select a blur value.",
                AllowDialogCancellation = true,
                //ButtonStyle = TaskDialogButtonStyle.CommandLinksNoIcon,
                RadioButtons =
                    {
                        new TaskDialogRadioButton() {Checked = found.Blur == BlurIntensity.None, Text = "None"},
                        new TaskDialogRadioButton() {Checked = found.Blur == BlurIntensity.Low, Text = "Low"},
                        new TaskDialogRadioButton() {Checked = found.Blur == BlurIntensity.Medium, Text = "Medium"},
                        new TaskDialogRadioButton() {Checked = found.Blur == BlurIntensity.High, Text = "High"},
                        new TaskDialogRadioButton() {Checked = found.Blur == BlurIntensity.Ultra, Text = "Ultra"},
                        
                    },
                CenterParent = true,
                Footer = "For further customizations, go to Options -> QuadSpinner -> Lighthouse"
            };
            
            td.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
            td.Buttons.Add(new TaskDialogButton(ButtonType.Cancel));

            var response = td.ShowDialog();

            if (response.ButtonType == ButtonType.Cancel)
                return;

            if (td.RadioButtons[0].Checked) found.Blur = BlurIntensity.None;
            if (td.RadioButtons[1].Checked) found.Blur = BlurIntensity.Low;
            if (td.RadioButtons[2].Checked) found.Blur = BlurIntensity.Medium;
            if (td.RadioButtons[3].Checked) found.Blur = BlurIntensity.High;
            if (td.RadioButtons[4].Checked) found.Blur = BlurIntensity.Ultra;

        }
    }
}