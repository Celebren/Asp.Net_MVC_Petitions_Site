using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VoteWithYourWallet.Startup))]

namespace VoteWithYourWallet {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}