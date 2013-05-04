// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System;
﻿using System.Diagnostics;
using System.Globalization;
using System.IO;
﻿using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
﻿using ManiaX.Beacons.Roles;
using ManiaX.Beacons.ViewModels;
﻿using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace ManiaX.Beacons
{
    
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "0.5.2.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(BeaconsToolWindow))]
    [Guid(GuidList.guidBeaconsPkgString)]
    public sealed partial class BeaconsPackage : Package, IDE, SettingsRepository
    {
        private const string BeaconsUserSettingsCollection = "Extensions\\Beacons";
        
        private MainViewModel _mainViewModel;
        private WritableSettingsStore _userSettingsStore;
       
        private UserNotifier _notifier;
        private TestRunnerProvider _testRunnerProvider;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public BeaconsPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        public UserNotifier Notifier
        {
            get { return _notifier; }
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            Logger.Log("Show tool window called");
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = GetToolWindow();
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            if (_mainViewModel != null)
                return;

            BindToolWindowToNewViewModel();
        }


        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidBeaconsCmdSet, (int)PkgCmdIDList.cmdidBeaconsShow);
                mcs.AddCommand( new MenuCommand(ShowToolWindow, toolwndCommandID) );

                CommandID toggleHatsCommandID = new CommandID(GuidList.guidBeaconsCmdSet, (int)PkgCmdIDList.cmdidToggleHat);
                mcs.AddCommand(new MenuCommand(ChangeHats, toggleHatsCommandID));

                CommandID togglePauseCommandID = new CommandID(GuidList.guidBeaconsCmdSet, (int)PkgCmdIDList.cmdidTogglePause);
                mcs.AddCommand(new MenuCommand(TogglePause, togglePauseCommandID));
            }


            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += SolveTheXAMLLoadingProblem;

                SubscribeForBuildEvents();

                SubscribeForSolutionEvents();

                var settingsManager = new ShellSettingsManager(this);
                _userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
                if (!_userSettingsStore.CollectionExists(BeaconsUserSettingsCollection))
                    _userSettingsStore.CreateCollection(BeaconsUserSettingsCollection);

                _notifier = new BeaconsTaskProvider(this);

                Logger.Log("Initialize Extension Called");

                BindToolWindowToNewViewModel();

                var closeListener = new WindowEventListener();
                closeListener.ToolWindowClosed += DeleteViewModel;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                    (GetToolWindow().Frame as IVsWindowFrame).SetProperty((int)__VSFPROPID.VSFPROPID_ViewHelper, new UnknownWrapper(closeListener)));
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }

        private void BindToolWindowToNewViewModel()
        {
            var mainWindow = GetToolWindow() as BeaconsToolWindow;
            ComposeMainViewModel();
            mainWindow.SetViewModel(_mainViewModel);
        }

        private void DeleteViewModel(object sender, EventArgs e)
        {
            _mainViewModel.TestRunnerConfigVM.SaveSettingsTo(this);
            _mainViewModel.TDDRhythmBeaconVM.SaveSettingsTo(this);

            _mainViewModel.Dispose();
            _mainViewModel = null;

            _testRunnerProvider.Dispose();
            _testRunnerProvider = null;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (_vsBuildManager != null)
                UnsubscribeForBuildEvents();
            if (_vsSolutionManager != null)
                UnsubscribeForSolutionEvents();
            AppDomain.CurrentDomain.AssemblyResolve -= SolveTheXAMLLoadingProblem;

            var notifier = _notifier as IDisposable;
            if (notifier != null)
                notifier.Dispose();

            base.Dispose(disposing);
            
        }

        #region IDE implementation
        public event EventHandler SolutionOpened;

        public event EventHandler SolutionClosed;

        public event EventHandler BuildInitiated;

        public event EventHandler BuildSucceeded;

        public event EventHandler BuildFailed;

        #endregion
        
        private ToolWindowPane GetToolWindow()
        {
            ToolWindowPane window = this.FindToolWindow(typeof(BeaconsToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Strings.CanNotCreateWindow);
            }
            return window;
        }

        private void ComposeMainViewModel()
        {
            try       
            {
                var codebaseStateTracker = new CodebaseStateTracker(this,
                                                                    new CsvStateRepository(Beacons.OutputFolderPath),
                                                                    new DotNetStopwatchWithUpdates());
                
                _testRunnerProvider = new TestRunnerProvider(new DefaultTestRunnerFactory(), this);
                FileSystem fileSystem = new WindowsFileSystem();
                var uiUpdater = new WpfDispatcher();
                var stateBeacon = new StateBeacon(_testRunnerProvider, codebaseStateTracker, this.Notifier, uiUpdater);
                
                var tddRhythmBeacon = new TDDRhythmBeacon(_testRunnerProvider, new DotNetStopwatch(), uiUpdater);
                var testRunnerConfiguration = new TestRunnerConfiguration(_testRunnerProvider, fileSystem, uiUpdater);
                _mainViewModel = new MainViewModel(stateBeacon, tddRhythmBeacon, testRunnerConfiguration, uiUpdater);

                
                testRunnerConfiguration.LoadSettingsFrom(this);
                tddRhythmBeacon.LoadSettingsFrom(this);

                if (testRunnerConfiguration.MRUPaths.Count > 0)
                    testRunnerConfiguration.ConfigArguments = testRunnerConfiguration.MRUPaths[0];
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }

        // If an assembly is loaded as part of XAML loading, then the current exe's folder is used for
        // probing i.e. devenv.exe. To avoid the assembly load failures, we hook in to assembly load failure and 
        // provide the requested dll from the extension home folder
        // http://social.msdn.microsoft.com/Forums/en-US/vsx/thread/a04b45d2-1d0a-4cfc-a0f0-1d458b2d6e26
        private static Assembly SolveTheXAMLLoadingProblem(object sender, ResolveEventArgs args)
        {
            try
            {
                if (args.Name.Contains("WpfGauge,"))
                    return LoadAssemblyFromExtensionFolder("WpfGauge.dll");

                if (args.Name.Contains("System.Windows.Controls.DataVisualization.Toolkit,"))
                    return LoadAssemblyFromExtensionFolder("System.Windows.Controls.DataVisualization.Toolkit.dll");

                if (args.Name.Contains("WPFToolkit,"))
                    return LoadAssemblyFromExtensionFolder("WPFToolkit.dll");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
            return null;
        }

        private static Assembly LoadAssemblyFromExtensionFolder(string dllName)
        {
            var folder = Uri.UnescapeDataString(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath));
            var pathToAssembly = Path.Combine(folder, dllName);
            return Assembly.LoadFrom(pathToAssembly);
        }

        #region VS Command Handlers
        private void TogglePause(object sender, EventArgs e)
        {
            try
            {
                _mainViewModel.TogglePauseCommand.Execute(null);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }
        }

        private void ChangeHats(object sender, EventArgs e)
        {
            try
            {
                _mainViewModel.TDDRhythmBeaconVM.ToggleRefactoringHat.Execute(null);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.ToString());
            }
        } 
        #endregion

        public string this[string key]
        {
            get
            {
                if (!_userSettingsStore.PropertyExists(BeaconsUserSettingsCollection, key))
                    return string.Empty;

                return _userSettingsStore.GetString(BeaconsUserSettingsCollection, key);
            }
            set { _userSettingsStore.SetString(BeaconsUserSettingsCollection, key, value); }
        }

    }
}
