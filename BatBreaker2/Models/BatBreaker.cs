namespace BatBreaker2.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BatBreakerContext : DbContext
    {
        public BatBreakerContext()
            : base("name=BatBreaker")
        {
        }

        public virtual DbSet<AtBat> AtBats { get; set; }
        public virtual DbSet<BaseRunning> BaseRunnings { get; set; }
        public virtual DbSet<BattingResult> BattingResults { get; set; }
        public virtual DbSet<BattingResultType> BattingResultTypes { get; set; }
        public virtual DbSet<DefensivePlay> DefensivePlays { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<FirstName> FirstNames { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Innings> Innings { get; set; }
        public virtual DbSet<LastName> LastNames { get; set; }
        public virtual DbSet<League> Leagues { get; set; }
        public virtual DbSet<Lineup> Lineups { get; set; }
        public virtual DbSet<Pitch> Pitches { get; set; }
        public virtual DbSet<PitchesThrown> PitchesThrowns { get; set; }
        public virtual DbSet<PitchResultType> PitchResultTypes { get; set; }
        public virtual DbSet<PlayerAttribute> PlayerAttributes { get; set; }
        public virtual DbSet<PlayerPitch> PlayerPitches { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<RunsScored> RunsScoreds { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<TeamName> TeamNames { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AtBat>()
                .HasMany(e => e.BaseRunnings)
                .WithRequired(e => e.AtBat)
                .HasForeignKey(e => e.abId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AtBat>()
                .HasMany(e => e.DefensivePlays)
                .WithRequired(e => e.AtBat)
                .HasForeignKey(e => e.abId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AtBat>()
                .HasMany(e => e.PitchesThrowns)
                .WithRequired(e => e.AtBat)
                .HasForeignKey(e => e.abId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AtBat>()
                .HasMany(e => e.RunsScoreds)
                .WithRequired(e => e.AtBat)
                .HasForeignKey(e => e.abId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BattingResult>()
                .HasMany(e => e.AtBats)
                .WithRequired(e => e.BattingResult)
                .HasForeignKey(e => e.resultId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.OutRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.SingleRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.DoubleRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.TripleRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.HRRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.DPRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.TPRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerThirdScoreRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerSecondScoreRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerSecondToThirdRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerFirstScoreRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerFirstToThirdRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerFirstToSecondRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.OutOfPlayRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerFirstExtraBases)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerSecondExtraBases)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerThirdExtraBases)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerFirstBasesOnOut)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerSecondBasesOnOut)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.RunnerThirdBasesOnOut)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.Bases)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.ExtraBasesOnError)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .Property(e => e.ErrorRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<BattingResultType>()
                .HasMany(e => e.AtBats)
                .WithRequired(e => e.BattingResultType)
                .HasForeignKey(e => e.resultTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Division>()
                .HasMany(e => e.Teams)
                .WithRequired(e => e.Division)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.AtBats)
                .WithRequired(e => e.Game)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.Innings)
                .WithRequired(e => e.Game)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.Lineups)
                .WithRequired(e => e.Game)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.RunsScoreds)
                .WithRequired(e => e.Game)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Innings>()
                .HasMany(e => e.AtBats)
                .WithRequired(e => e.Innings)
                .HasForeignKey(e => e.inningId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Innings>()
                .HasMany(e => e.RunsScoreds)
                .WithRequired(e => e.Innings)
                .HasForeignKey(e => e.inningId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<League>()
                .HasMany(e => e.Divisions)
                .WithRequired(e => e.League)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<League>()
                .HasMany(e => e.Schedules)
                .WithRequired(e => e.League)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<League>()
                .HasMany(e => e.Teams)
                .WithRequired(e => e.League)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.KRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.HRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.HRRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.BBRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.PopupRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.GroundRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.FlyRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.ChopperRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.LineRadio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .Property(e => e.FoulRatio)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Pitch>()
                .HasMany(e => e.PitchesThrowns)
                .WithRequired(e => e.Pitch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pitch>()
                .HasMany(e => e.PlayerPitches)
                .WithRequired(e => e.Pitch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PitchesThrown>()
                .Property(e => e.PlayDifficulty)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PitchResultType>()
                .HasMany(e => e.PitchesThrowns)
                .WithRequired(e => e.PitchResultType)
                .HasForeignKey(e => e.resultTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Batting)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Throwing)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.AtBats)
                .WithRequired(e => e.PlayerOnFirst)
                .HasForeignKey(e => e.AtFirst)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.AtBats1)
                .WithRequired(e => e.PlayerOnSecond)
                .HasForeignKey(e => e.AtSecond)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.AtBats2)
                .WithRequired(e => e.PlayerOnThird)
                .HasForeignKey(e => e.AtThird)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.AtBatsHit)
                .WithRequired(e => e.Batter)
                .HasForeignKey(e => e.batterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.AtBatsPitched)
                .WithRequired(e => e.Pitcher)
                .HasForeignKey(e => e.pitcherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.BaseRunnings)
                .WithRequired(e => e.Player)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.DefensivePlays)
                .WithRequired(e => e.Player)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.Lineups)
                .WithRequired(e => e.Player)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.PitchesThrowns)
                .WithRequired(e => e.Player)
                .HasForeignKey(e => e.batterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.PitchesThrowns1)
                .WithRequired(e => e.Player1)
                .HasForeignKey(e => e.pitcherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.PlayerAttributes)
                .WithRequired(e => e.Player)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.PlayerPitches)
                .WithRequired(e => e.Player)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.RunsScoreds)
                .WithRequired(e => e.Runner)
                .HasForeignKey(e => e.runnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Player>()
                .HasMany(e => e.RunsScoredAgainst)
                .WithRequired(e => e.Player1)
                .HasForeignKey(e => e.pitcherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Position>()
                .HasMany(e => e.Lineups)
                .WithRequired(e => e.Position)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Position>()
                .HasMany(e => e.Players)
                .WithRequired(e => e.Position)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Schedule>()
                .HasMany(e => e.Games)
                .WithRequired(e => e.Schedule)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.AwayGames)
                .WithRequired(e => e.AwayTeam)
                .HasForeignKey(e => e.awayTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.HomeGames)
                .WithRequired(e => e.HomeTeam)
                .HasForeignKey(e => e.homeTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.Lineups)
                .WithRequired(e => e.Team)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.Players)
                .WithRequired(e => e.Team)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.Schedules)
                .WithRequired(e => e.AwayTeam)
                .HasForeignKey(e => e.awayTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.Schedules1)
                .WithRequired(e => e.HomeTeam)
                .HasForeignKey(e => e.homeTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AtBats)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Games)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Players)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}
