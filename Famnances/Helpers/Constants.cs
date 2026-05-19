namespace Famnances.Helpers
{
    public enum IncomeType
    {
        Fixed,
        Variable
    }

    public class Constants
    {
        public const string TOTAL = "TOTAL";

        public const string ERROR = "ERROR";

        public const string EMAIL = "EMAIL";
        public const string TOKEN = "TOKEN";
        public const string ACCOUNT_ID = "ACCOUNT";
     
        public const string DATE_FROM = "FROM";
        public const string DATE_TO = "TO";
        public const string SAVINGS = "SAVINGS";
        public const string CHEQUING = "CHEQUING";

        //Temporal
#if DEBUG
        public const string AUTH_SERVICES_URI = "https://localhost:7048/auth-services/";
        public const string FAMNACES_SERVICES_URI = "https://localhost:7048/famnances-services/";
        //public const string AUTH_SERVICES_URI = "https://localhost:7238/Api/";
        //public const string FAMNACES_SERVICES_URI = "https://localhost:7246/Api/";
#else
        public const string AUTH_SERVICES_URI = "https://frustrate-affiliate-devourer.ngrok-free.dev/auth-services/";
        public const string FAMNACES_SERVICES_URI = "https://frustrate-affiliate-devourer.ngrok-free.dev/famnances-services/";
#endif

        public const string AUTH_URI = "Auth";
        public const string ACCOUNT_URI = "Account";

        public const string ACCOUNTING_URI = "Accounting";
        public const string BUDGETS_URI = "Budgets";
        public const string BUDGET_TYPES_URI = "BudgetTypes";
        public const string CITIES_URI = "Cities";
        public const string COUNTRIES_URI = "Countries";
        public const string FIXED_EXPENSES_URI = "FixedExpenses";
        public const string FIXED_INCOMES_URI = "FixedIncomes";
        public const string INCOME_DISCOUNTS_URI = "IncomeDiscounts";
        public const string INFLOWS_URI = "Inflows";
        public const string OUTFLOWS_URI = "Outflows";
        public const string PERIODS_URI = "Periods";
        public const string PROVINCES_URI = "Provinces";
        public const string SAVING_SOURCES_URI = "SavingSources";
        public const string SAVINGS_URI = "Savings";
        public const string FIXED_SAVINGS_URI = "FixedSavings";
        public const string SAVINGS_POCKETS_URI = "SavingPockets";
        public const string SOCIALMEDIA_URI = "SocialMedia";
        public const string TOTALSBYPERIOD_URI = "TotalsByPeriod";
        public const string USER_URI = "Users";
        public const string HOME_URI = "Homes";
        public const string ERROR_LOG_URI = "ErrorLog";
    }
}
