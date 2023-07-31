namespace FlexibleDataApi.Models
{
	public class Statistics
	{
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public int Count { get; set; }
        public string UniqueValues { get; set; } = string.Empty;
	}
}

