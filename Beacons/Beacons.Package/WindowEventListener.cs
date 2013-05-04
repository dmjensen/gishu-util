using System;
using System.Diagnostics;
using ManiaX.Beacons.ViewModels;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ManiaX.Beacons
{
    public class WindowEventListener : IVsWindowFrameNotify
    {
        private bool _windowTornDown;
        public event EventHandler ToolWindowClosed;

        int IVsWindowFrameNotify.OnDockableChange(int fDockable)
        {
            return VSConstants.S_OK;
        }

        int IVsWindowFrameNotify.OnMove()
        {
            return VSConstants.S_OK;
        }

        int IVsWindowFrameNotify.OnShow(int fShow)
        {
            switch (fShow)
            {
                case (int)__FRAMESHOW.FRAMESHOW_AutoHideSlideBegin:
                    Logger.Log("FRAMESHOW_AutoHideSlideBegin");
                    break;

                case (int)__FRAMESHOW.FRAMESHOW_WinClosed:

                    RaiseWindowClosedEvent();
                    _windowTornDown = true;
                    break;

                case (int)__FRAMESHOW.FRAMESHOW_WinShown:

                    Logger.Log("FRAMESHOW_WinShown");

                    break;

                case (int)__FRAMESHOW.FRAMESHOW_WinHidden:

                    // when IDE is torn down, closed is called before hidden. 
                    // (Accessing window after closed throws.) Go figure!
                    if (_windowTornDown)
                        break;
                    
                    RaiseWindowClosedEvent();

                    break;

                default:
                    break;

            }

            return VSConstants.S_OK;
        }

        private void RaiseWindowClosedEvent()
        {
            if (this.ToolWindowClosed != null)
                this.ToolWindowClosed(this, EventArgs.Empty);
        }


        int IVsWindowFrameNotify.OnSize()
        {
            return VSConstants.S_OK;
        }

    }
}
