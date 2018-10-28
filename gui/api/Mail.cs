using System.Net.Mail;
using System.Text;

namespace dfbanka.gui.api
{
    internal class Mail
    {
        public static void Send(string serverAddress, int port, string userName, string password, string to)
        {
            using (var mail = new MailMessage(userName, to))
            {
                mail.Subject = "Test";
                mail.Body = "Ahoj, test";
                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = Encoding.UTF8;

                using (var client = new SmtpClient(serverAddress, port))
                {
                    client.UseDefaultCredentials = false;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    client.Timeout = 30000;
                    client.Credentials = new System.Net.NetworkCredential(userName, password);
                    client.Send(mail);
                }
            }
        }
    }
}