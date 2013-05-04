// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ManiaX.Beacons
{
    public sealed partial class BeaconsPackage : IVsUpdateSolutionEvents2
    {
        private uint _vsBuildManagerCookie;
        private IVsSolutionBuildManager2 _vsBuildManager;


        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            if (this.BuildInitiated != null)
                this.BuildInitiated(this, EventArgs.Empty);

            return VSConstants.S_OK;
        }
        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            if (fSucceeded == 0)
            {
                if (this.BuildFailed!= null)
                    this.BuildFailed(this, EventArgs.Empty);
            }
            else
            {
                if (this.BuildSucceeded != null)
                    this.BuildSucceeded(this, EventArgs.Empty);
            }
            
            return VSConstants.S_OK;
        }

        #region Irrelevant IVsUpdateSolutionEvents2 notifications that I don't care about yet
        

        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Cancel()
        {
            return VSConstants.S_OK;
        }
        
        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return VSConstants.S_OK;
        }
        #endregion

        private void SubscribeForBuildEvents()
        {
            _vsBuildManager = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
            //TODO 20100821: notify the user if we are not able to get the build manager
            if (_vsBuildManager == null)
                return;

            _vsBuildManager.AdviseUpdateSolutionEvents(this, out _vsBuildManagerCookie);
        }

        private void UnsubscribeForBuildEvents()
        {
            _vsBuildManager.UnadviseUpdateSolutionEvents(_vsBuildManagerCookie);
        }
    }
}