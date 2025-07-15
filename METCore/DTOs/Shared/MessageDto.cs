namespace METCore.DTOs.Shared
{
    public class MessageDto
    {
        public string Message { get; set; }

        public MessageDto()
        {
            this.Message = string.Empty;
        }

        public MessageDto(string message)
        {
            this.Message = message;
        }
    }
}
