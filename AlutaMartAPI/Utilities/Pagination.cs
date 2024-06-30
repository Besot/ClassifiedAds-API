namespace AlutaMartAPI.Utilities;
public class PagedList<T>
{
	public PagedList(List<T> items, int count, int page, int pageSize, string entityName = null)
	{
		int lastItem = pageSize * page;
		int firstItem = lastItem - pageSize + 1;
		lastItem = lastItem > count ? count : lastItem;
		firstItem = count == 0 ? 0 : firstItem;

		TotalCount = count;
		PageSize = pageSize;
		CurrentPage = page;
		TotalPages = (int)Math.Ceiling(count / (double)pageSize);
		Summary = count > 0 ? $"Showing {firstItem} to {lastItem} of {count} {entityName}" : "No record was found";
		ItemList = items;
		HasPrevious = CurrentPage > 1;
		HasNext = CurrentPage < TotalPages;
	}
	public IList<T> ItemList { get; set; }
	public int CurrentPage { get; set; }
	public int TotalPages { get; set; }
	public int PageSize { get; set; }
	public int TotalCount { get; set; }
	public string Summary { get; set; }
	public bool HasPrevious { get; set; }
	public bool HasNext { get; set; }
}