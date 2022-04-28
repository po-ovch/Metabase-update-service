namespace EntityLib;

public class MetabaseSystem
{
	public int DBID { get; set; }
        
	public int SystemId { get; set; }
        
	public int ElemNumber { get; set; }
	
	public int UpdateStatus { get; set; }
        
	public DateTime _date { get; set; }
	
	public string Elements { get; set; }
	
	public string SystemInfo { get; set; }

	public string? Description { get; set; }
	
	public override bool Equals(Object other)
	{
		var otherSystem = (MetabaseSystem) other;
		return DBID == otherSystem.DBID && SystemId == otherSystem.SystemId;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(DBID, SystemId);
	}
	
	public void Modify(MetabaseSystem system)
	{
		ElemNumber = system.ElemNumber;
		UpdateStatus = system.UpdateStatus;
		_date = system._date;
		Elements = system.Elements;
		SystemInfo = system.SystemInfo;
		Description = system.Description;
	}
}