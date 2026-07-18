using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EaaSAPI.Models
{
    public class SmtpConfig
    {
        /// <summary>Host do servidor SMTP</summary>
        [Required]
        public string Host { get; set; } = string.Empty;

        /// <summary>Porta do servidor SMTP</summary>
        [Range(1, 65535)]
        public int Port { get; set; } = 587;

        /// <summary>Se true, usa SSL/TLS (porta 465). Se false, usa STARTTLS (porta 587)</summary>
        public bool Secure { get; set; }

        /// <summary>Usuário para autenticação</summary>
        [Required]
        public string User { get; set; } = string.Empty;

        /// <summary>Senha para autenticação</summary>
        [Required]
        public string Pass { get; set; } = string.Empty;
    }

    public class EmailMessage
    {
        /// <summary>Remetente (ex: "Nome <email@dominio.com>")</summary>
        [Required]
        public string From { get; set; } = string.Empty;

        /// <summary>Destinatário(s) separados por vírgula</summary>
        [Required]
        public string To { get; set; } = string.Empty;

        /// <summary>Assunto do e-mail</summary>
        [Required]
        public string Subject { get; set; } = string.Empty;

        /// <summary>Corpo HTML do e-mail</summary>
        [Required]
        public string Html { get; set; } = string.Empty;
    }

    public class SmtpSendRequest
    {
        /// <summary>Configurações do servidor SMTP</summary>
        [Required]
        [JsonPropertyName("smtpConfig")]
        public SmtpConfig SmtpConfig { get; set; } = new();

        /// <summary>Dados do e-mail a ser enviado</summary>
        [Required]
        [JsonPropertyName("email")]
        public EmailMessage Email { get; set; } = new();
    }
}
