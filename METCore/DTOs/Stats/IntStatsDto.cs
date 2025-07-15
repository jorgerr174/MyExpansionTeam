namespace METCore.DTOs.Stats
{
    public class IntStatsDto(int INT, int TD, int Yds, int Lng)
    {
        public int INT { get; private set; } = INT;
        public int TD { get; private set; } = TD;
        public int Yds { get; private set; } = Yds;
        public int Lng { get; private set; } = Lng;
    }
}
