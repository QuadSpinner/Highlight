global using System;
global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lighthouse2
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.Lighthouse2String)]
    //[ProvideOptionPage(typeof(OptionsProvider.LighthouseOptionsOptions), "Lighthouse2", "LighthouseOptions", 0, 0, true)]
    //[ProvideProfile(typeof(OptionsProvider.LighthouseOptionsOptions), "Lighthouse2", "LighthouseOptions", 0, 0, true)]
    public sealed class Lighthouse2Package : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }
    }
}