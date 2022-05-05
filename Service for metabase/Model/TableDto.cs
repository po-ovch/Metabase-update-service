using System.Reflection;

namespace Service_for_metabase.Model;

public class TableDto<T>
{
	public IEnumerable<PropertyInfo> Columns { get; set; }
	
	public IEnumerable<T> Items { get; set; }

	public static TableDto<T> BuildModel(IEnumerable<T> properties)
	{
		return new TableDto<T>
		{
			Columns = typeof(T).GetProperties()
				.Where(col => col.Name != "UpdateStatus"),
			Items = properties
		};
	}
}