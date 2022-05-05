namespace Service_for_metabase.Model;

public class MetabaseDbDto
{
	public int DBID { get; set; }
	
	public string Name { get; set; }
	
	public string EmailManager { get; set; }
	
	public string Language { get; set; }
	
	public string Description { get; set; }
	
	public string? ServiceHost { get; set; }

	public static MetabaseDbDto FromMetabaseDb(MetabaseDb metabaseDb)
	{
		return new MetabaseDbDto()
		{
			DBID = metabaseDb.DBID,
			Name = metabaseDb.Name,
			EmailManager = metabaseDb.EmailManager,
			Language = metabaseDb.Language,
			Description = metabaseDb.Description,
			ServiceHost = metabaseDb.DBServiceHost
		};
	} 
}