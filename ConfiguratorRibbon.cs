using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Office = Microsoft.Office.Core;
using ReferenceConfigurator.utils;
using ReferenceConfigurator.mainWindow;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Diagnostics;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new ConfiguratorRibbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace ReferenceConfigurator {
    [ComVisible(true)]
    public class ConfiguratorRibbon : Office.IRibbonExtensibility {
        private Office.IRibbonUI ribbon;

        System.Windows.Application _app;

        public ConfiguratorRibbon() {
        }

        public void openPopUp(Office.IRibbonControl control) {
            try {
                // if it's the first time run, initialize an application so the resourcedictionary is loaded by initializeComponent.
                //if (System.Windows.Application.Current == null) {
                if (_app == null) {
                    // initialize an wpf application and load its resources dictionaries and take control of app's shutdown
                    if (Application.Current == null) {
                        _app = new App {
                            ShutdownMode = ShutdownMode.OnExplicitShutdown
                        };
                    } else
                        _app = Application.Current;
                }
                // initialize mainwindow if it's the first time to call this
                if (_app.MainWindow == null) {
                    try {
                        _app.MainWindow = new MainWindow();
                    } catch (Exception ex) {
                        int a = 0;
                    }
                    _app.MainWindow.Closing += (s1, e) => {
                        Dispatcher.ExitAllFrames();
                    };
                }

                // bring main window to front 
                _app.MainWindow.Show();
                _app.MainWindow.Activate();

                Dispatcher.Run();

                //_app.Run();

                //var window = new MainWindow() {
                //    ShowInTaskbar = false,
                //};
                //new WindowInteropHelper(window) {
                //    Owner = Process.GetCurrentProcess().MainWindowHandle
                //};

                //window.Show();
                //window.Activate();
            } catch (Exception e) {
                int a = 0;
            }
            return;
        }

        public Bitmap loadPopUpImage(Office.IRibbonControl control) {
            var result = Utils.LoadImageResource("ReferenceConfigurator.icons.PopUp.png");
            if (result == null) {
                result = Utils.LoadImageResource("ReferenceConfigurator.icons.question.png");
            }
            return result;
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID) {
            return GetResourceText("ReferenceConfigurator.ConfiguratorRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI) {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName) {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i) {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0) {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i]))) {
                        if (resourceReader != null) {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
