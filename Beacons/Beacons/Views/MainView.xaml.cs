// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ManiaX.Beacons.ViewModels;

namespace ManiaX.Beacons.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            
        }

        public void Refresh()
        {
            var mainVM = this.DataContext as MainViewModel;
            if (mainVM == null)
                return;

            this._tddRhythmBeaconView.DataContext = mainVM.TDDRhythmBeaconVM;
            this._stateBeaconView.DataContext = mainVM.StateBeaconVM;
        }
        
    }
}
