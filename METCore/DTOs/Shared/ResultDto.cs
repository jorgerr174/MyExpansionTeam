namespace METCore.DTOs.Shared
{
    public class ResultDto<T>(string message, T obj) : MessageDto(message)
    {
        public T Value { get; set; } = obj;
    }
}
