namespace MobileApp.Models.Shared
{
    public class FormationPosition(string id, string name, string position, int x, int y)
    {
        public string Id { get; set; } = id;
        public string Name { get; set; } = name;
        public string Position { get; set; } = position;
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
    }

    public class FormationInfo(string key, string name, List<FormationPosition> positions)
    {
        public string Key { get; set; } = key;
        public string Name { get; set; } = name;
        public List<FormationPosition> Positions { get; set; } = positions;
    }

    public static class FormationData
    {
        public static List<FormationInfo> GetFormationsForType(string formationType)
        {
            return formationType switch
            {
                "offense" => GetOffenseFormations(),
                "defense" => GetDefenseFormations(),
                "special" => GetSpecialTeamsFormations(),
                _ => []
            };
        }

        public static List<FormationInfo> GetOffenseFormations()
        {
            return
            [
                new("ZeroOne", "ZeroOne (0RB-1TE-4WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("LWRX", "X-WR", "WR", 10, 45),
                new("RWRX", "X-WR", "WR", 90, 45),
                new("WRZ", "Z-WR", "WR", 82, 50),
                new("WRS", "Slot", "WR", 74, 55),
                new("TE", "TE", "TE", 25, 58)
            ]),
            new("ZeroTwo", "ZeroTwo (0RB-2TE-3WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("WRX", "X-WR", "WR", 10, 45),
                new("WRZ", "Z-WR", "WR", 90, 45),
                new("WRS", "Slot", "WR", 82, 55),
                new("LTE", "TE", "TE", 25, 58),
                new("RTE", "TE", "TE", 71, 50)
            ]),
            new("Ten", "Ten (1RB-0TE-4WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("RB", "RB", "RB", 42, 75),
                new("LWRX", "X-WR", "WR", 10, 45),
                new("RWRX", "X-WR", "WR", 90, 45),
                new("WRZ", "Z-WR", "WR", 20, 50),
                new("WRS", "Slot", "WR", 75, 50)
            ]),
            new("Eleven", "Eleven (1RB-1TE-3WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("RB", "RB", "RB", 42, 75),
                new("WRX", "X-WR", "WR", 10, 45),
                new("WRZ", "Z-WR", "WR", 83, 45),
                new("WRS", "Slot", "WR", 73, 55),
                new("TE", "TE", "TE", 25, 58)
            ]),
            new("Twelve", "Twelve (1RB-2TE-2WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("RB", "RB", "RB", 42, 75),
                new("WRX", "X-WR", "WR", 10, 45),
                new("WRZ", "Z-WR", "WR", 90, 45),
                new("LTE", "TE", "TE", 25, 58),
                new("RTE", "TE", "TE", 71, 50)
            ]),
            new("Thirteen", "Thirteen (1RB-3TE-1WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("RB", "RB", "RB", 42, 75),
                new("WRX", "X-WR", "WR", 10, 45),
                new("LTE", "TE", "TE", 29, 50),
                new("RTE", "TE", "TE", 71, 45),
                new("YTE", "Y-TE", "TE", 83, 50)
            ]),
            new("Twenty", "Twenty (2RB-0TE-3WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("RB", "RB", "RB", 42, 75),
                new("FB", "FB", "RB", 58, 72),
                new("WRX", "X-WR", "WR", 10, 45),
                new("WRZ", "Z-WR", "WR", 85, 45),
                new("WRS", "Slot", "WR", 75, 50)
            ]),
            new("TwentyOne", "TwentyOne (2RB-1TE-2WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("RB", "RB", "RB", 42, 75),
                new("FB", "FB", "RB", 58, 72),
                new("WRX", "X-WR", "WR", 10, 45),
                new("WRZ", "Z-WR", "WR", 80, 45),
                new("TE", "TE", "TE", 25, 58)
            ]),
            new("TwentyTwo", "TwentyTwo (2RB-2TE-1WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("RB", "RB", "RB", 42, 75),
                new("FB", "FB", "RB", 58, 72),
                new("WRX", "X-WR", "WR", 10, 45),
                new("LTE", "TE", "TE", 25, 58),
                new("RTE", "TE", "TE", 80, 45)
            ]),
            new("Jumbo", "Jumbo (6OL-2RB-1TE-1WR)",
            [
                new("EOL", "OL", "OL", 29, 45),
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("QB", "QB", "QB", 50, 70),
                new("RB", "RB", "RB", 42, 75),
                new("FB", "FB", "RB", 58, 72),
                new("WRX", "X-WR", "WR", 10, 45),
                new("TE", "TE", "TE", 71, 45)
            ]),
            new("TwoQB", "TwoQB (2QB-1RB-1TE-2WR)",
            [
                new("LT", "LT", "OL", 36, 45),
                new("LG", "LG", "OL", 43, 44),
                new("C", "C", "OL", 50, 43),
                new("RG", "RG", "OL", 57, 44),
                new("RT", "RT", "OL", 64, 45),
                new("LQB", "QB", "QB", 46, 72),
                new("RQB", "QB", "QB", 54, 72),
                new("RB", "RB", "RB", 62, 78),
                new("WRX", "X-WR", "WR", 10, 45),
                new("WRZ", "Z-WR", "WR", 90, 45),
                new("TE", "TE", "TE", 25, 58)
            ])
            ];
        }

        public static List<FormationInfo> GetDefenseFormations()
        {
            return
            [
                new("Bear", "Bear (5-2-4)",
            [
                new("LE", "EDGE", "DL", 34, 70),
                new("LDE", "DE", "DL", 42, 70),
                new("NT", "NT", "DL", 50, 70),
                new("RDE", "DE", "DL", 58, 70),
                new("RE", "EDGE", "DL", 66, 70),
                new("LLB", "LB", "LB", 45, 40),
                new("RLB", "LB", "LB", 55, 40),
                new("LCB", "CB", "DB", 15, 55),
                new("RCB", "CB", "DB", 85, 55),
                new("FS", "FS", "DB", 35, 15),
                new("SS", "SS", "DB", 65, 15)
            ]),
            new("FourThree", "4-3 Defense (4-3-4)",
            [
                new("LE", "EDGE", "DL", 35, 70),
                new("LDT", "DT", "DL", 46, 70),
                new("RDT", "DT", "DL", 54, 70),
                new("RE", "EDGE", "DL", 65, 70),
                new("WILL", "WILL", "LB", 30, 43),
                new("MIKE", "MIKE", "LB", 50, 40),
                new("SAM", "SAM", "LB", 70, 43),
                new("LCB", "CB", "DB", 15, 55),
                new("RCB", "CB", "DB", 85, 55),
                new("FS", "FS", "DB", 35, 15),
                new("SS", "SS", "DB", 65, 15)
            ]),
            new("ThreeFour", "3-4 Defense (3-4-4)",
            [
                new("LDE", "DE", "DL", 40, 70),
                new("DT", "DT", "DL", 50, 70),
                new("RDE", "DE", "DL", 60, 70),
                new("ROLB", "OLB", "LB", 30, 50),
                new("RMLB", "MLB", "LB", 70, 50),
                new("LMLB", "MLB", "LB", 45, 40),
                new("LOLB", "OLB", "LB", 55, 40),
                new("LCB", "CB", "DB", 15, 55),
                new("RCB", "CB", "DB", 85, 55),
                new("FS", "FS", "DB", 35, 15),
                new("SS", "SS", "DB", 65, 15)
            ]),
            new("Nickel", "Nickel (4-2-5)",
            [
                new("LE", "EDGE", "DL", 35, 70),
                new("LDT", "DT", "DL", 46, 70),
                new("RDT", "DT", "DL", 54, 70),
                new("RE", "EDGE", "DL", 65, 70),
                new("LLB", "LB", "LB", 45, 40),
                new("RLB", "LB", "LB", 55, 40),
                new("LCB", "CB", "DB", 15, 55),
                new("RCB", "CB", "DB", 85, 55),
                new("NCB", "NCB", "DB", 25, 65),
                new("FS", "FS", "DB", 35, 15),
                new("SS", "SS", "DB", 65, 15)
            ]),
            new("Dime", "Dime (3-2-6)",
            [
                new("LDE", "DE", "DL", 40, 70),
                new("DT", "DT", "DL", 50, 70),
                new("RDE", "DE", "DL", 60, 70),
                new("LLB", "LB", "LB", 45, 40),
                new("RLB", "LB", "LB", 55, 40),
                new("LCB", "CB", "DB", 15, 55),
                new("RCB", "CB", "DB", 85, 55),
                new("NCB", "NCB", "DB", 25, 65),
                new("DCB", "DCB", "DB", 70, 53),
                new("FS", "FS", "DB", 35, 15),
                new("SS", "SS", "DB", 65, 15)
            ])
            ];
        }

        public static List<FormationInfo> GetSpecialTeamsFormations()
        {
            return
            [
                new("SpecialTeams", "Special Teams",
            [
                new("K", "K", "K", 40, 75),
                new("P", "P", "P", 60, 75),
                new("LS", "LS", "LS", 50, 45),
                new("KR", "KR", "ATH", 25, 55),
                new("PR", "PR", "ATH", 75, 55)
            ])
            ];
        }
    }
}