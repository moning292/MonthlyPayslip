using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MonthlyPayslip.Startup))]
namespace MonthlyPayslip
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
