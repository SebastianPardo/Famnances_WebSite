namespace Famnances.Helpers
{
    public class Constants
    {
#if DEBUG

        public const string AUTH_SERVICES_URI = "https://localhost:7238/Api/";
        public const string FAMNACES_SERVICES_URI = "https://localhost:7246/Api/";
#else
        public const string FAMNACES_SERVICES_URI = "";  
#endif
        
        public const string ACCOUNT_URI = AUTH_SERVICES_URI + "Account";

        public const string USER_URI = FAMNACES_SERVICES_URI + "User";
    }
}
