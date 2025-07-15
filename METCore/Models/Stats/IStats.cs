using METCore.Interfaces.Importing;

namespace METCore.Models.Stats
{
    public interface IStats : IImportable
    {
        int Id { get; set; }
    }
}
