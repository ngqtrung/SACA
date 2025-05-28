namespace SACA_Common.DTOs
{
    public class Paging
    {
        public Paging()
        {
        }
        public virtual int page_size { get; set; } = 10;
        public int page_index { get; set; } = 1;
    }
    public class Paged : Paging
    {
        public int total_items { get; set; }
    }
}

