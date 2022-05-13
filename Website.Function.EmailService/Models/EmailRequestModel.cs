namespace Website.Function.EmailService.Models
{
    public class EmailRequestModel
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
    }
}
