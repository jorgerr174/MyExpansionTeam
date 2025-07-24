using METCore.Models;
using METCore.Models.Players;
using METCore.Models.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace METDAL.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Franchise> Franchises { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<SeasonStats> SeasonStats { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Trade> Trades { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            #region Player
            modelBuilder.Entity<Player>().Ignore(p => p.FranchiseId);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.Stats)
                .WithOne()
                .HasForeignKey("PlayerId");

            modelBuilder.Entity<Player>()
                .OwnsMany(p => p.Contracts, contract =>
                {
                    contract.ToTable("Contracts");
                    contract.HasKey(c => c.Id);

                    contract.Property(c => c.Id);
                    contract.Property(c => c.FranchiseId);
                    contract.Property(c => c.YearSigned);
                    contract.Property(c => c.Length);
                    contract.Property(c => c.Total);
                    contract.Property(c => c.Guaranteed);
                    contract.Property(c => c.Active);

                    contract.Ignore(c => c.APY);
                    contract.Ignore(c => c.LastSeason);

                    contract.HasIndex(["FranchiseId", "Active"]);
                });

            modelBuilder.Entity<Player>()
                .OwnsOne(p => p.Prospect, prospect =>
                {
                    prospect.ToTable("Prospects");
                    prospect.HasKey(c => c.Id);

                    prospect.Property<int>("PlayerId");

                    prospect.Property(c => c.Year);
                    prospect.Property(c => c.Consensus);
                    prospect.Property(c => c.HandSize);
                    prospect.Property(c => c.ArmLength);
                    prospect.Property(c => c.Wingspan);
                    prospect.Property(c => c.FortyYardDash);
                    prospect.Property(c => c.BenchPress);
                    prospect.Property(c => c.VertJump);
                    prospect.Property(c => c.BroadJump);
                    prospect.Property(c => c.ThreeConeDrill);
                    prospect.Property(c => c.TwentyYardShuttle);
                    prospect.Property(c => c.AthScore);

                    prospect.Ignore(c => c.ImportAttributes);

                    prospect.HasIndex(["PlayerId"]);
                });
            #endregion Player


            #region SeasonStats
            modelBuilder.Entity<SeasonStats>()
                .HasIndex(["PlayerId", "Season"])
                .IsUnique();
            #endregion SeasonStats


            #region Franchise
            modelBuilder.Entity<Franchise>().ToTable(f => f.HasCheckConstraint("NoNewFranchises", "Id < 33"));

            //modelBuilder.Entity<Franchise>().Ignore(c => c.Players);
            modelBuilder.Entity<Franchise>()
                .HasMany(t => t.Players)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "FranchiseActivePlayers",
                    fap => fap
                        .HasOne<Player>()
                        .WithMany()
                        .HasForeignKey("PlayerId"),
                    fap => fap
                        .HasOne<Franchise>()
                        .WithMany()
                        .HasForeignKey("FranchiseId"),
                    fap =>
                    {
                        fap.ToView("FranchiseActivePlayers");
                        fap.HasKey("FranchiseId", "PlayerId");
                        fap.HasIndex("PlayerId").IsUnique();
                    }
                );

            modelBuilder.Entity<Franchise>().Navigation(f => f.Protected1);
            modelBuilder.Entity<Franchise>().Navigation(f => f.Protected2);
            modelBuilder.Entity<Franchise>().Navigation(f => f.Protected3);
            #endregion Franchise


            #region Team
            modelBuilder.Entity<Team>()
                .Property(e => e.PlayersIds)
                .HasConversion(
                    v => v != null && v.Any() ? string.Join(',', v) : string.Empty,
                    v => string.IsNullOrEmpty(v)
                        ? new List<int>()
                        : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(int.Parse).ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<IList<int>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            modelBuilder.Entity<Team>()
                .HasMany(p => p.Trades)
                .WithOne()
                .HasForeignKey("TeamId");

            modelBuilder.Entity<Team>()
                .Property(e => e.ProtectedPlayersIds)
                .HasConversion(
                    v => v != null && v.Any() ? string.Join(',', v) : string.Empty,
                    v => string.IsNullOrEmpty(v)
                        ? new List<int>()
                        : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(int.Parse).ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<IList<int>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            modelBuilder.Entity<Team>()
                .OwnsOne(t => t.RosterSettings, ts =>
                {
                    ts.Property(s => s.Cap).HasColumnName("Cap").HasColumnType("DECIMAL(3,2)");
                    ts.Property(s => s.MaxPerTeam).HasColumnName("MaxPerTeam");
                    ts.Property(s => s.ProtectedPerTeam).HasColumnName("ProtectedPerTeam");
                });

            modelBuilder.Entity<Team>()
                .Property(e => e.Selections)
                .HasConversion(
                    v => v != null && v.Any() ? string.Join(',', v.Select(kvp => $"{kvp.Key}:{kvp.Value}")) : string.Empty,
                    v => ConvertStringToSelections(v))
                .Metadata.SetValueComparer(
                    new ValueComparer<IDictionary<int, int>>(
                        (d1, d2) => d1 != null && d2 != null && d1.Count == d2.Count && d1.All(kvp => d2.ContainsKey(kvp.Key) && d2[kvp.Key] == kvp.Value),
                        d => d != null ? d.Aggregate(0, (hash, kvp) => HashCode.Combine(hash, kvp.Key.GetHashCode(), kvp.Value.GetHashCode())) : 0,
                        d => d != null ? d.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<int, int>()));

            modelBuilder.Entity<Team>()
                .Property(e => e.OffLineup)
                .HasConversion(
                    v => SerializeLineup(v),
                    v => DeserializeLineup<Lineup>(v))
                .Metadata.SetValueComparer(
                    new ValueComparer<Lineup>(
                        (l1, l2) => CompareLineups(l1, l2),
                        l => GetLineupHashCode(l),
                        l => CloneLineup(l)));

            modelBuilder.Entity<Team>()
                .Property(e => e.DefLineup)
                .HasConversion(
                    v => SerializeLineup(v),
                    v => DeserializeLineup<Lineup>(v))
                .Metadata.SetValueComparer(
                    new ValueComparer<Lineup>(
                        (l1, l2) => CompareLineups(l1, l2),
                        l => GetLineupHashCode(l),
                        l => CloneLineup(l)));

            modelBuilder.Entity<Team>()
                .Property(e => e.SPLineup)
                .HasConversion(
                    v => SerializeLineup(v),
                    v => DeserializeLineup<SPLineup>(v))
                .Metadata.SetValueComparer(
                    new ValueComparer<SPLineup>(
                        (l1, l2) => CompareLineups(l1, l2),
                        l => GetLineupHashCode(l == null ? new Lineup() : new Lineup { Formation = l.Formation, Player1 = l.Player1, Player2 = l.Player2, Player3 = l.Player3, Player4 = l.Player4, Player5 = l.Player5 }),
                        l => l == null ? new Lineup() : new SPLineup { Formation = l.Formation, Player1 = l.Player1, Player2 = l.Player2, Player3 = l.Player3, Player4 = l.Player4, Player5 = l.Player5 }));
            #endregion Team


            #region Trade
            modelBuilder.Entity<Trade>()
                .Property(e => e.TeamPlayers)
                .HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<IList<int>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            modelBuilder.Entity<Trade>()
                .Property(e => e.TeamPicks)
                .HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<IList<int>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            modelBuilder.Entity<Trade>()
                .Property(e => e.FranchisePlayers)
                .HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<IList<int>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            modelBuilder.Entity<Trade>()
                .Property(e => e.FranchisePicks)
                .HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<IList<int>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));
            #endregion Trade        
        }

        private static IDictionary<int, int> ConvertStringToSelections(string v)
        {
            if (string.IsNullOrEmpty(v))
                return new Dictionary<int, int>();

            var result = new Dictionary<int, int>();
            var pairs = v.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int key) && int.TryParse(parts[1], out int value))
                    {
                        result[key] = value;
                    }
                }
            }

            return result;
        }

        private static string SerializeLineup(SPLineup spLineup)
        {
            if (spLineup == null) return string.Empty;

            List<int> players = [spLineup.Player1, spLineup.Player2, spLineup.Player3, spLineup.Player4, spLineup.Player5];
            if (spLineup is Lineup lineup) players.AddRange([lineup.Player6, lineup.Player7, lineup.Player8, lineup.Player9, lineup.Player10, lineup.Player11]);

            var playerList = string.Join(",", players);
            return $"[{spLineup.Formation},{{{playerList}}}]";
        }

        private static T DeserializeLineup<T>(string lineupStr)
            where T : SPLineup, new()
        {
            if (string.IsNullOrEmpty(lineupStr)) return null;

            try
            {
                var trimmed = lineupStr.Trim('[', ']');
                var parts = trimmed.Split(',', 2);

                if (parts.Length != 2) return null;

                var formation = parts[0];
                var playersStr = parts[1].Trim('{', '}');
                var playerIds =
                    playersStr.Split(',')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => int.TryParse(s, out var id) ? id : 0)
                    .ToArray();

                var spLineup = new T { Formation = formation };

                spLineup.Formation = formation;
                if (playerIds.Length > 0) spLineup.Player1 = playerIds[0];
                if (playerIds.Length > 1) spLineup.Player2 = playerIds[1];
                if (playerIds.Length > 2) spLineup.Player3 = playerIds[2];
                if (playerIds.Length > 3) spLineup.Player4 = playerIds[3];
                if (playerIds.Length > 4) spLineup.Player5 = playerIds[4];
                if (spLineup is Lineup lineup)
                {
                    if (playerIds.Length > 5) lineup.Player6 = playerIds[5];
                    if (playerIds.Length > 6) lineup.Player7 = playerIds[6];
                    if (playerIds.Length > 7) lineup.Player8 = playerIds[7];
                    if (playerIds.Length > 8) lineup.Player9 = playerIds[8];
                    if (playerIds.Length > 9) lineup.Player10 = playerIds[9];
                    if (playerIds.Length > 10) lineup.Player11 = playerIds[10];
                }

                return spLineup;
            }
            catch
            {
                return null;
            }
        }

        private static bool CompareLineups(SPLineup spl1, SPLineup spl2)
        {
            if (spl1 == null && spl2 == null) return true;
            if (spl1 == null || spl2 == null) return false;
            bool result =
                spl1.Formation == spl2.Formation &&
                spl1.Player1 == spl2.Player1 &&
                spl1.Player2 == spl2.Player2 &&
                spl1.Player3 == spl2.Player3 &&
                spl1.Player4 == spl2.Player4 &&
                spl1.Player5 == spl2.Player5;

            return spl1 is Lineup l1 && spl2 is Lineup l2
                ? result &&
                    (l1.Player6 == l2.Player6 &&
                    l1.Player7 == l2.Player7 &&
                    l1.Player8 == l2.Player8 &&
                    l1.Player9 == l2.Player9 &&
                    l1.Player10 == l2.Player10 &&
                    l1.Player11 == l2.Player11)
                : result;
        }

        private static int GetLineupHashCode(Lineup lineup)
        {
            if (lineup == null) return 0;

            return HashCode.Combine(
                lineup.Formation,
                lineup.Player1,
                lineup.Player2,
                lineup.Player3,
                lineup.Player4,
                lineup.Player5,
                lineup.Player6,
                lineup.Player7);
        }

        private static Lineup CloneLineup(Lineup lineup)
        {
            if (lineup == null) return null;

            return new Lineup
            {
                Formation = lineup.Formation,
                Player1 = lineup.Player1,
                Player2 = lineup.Player2,
                Player3 = lineup.Player3,
                Player4 = lineup.Player4,
                Player5 = lineup.Player5,
                Player6 = lineup.Player6,
                Player7 = lineup.Player7,
                Player8 = lineup.Player8,
                Player9 = lineup.Player9,
                Player10 = lineup.Player10,
                Player11 = lineup.Player11
            };
        }
    }
}
