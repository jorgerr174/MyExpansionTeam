namespace METCore.DTOs.Shared
{
    public class SearchDto
    {
        public string Filter { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }


        public SearchDto() 
        {
            this.Filter = String.Empty;
        }

        public SearchDto(string Filter, int Page)
        {
            this.Filter = Filter;
            this.Page = Page;
        }
    }
}
