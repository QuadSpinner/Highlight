using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Lighthouse
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#2110",
        "#2112",
        "1.0",
        IconResourceID = 2400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideToolWindow(typeof(LighthouseManager))]
    public sealed class LighthousePackage : AsyncPackage
    {
        public const string PackageGuidString = "f883a970-94e7-46ed-89f4-eb4e736a1905";

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken,
                                                      IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await Lighthouse.InitializeAsync(this);
        }


        #endregion Package Members
    }
}