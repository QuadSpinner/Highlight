using Lighthouse2.UI;

namespace Lighthouse2.Commands
{
    [Command(PackageIds.CreateHighlight)]
    internal sealed class CreateHighlight : BaseCommand<CreateHighlight>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CreateMark ch = new CreateMark();
            ch.ShowDialog();
        }
    }
}
