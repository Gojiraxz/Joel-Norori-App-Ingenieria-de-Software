using System;


namespace Joel_Norori_App_Ingenieria_de_Software.Models.Settings
{
	public class JwtSettings
	{
		public string Key { get; set; } = string.Empty;
		public string Issuer { get; set; } = string.Empty;
		public string Audience { get; set; } = string.Empty;
		public int TokenExpirationInMinutes { get; set; } = 10;


    }
}
