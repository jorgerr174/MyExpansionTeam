namespace MobileApp.Models.Shared
{
    public class FranchiseInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public FranchiseInfo(int id, string name, string abbreviation)
        {
            Id = id;
            Name = name;
            Abbreviation = abbreviation;
        }

        public static List<FranchiseInfo> GetAllFranchises()
        {
            return
            [
                new(1, "Arizona Cardinals", "ARI"),
                new(2, "Atlanta Falcons", "ATL"),
                new(3, "Baltimore Ravens", "BAL"),
                new(4, "Buffalo Bills", "BUF"),
                new(5, "Carolina Panthers", "CAR"),
                new(6, "Chicago Bears", "CHI"),
                new(7, "Cincinnati Bengals", "CIN"),
                new(8, "Cleveland Browns", "CLE"),
                new(9, "Dallas Cowboys", "DAL"),
                new(10, "Denver Broncos", "DEN"),
                new(11, "Detroit Lions", "DET"),
                new(12, "Green Bay Packers", "GB"),
                new(13, "Houston Texans", "HOU"),
                new(14, "Indianapolis Colts", "IND"),
                new(15, "Jacksonville Jaguars", "JAX"),
                new(16, "Kansas City Chiefs", "KC"),
                new(17, "Las Vegas Raiders", "LV"),
                new(18, "Los Angeles Chargers", "LAC"),
                new(19, "Los Angeles Rams", "LAR"),
                new(20, "Miami Dolphins", "MIA"),
                new(21, "Minnesota Vikings", "MIN"),
                new(22, "New England Patriots", "NE"),
                new(23, "New Orleans Saints", "NO"),
                new(24, "New York Giants", "NYG"),
                new(25, "New York Jets", "NYJ"),
                new(26, "Philadelphia Eagles", "PHI"),
                new(27, "Pittsburgh Steelers", "PIT"),
                new(28, "San Francisco 49ers", "SF"),
                new(29, "Seattle Seahawks", "SEA"),
                new(30, "Tampa Bay Buccaneers", "TB"),
                new(31, "Tennessee Titans", "TEN"),
                new(32, "Washington Commanders", "WAS")
            ];
        }
    }
}