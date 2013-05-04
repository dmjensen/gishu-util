// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System.Collections.Generic;
﻿using System.ComponentModel;
﻿using System.Linq;

namespace ManiaX.Test.Beacons.Infrastructure
{
    public class PropertyChangeListener
    {
        private readonly List<string> _modifiedProperties;

        public PropertyChangeListener(INotifyPropertyChanged aViewModel)
        {
            _modifiedProperties = new List<string>();
            aViewModel.PropertyChanged += (args, sender) => _modifiedProperties.Add(sender.PropertyName);
        }

        public bool HasReceivedChangeNotificationFor(string propertyName)
        {
            return _modifiedProperties.Contains(propertyName);
        }

        public int GetCountOfChangeNotificationsFor(string  propertyName)
        {
            return _modifiedProperties.Where(prop => prop == propertyName).Count();
        }
    }
}