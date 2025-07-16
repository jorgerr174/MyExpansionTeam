using System.Globalization;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using METCore.DTOs.Admin;
using METCore.DTOs.Contract;
using METCore.DTOs.Player;
using METCore.DTOs.Stats;
using METCore.Interfaces;
using METCore.Interfaces.Importing;
using METCore.Models;
using METCore.Models.Players;
using METCore.Models.Stats;
using METCore.Utilities;
using Microsoft.Extensions.Configuration;
using static METCore.Enums.Types;


namespace METCore.Services
{
    public class ImportService(IConfiguration configuration, IPlayerRepository playerRepository, ISeasonStatsRepository seasonStatsRepository, IContractRepository contractRepository,
        IFranchiseRepository franchiseRepository, IMapper mapper)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IPlayerRepository _playerRepository = playerRepository;
        private readonly ISeasonStatsRepository _seasonStatsRepository = seasonStatsRepository;
        private readonly IContractRepository _contractRepository = contractRepository;
        private readonly IFranchiseRepository _franchiseRepository = franchiseRepository;
        private readonly IMapper _mapper = mapper;
        private static readonly CsvConfiguration csvConfig = new(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            PrepareHeaderForMatch = args => args.Header.Trim('"')
        };


        #region Prospects
        private async Task<IList<IList<string>>> ImportProspects(ImportDto dto)
        {
            IList<IList<string>> errorList = [];
            IList<Player> list = _mapper.Map<IList<Player>>(ReadData<ImportProspectDto>(dto, errorList));

            foreach (Player newProspect in list)
            {
                if (newProspect.Prospect is null) errorList.Add([.. newProspect.ImportProspectAttrs, "Problem during import. try again"]);
                else if (await _playerRepository.GetProspect(newProspect) is Player prospect)
                {
                    newProspect.Prospect.Year = DateTime.Now.Year;
                    if (prospect.Id == 0) await _playerRepository.CreateT(prospect);
                    else
                    {
                        if (prospect.Prospect is null || prospect.Prospect.Id == 0)
                        {
                            prospect.Prospect = newProspect.Prospect;
                            await _playerRepository.UpdateT(prospect);
                        }
                        else errorList.Add([.. newProspect.ImportProspectAttrs,
                                "Prospect already created" + (prospect.Prospect.Year!=newProspect.Prospect.Year ? "FROM DIFFERENT YEAR." : ".")]);
                    }
                }
                else errorList.Add([.. newProspect.ImportProspectAttrs, "MULTIPLE PLAYERS WITH SAME DATA."]);
            }
            return errorList;
        }
        #endregion Prospects


        #region Players
        private async Task<IList<IList<string>>> ImportPlayers(ImportDto dto)
        {
            IList<IList<string>> errorList = [];
            IList<Player> list = _mapper.Map<IList<Player>>(ReadData<ImportPlayerDto>(dto, errorList));

            foreach (Player newPlayer in list)
            {
                bool? exists = await _playerRepository.PlayerExists(newPlayer);

                if (!exists.HasValue || exists.Value)
                    errorList.Add([ newPlayer.Name, newPlayer.Height.ToString(), newPlayer.Weight.ToString(), newPlayer.BirthDate?.ToString(),
                        newPlayer.Position.ToString(), newPlayer.College, !exists.HasValue ? "MULTIPLE PLAYERS WITH SAME DATA." : "Player already created."]);
                else await _playerRepository.CreateT(newPlayer);
            }
            return errorList;
        }
        #endregion Players


        #region Stats
        private async Task<IList<IList<string>>> CreateStatsFromImport<T, M>(ImportDto dto, PositionEnum defaultPosition)
            where T : class, IImportStatsDto
            where M : class, IStats
        {
            Player? player = null;
            IList<Player> playerList;
            SeasonStats? seasonStats;
            List<string> failedPlayer;
            IList<IList<string>> errorRecordList = [];

            IList<T> statDtoList = ReadData<T>(dto, errorRecordList);

            var statProperty = typeof(SeasonStats).GetProperties()
                    .FirstOrDefault(p => p.PropertyType == typeof(M) ||
                                     (p.PropertyType.IsGenericType && Nullable.GetUnderlyingType(p.PropertyType) == typeof(M)));
            if (statProperty is null) return [["Error during import."]];

            foreach (T statT in statDtoList)
            {
                playerList = await _playerRepository.GetByName(statT.Player);
                player = playerList.Count == 1 ? playerList.FirstOrDefault()
                    : playerList.Count == 0 ? await _playerRepository.CreateBasic(statT.Player, defaultPosition) : null;

                if (player is null)
                {
                    failedPlayer = [];
                    foreach (var property in typeof(T).GetProperties())
                        failedPlayer.Add(property.GetValue(statT)?.ToString() ?? "");
                    failedPlayer.Add(playerList.Count > 1 ? "Multiple players with the same Name and Position. Insert manually." : "Error while creating new player.");
                    errorRecordList.Add(failedPlayer);
                    continue;
                }

                seasonStats = player.Stats.FirstOrDefault(x => x.Season == dto.Year) ?? new((int)dto.Year);
                // checks if the property of the stat to be added (passing, receiving, etc) is null in the object
                if (statProperty.GetValue(seasonStats) is null)
                {
                    M statM = _mapper.Map<M>(statT);
                    statProperty.SetValue(seasonStats, statM);
                    if (seasonStats.Id is 0 && await _seasonStatsRepository.CreateT(seasonStats) > 1)
                    {
                        player.Stats.Add(seasonStats);
                        await _playerRepository.UpdateT(player);
                    }
                }
            }

            return errorRecordList;
        }

        private async Task<IList<IList<string>>> ImportStats(ImportDto dto)
        {
            return dto.StatsType switch
            {
                StatsEnum.FGStats => await CreateStatsFromImport<ImportFGStatsDto, FGStats>(dto, PositionEnum.K),
                StatsEnum.IntStats => await CreateStatsFromImport<ImportIntStatsDto, IntStats>(dto, PositionEnum.DB),
                StatsEnum.KOStats => await CreateStatsFromImport<ImportKOStatsDto, KOStats>(dto, PositionEnum.K),
                StatsEnum.PuntStats => await CreateStatsFromImport<ImportPuntStatsDto, PuntStats>(dto, PositionEnum.P),
                StatsEnum.KRStats => await CreateStatsFromImport<ImportKRStatsDto, KRStats>(dto, PositionEnum.ATH),
                StatsEnum.PRStats => await CreateStatsFromImport<ImportPRStatsDto, PRStats>(dto, PositionEnum.ATH),
                StatsEnum.PassStats => await CreateStatsFromImport<ImportPassStatsDto, PassStats>(dto, PositionEnum.QB),
                StatsEnum.RecStats => await CreateStatsFromImport<ImportRecStatsDto, RecStats>(dto, PositionEnum.WR),
                StatsEnum.RushStats => await CreateStatsFromImport<ImportRushStatsDto, RushStats>(dto, PositionEnum.RB),
                StatsEnum.TackleStats => await CreateStatsFromImport<ImportTackleStatsDto, TackleStats>(dto, PositionEnum.MLB),
                _ => [["Error during import."]],
            };
        }
        #endregion Stats


        #region Contracts
        private async Task<IList<IList<string>>> ImportContracts(ImportDto dto)
        {
            bool? exists;
            Player? player;
            IList<Player> playerList;
            Franchise? franchise;
            IList<IList<string>> errorList = [];
            IList<ImportContractDto> dtoList = ReadData<ImportContractDto>(dto, errorList);

            foreach (ImportContractDto newDto in dtoList)
            {
                playerList = await _playerRepository.GetByNamePosition(newDto.Player, newDto.Position);

                if (playerList.Count > 1)
                {
                    errorList.Add([ newDto.Player, newDto.Position.ToString(), newDto.Franchise.ToString(), newDto.YearSigned.ToString(), newDto.Length.ToString(),
                            newDto.Total.ToString(), newDto.Guaranteed.ToString(), "Multiple players with the same Name. Insert manually."]);
                    continue;
                }
                else if (playerList.Count == 0) player = await _playerRepository.CreateBasic(newDto.Player, newDto.Position);
                else player = playerList.First();

                try
                {
                    Contract newContract = _mapper.Map<Contract>(newDto);
                    franchise = await _franchiseRepository.GetTById((int)newDto.Franchise);
                    if (franchise == null)
                    {
                        errorList.Add([ newDto.Player, newDto.Position.ToString(), newDto.Franchise.ToString(), newDto.YearSigned.ToString(), newDto.Length.ToString(),
                            newDto.Total.ToString(), newDto.Guaranteed.ToString(), "Franchise not valid."]);
                        continue;
                    }

                    exists = _contractRepository.ContractExists(player, franchise.Id, newContract.YearSigned, newContract.Length, newContract.Total);
                    if (!exists.HasValue || exists.Value)
                    {
                        errorList.Add([ newDto.Player, newDto.Position.ToString(), newDto.Franchise.ToString(), newDto.YearSigned.ToString(), newDto.Length.ToString(),
                            newDto.Total.ToString(), newDto.Guaranteed.ToString(), !exists.HasValue ? "MULTIPLE CONTRACTS WITH SAME DATA." : "Contract already created."]);
                        continue;
                    }

                    if (player.ActiveContract != null) player.ActiveContract.Active = false;
                    if (player.Position == PositionEnum.ATH) player.Position = newDto.Position;
                    await _contractRepository.CreateT(newContract);
                    player.Contracts.Add(newContract);
                    await _playerRepository.UpdateT(player);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return errorList;
        }
        #endregion Contracts


        #region Main
        private static IList<T> ReadData<T>(ImportDto dto, IList<IList<string>> errorRecordList)
            where T : class, IImportableDto
        {
            T recordDto;
            IList<T> recordDtoList = [];
            IList<String> errorRecord;

            using (var csv = new CsvReader(new StreamReader(dto.File.OpenReadStream()), csvConfig))
            {
                try
                {
                    if (dto.Type is not ImportEnum.Stats)
                    {
                        csv.Context.TypeConverterCache.AddConverter<PositionEnum?>(new EnumConverter(typeof(PositionEnum)));
                        if (dto.Type is ImportEnum.Players) csv.Context.TypeConverterCache.AddConverter<DateOnly?>(new SafeDateOnlyConverter());
                        if (dto.Type is ImportEnum.Contracts) csv.Context.TypeConverterCache.AddConverter<FranchiseEnum?>(new SafeEnumConverter<FranchiseEnum>());
                    }

                    csv.Read();
                    csv.ReadHeader();
                    if (csv.HeaderRecord is null) { errorRecordList = [["ErrorMessage"], ["No headers read."]]; return []; }

                    errorRecord = [.. csv.HeaderRecord];
                    errorRecord.Add("ErrorMessage");
                    errorRecordList.Add(errorRecord);

                    while (csv.Read())
                    {
                        try
                        {
                            recordDto = csv.GetRecord<T>();
                            recordDto.Player = recordDto.Player.Replace(".", "").Replace(",", "");
                            recordDtoList.Add(recordDto);
                        }
                        catch (Exception ex)
                        {
                            errorRecord = [];
                            for (int i = 0; i < csv.HeaderRecord.Length; i++)
                                errorRecord.Add(csv.GetField(i) ?? "");
                            errorRecord.Add(ex.Message.Replace("\r\n", " "));
                            errorRecordList.Add(errorRecord);
                        }
                    }
                }
                catch
                {
                    if (csv.HeaderRecord is null) { errorRecordList = [["ErrorMessage"], ["No headers read."]]; return []; }
                }
            }

            return recordDtoList;
        }


        private static Byte[] CreateErrorsFile(IList<IList<string>> records)
        {
            if (records.Count < 2) return [];

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            using var csvWriter = new CsvWriter(streamWriter, csvConfig);
            foreach (List<string> r in records)
            {
                foreach (string f in r)
                {
                    csvWriter.WriteField(f);
                }
                csvWriter.NextRecord();
            }
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }


        public async Task<Byte[]> Import(ImportDto dto)
        {
            return CreateErrorsFile(
                dto.Type switch
                {
                    ImportEnum.Players => await ImportPlayers(dto),
                    ImportEnum.Stats => await ImportStats(dto),
                    ImportEnum.Contracts => await ImportContracts(dto),
                    ImportEnum.Prospects => await ImportProspects(dto),
                    _ => [["ErrorMessage"], ["Error during import."]]
                }
            );
        }
        #endregion Main
    }

}
