namespace METCore.DTOs.Shared
{
    public class ResultDto<T> : MessageDto
    {
        public T Value { get; set; }


        public ResultDto() { }

        public ResultDto(string message) : base(message) { }

        public ResultDto(string message, T obj)
            : base(message)
        {
            this.Value = obj;
        }
    }
}
