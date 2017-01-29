using System.Web;
using System.Web.Http;

namespace Netgular.Examples.WebApi
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
