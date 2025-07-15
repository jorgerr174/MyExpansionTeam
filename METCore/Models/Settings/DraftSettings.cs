using System.ComponentModel.DataAnnotations;

namespace METCore.Models.Settings
{
    public class DraftSettings(bool[]? Selected)
    {
        [Required]
        [Length(32, 32)]
        public bool[] Selected { get; set; } = Selected ?? [];
    }
}
