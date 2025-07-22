﻿using CoolapkUNO.Helpers;
using CoolapkUNO.Pages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoolapkUNO
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeLogging();

            InitializeComponent();

#if HAS_UNO || NETFX_CORE
            Suspending += OnSuspending;
#endif
            UnhandledException += Application_UnhandledException;

#if !WINDOWS_UWP
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Themes/Generic.xaml") });
#endif
        }

        /// <summary>
        /// Gets the main window of the app.
        /// </summary>
        internal static Window MainWindow { get; private set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            EnsureWindow(args);
        }

        private void EnsureWindow(IActivatedEventArgs e)
        {
            if (MainWindow == null)
            {
                RegisterExceptionHandlingSynchronizationContext();
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    DebugSettings.EnableFrameRateCounter = true;
                }
#endif

#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
			    MainWindow = new Window();
			    MainWindow.Activate();
#else
                MainWindow = Windows.UI.Xaml.Window.Current;
#endif
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(MainWindow.Content is Frame rootFrame))
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                MainWindow.Content = rootFrame;

                ThemeHelper.Initialize();
            }

            if (e is LaunchActivatedEventArgs args)
            {
#if !(NET6_0_OR_GREATER && WINDOWS)
                if (!args.PrelaunchActivated)
                {
                    CoreApplication.EnablePrelaunch(true);
                }
                else { return; }
#endif
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e);
            }

            // Ensure the current window is active
            MainWindow.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {
#if DEBUG
			// Logging is disabled by default for release builds, as it incurs a significant
			// initialization cost from Microsoft.Extensions.Logging setup. If startup performance
			// is a concern for your application, keep this disabled. If you're running on web or
			// desktop targets, you can use url or command line parameters to enable it.
			//
			// For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

			var factory = LoggerFactory.Create(builder =>
			{
#if __WASM__
				builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ && !__MACCATALYST__
				builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
				builder.AddDebug();
#else
				builder.AddConsole();
#endif

				// Exclude logs below this level
				builder.SetMinimumLevel(LogLevel.Information);

				// Default filters for Uno Platform namespaces
				builder.AddFilter("Uno", LogLevel.Warning);
				builder.AddFilter("Windows", LogLevel.Warning);
				builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug);
                builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug);
                builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug);
                builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug);
                builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace);

                // Layouter specific messages
                builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug);
                builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug);
                builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug);

                builder.AddFilter("Windows.Storage", LogLevel.Debug);

                // Binding related messages
                builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug);
                builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug);

                // Binder memory references tracking
                builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug);

                // RemoteControl and HotReload related
                builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug);
            });

			global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
			global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
        }

        private void Application_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            if (!(!SettingsHelper.Get<bool>(SettingsHelper.ShowOtherException) || e.Exception is TaskCanceledException || e.Exception is OperationCanceledException))
            {
                //ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
                //UIHelper.ShowMessage($"{(string.IsNullOrEmpty(e.Exception.Message) ? loader.GetString("ExceptionThrown") : e.Exception.Message)} (0x{Convert.ToString(e.Exception.HResult, 16)})");
            }
            SettingsHelper.LogManager.GetLogger("Unhandled Exception - Application").Error(e.Exception.ExceptionToMessage(), e.Exception);
            e.Handled = true;
        }

        /// <summary>
        /// Should be called from OnActivated and OnLaunched
        /// </summary>
        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private void SynchronizationContext_UnhandledException(object sender, Helpers.UnhandledExceptionEventArgs e)
        {
            if (!(e.Exception is TaskCanceledException) && !(e.Exception is OperationCanceledException))
            {
                //ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
                //if (e.Exception is HttpRequestException || (e.Exception.HResult <= -2147012721 && e.Exception.HResult >= -2147012895))
                //{
                //    UIHelper.ShowMessage($"{loader.GetString("NetworkError")}(0x{Convert.ToString(e.Exception.HResult, 16)})");
                //}
                //else if (e.Exception is CoolapkMessageException)
                //{
                //    UIHelper.ShowMessage(e.Exception.Message);
                //}
                //else if (SettingsHelper.Get<bool>(SettingsHelper.ShowOtherException))
                //{
                //    UIHelper.ShowMessage($"{(string.IsNullOrEmpty(e.Exception.Message) ? loader.GetString("ExceptionThrown") : e.Exception.Message)} (0x{Convert.ToString(e.Exception.HResult, 16)})");
                //}
            }
            SettingsHelper.LogManager.GetLogger("Unhandled Exception - SynchronizationContext").Error(e.Exception.ExceptionToMessage(), e.Exception);
            e.Handled = true;
        }
    }
}
