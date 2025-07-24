using System.ComponentModel.DataAnnotations;

namespace METCore.Models.Settings
{
    public class DraftSettings
    {
        [Required]
        [Length(32, 32)]
        public bool[] Selected { get; set; }


        public DraftSettings(bool[]? Selected)
        {
            this.Selected = Selected ?? [];
        }
    }
}
