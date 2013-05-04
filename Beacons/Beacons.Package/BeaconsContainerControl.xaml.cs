// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;

namespace ManiaX.Beacons
{
    /// <summary>
    /// Interaction logic for BeaconsContainerControl.xaml
    /// </summary>
    public partial class BeaconsContainerControl
    {
        public BeaconsContainerControl()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            _mainVM.Refresh();
        }
    }
}