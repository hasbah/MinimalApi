namespace MinimalApi.Core
{
    public class Constants
    {
        public const int ExpireDays = 7;
        public const string Admin = "Admin";
        public const string User = "User";

        public static string GetRegistrationEmailBody(string url)
        {
            return  $"Your registration was successful. <br/><br/> " +
                    $"Please confirm your email by clicking the <a href='{url}'>link.</a> <br/><br/>" +
                    $"If the link doesn't work, please, copy the following URL and paste it in the address bar:<br/><br/>{url}";
        }
        public static string GetForgotPasswordEmailBody(string url)
        {
            return  $"You have requested to reset your password. <br/><br/>" +
                    $"To proceed, please click on the following <a href='{url}'>link</a> <br/><br/>" +
                    $"If the link doesn't work, please, copy the following URL and paste it in the address bar:<br/><br/>{url}";
        }
    }
}
