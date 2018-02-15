using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Web.JSON
{
	using J = JsonPropertyAttribute;
	public class OAuthResponse
    {
		[J("access_token")] public string AccessToken { get; set; }
		[J("token_type")] public string TokenType { get; set; }
		[J("expires_in")] public string ExpiresInSeconds { get; set; }
		[J("scope")] public string scopes { get; set; }
	}
}
