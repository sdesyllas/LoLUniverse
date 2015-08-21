using NLog;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace LoLUniverse.Utilities
{
    public class EmailSender
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool SendEmail(string recipient, string subject, string body)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(ConfigurationManager.AppSettings["appEmail"]);
                msg.To.Add(new MailAddress(recipient));
                msg.Subject = subject;
                //msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));

                using (SmtpClient smtpClient = new SmtpClient())
                {
                    smtpClient.Send(msg);
                    logger.Debug($"Email send to {recipient} subject : {subject}");
                }
                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }
        }
    }
}