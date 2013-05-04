// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.ComponentModel;
using System.Windows;
﻿using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons.ViewModels
{
    public abstract class ViewModelBase: INotifyPropertyChanged
    {
        private readonly UiUpdateDispatcher _ui;

        protected ViewModelBase(UiUpdateDispatcher ui)
        {
            _ui = ui;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void NotifyPropertyChangedFor(string propertyName)
        {
            DoOnUIThread(delegate
                             {
                                 if (this.PropertyChanged != null)
                                     this.PropertyChanged(this,
                                                          new PropertyChangedEventArgs(propertyName));
                             });
        }

        protected void DoOnUIThread(Action updateAction)
        {
            try
            {
                _ui.UpdateOnOwnerThread(updateAction);
            }
            catch (Exception)
            {
                //TODO : Move Logger into its own Beacon.. and log there.
            }
        }
    }
}