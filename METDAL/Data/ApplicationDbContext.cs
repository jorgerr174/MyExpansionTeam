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
            /*            modelBuilder.Entity<Team>()
                            .HasMany(t => t.Players)
                            .WithMany()
                            .UsingEntity<Dictionary<string, object>>(
                                "TeamPlayer",
                                tp => tp
                                    .HasOne<Player>()
                                    .WithMany()
                                    .HasForeignKey("PlayerId"),
                                tp => tp
                                    .HasOne<Team>()
                                    .WithMany()
                                    .HasForeignKey("TeamId"),
                                tp =>
                                {
                                    tp.ToTable("TeamPlayer");
                                    tp.HasKey("TeamId", "PlayerId");
                                    tp.HasIndex("TeamId");
                                }
                            );*/
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
            #endregion Team


            #region Trade
            modelBuilder.Entity<Trade>()
                .Property(e => e.TeamPlayers)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );

            modelBuilder.Entity<Trade>()
                .Property(e => e.TeamPicks)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );

            modelBuilder.Entity<Trade>()
                .Property(e => e.FranchisePlayers)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );

            modelBuilder.Entity<Trade>()
                .Property(e => e.FranchisePicks)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );
            #endregion Trade        
        }
    }
}
