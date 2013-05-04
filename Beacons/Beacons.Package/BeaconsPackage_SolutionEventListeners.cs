// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ManiaX.Beacons
{
    public sealed partial class BeaconsPackage : IVsSolutionEvents
    {
        private uint _vsSolutionManagerCookie;
        private IVsSolution2 _vsSolutionManager;

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            if (this.SolutionOpened != null)
                this.SolutionOpened(this, EventArgs.Empty);

            return VSConstants.S_OK;
        }
        public int OnAfterCloseSolution(object pUnkReserved)
        {
            if (this.SolutionClosed != null)
                this.SolutionClosed(this, EventArgs.Empty);

            return VSConstants.S_OK;
        }

        #region VS Solution Events I don't care about
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }


        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }
        
        #endregion
        

        private void SubscribeForSolutionEvents()
        {
            _vsSolutionManager = ServiceProvider.GlobalProvider.GetService(typeof (SVsSolution)) as IVsSolution2;

            // TODO 20100822 : notify the user if we are not able to get thesolution  mgr
            if (_vsSolutionManager == null)
                return;
            _vsSolutionManager.AdviseSolutionEvents(this, out _vsSolutionManagerCookie);
        }
        private void UnsubscribeForSolutionEvents()
        {
            _vsSolutionManager.UnadviseSolutionEvents(_vsSolutionManagerCookie);
        }
    }

}
