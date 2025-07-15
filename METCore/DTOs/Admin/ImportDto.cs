using static METCore.Enums.Types;

namespace METCore.DTOs.Admin
{
    public class ImportDto : Shared.FileDto
    {
        public ImportEnum Type { get; set; }
        public StatsEnum StatsType { get; set; }
        public int? Year { get; set; }


        public ImportDto() : base() { }

        public ImportDto(ImportEnum Type, StatsEnum StatsType, int? Year)
            : base()
        {
            this.Type = Type;
            this.StatsType = StatsType;
            this.Year = Year;
        }
    }
}
