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
            TEN = 1,
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

        public static class NFLDivisions
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
    }
}
