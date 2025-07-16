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
            public static int[] Team => [101, 201, 301, 401, 501, 601, 701];
            // 1. Arizona Cardinals
            public static int[] ARI => [117, 216, 315, 414, 536, 636, 726];
            // 2. Atlanta Falcons  
            public static int[] ATL => [116, 127, 333, 417, 702];
            // 3. Baltimore Ravens
            public static int[] BAL => [128, 228, 324, 428, 504, 603, 611, 628, 635, 637, 728];
            // 4. Buffalo Bills
            public static int[] BUF => [131, 210, 306, 408, 533, 536, 602, 631, 725];
            // 5. Carolina Panthers
            public static int[] CAR => [109, 220, 314, 413, 421, 503, 526, 633];
            // 6. Chicago Bears
            public static int[] CHI => [111, 208, 225, 329, 431, 532, 619, 717];
            // 7. Cincinnati Bengals
            public static int[] CIN => [118, 218, 318, 418, 516, 618];
            // 8. Cleveland Browns
            public static int[] CLE => [106, 202, 205, 334, 329, 425, 529];
            // 9. Dallas Cowboys
            public static int[] DAL => [113, 213, 313, 512, 515, 629, 702, 724, 732];
            // 10. Denver Broncos
            public static int[] DEN => [121, 229, 311, 336, 433, 641, 726];
            // 11. Detroit Lions
            public static int[] DET => [129, 226, 307, 534, 621, 715, 729];
            // 12. Green Bay Packers
            public static int[] GB => [124, 223, 324, 423, 522, 623, 722, 735];
            // 13. Houston Texans
            public static int[] HOU => [203, 217, 316, 332, 415, 612, 622, 709, 740];
            // 14. Indianapolis Colts
            public static int[] IND => [115, 214, 317, 426, 514, 614, 615, 716];
            // 15. Jacksonville Jaguars
            public static int[] JAX => [103, 325, 326, 403, 406, 619, 625, 706, 721];
            // 16. Kansas City Chiefs
            public static int[] KC => [133, 232, 335, 320, 432, 521, 713];
            // 17. Las Vegas Raiders
            public static int[] LV => [107, 227, 337, 333, 334, 407, 434, 605, 638, 640, 707];
            // 18. Los Angeles Chargers
            public static int[] LAC => [123, 224, 323, 424, 521, 528, 624, 639, 741];
            // 19. Los Angeles Rams
            public static int[] LAR => [215, 327, 416, 511, 535, 727];
            // 20. Miami Dolphins
            public static int[] MIA => [114, 206, 506, 513, 518, 604, 716, 738];
            // 21. Minnesota Vikings
            public static int[] MIN => [125, 339, 502, 626, 627];
            // 22. New England Patriots
            public static int[] NP => [105, 207, 306, 332, 405, 502, 511, 607, 705, 736, 742];
            // 23. New Orleans Saints
            public static int[] NO => [110, 209, 308, 328, 411, 430, 609, 733, 739];
            // 24. New York Giants
            public static int[] NYG => [104, 126, 302, 404, 517, 704, 731];
            // 25. New York Jets
            public static int[] NYJ => [108, 211, 309, 409, 529, 525, 537];
            // 26. Philadelphia Eagles
            public static int[] PHI => [132, 233, 412, 508, 524, 531, 606, 616, 632, 634];
            // 27. Pittsburgh Steelers
            public static int[] PIT => [122, 318, 422, 527, 610, 711, 714];
            // 28. San Francisco 49ers
            public static int[] SF => [112, 212, 312, 335, 412, 437, 510, 523, 712, 734, 737];
            // 29. Seattle Seahawks
            public static int[] SEA => [119, 204, 219, 329, 505, 529, 538, 617, 708, 719, 723];
            // 30. Tampa Bay Buccaneers
            public static int[] TB => [120, 222, 321, 420, 520, 720];
            // 31. Tennessee Titans
            public static int[] TEN => [102, 221, 319, 402, 419, 501, 530, 608, 613];
            // 32. Washington Commanders
            public static int[] WSH => [130, 230, 427, 630, 730];


            public static IList<int> GetFranchisePicks(FranchiseEnum franchise) => GetAllPicks()[(int)franchise];
            public static IList<int> GetFranchisePicks(int franchise) => franchise < 1 || franchise > 32 ? [] : GetAllPicks()[franchise];

            public static IList<int>[] GetAllPicks() =>
                [Team, ARI, ATL, BAL, BUF, CAR, CHI, CIN, CLE, DAL, DEN, DET, GB, HOU, IND, JAX, KC, LV, LAC, LAR, MIA, MIN, NP, NO, NYG, NYJ, PHI, PIT, SF, SEA, TB, TEN, WSH];
        }
    }
}
