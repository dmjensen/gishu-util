// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ManiaX.Beacons.Roles;

namespace ManiaX.Beacons.ViewModels
{
    public class TestRunnerConfiguration : ViewModelBase, IDataErrorInfo, HasSettings
    {
        private readonly TestRunnerProvider _testRunnerProvider;
        private readonly FileSystem _fileSystem;
        private string _configArguments;
        private readonly MruItemsStack _mruPathsStack;

        public TestRunnerConfiguration(TestRunnerProvider testRunnerProvider, FileSystem fileSystem, UiUpdateDispatcher uiUpdater) : base(uiUpdater)
        {
            _testRunnerProvider = testRunnerProvider;
            _fileSystem = fileSystem;
            _mruPathsStack = new MruItemsStack("MRUTestResultsFilePath", 5);
        }

        public ObservableCollection<string> MRUPaths
        {
            get { return _mruPathsStack.Items; }
        }


        public string ConfigArguments
        {
            get { return _configArguments; }

            set
            {
                _testRunnerProvider.ClearTestRunner();
                if (String.IsNullOrWhiteSpace(value))
                {    
                    _configArguments = string.Empty;
                    NotifyPropertyChangedFor("ConfigArguments");
                    return;
                }
                
                _configArguments = value.Trim();
                NotifyPropertyChangedFor("ConfigArguments");

                if ((SelectedRunnerType == RunnerType.NUnitResultsFileWatcher) && !_fileSystem.FolderExists(_configArguments))
                    return;

                _testRunnerProvider.ConfigureTestRunnerFor(this.SelectedRunnerType, _configArguments);
                _mruPathsStack.Push(_configArguments);
            }
        }
        public string ConfigArgumentsDescription
        {
            get
            {
                if (SelectedRunnerType == RunnerType.NUnitResultsFileWatcher)
                    return Strings.Config_NUnitGuiArgumentDescription;

                return Strings.Config_NUnitConsoleArgumentDescription;
            }
        }
        public string Error
        {
            get { return this["ConfigArguments"]; }
        }

        private RunnerType _selectedRunnerType;
        public RunnerType SelectedRunnerType
        {
            get { return _selectedRunnerType; }
            set 
            {
                if (value == _selectedRunnerType)
                    return;

                _selectedRunnerType = value;
                NotifyPropertyChangedFor("ConfigArgumentsDescription");
                ConfigArguments = string.Empty;
            }
        }

        public RunnerDTO[] RunnerTypes
        {
            get
            {
                return new[]
                           {
                               new RunnerDTO
                                    {Id = RunnerType.NUnitConsole, Description = "NUnit Console Runner"},
                               new RunnerDTO
                                    {Id = RunnerType.NUnitResultsFileWatcher, Description = "Watch NUnit Results File"}
                           };
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (SelectedRunnerType)
                {
                    case RunnerType.NUnitResultsFileWatcher:
                        if (ConfigArguments == String.Empty)
                            return Strings.Config_NUnitGuiArgumentsNotSpecified;

                        if (!_fileSystem.FolderExists(ConfigArguments))
                            return Strings.Config_TestResultsFolderDoesNotExist;
                        break;
                    case RunnerType.NUnitConsole:
                        if (ConfigArguments == String.Empty)
                            return Strings.Config_NUnitConsoleArgumentsNotSpecified;
                        break;
                }
                
                return null;
            }
        }

        public void LoadSettingsFrom(SettingsRepository settingsStore)
        {
            _mruPathsStack.LoadFrom(settingsStore);
        }

        public void SaveSettingsTo(SettingsRepository settingsStore)
        {
            _mruPathsStack.SaveTo(settingsStore);
        }

        public class RunnerDTO
        {
            public RunnerType Id { get; set; }

            public string Description { get; set; }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum RunnerType
    {
        NUnitConsole,
        NUnitResultsFileWatcher
    }
}