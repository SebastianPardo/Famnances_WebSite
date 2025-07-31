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

        public const string ACCOUNTING_URI = FAMNACES_SERVICES_URI + "Accounting";
        public const string BUDGETS_URI = FAMNACES_SERVICES_URI + "Budgets";
        public const string CITIES_URI = FAMNACES_SERVICES_URI + "Cities";
        public const string COUNTRIES_URI = FAMNACES_SERVICES_URI + "Countries";
        public const string FIXED_EXPENSES_URI = FAMNACES_SERVICES_URI + "FixedExpenses";
        public const string FIXED_INCOMES_URI = FAMNACES_SERVICES_URI + "FixedIncomes";
        public const string INCOME_DISCOUNTS_URI = FAMNACES_SERVICES_URI + "IncomeDiscounts";
        public const string INFLOWS_URI = FAMNACES_SERVICES_URI + "Inflows";
        public const string OUTFLOWS_URI = FAMNACES_SERVICES_URI + "Outflows";
        public const string PERIODS_URI = FAMNACES_SERVICES_URI + "Periods";
        public const string PROVINCES_URI = FAMNACES_SERVICES_URI + "Provinces";
        public const string SAVINGS_URI = FAMNACES_SERVICES_URI + "Savings";
        public const string SAVINGS_POCKETS_URI = FAMNACES_SERVICES_URI + "SavingPockets";
        public const string SOCIALMEDIA_URI = FAMNACES_SERVICES_URI + "SocialMedia";
        public const string TOTALSBYPERIOD_URI = FAMNACES_SERVICES_URI + "TotalsByPeriod";
        public const string USER_URI = FAMNACES_SERVICES_URI + "Users";
    }
}
