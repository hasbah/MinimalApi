namespace MinimalApi.Core
{
    public class EmailResult
    {
        public EmailResult()
        {
            ErrorMessage = new List<string>();
        }

        public bool IsSuccess { get; set; }
        public List<string> ErrorMessage { get; set; }
    }
}
