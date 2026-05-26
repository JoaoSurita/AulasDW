using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using MimeKit.Text;
using VasosInteligentes.Settings;

namespace VasosInteligentes.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;    
        }
        public async Task SendEmailAsync(string ToEmail, 
            string subject, string message)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName,
                    _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(ToEmail));
                email.Subject = subject;

                email.Body = new TextPart(TextFormat.Html) { Text = message };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer,
                    _emailSettings.SmtpPort, 
                    MailKit.Security.SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_emailSettings.Username, 
                    _emailSettings.Password);

                await smtp.SendAsync(email);

                await smtp.DisconnectAsync(true);
                Console.WriteLine("Enviei o e-mail");
            }
            catch (Exception ex)
            {
                // Logue o erro para diagnosticar depois
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
                // Opcionalmente, lance a exceção para ser tratada mais acima
                throw;
            }

        }
            
        

    }//classe
}//namespace
