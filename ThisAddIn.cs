﻿using System;
using System.Security;
using System.Net;
using System.Linq;
using Microsoft.SharePoint.Client;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using ReferenceConfigurator.Properties;
using PnP.Framework;


namespace ReferenceConfigurator {
    public partial class ThisAddIn {
        private void ThisAddIn_Startup(object sender, System.EventArgs e) {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e) {
            if (System.Windows.Application.Current != null) {
                System.Windows.Application.Current.Shutdown();
            }
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject() {
            return new ConfiguratorRibbon();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup() {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }



}
