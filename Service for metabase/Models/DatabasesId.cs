namespace Service_for_metabase.Models
{
	public static class DatabasesId
	{
		private const int Crystal = 1;
		private const int BandGap = 3;

		public static int GetByName(string dbName)
		{
			return dbName switch
			{
				"crystal" => Crystal,
				"bandgap" => BandGap,
				_ => throw new ArgumentException()
			};
		}
	}
}