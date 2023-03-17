using Microsoft.Graph.ExternalConnectors;
using Microsoft.SharePoint.Client;
using ReferenceConfigurator.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;

namespace ReferenceConfigurator
{
    class SharepointConnection
    {

        private static ClientContext createClientContext()
        {
            ClientContext ctx = Weblogin.GetWebLoginClientContext(Properties.Settings.Default.url, false);
            return ctx;
        }


        public static ClientContext GetClientContext()
        {
            return SharepointConnection.createClientContext();
        }

        public static ListItemCollection getSharepointList()
        {
            ClientContext ctx = GetClientContext();
            List list = ctx.Web.Lists.GetByTitle("Project list");
            ListItemCollection itemColl = list.GetItems(CamlQuery.CreateAllItemsQuery());
            ctx.Load(itemColl);
            ctx.ExecuteQuery();
            return itemColl;
        }



    }
}
