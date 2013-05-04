// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;

namespace ManiaX.Beacons.DataStructs
{
    public struct GroupedAnnotation
    {
        public string Text { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return String.Format("Text={0}, Count={1}", Text, Count);
        }
    }
}