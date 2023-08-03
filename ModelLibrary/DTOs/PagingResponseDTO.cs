

namespace ModelLibrary.DTOs
{
    public class PagingResponseDTO<T>
    {
        public PagingResponseDTO()
        {
            this.PageSize = 15;
        }

        public int MaxResults
        {
            get; set;
        }
        public int StartAt
        {
            get; set;
        }
        public int Total
        {
            get; set;
        }
        public int PageIndex
        {
            get; set;
        }
        public int PageSize
        {
            get; set;
        }
        public ICollection<T> Values
        {
            get; set;
        }
    }
}
