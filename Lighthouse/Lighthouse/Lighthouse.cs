using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Lighthouse.UI;
using Lighthouse.Utilities;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace Lighthouse
{
    internal sealed class Lighthouse
    {
        public const int CommandId = 0x0100;
        public const int WindowCommandId = 4129;

        //public const int Command2Id = 0x0101;

        private const string title = "Lighthouse";
        public static readonly Guid CommandSet = new Guid("2ebdfd89-9a11-4d8f-ba1c-f24d000cf038");

        internal static string Selection;

        internal static IVsOutputWindowPane output;
        internal static IVsActivityLog win;

        internal static List<Color> colors = new List<Color>();
        internal static LighthouseOptions Options;
        public static Dictionary<string, FormatInfo> keywordFormats = new Dictionary<string, FormatInfo>();

        private static DTE dte;

        private readonly AsyncPackage package;

        private Lighthouse(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);

            var menuCommandID2 = new CommandID(CommandSet, WindowCommandId);
            var menuItem2 = new MenuCommand(ExecuteWindowCommand, menuCommandID2);
            commandService.AddCommand(menuItem2);

            //commandService.AddCommand(menuItem2);
        }

        public static Lighthouse Instance { get; private set; }

        internal IAsyncServiceProvider ServiceProvider => package;

        public static bool IsEnabled { get; set; } = true;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService =
                await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new Lighthouse(package, commandService);

            output = package.GetOutputPane(new Guid("25E5EFF8-A5E5-48E1-B607-B1C9410BFC04"), title);

            Init();
        }

        internal static void Init()
        {
            Options = new LighthouseOptions
                      {
                          Blurred = BlurType.Selective,
                          Blur = BlurIntensity.Medium,
                          HighlightCorner = CornerStyle.RoundedCorner,
                          OverrideStyles = false
                      };
            Options.LoadSettingsFromStorage();
            LoadColorTags();
        }

        private static void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CreateHighlights ch = new CreateHighlights();
            ch.ShowDialog();
        }

        private void ExecuteWindowCommand(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = package.FindToolWindow(typeof(LighthouseManager), 0, true);
            if (null == window || null == window.Frame)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        public static void LoadColorTags()
        {
            try
            {
                if (dte == null)
                    dte = (DTE) Package.GetGlobalService(typeof(DTE));

                string colorFile = new FileInfo(dte.Solution.FullName).DirectoryName + @"\solution.qm";

                keywordFormats.Clear();

                List<ColorTag> ct = new List<ColorTag>();

                ct.AddRange(Options.ColorTags);

                if (File.Exists(colorFile))
                {
                    ct = HelperFunctions.LoadTagsFromFile(colorFile);
                }

                foreach (var colorTag in ct)
                {
                    if (keywordFormats.Any(x => x.Key == colorTag.Criteria))
                    {
                        // if exists in file, and override is on
                        if (Options.OverrideStyles && colorTag.isActive)
                        {
                            keywordFormats.Remove(colorTag.Criteria);
                            keywordFormats.Add(colorTag.Criteria,
                                               new FormatInfo(
                                                   Color.FromArgb(60,
                                                                  colorTag.ColorSwatch.R,
                                                                  colorTag.ColorSwatch.G,
                                                                  colorTag.ColorSwatch.B),
                                                   Color.FromArgb(100,
                                                                  colorTag.ColorSwatch.R,
                                                                  colorTag.ColorSwatch.G,
                                                                  colorTag.ColorSwatch.B),
                                                   colorTag.isFullLine,
                                                   colorTag.Blur));
                        }
                    }
                    else
                    {
                        // does not exist, let's add it

                        if (colorTag.isActive)
                        {
                            keywordFormats.Add(colorTag.Criteria,
                                               new FormatInfo(
                                                   Color.FromArgb(60,
                                                                  colorTag.ColorSwatch.R,
                                                                  colorTag.ColorSwatch.G,
                                                                  colorTag.ColorSwatch.B),
                                                   Color.FromArgb(100,
                                                                  colorTag.ColorSwatch.R,
                                                                  colorTag.ColorSwatch.G,
                                                                  colorTag.ColorSwatch.B),
                                                   colorTag.isFullLine,
                                                   colorTag.Blur));
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}