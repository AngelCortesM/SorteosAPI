public class PaginatedResult<T>
{
    public List<T> Data { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }

    public PaginatedResult()
    {
        Data = new List<T>();
    }
}