using MailKit.Net.Smtp;
using MimeKit;

namespace AmarantaAPI.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _config;

        public EmailHelper(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            var email = _config["EmailSettings:Email"];
            var appPassword = _config["EmailSettings:AppPassword"];

            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Amaranta", email));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;
            mensaje.Body = new TextPart("plain") { Text = cuerpo };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.Auto);
                await smtp.AuthenticateAsync(email, appPassword);
                await smtp.SendAsync(mensaje);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
