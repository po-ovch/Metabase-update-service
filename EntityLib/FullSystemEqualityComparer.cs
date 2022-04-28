namespace EntityLib;

public class FullSystemEqualityComparer: IEqualityComparer<MetabaseSystem>
{
	public bool Equals(MetabaseSystem x, MetabaseSystem y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (ReferenceEquals(x, null)) return false;
		if (ReferenceEquals(y, null)) return false;
		if (x.GetType() != y.GetType()) return false;
		return x.DBID == y.DBID && x.SystemId == y.SystemId && x.ElemNumber == y.ElemNumber && 
		       x.UpdateStatus == y.UpdateStatus && x.Elements == y.Elements && 
		       x.SystemInfo == y.SystemInfo && x.Description == y.Description;
	}

	public int GetHashCode(MetabaseSystem obj)
	{
		return HashCode.Combine(obj.DBID, obj.SystemId, obj.ElemNumber, 
			obj.UpdateStatus, obj._date, obj.Elements, obj.SystemInfo, 
			obj.Description);
	}
}