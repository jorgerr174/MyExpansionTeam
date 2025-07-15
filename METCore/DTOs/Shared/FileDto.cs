using Microsoft.AspNetCore.Http;

namespace METCore.DTOs.Shared
{
    public abstract class FileDto
    {
        public IFormFile File { get; set; }


        public FileDto() { }

        public FileDto(IFormFile File)
        {
            this.File = File;
        }
    }
}
