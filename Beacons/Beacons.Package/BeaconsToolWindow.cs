// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System.Runtime.InteropServices;
using ManiaX.Beacons.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace ManiaX.Beacons
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("80fa5586-7181-4335-9fc2-c08b5bbb06b5")]
    public class BeaconsToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public BeaconsToolWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Strings.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            base.Content = new BeaconsContainerControl();
        }

        internal void SetViewModel(MainViewModel mainViewModel)
        {
            var beaconsContainerControl = base.Content as BeaconsContainerControl;
            if (beaconsContainerControl == null)
                return;
            
            beaconsContainerControl.DataContext = mainViewModel;
            //TODO:20101202- Workaround for Issue#8 - http://stackoverflow.com/questions/4331424
            beaconsContainerControl.Refresh();
        }
    }
}
