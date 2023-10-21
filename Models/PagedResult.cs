namespace CDNFreelancer.Models
{
    public class PagedResult<T>
    {
        public List<T> Results { get; set; }
        public int TotalCount { get; set; }
    }
}
