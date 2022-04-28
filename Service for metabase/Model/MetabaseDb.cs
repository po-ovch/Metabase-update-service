using System.ComponentModel.DataAnnotations;

namespace Service_for_metabase.Model;

public class MetabaseDb
{
	[Key]
	public int DBID { get; set; }
	
	public Boolean Enabled { get; set; }
	
	public DateTime LastUpdate { get; set; }
	
	public string Name { get; set; }
	
	public string Login { get; set; }
	
	public string Password { get; set; }
	
	public string DBURL { get; set; }
	
	public string DBGateRedirect { get; set; }
	
	public string DBGateURL { get; set; }
	
	public string EmailManager { get; set; }
	
	public string WWWTemplatePage { get; set; }
	
	public string Language { get; set; }
	
	public string Description { get; set; }
	
	public string? DBServiceHost { get; set; }
}