namespace Service_for_metabase.Model;

[Serializable]
public class ServiceUser
{
	public string Username { get; set; }
	
	public string Password { get; set; }
	
	public string Role { get; set; }
}