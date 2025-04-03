using Microsoft.Owin;
using Owin;
using System.Web.Services.Description;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using LoginTest.Models;

[assembly: OwinStartupAttribute(typeof(LoginTest.Startup))]
namespace LoginTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
