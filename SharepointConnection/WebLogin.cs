using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.SharePoint.Client;
using PnP.Framework;

namespace ReferenceConfigurator
{
    class Weblogin
    {
        public static ClientContext GetWebLoginClientContext(string siteUrl, bool clearCookies, bool scriptErrorsSuppressed = true, Uri loginRequestUri = null, AzureEnvironment azureEnvironment = AzureEnvironment.Production)
        {

            var authCookiesContainer = new CookieContainer();
            var siteUri = new Uri(siteUrl);
            var cookieUrl = $"{siteUri.Scheme}://{siteUri.Host}";
            var thread = new Thread(() =>
            {
                if (clearCookies)
                {
                    CookieReader.SetCookie(cookieUrl, "FedAuth", "ignore;expires=Mon, 01 Jan 0001 00:00:00 GMT");
                    CookieReader.SetCookie(cookieUrl, "rtFa", "ignore;expires=Mon, 01 Jan 0001 00:00:00 GMT");
                    CookieReader.SetCookie(cookieUrl, "EdgeAccessCookie", "ignore;expires=Mon, 01 Jan 0001 00:00:00 GMT");
                }
                var form = new System.Windows.Forms.Form();

                var browser = new System.Windows.Forms.WebBrowser
                {
                    ScriptErrorsSuppressed = scriptErrorsSuppressed,
                    Dock = System.Windows.Forms.DockStyle.Fill
                };

                form.SuspendLayout();
                form.Icon = null;
                form.Width = 1024;
                form.Height = 768;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.Text = $"Log in to {siteUrl}";
                form.Controls.Add(browser);
                form.ResumeLayout(false);

                browser.Navigate(loginRequestUri ?? siteUri);

                browser.Navigated += (sender, args) =>
                {
                    if ((loginRequestUri ?? siteUri).Host.Equals(args.Url.Host))
                    {
                        var cookieString = CookieReader.GetCookie(siteUrl).Replace("; ", ",").Replace(";", ",");

                        // Get FedAuth and rtFa cookies issued by ADFS when accessing claims aware applications.
                        // - or get the EdgeAccessCookie issued by the Web Application Proxy (WAP) when accessing non-claims aware applications (Kerberos).
                        IEnumerable<string> authCookies = null;
                        if (Regex.IsMatch(cookieString, "FedAuth", RegexOptions.IgnoreCase))
                        {
                            authCookies = cookieString.Split(',').Where(c => c.StartsWith("FedAuth", StringComparison.InvariantCultureIgnoreCase) || c.StartsWith("rtFa", StringComparison.InvariantCultureIgnoreCase));
                        }
                        else if (Regex.IsMatch(cookieString, "EdgeAccessCookie", RegexOptions.IgnoreCase))
                        {
                            authCookies = cookieString.Split(',').Where(c => c.StartsWith("EdgeAccessCookie", StringComparison.InvariantCultureIgnoreCase));
                        }
                        if (authCookies != null)
                        {
                            // Set the authentication cookies both on the SharePoint Online Admin as well as on the SharePoint Online domains to allow for APIs on both domains to be used
                            //var authCookiesString = string.Join(",", authCookies);
                            //authCookiesContainer.SetCookies(siteUri, authCookiesString);
                            var extension = PnP.Framework.AuthenticationManager.GetSharePointDomainSuffix(azureEnvironment);
                            var cookieCollection = new CookieCollection();
                            foreach (var cookie in authCookies)
                            {
                                var cookieName = cookie.Substring(0, cookie.IndexOf("=")); // cannot use split as there might '=' in the value
                                var cookieValue = cookie.Substring(cookieName.Length + 1);
                                cookieCollection.Add(new Cookie(cookieName, cookieValue));
                            }
                            authCookiesContainer.Add(new Uri(cookieUrl), cookieCollection);
                            var adminSiteUri = new Uri(siteUri.Scheme + "://" + siteUri.Authority.Replace($".sharepoint.{extension}", $"-admin.sharepoint.{extension}"));
                            authCookiesContainer.Add(adminSiteUri, cookieCollection);
                            form.Close();
                        }
                    }
                };
                form.Focus();
                form.ShowDialog();
                browser.Dispose();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (authCookiesContainer.Count > 0)
            {
                var ctx = new ClientContext(siteUrl);

                ctx.DisableReturnValueCache = true;
                ctx.ExecutingWebRequest += (sender, e) =>
                {
                    e.WebRequestExecutor.WebRequest.CookieContainer = authCookiesContainer;
                };

                var settings = new PnP.Framework.Utilities.Context.ClientContextSettings();
                settings.Type = PnP.Framework.Utilities.Context.ClientContextType.Cookie;
                settings.AuthenticationManager = new PnP.Framework.AuthenticationManager();
                settings.AuthenticationManager.CookieContainer = authCookiesContainer;
                settings.SiteUrl = siteUrl;

                ctx.AddContextSettings(settings);
                return ctx;
            }

            return null;

        }

        internal static class CookieReader
        {
            /// <summary>
            /// Enables the retrieval of cookies that are marked as "HTTPOnly". 
            /// Do not use this flag if you expose a scriptable interface, 
            /// because this has security implications. It is imperative that 
            /// you use this flag only if you can guarantee that you will never 
            /// expose the cookie to third-party code by way of an 
            /// extensibility mechanism you provide. 
            /// Version:  Requires Internet Explorer 8.0 or later.
            /// </summary>
            private const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

            /// <summary>
            /// Returns cookie contents as a string
            /// </summary>
            /// <param name="url">Url to get cookie</param>
            /// <returns>Returns Cookie contents as a string</returns>
            public static string GetCookie(string url)
            {

                int size = 512;
                var sb = new StringBuilder(size);
                if (!NativeMethods.InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                {
                    if (size < 0)
                    {
                        return null;
                    }
                    sb = new StringBuilder(size);
                    if (!NativeMethods.InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                    {
                        return null;
                    }
                }
                return sb.ToString();
            }

            public static void SetCookie(string url, string cookiename, string cookiedata)
            {
                NativeMethods.InternetSetCookieEx(url, cookiename, cookiedata, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero);
            }

            private static class NativeMethods
            {

                [DllImport("wininet.dll", EntryPoint = "InternetGetCookieEx", CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern bool InternetGetCookieEx(
                    string url,
                    string cookieName,
                    StringBuilder cookieData,
                    ref int size,
                    int flags,
                    IntPtr pReserved);

                [DllImport("wininet.dll", EntryPoint = "InternetSetCookieEx", CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern bool InternetSetCookieEx(
                    string url,
                    string cookieName,
                    string cookieData,
                    int flags,
                    IntPtr pReserved);

            }
        }
    }

}