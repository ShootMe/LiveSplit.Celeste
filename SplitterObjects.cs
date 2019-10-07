using System;
using System.ComponentModel;
namespace LiveSplit.Celeste {
    public enum LogObject {
        CurrentSplit,
        Pointers,
        PointerVersion,
        GameTime,
        LevelTime,
        ShowInputUI,
        Menu,
        Completed,
        Started,
        AreaID,
        AreaMode,
        LevelName,
        Strawberries,
        HeartGems,
        Cassettes
    }
    public enum Area {
        Menu = -1,
        Prologue = 0,
        ForsakenCity = 1,
        OldSite = 2,
        CelestialResort = 3,
        GoldenRidge = 4,
        MirrorTemple = 5,
        Reflection = 6,
        TheSummit = 7,
        Epilogue = 8,
        Core = 9,
        Farewell = 10
    }
    public enum AreaMode {
        None = -1,
        ASide = 0,
        BSide = 1,
        CSide = 2
    }
    public enum Menu {
        InGame = 0,
        Intro = 14,
        FileSelect = 60,
        MainMenu = 64,
        ChapterSelect = 80,
        ChapterPanel = 168,
        FileRename = 180
    }
    public enum SplitType {
        [Description("Manual Split (Not Automatic)")]
        Manual,
        [Description("Any Chapter (Complete)")]
        ChapterA,
        [Description("Any Heart Gem (Pickup)")]
        HeartGemAny,
        [Description("Level (On Enter)")]
        LevelEnter,
        [Description("Level (On Exit)")]
        LevelExit,
        [Description("Prologue (Complete)")]
        Prologue,
        [Description("Chapter 1 - Crossing (A) / Contraption (B) (CP 1)")]
        Chapter1Checkpoint1,
        [Description("Chapter 1 - Chasm (A) / Scrap Pit (B) (CP 2)")]
        Chapter1Checkpoint2,
        [Description("Chapter 1 - Forsaken City A/B/C (Complete)")]
        Chapter1,
        [Description("Chapter 2 - Intervention (A) / Combination Lock (B) (CP 1)")]
        Chapter2Checkpoint1,
        [Description("Chapter 2 - Awake (A) / Dream Altar (B) (CP 2)")]
        Chapter2Checkpoint2,
        [Description("Chapter 2 - Old Site A/B/C (Complete)")]
        Chapter2,
        [Description("Chapter 3 - Huge Mess (A) / Staff Quarters (B) (CP 1)")]
        Chapter3Checkpoint1,
        [Description("Chapter 3 - Elevator Shaft (A) / Library (B) (CP 2)")]
        Chapter3Checkpoint2,
        [Description("Chapter 3 - Presidential Suite (A) / Rooftop (B) (CP 3)")]
        Chapter3Checkpoint3,
        [Description("Chapter 3 - Celestial Resort A/B/C (Complete)")]
        Chapter3,
        [Description("Chapter 4 - Shrine (A) / Stepping Stones (B) (CP 1)")]
        Chapter4Checkpoint1,
        [Description("Chapter 4 - Old Trail (A) / Gusty Canyon (B) (CP 2)")]
        Chapter4Checkpoint2,
        [Description("Chapter 4 - Cliff Face (A) / Eye Of The Storm (B) (CP 3)")]
        Chapter4Checkpoint3,
        [Description("Chapter 4 - Golden Ridge A/B/C (Complete)")]
        Chapter4,
        [Description("Chapter 5 - Depths (A) / Central Chamber (B) (CP 1)")]
        Chapter5Checkpoint1,
        [Description("Chapter 5 - Unravelling (A) / Through The Mirror (B) (CP 2)")]
        Chapter5Checkpoint2,
        [Description("Chapter 5 - Search (A) / Mix Master (B) (CP 3)")]
        Chapter5Checkpoint3,
        [Description("Chapter 5 - Rescue (A) (CP 4)")]
        Chapter5Checkpoint4,
        [Description("Chapter 5 - Mirror Temple A/B/C (Complete)")]
        Chapter5,
        [Description("Chapter 6 - Lake (A) / Reflection (B) (CP 1)")]
        Chapter6Checkpoint1,
        [Description("Chapter 6 - Hollows (A) / Rock Bottom (B) (CP 2)")]
        Chapter6Checkpoint2,
        [Description("Chapter 6 - Reflection (A) / Reprieve (B) (CP 3)")]
        Chapter6Checkpoint3,
        [Description("Chapter 6 - Rock Bottom (A) (CP 4)")]
        Chapter6Checkpoint4,
        [Description("Chapter 6 - Resolution (A) (CP 5)")]
        Chapter6Checkpoint5,
        [Description("Chapter 6 - Reflection A/B/C (Complete)")]
        Chapter6,
        [Description("Chapter 7 - 500M (A) / 500M (B) (CP 1)")]
        Chapter7Checkpoint1,
        [Description("Chapter 7 - 1000M (A) / 1000M (B) (CP 2)")]
        Chapter7Checkpoint2,
        [Description("Chapter 7 - 1500M (A) / 1500M (B) (CP 3)")]
        Chapter7Checkpoint3,
        [Description("Chapter 7 - 2000M (A) / 2000M (B) (CP 4)")]
        Chapter7Checkpoint4,
        [Description("Chapter 7 - 2500M (A) / 2500M (B) (CP 5)")]
        Chapter7Checkpoint5,
        [Description("Chapter 7 - 3000M (A) / 3000M (B) (CP 6)")]
        Chapter7Checkpoint6,
        [Description("Chapter 7 - The Summit A/B/C (Complete)")]
        Chapter7,
        [Description("Epilogue (Complete)")]
        Epilogue,
        [Description("Chapter 8 - Into The Core (A) / Into The Core (B) (CP 1)")]
        Chapter8Checkpoint1,
        [Description("Chapter 8 - Hot And Cold (A) / Burning Or Freezing (B) (CP 2)")]
        Chapter8Checkpoint2,
        [Description("Chapter 8 - Heart Of The Mountain (A) / Heartbeat (B) (CP 3)")]
        Chapter8Checkpoint3,
        [Description("Chapter 8 - Core A/B/C (Complete)")]
        Chapter8,
        [Description("Chapter 9 - Singular (CP 1)")]
        Chapter9Checkpoint1,
        [Description("Chapter 9 - Power Source (CP 2)")]
        Chapter9Checkpoint2,
        [Description("Chapter 9 - Remembered (CP 3)")]
        Chapter9Checkpoint3,
        [Description("Chapter 9 - Event Horizon (CP 4)")]
        Chapter9Checkpoint4,
        [Description("Chapter 9 - Determination (CP 5)")]
        Chapter9Checkpoint5,
        [Description("Chapter 9 - Stubbornness (CP 6)")]
        Chapter9Checkpoint6,
        [Description("Chapter 9 - Reconciliation (CP 7)")]
        Chapter9Checkpoint7,
        [Description("Chapter 9 - Farewell (CP 8)")]
        Chapter9Checkpoint8,
        [Description("Chapter 9 - Farewell (Complete)")]
        Chapter9,
        [Description("Chapter 1 - Cassette (Pickup)")]
        Chapter1Cassette,
        [Description("Chapter 1 - Heart Gem A/B/C (Pickup)")]
        Chapter1HeartGem,
        [Description("Chapter 2 - Cassette (Pickup)")]
        Chapter2Cassette,
        [Description("Chapter 2 - Heart Gem A/B/C (Pickup)")]
        Chapter2HeartGem,
        [Description("Chapter 3 - Cassette (Pickup)")]
        Chapter3Cassette,
        [Description("Chapter 3 - Heart Gem A/B/C (Pickup)")]
        Chapter3HeartGem,
        [Description("Chapter 4 - Cassette (Pickup)")]
        Chapter4Cassette,
        [Description("Chapter 4 - Heart Gem A/B/C (Pickup)")]
        Chapter4HeartGem,
        [Description("Chapter 5 - Cassette (Pickup)")]
        Chapter5Cassette,
        [Description("Chapter 5 - Heart Gem A/B/C (Pickup)")]
        Chapter5HeartGem,
        [Description("Chapter 6 - Cassette (Pickup)")]
        Chapter6Cassette,
        [Description("Chapter 6 - Heart Gem A/B/C (Pickup)")]
        Chapter6HeartGem,
        [Description("Chapter 7 - Cassette (Pickup)")]
        Chapter7Cassette,
        [Description("Chapter 7 - Heart Gem A/B/C (Pickup)")]
        Chapter7HeartGem,
        [Description("Chapter 8 - Cassette (Pickup)")]
        Chapter8Cassette,
        [Description("Chapter 8 - Heart Gem A/B/C (Pickup)")]
        Chapter8HeartGem,
    }
    public class SplitInfo {
        public SplitType Type { get; set; }
        public string Value { get; set; }
        public SplitInfo() { }
        public SplitInfo(string copy) {
            string[] info = copy.Split(',');
            if (info.Length > 0) {
                SplitType temp;
                if (Enum.TryParse(info[0], out temp)) {
                    Type = temp;
                }
            }

            if (info.Length > 1) {
                Value = info[1];
            }
        }
        public override string ToString() {
            return Type.ToString() + (string.IsNullOrEmpty(Value) ? string.Empty : "," + Value);
        }
    }
}