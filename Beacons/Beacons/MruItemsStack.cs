// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons
{
    public class MruItemsStack
    {
        private readonly string _name;
        private readonly int _maxSize;

        public MruItemsStack(string name, int maxSize)
        {
            _name = name;
            _maxSize = maxSize;
            Items = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Items { get; set; }

        public void LoadFrom(SettingsRepository settingsStore)
        {
            var loadedItems = new List<string>();
            for(int looper=1; looper<=_maxSize; looper++)
            {
                var value = settingsStore[_name + looper];
                loadedItems.Add( value );
            }
            Items = new ObservableCollection<string>(loadedItems.Where(item => !String.IsNullOrEmpty(item)));
        }

        public void SaveTo(SettingsRepository settingsStore)
        {
            var listToStore = new List<string>(Items);
            int numberOfEmptySlots = _maxSize - Items.Count();
            for (int looper = 0; looper < numberOfEmptySlots; looper++)
                listToStore.Add(String.Empty);

            for (int looper = 1; looper <= _maxSize; looper++)
                settingsStore[_name + looper] = listToStore[looper - 1];

        }

        public void Push(string value)
        {
            var indexOfExistingItem = Items.IndexOf(value);
            const int NOT_FOUND = -1;

            if (indexOfExistingItem == NOT_FOUND)
            {
                if (Items.Count == _maxSize)
                    Items.RemoveAt(_maxSize - 1);
            }
            else
            {
                Items.RemoveAt(indexOfExistingItem);
            }

            Items.Insert(0, value);
        }
    }
}