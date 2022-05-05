namespace Service_for_metabase.Model;

public static class SpecialHttpClient
{
	public static HttpClient GetHttpClient()
	{
		var clientHandler = new HttpClientHandler()
		{
			ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
		};

		return new HttpClient(clientHandler);
	}
}