namespace Service_for_metabase.Model;

public class DatabaseModel
{
	public int DBID { get; set; }
	
	public string Name { get; set; }
	
	public string EmailManager { get; set; }
	
	public string Language { get; set; }
	
	public string Description { get; set; }

	public static DatabaseModel FromMetabaseDb(MetabaseDb metabaseDb)
	{
		return new DatabaseModel()
		{
			DBID = metabaseDb.DBID,
			Name = metabaseDb.Name,
			EmailManager = metabaseDb.EmailManager,
			Language = metabaseDb.Language,
			Description = metabaseDb.Description
		};
	} 
}