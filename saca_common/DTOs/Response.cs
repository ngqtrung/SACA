namespace SACA_Common.DTOs
{
    public class Response<T>
    {
        public Response()
        {

        }

        public Response(T result)
        {
            Result = result;
        }

        public T? Result { get; set; }

        public string[]? Errors { get; set; }

        public string? Message { get; set; }
    }

    public class PagedResponse<T> : Paged
    {
        public PagedResponse()
        {

        }

        public PagedResponse(List<T> items)
        {
            Items = items;
        }
        public List<T>? Items { get; set; }
    }
    public class ExtendedPageResponse<T> : PagedResponse<T>
    {
        public object? metadata { get; set; }
    }
    public class CreateResult
    {
        public CreateResult(string created_id)
        {
            id = created_id;
        }
        public string id { get; set; }
    }

    public static class PagedExtensions
    {
        public static IQueryable<T> Paged<T>(this IQueryable<T> query, int index, int size)
        {
            if (index < 1) index = 1;
            return query.Skip((index - 1) * size).Take(size);
        }
    }
}

