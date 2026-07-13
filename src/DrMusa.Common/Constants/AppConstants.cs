namespace DrMusa.Common.Constants;

public static class AppConstants
{
    public const string AppName = "DrMusa";
    public const string AppVersion = "1.0.0";
    public const string DatabaseFileName = "DrMusa.db";
    public const string DefaultCurrency = "PKR";
    public const string DefaultDateFormat = "dd/MM/yyyy";
    public const string DefaultDateTimeFormat = "dd/MM/yyyy HH:mm:ss";

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Cashier = "Cashier";
        public const string Manager = "Manager";
    }

    public static class SessionKeys
    {
        public const string CurrentUser = "CurrentUser";
        public const string CurrentRole = "CurrentRole";
    }
}
