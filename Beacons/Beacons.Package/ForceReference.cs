// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using Microsoft.Windows.Controls;
using WpfGauge;

namespace ManiaX.Beacons
{
    // Class added so that explicit references are maintained to the assemblies containing the following types
    // See also : BeaconPackage.SolveTheXAMLLoadingProblem
    public class ForceReference
    {
        public Gauge Control { get; set; }
        public Calendar Grid { get; set; }
    }
}