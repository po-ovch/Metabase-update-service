namespace EntityLib;

public class FullPropertyEqualityComparer : IEqualityComparer<MetabaseProperty>
{
	public bool Equals(MetabaseProperty? x, MetabaseProperty? y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (ReferenceEquals(x, null)) return false;
		if (ReferenceEquals(y, null)) return false;
		if (x.GetType() != y.GetType()) return false;
		return x.DBID == y.DBID && x.PropId == y.PropId && x.Name == y.Name && 
		       x.Description == y.Description && x.WWWTemplatePage == y.WWWTemplatePage && 
		       x.UpdateStatus == y.UpdateStatus;
	}

	public int GetHashCode(MetabaseProperty obj)
	{
		return HashCode.Combine(obj.DBID, obj.PropId, obj.Name, obj.Description, 
			obj.WWWTemplatePage, obj.UpdateStatus);
	}
}