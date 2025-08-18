namespace METCore.DTOs.Shared
{
    public class SearchResultDto<T>
    {
        public IList<T> List { get; set; }
        public int Total { get; set; }


        public SearchResultDto()
        {
            this.List = [];
        }

        public SearchResultDto(IList<T> List, int Total)
        {
            this.List = [];
            this.Total = Total;
        }
    }
}
