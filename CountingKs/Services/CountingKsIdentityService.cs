using System.Threading;

namespace CountingKs.Services
{
    public class CountingKsIdentityService : ICountingKsIdentityService
    {
        //public string CurrentUser => "shawnwildermuth";
        public string CurrentUser => Thread.CurrentPrincipal.Identity.Name;
    }
}