namespace METCore.DTOs.Stats
{
    public class TackleStatsDto(int Comb, int Solo, int Asst, double Sck)
    {
        public int Comb { get; private set; } = Comb;
        public int Solo { get; private set; } = Solo;
        public int Asst { get; private set; } = Asst;
        public double Sck { get; private set; } = Sck;
    }
}
