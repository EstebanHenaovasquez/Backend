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

        /// <summary>
        /// Envía un correo usando la plantilla HTML de Amaranta.
        /// </summary>
        public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string codigo, string mensajePersonalizado = "")
        {
            var email = _config["EmailSettings:Email"];
            var appPassword = _config["EmailSettings:AppPassword"];

            // Ruta de la plantilla HTML
            var rutaPlantilla = Path.Combine(Directory.GetCurrentDirectory(), "Template", "correo.html");
            if (!File.Exists(rutaPlantilla))
                throw new FileNotFoundException("No se encontró la plantilla de correo.", rutaPlantilla);

            // Cargar el HTML y reemplazar los marcadores
            string cuerpoHtml = await File.ReadAllTextAsync(rutaPlantilla);
            cuerpoHtml = cuerpoHtml
                .Replace("{{TITULO}}", asunto)
                .Replace("{{MENSAJE}}", mensajePersonalizado)
                .Replace("{{CODIGO}}", codigo);

            // Crear el mensaje MIME
            var mensajeCorreo = new MimeMessage();
            mensajeCorreo.From.Add(new MailboxAddress("Amaranta", email));
            mensajeCorreo.To.Add(MailboxAddress.Parse(destinatario));
            mensajeCorreo.Subject = asunto;
            mensajeCorreo.Body = new TextPart("html") { Text = cuerpoHtml };

            // Enviar por SMTP
            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(email, appPassword);
                await smtp.SendAsync(mensajeCorreo);
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
