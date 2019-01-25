using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Lighthouse
{
    [Guid("839f0ad8-28ac-4cbe-966e-e67d78787eeb")]
    public sealed class LighthouseManager : ToolWindowPane
    {

        public LighthouseManager() : base(null)
        {
            Caption = "Lighthouse Manager";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Content = new LighthouseManagerControl();
        }
    }
}
