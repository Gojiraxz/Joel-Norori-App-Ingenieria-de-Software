using System;

namespace Joel_Norori_App_Ingenieria_de_Software.Models.Settings
{
	public class SmtpSettings()
	{
		public string Host { get; set; } = string.Empty;

		public int Port { get; set; }

		public bool EnableSsl { get; set; }

		public string User { get; set; } = null!;

        public string Password { get; set; } = null!;

    }
}
