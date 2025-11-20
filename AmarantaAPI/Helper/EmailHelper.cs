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

            // Plantilla HTML embebida para evitar errores de archivo
            string cuerpoHtml = @"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>{{TITULO}}</title>
</head>
<body style=""font-family: Georgia, 'Times New Roman', serif; background-color: #f9fafb; padding: 30px; color: #1c1917;"">
    <div style=""max-width: 500px; margin: auto; background: #fffef9; border-radius: 12px; box-shadow: 0 4px 15px rgba(180, 83, 9, 0.3); padding: 2rem; text-align: center;"">

        <!-- Encabezado -->
        <h1 style=""color: #92400e; font-size: 2rem; font-weight: bold; margin-bottom: 1rem;"">Amaranta</h1>

        <!-- Título del mensaje -->
        <h2 style=""color: #b45309; font-size: 1.3rem; margin-bottom: 1rem;"">{{TITULO}}</h2>

        <!-- Texto principal -->
        <p style=""font-size: 1rem; color: #3f3f3f; margin-bottom: 1.5rem;"">{{MENSAJE}}</p>

        <!-- Código -->
        <p style=""font-size: 2rem; font-weight: bold; color: #92400e; letter-spacing: 4px; margin-bottom: 1.5rem;"">{{CODIGO}}</p>

        <!-- Pie -->
        <p style=""font-size: 0.9rem; color: #5b4636;"">Este código expirará en pocos minutos. No lo compartas con nadie.</p>
        <hr style=""border: none; border-top: 1px solid #e5e7eb; margin: 2rem 0;"" />
        <p style=""font-size: 0.8rem; color: #9ca3af;"">© 2025 Amaranta. Todos los derechos reservados.</p>
    </div>
</body>
</html>";

            // Reemplazar los marcadores
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
            catch (Exception ex)
            {
                // Opcional: Log del error
                Console.WriteLine($"Error enviando correo: {ex.Message}");
                return false;
            }
        }
    }
}