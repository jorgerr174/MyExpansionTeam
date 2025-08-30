using System.ComponentModel;
using static METCore.Enums.Types;

namespace METCore.DTOs.Admin
{
    public class ImportDto : Shared.FileDto
    {
        [DisplayName("Tipo")]
        public ImportEnum Type { get; set; }

        [DisplayName("Tipo de estadística")]
        public StatsEnum StatsType { get; set; }

        [DisplayName("Temporada")]
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
