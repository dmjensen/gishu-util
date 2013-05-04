// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
﻿using System.Collections.Generic;
﻿using System.ComponentModel;

namespace ManiaX.Beacons.DataStructs
{
    public class TestRun : INotifyPropertyChanged
    {
        public int TestCount { get; set; }

        public TestResult Result { get; set; }

        private string _note;
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Note"));
            }
        }

        public IEnumerable<FailureVM> Failures { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum TestResult
    {
        Red,
        Green,
        RefactoringWin
    }
}