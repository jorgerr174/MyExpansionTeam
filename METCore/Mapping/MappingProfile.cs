using AutoMapper;
using METCore.DTOs.Contract;
using METCore.DTOs.Player;
using METCore.DTOs.Stats;
using METCore.DTOs.Team;
using METCore.DTOs.User;
using METCore.Models;
using METCore.Models.Players;
using METCore.Models.Stats;
using METCore.Models.Teams;

namespace METCore.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region User
            CreateMap<NewUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Active, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ReverseMap()
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore());

            CreateMap<User, UserDto>()
                .IncludeBase<User, NewUserDto>()
            .ReverseMap()
                .IncludeBase<NewUserDto, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            #endregion User


            #region Team
            CreateMap<Team, TeamBasicInfoDto>()
                .ForMember(dest => dest.UserUsername, opt => opt.MapFrom(x => x.User != null ? x.User.Username : null))
            .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Team, DraftDto>()
                .IncludeBase<Team, TeamBasicInfoDto>()
                .ForMember(dest => dest.Picks, opt => opt.Ignore())
                .ForMember(dest => dest.Prospects, opt => opt.Ignore())
                .ForMember(dest => dest.Rounds, opt => opt.Ignore())
            .ReverseMap()
                .IncludeBase<TeamBasicInfoDto, Team>();


            CreateMap<Team, TeamInfoDto>()
                .IncludeBase<Team, TeamBasicInfoDto>()
                .ForMember(dest => dest.RosterSettingsCap, opt => opt.MapFrom(x => x.RosterSettings != null ? x.RosterSettings.Cap * 100 : 80))
                .ForMember(dest => dest.RosterSettingsMaxPerTeam, opt => opt.MapFrom(x => x.RosterSettings != null ? x.RosterSettings.MaxPerTeam : 3))
                .ForMember(dest => dest.RosterSettingsProtectedPerTeam, opt => opt.MapFrom(x => x.RosterSettings != null ? x.RosterSettings.ProtectedPerTeam : 3))
            .ReverseMap()
                .IncludeBase<TeamBasicInfoDto, Team>()
                .ForMember(dest => dest.RosterSettings, opt => opt.MapFrom(src => new RosterSettings
                {
                    Cap = src.RosterSettingsCap / 100,
                    MaxPerTeam = src.RosterSettingsMaxPerTeam,
                    ProtectedPerTeam = src.RosterSettingsProtectedPerTeam
                }));

            CreateMap<Team, TeamDto>()
                .IncludeBase<Team, TeamInfoDto>()
                .ForMember(dest => dest.Players, opt => opt.Ignore())
                .ForMember(dest => dest.SelectedIds, opt => opt.Ignore())
                .ForMember(dest => dest.Picks, opt => opt.Ignore())
                .ForMember(dest => dest.TradedPlayers, opt => opt.Ignore())
            .ReverseMap()
                .IncludeBase<TeamInfoDto, Team>();


            CreateMap<TeamInfoDto, RosterSettings>()
                .ForMember(dest => dest.Cap, opt => opt.MapFrom(x => (decimal)x.RosterSettingsCap / 100))
                .ForMember(dest => dest.MaxPerTeam, opt => opt.MapFrom(x => x.RosterSettingsMaxPerTeam))
                .ForMember(dest => dest.ProtectedPerTeam, opt => opt.MapFrom(x => x.RosterSettingsProtectedPerTeam))
                .ForMember(dest => dest.ProtectedPlayersIds, opt => opt.MapFrom(x => x.RosterSettingsProtectedPlayersIds));

            CreateMap<SPLineup, SPLineupDto>()
            .ReverseMap();

            CreateMap<Lineup, LineupDto>()
                .IncludeBase<SPLineup, SPLineupDto>()
            .ReverseMap()
                .IncludeBase<SPLineupDto, SPLineup>();
            #endregion Team


            #region Player
            CreateMap<Player, AthleteDto>()
                .ForMember(dest => dest.Position, opt => opt.MapFrom(x => x.Position.ToString()))
            .ReverseMap();

            CreateMap<Player, PlayerDto>()
                .IncludeBase<Player, AthleteDto>()
            .ReverseMap()
                .IncludeBase<AthleteDto, Player>();

            CreateMap<Player, ProspectDto>()
                .IncludeBase<Player, AthleteDto>()
                .ForMember(dest => dest.Year, opt => opt.MapFrom(x => x.Prospect == null ? 0 : x.Prospect.Year))
                .ForMember(dest => dest.Consensus, opt => opt.MapFrom(x => x.Prospect == null ? 0 : x.Prospect.Consensus))
                .ForMember(dest => dest.HandSize, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.HandSize))
                .ForMember(dest => dest.ArmLength, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.ArmLength))
                .ForMember(dest => dest.Wingspan, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.Wingspan))
                .ForMember(dest => dest.BenchPress, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.BenchPress))
                .ForMember(dest => dest.VertJump, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.VertJump))
                .ForMember(dest => dest.BroadJump, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.BroadJump))
                .ForMember(dest => dest.FortyYardDash, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.FortyYardDash))
                .ForMember(dest => dest.ThreeConeDrill, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.ThreeConeDrill))
                .ForMember(dest => dest.TwentyYardShuttle, opt => opt.MapFrom(x => x.Prospect == null ? string.Empty : x.Prospect.TwentyYardShuttle))
                .ForMember(dest => dest.AthScore, opt => opt.MapFrom(x => x.Prospect == null ? 0 : x.Prospect.AthScore))
            .ReverseMap()
                .IncludeBase<AthleteDto, Player>()
                .ForMember(dest => dest.Prospect,
                    opt => opt.MapFrom(src => new Prospect
                    {
                        Year = src.Year,
                        Consensus = src.Consensus,
                        HandSize = src.HandSize == null ? string.Empty : src.HandSize.Trim(),
                        ArmLength = src.ArmLength == null ? string.Empty : src.ArmLength.Trim(),
                        Wingspan = src.Wingspan == null ? string.Empty : src.Wingspan.Trim(),
                        BenchPress = src.BenchPress == null ? string.Empty : src.BenchPress.Trim(),
                        VertJump = src.VertJump == null ? string.Empty : src.VertJump.Trim(),
                        BroadJump = src.BroadJump == null ? string.Empty : src.BroadJump.Trim(),

                        FortyYardDash = (src.FortyYardDash ?? string.Empty).Replace(',', '.').Trim(),
                        ThreeConeDrill = (src.ThreeConeDrill ?? string.Empty).Replace(',', '.').Trim(),
                        TwentyYardShuttle = (src.TwentyYardShuttle ?? string.Empty).Replace(',', '.').Trim(),
                        AthScore = src.AthScore
                    }));

            CreateMap<Player, PlayerBasicDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(x => x.Position.ToString()))
                .ForMember(dest => dest.APY, opt => opt.MapFrom(x => x.APY == 0 ? string.Empty : ("$" + x.APY.ToString() + "M")));

            CreateMap<Player, ProtectableDto>()
                .IncludeBase<Player, PlayerBasicDto>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(x => (x.Height / 12) + "-" + (x.Height % 12)))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(x => x.Weight + "lb"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(x => x.Age == null ? string.Empty : x.Age + "yo"))
                .ForMember(dest => dest.DefaultProtected, opt => opt.Ignore());

            CreateMap<Player, SelectableDto>()
                .IncludeBase<Player, ProtectableDto>()
                .ForMember(dest => dest.PureAPY, opt => opt.MapFrom(x => x.APY == 0 ? string.Empty : x.APY.ToString().Replace(',', '.')));

            CreateMap<Player, RosteredDto>()
                .IncludeBase<Player, SelectableDto>();

            CreateMap<ImportAthleteDto, Player>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Player))
                .ForMember(dest => dest.BirthDate, opt => opt.Ignore())
                .ForMember(dest => dest.Position2, opt => opt.Ignore())
                .ForMember(dest => dest.Position3, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .ForMember(dest => dest.Stats, opt => opt.Ignore())
                .ForMember(dest => dest.Prospect, opt => opt.Ignore())
                .ForMember(dest => dest.Retired, opt => opt.Ignore())
                .ForMember(dest => dest.ImportProspectAttrs, opt => opt.Ignore())
                .ForMember(dest => dest.Madden, opt => opt.Ignore())
                .ForMember(dest => dest.Jersey, opt => opt.Ignore())
                .ForMember(dest => dest.DraftYear, opt => opt.Ignore());

            CreateMap<ImportPlayerDto, Player>()
                .IncludeBase<ImportAthleteDto, Player>();

            CreateMap<ImportProspectDto, Player>()
                .IncludeBase<ImportAthleteDto, Player>()
                .ForMember(dest => dest.Prospect,
                    opt => opt.MapFrom(src => new Prospect
                    {
                        Year = src.Year,
                        Consensus = src.Consensus,
                        HandSize = src.HandSize == null ? string.Empty : src.HandSize.Trim(),
                        ArmLength = src.ArmLength == null ? string.Empty : src.ArmLength.Trim(),
                        Wingspan = src.Wingspan == null ? string.Empty : src.Wingspan.Trim(),
                        BenchPress = src.BenchPress == null ? string.Empty : src.BenchPress.Trim(),
                        VertJump = src.VertJump == null ? string.Empty : src.VertJump.Trim(),
                        BroadJump = src.BroadJump == null ? string.Empty : src.BroadJump.Trim(),

                        FortyYardDash = (src.FortyYardDash ?? string.Empty).Replace(',', '.').Trim(),
                        ThreeConeDrill = (src.ThreeConeDrill ?? string.Empty).Replace(',', '.').Trim(),
                        TwentyYardShuttle = (src.TwentyYardShuttle ?? string.Empty).Replace(',', '.').Trim(),
                        AthScore = src.AthScore
                    }));
            #endregion Player


            #region Contract
            CreateMap<ImportContractDto, Contract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FranchiseId, opt => opt.MapFrom(x => (int)x.Franchise));
            #endregion Contract


            #region Stats
            CreateMap<IImportStatsDto, IStats>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .Include<ImportFGStatsDto, FGStats>()
                .Include<ImportIntStatsDto, IntStats>()
                .Include<ImportKOStatsDto, KOStats>()
                .Include<ImportPuntStatsDto, PuntStats>()
                .Include<ImportKRStatsDto, KRStats>()
                .Include<ImportPRStatsDto, PRStats>()
                .Include<ImportPassStatsDto, PassStats>()
                .Include<ImportRecStatsDto, RecStats>()
                .Include<ImportRushStatsDto, RushStats>()
                .Include<ImportTackleStatsDto, TackleStats>();

            CreateMap<ImportFGStatsDto, FGStats>();
            CreateMap<ImportIntStatsDto, IntStats>();
            CreateMap<ImportKOStatsDto, KOStats>();
            CreateMap<ImportPuntStatsDto, PuntStats>();
            CreateMap<ImportKRStatsDto, KRStats>();
            CreateMap<ImportPRStatsDto, PRStats>();
            CreateMap<ImportPassStatsDto, PassStats>();
            CreateMap<ImportRecStatsDto, RecStats>();
            CreateMap<ImportRushStatsDto, RushStats>();
            CreateMap<ImportTackleStatsDto, TackleStats>();
            #endregion Stats


            #region Trade
            CreateMap<Trade, TradeDto>()
                .ForMember(dest => dest.TeamPlayers, opt => opt.Ignore())
                .ForMember(dest => dest.FranchisePlayers, opt => opt.Ignore())
                .ForMember(dest => dest.Force, opt => opt.Ignore())
                .ForMember(dest => dest.TeamCurrentCap, opt => opt.Ignore())
            .ReverseMap()
                .ForMember(dest => dest.TeamPlayers, opt => opt.MapFrom(src => src.TeamPlayers.Select(tpl => tpl.Id)))
                .ForMember(dest => dest.FranchisePlayers, opt => opt.MapFrom(src => src.FranchisePlayers.Select(fpl => fpl.Id)));
            #endregion Trade
        }
    }
}
