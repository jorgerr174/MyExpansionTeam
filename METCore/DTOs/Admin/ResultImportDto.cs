using static METCore.Enums.Types;

namespace METCore.DTOs.Admin
{
    public class ResultImportDto
    {
        public Byte[] Content { get; set; } = [];

        public ImportEnum Type { get; set; } = ImportEnum.None;
    }
}
