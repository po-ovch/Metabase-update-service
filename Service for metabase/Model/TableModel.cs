using System.Reflection;

namespace Service_for_metabase.Model;

public class TableModel<T>
{
	public IEnumerable<PropertyInfo> Columns { get; set; }
	
	public IEnumerable<T> Items { get; set; }

	public static TableModel<T> BuildModel(IEnumerable<T> properties)
	{
		return new TableModel<T>
		{
			Columns = typeof(T).GetProperties()
				.Where(col => col.Name != "UpdateStatus"),
			Items = properties
		};
	}
}