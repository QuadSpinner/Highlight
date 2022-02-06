namespace Lighthouse2
{
    [Command(PackageIds.CreateHighlight)]
    internal sealed class CreateHighlight : BaseCommand<CreateHighlight>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("Lighthouse2", "Button clicked");
        }
    }
}
