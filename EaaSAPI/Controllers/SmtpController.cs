using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EaaSAPI.Models;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.RateLimiting;

namespace EaaSAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class SmtpController : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SmtpSendRequest request)
        {
            var message = new MimeMessage();

            try
            {
                message.From.Add(MailboxAddress.Parse(request.Email.From));

                foreach (var addr in request.Email.To.Split(',', StringSplitOptions.TrimEntries))
                    message.To.Add(MailboxAddress.Parse(addr));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = "Erro ao processar os remetentes/destinatarios.", details = ex.Message});
            }

            message.Subject = request.Email.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = request.Email.Html
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                var socketOptions = request.SmtpConfig.Secure
                    ? MailKit.Security.SecureSocketOptions.SslOnConnect
                    : MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable;
 
                await client.ConnectAsync(
                    request.SmtpConfig.Host,
                    request.SmtpConfig.Port,
                    socketOptions
                    );

                await client.AuthenticateAsync(request.SmtpConfig.User, request.SmtpConfig.Pass);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);

                return Ok(new { success = true, message = "Email enviado com sucesso." });
            }
            catch (MailKit.Security.AuthenticationException)
            {
                return BadRequest(new { error = "Falha na autenticação SMTP. Verifique o usuário e a senha fornecidos." });
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                return StatusCode(503, new { error = "Não foi possível conectar ao servidor SMTP informado.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro interno ao tentar enviar o e-mail.", details = ex.Message });
            }
        }
    }
}
