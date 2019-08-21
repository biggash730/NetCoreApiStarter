namespace NetCoreStarter.Utils
{
    public static class AppConfig
    {
        public static Setting Setting { get; set; }
    }

    public class Setting
    {
        public PasswordPolicies PasswordPolicies { get; set; }
        public MessageService MessageService { get; set; }
    }

    public class PasswordPolicies
    {
        public int MinimumLength { get; set; }
        public int RequiredUniqueChars { get; set; }
        public string SpecialCharacters { get; set; }
        public int PreviousPasswordCount { get; set; }
        public bool RequireNonLetterOrDigit { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
    }

    public class MessageService
    {
        public string SenderId { get; set; }
        public string BaseUrl { get; set; }
        public string SendMessageUrl { get; set; }
        public string CheckBalanceUrl { get; set; }
        public string ApiKey { get; set; }
        public bool IsActive { get; set; }
    }

}
