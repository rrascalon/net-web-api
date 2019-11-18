using Net.Web.Api.Sdk.Initialization;
using System;
using System.Web;
using System.Web.Http;

namespace Net.Web.Api.Sdk.Web.Examples
{
    /// <summary>
    /// Class Global.
    /// Implements the <see cref="HttpApplication" />
    /// </summary>
    /// <seealso cref="HttpApplication" />
    public class Global : HttpApplication
    {
        #region Protected Methods

        /// <summary>
        /// Handles the Start event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configuration.RegisterWebApi();
        }

        /// <summary>
        /// Handles the End event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_End(object sender, EventArgs e)
        {
            GlobalConfiguration.Configuration.UnRegisterWebApi();
        }

        #endregion
    }
}