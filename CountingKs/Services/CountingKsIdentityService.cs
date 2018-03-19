namespace CountingKs.Services
{
    public class CountingKsIdentityService : ICountingKsIdentityService
    {
#if NoAuth
        public string CurrentUser => "qwerty";
#else
        public string CurrentUser => Thread.CurrentPrincipal.Identity.Name;
#endif
    }
}