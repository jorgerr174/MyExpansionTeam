using METCore.Models.Players;

namespace METCore.Enums
{
    public class Types
    {
        public enum PositionEnum
        {
            ATH,

            //Offense
            //BackField
            QB,
            RB,
            FB,
            //OLine
            OT,
            G,
            C,
            //Receiver
            WR,
            TE,

            //Defense
            //Front
            NT,
            DT,
            ED,
            //Middle
            OLB,
            MLB,
            //BackEnd
            DB,
            CB,
            S,
            SS,
            FS,

            //Special teams
            P,
            K,
            PR,
            KR,
            LS
        }

        public enum RoleEnum
        {
            User,
            Manager,
            Admin
        }

        public enum ImportEnum
        {
            None,
            Players,
            Stats,
            Contracts,
            Prospects
        }

        public enum StatsEnum
        {
            PassStats,
            RecStats,
            RushStats,
            IntStats,
            TackleStats,
            KOStats,
            KRStats,
            PuntStats,
            PRStats,
            FGStats
        }

        public enum FranchiseEnum
        {
            ARI = 1,
            ATL,
            BAL,
            BUF,
            CAR,
            CHI,
            CIN,
            CLE,
            DAL,
            DEN,
            DET,
            GB,
            HOU,
            IND,
            JAX,
            KC,
            LV,
            LAC,
            LAR,
            MIA,
            MIN,
            NE,
            NO,
            NYG,
            NYJ,
            PHI,
            PIT,
            SF,
            SEA,
            TB,
            TEN,
            WSH
        }

        public enum DraftOrderEnum
        {
            TEN = 2,
            CLE,
            NYG,
            NE,
            JAX,
            LV,
            NYJ,
            CAR,
            NO,
            CHI,
            SF,
            DAL,
            MIA,
            IND,
            ATL,
            ARI,
            CIN,
            SEA,
            TB,
            DEN,
            PIT,
            LAC,
            GB,
            MIN,
            HOU,
            LAR,
            BAL,
            DET,
            WSH,
            BUF,
            KC,
            PHI
        }

        public enum DivisionEnum
        {
            AFC_North = 1,
            AFC_East,
            AFC_South,
            AFC_West,
            NFC_North,
            NFC_East,
            NFC_South,
            NFC_West

        }

        public static class Divisions
        {
            public static DivisionEnum[] GetDivisions()
            {
                return [DivisionEnum.AFC_North, DivisionEnum.AFC_East, DivisionEnum.AFC_South, DivisionEnum.AFC_West,
                    DivisionEnum.NFC_North, DivisionEnum.NFC_East, DivisionEnum.NFC_South, DivisionEnum.NFC_West];
            }

            public static DivisionEnum GetDivision(FranchiseEnum franchise)
            {
                return franchise switch
                {
                    FranchiseEnum.BAL or FranchiseEnum.CIN or FranchiseEnum.CLE or FranchiseEnum.PIT => DivisionEnum.AFC_North,

                    FranchiseEnum.BUF or FranchiseEnum.MIA or FranchiseEnum.NE or FranchiseEnum.NYJ => DivisionEnum.AFC_East,

                    FranchiseEnum.HOU or FranchiseEnum.IND or FranchiseEnum.JAX or FranchiseEnum.TEN => DivisionEnum.AFC_South,

                    FranchiseEnum.DEN or FranchiseEnum.KC or FranchiseEnum.LV or FranchiseEnum.LAC => DivisionEnum.AFC_West,

                    FranchiseEnum.CHI or FranchiseEnum.DET or FranchiseEnum.GB or FranchiseEnum.MIN => DivisionEnum.NFC_North,

                    FranchiseEnum.DAL or FranchiseEnum.NYG or FranchiseEnum.PHI or FranchiseEnum.WSH => DivisionEnum.NFC_East,

                    FranchiseEnum.ATL or FranchiseEnum.CAR or FranchiseEnum.NO or FranchiseEnum.TB => DivisionEnum.NFC_South,

                    FranchiseEnum.ARI or FranchiseEnum.LAR or FranchiseEnum.SF or FranchiseEnum.SEA => DivisionEnum.NFC_West,

                    _ => throw new ArgumentException($"Unknown franchise: {franchise}")
                };
            }

            public static FranchiseEnum[] GetFranchisesInDivision(DivisionEnum division)
            {
                return division switch
                {
                    DivisionEnum.AFC_North => [FranchiseEnum.BAL, FranchiseEnum.CIN, FranchiseEnum.CLE, FranchiseEnum.PIT],
                    DivisionEnum.AFC_East => [FranchiseEnum.BUF, FranchiseEnum.MIA, FranchiseEnum.NE, FranchiseEnum.NYJ],
                    DivisionEnum.AFC_South => [FranchiseEnum.HOU, FranchiseEnum.IND, FranchiseEnum.JAX, FranchiseEnum.TEN],
                    DivisionEnum.AFC_West => [FranchiseEnum.DEN, FranchiseEnum.KC, FranchiseEnum.LV, FranchiseEnum.LAC],
                    DivisionEnum.NFC_North => [FranchiseEnum.CHI, FranchiseEnum.DET, FranchiseEnum.GB, FranchiseEnum.MIN],
                    DivisionEnum.NFC_East => [FranchiseEnum.DAL, FranchiseEnum.NYG, FranchiseEnum.PHI, FranchiseEnum.WSH],
                    DivisionEnum.NFC_South => [FranchiseEnum.ATL, FranchiseEnum.CAR, FranchiseEnum.NO, FranchiseEnum.TB],
                    DivisionEnum.NFC_West => [FranchiseEnum.ARI, FranchiseEnum.LAR, FranchiseEnum.SF, FranchiseEnum.SEA],
                    _ => throw new ArgumentException($"Unknown division: {division}")
                };
            }
        }

        public static class DraftPicks
        {
            // 0. Own Team
            public static IList<int> Team => [101, 201, 301, 401, 501, 601, 701];
            // 1. Arizona Cardinals
            public static IList<int> ARI => [117, 216, 315, 414, 537, 636, 710];
            // 2. Atlanta Falcons  
            public static IList<int> ATL => [116, 127, 333, 417, 703];
            // 3. Baltimore Ravens
            public static IList<int> BAL => [128, 228, 328, 428, 504, 603, 611, 628, 635, 637, 728];
            // 4. Buffalo Bills
            public static IList<int> BUF => [131, 210, 309, 408, 533, 536, 602, 631, 725];
            // 5. Carolina Panthers
            public static IList<int> CAR => [109, 220, 314, 413, 421, 503, 526, 633];
            // 6. Chicago Bears
            public static IList<int> CHI => [111, 208, 225, 231, 431, 532, 620, 718];
            // 7. Cincinnati Bengals
            public static IList<int> CIN => [118, 218, 318, 418, 516, 618];
            // 8. Cleveland Browns
            public static IList<int> CLE => [106, 202, 205, 304, 331, 425, 507];
            // 9. Dallas Cowboys
            public static IList<int> DAL => [113, 213, 313, 512, 515, 629, 702, 724, 732];
            // 10. Denver Broncos
            public static IList<int> DEN => [121, 229, 311, 338, 433, 641, 726];
            // 11. Detroit Lions
            public static IList<int> DET => [129, 226, 307, 534, 621, 715, 729];
            // 12. Green Bay Packers
            public static IList<int> GB => [124, 223, 324, 423, 522, 623, 722, 735];
            // 13. Houston Texans
            public static IList<int> HOU => [203, 217, 316, 334, 415, 612, 622, 709, 740];
            // 14. Indianapolis Colts
            public static IList<int> IND => [115, 214, 317, 426, 514, 614, 615, 717];
            // 15. Jacksonville Jaguars
            public static IList<int> JAX => [103, 325, 326, 403, 406, 619, 625, 706, 721];
            // 16. Kansas City Chiefs
            public static IList<int> KC => [133, 232, 303, 322, 432, 519, 713];
            // 17. Las Vegas Raiders
            public static IList<int> LV => [107, 227, 305, 335, 336, 407, 434, 605, 638, 640, 707];
            // 18. Los Angeles Chargers
            public static IList<int> LAC => [123, 224, 323, 424, 521, 528, 624, 639, 741];
            // 19. Los Angeles Rams
            public static IList<int> LAR => [215, 327, 416, 511, 535, 727];
            // 20. Miami Dolphins
            public static IList<int> MIA => [114, 206, 506, 513, 518, 604, 716, 738];
            // 21. Minnesota Vikings
            public static IList<int> MIN => [125, 339, 502, 626, 627];
            // 22. New England Patriots
            public static IList<int> NE => [105, 207, 306, 332, 405, 436, 509, 607, 705, 736, 742];
            // 23. New Orleans Saints
            public static IList<int> NO => [110, 209, 308, 330, 411, 430, 609, 733, 739];
            // 24. New York Giants
            public static IList<int> NYG => [104, 126, 302, 404, 517, 704, 731];
            // 25. New York Jets
            public static IList<int> NYJ => [108, 211, 310, 409, 527, 525, 539];
            // 26. Philadelphia Eagles
            public static IList<int> PHI => [132, 233, 410, 508, 524, 531, 606, 616, 632, 634];
            // 27. Pittsburgh Steelers
            public static IList<int> PIT => [122, 320, 422, 527, 610, 711, 714];
            // 28. San Francisco 49ers
            public static IList<int> SF => [112, 212, 312, 337, 412, 437, 510, 523, 712, 734, 737];
            // 29. Seattle Seahawks
            public static IList<int> SEA => [119, 204, 219, 329, 505, 529, 538, 617, 708, 719, 723];
            // 30. Tampa Bay Buccaneers
            public static IList<int> TB => [120, 222, 321, 420, 520, 720];
            // 31. Tennessee Titans
            public static IList<int> TEN => [102, 221, 319, 402, 419, 435, 530, 608, 613];
            // 32. Washington Commanders
            public static IList<int> WSH => [130, 230, 427, 630, 730];

            public static int[] TotalPerRound => [33, 33, 39, 37, 39, 41, 42];
            public static int[] TotalAtRound => [0, 33, 66, 105, 142, 181, 222];

            public static int[] PickValues =>
                // 20 en 20

                // Round 1
                [3000,2649,2443,2297,2184,2092,2014,1946,1887,1833,1785,1741,1700,1663,1628,1595,1564,1535,1508,1482,
                1457,1434,1411,1389,1369,1349,1330,1311,1294,1276,1260,1244,1228,

                // Round 2
                1213,1198,1184,1170,1157,1143,1131,1118,1106,1094,1082,1071,1060,1049,1038,1028,1018,1007,998,988,
                979,969,960,951,942,934,925,917,909,900,892,885,877,

                // Round 3
                869,862,854,847,840,833,826,819,812,805,799,792,786,779,773,767,761,755,749,743,
                737,731,725,720,714,709,703,698,692,687,682,676,671,666,661,656,651,646,642,
                
                // Round 4
                637,632,627,623,618,613,609,604,600,595,591,587,582,578,574,570,565,561,557,553,
                549,545,541,537,533,529,526,522,518,514,510,507,503,499,496,492,489,
                
                // Round 5
                485,481,478,474,471,468,464,461,457,454,451,447,444,441,438,434,431,428,425,422,
                419,416,412,409,406,403,400,397,394,391,388,386,383,380,377,374,371,368,366,
                
                // Round 6
                363,360,357,354,352,349,346,344,341,338,336,333,330,328,325,323,320,318,315,312,
                310,307,305,302,300,298,295,293,290,288,285,283,281,278,276,274,271,269,267,264,
                262,
                
                // Round 7
                260,258,255,253,251,249,246,244,242,240,238,235,233,231,229,227,225,223,220,218,
                216,214,212,210,208,206,204,202,200,198,196,194,192,190,188,186,184,182,180,178,
                176,174];


            public static int[] PickAPYs =>
                // 10 en 10

                // Round 1
                [12209896,11662276,11313789,10915520,10218542,8973949,7978271,6982594,6932810,6658998,
                6235837,5638424,5489077,5240159,5140591,4841885,4742319,4617860,4543182,4518292,
                4493399,4443614,4393832,4294262,4244482,4194694,4144913,4120020,3920273,3814495,
                3725891,3668836,3435168,

                // Round 2
                2950100,2925207,2900315,2850530,2830618,2785815,2741007,2701180,2661352,2616546,
                2576722,2538771,2485615,2428365,2352694,2302909,2253125,2203342,2153559,2103775,
                2053990,2004206,1954423,1929533,1914593,1889704,1854853,1839918,1820005,1805070,
                1800094,1790134,1786517,

                // Round 3
                1690517,1686156,1685721,1681023,1675893,1666113,1659448,1654744,1648409,1645268,
                1640417,1626689,1625135,1622369,1616714,1611000,1605291,1601249,1593015,1587161,
                1583878,1578316,1573114,1570267,1569326,1568853,1568381,1566021,1561301,1556580,
                1551859,1547139,1540296,1540296,1540296,1540296,1540296,1540296,1540296,

                // Round 4
                1325237,1319130,1318260,1316612,1314148,1312724,1311762,1310411,1308917,1307092,
                1302773,1302187,1301191,1300942,1299696,1296538,1295528,1293228,1290816,1289279,
                1287733,1285463,1282643,1282643,1280317,1278460,1275724,1274535,1271810,1269894,
                1266800,1263562,1216452,1216452,1216452,1216452,1216452,

                // Round 5
                1169344,1168290,1167397,1166875,1164576,1161845,1161031,1160199,1159919,1159645,
                1156766,1156323,1156187,1155859,1155247,1154596,1153696,1153412,1152449,1152001,
                1151021,1150594,1148865,1148633,1147742,1144959,1143510,1141260,1140154,1138769,
                1130054,1130054,1130054,1130054,1130054,1130054,1130054,1130054,1130054,

                // Round 6
                1121339,1119287,1118166,1116785,1115308,1114121,1113254,1112828,1112025,1111193,
                1110950,1109627,1108811,1108590,1108316,1108065,1107714,1107631,1107515,1107473,
                1107121,1106596,1106301,1105545,1104767,1103956,1103717,1103494,1102085,1101232,
                1100392,1098860,1093572,1093572,1093572,1093572,1093572,1093572,1093572,1093572,
                1093572,

                // Round 7
                1088279,1088154,1087379,1087082,1086077,1085291,1084736,1084582,1084473,1084235,
                1083559,1083368,1083363,1082906,1082894,1082767,1082035,1081481,1080985,1079690,
                1078460,1078175,1077883,1077592,1077372,1077135,1076912,1076693,1076357,1076110,
                1075756,1075417,1075417,1075417,1075417,1075417,1075417,1075417,1075417,1075417,
                1075417,1075417];


            public static int GetPickOverall(int pick) { try { return TotalAtRound[(pick / 100) - 1] + (pick % 100); } catch { return 0; } }

            public static IList<int> GetFranchisePicks(FranchiseEnum franchise) => GetAllPicks()[(int)franchise];
            public static IList<int> GetFranchisePicks(int franchise) => franchise < 1 || franchise > 32 ? [] : GetAllPicks()[franchise];

            public static IList<IList<int>> GetAllPicks() =>
                [Team, ARI, ATL, BAL, BUF, CAR, CHI, CIN, CLE, DAL, DEN, DET, GB, HOU, IND, JAX, KC, LV, LAC, LAR, MIA, MIN, NE, NO, NYG, NYJ, PHI, PIT, SF, SEA, TB, TEN, WSH];

            public static int GetPickValue(int pick) { try { return PickValues[GetPickOverall(pick) - 1]; } catch { return 0; } }

            public static int GetPickAPY(int pick) { try { return PickAPYs[pick - 1]; } catch { return 0; } }


            public static int GetPlayerValue(Player player)
            {
                return (int)(player.Madden != 0 ? player.Madden : 70
                    * GetPositionMultiplier(player.Position)
                    * CalculateAgeFactor(player.BirthDate is null ? 30 : DateTime.Now.Year - player.BirthDate.Value.Year - (DateTime.Now.DayOfYear < player.BirthDate.Value.DayOfYear ? 1 : 0))
                    * CalculateContractValue(player.Position, player.ActiveContract?.APY)
                    * CalculateYearsRemaining(player.ActiveContract == null ? -1 : DateTime.Now.Year - player.ActiveContract.YearSigned));
            }

            private static double CalculateContractValue(PositionEnum position, double? apy)
            {
                if (apy is null || apy == 0) return 0.7;

                // Get average APY of top 5 players at this position (or top 10% for positions with many players)
                double topPlayersAvgAPY = GetTopPlayersAverageAPY[(int)position];

                // If no top players data available, use neutral multiplier
                if (topPlayersAvgAPY <= 0) return 1.0;

                double ratio = (double)apy / topPlayersAvgAPY;

                // Cheaper relative to top players = more valuable
                if (ratio < 0.3) return 1.8;      // Bargain deal (rookie contracts, etc.)
                if (ratio < 0.5) return 1.5;      // Very good value
                if (ratio < 0.7) return 1.3;      // Good value
                if (ratio < 0.9) return 1.1;      // Fair value
                if (ratio < 1.1) return 1.0;      // Market rate
                if (ratio < 1.3) return 0.8;      // Slightly overpaid
                if (ratio < 1.5) return 0.6;      // Overpaid
                return 0.4;                       // Severely overpaid
            }

            private static double[] GetTopPlayersAverageAPY =>
                [0, 51.0, 10.2, 4.0, 21.1, 17.6, 11.9, 17.9, 10.2,
                15.0, 20.5, 19.8, 14.1, 12.3, 14.7, 16.2, 13.7, 13.5, 14.1,
                3.0, 5.8, 7.2, 7.1, 2.3];

            // Keep the other methods unchanged
            private static double GetPositionMultiplier(PositionEnum pos)
            {
                return pos switch
                {
                    PositionEnum.QB => 2.0,
                    PositionEnum.RB => 0.9,
                    PositionEnum.FB => 0.7,
                    PositionEnum.ED => 1.6,
                    PositionEnum.OT => 1.5,
                    PositionEnum.WR => 1.3,

                    PositionEnum.DT => 1.2,
                    PositionEnum.CB => 1.4,
                    PositionEnum.OLB => 1.2,
                    PositionEnum.MLB => 1.1,

                    PositionEnum.K => 0.3,
                    PositionEnum.P => 0.3,
                    _ => 1.0
                };
            }

            private static double CalculateAgeFactor(int age)
            {
                if (age <= 23) return 0.9;        // Too young/unproven
                if (age <= 27) return 1.0;         // Prime years
                if (age <= 30) return 0.8;         // Still good
                if (age <= 32) return 0.5;         // Declining
                return 0.2;                        // Veteran minimum
            }

            private static double CalculateYearsRemaining(int yearsLeft)
            {
                return yearsLeft switch
                {
                    0 => 0.6,  // Expiring contract
                    1 => 0.8,  // One year left
                    2 => 1.0,  // Sweet spot
                    3 => 1.1,  // Good control
                    4 => 0.9,  // Still good
                    _ => 0.6   // Too long-term
                };
            }
        }
    }
}
