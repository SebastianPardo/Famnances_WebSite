namespace Famnances.Helpers
{
    public class Constants
    {
        public const string TOKEN = "TOKEN";
        public const string ACCOUNT_ID = "ACCOUNT";

#if DEBUG

        private const string AUTH_SERVICES_URI = "https://localhost:7238/Api/";
        private const string FAMNACES_SERVICES_URI = "https://localhost:7246/Api/";
#else
        public const string FAMNACES_SERVICES_URI = "";  
#endif
        
        public const string AUTH_URI = AUTH_SERVICES_URI + "Auth";
        public const string ACCOUNT_URI = AUTH_SERVICES_URI + "Account";

        public const string USER_URI = FAMNACES_SERVICES_URI + "User";
        public const string MANAGEMENT_URI = FAMNACES_SERVICES_URI + "Management";
        public const string ACCOUNTING_URI = FAMNACES_SERVICES_URI + "Accounting";
    }
}
