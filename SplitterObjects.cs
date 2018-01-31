using System;
using System.ComponentModel;
namespace LiveSplit.Celeste {
	public enum LogObject {
		CurrentSplit,
		Pointers,
		GameTime,
		LevelTime,
		ShowInputUI,
		Menu,
		Completed,
		Started,
		Deaths,
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
		Core = 9
	}
	public enum AreaMode {
		ASide,
		BSide,
		CSide
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
		[Description("Prologue (Complete)")]
		Prologue,
		[Description("Chapter 1 - Crossing (Checkpoint 1)")]
		Chapter1Checkpoint1,
		[Description("Chapter 1 - Chasm (Checkpoint 2)")]
		Chapter1Checkpoint2,
		[Description("Chapter 1 - Forsaken City (Complete)")]
		Chapter1,
		[Description("Chapter 2 - Intervention (Checkpoint 1)")]
		Chapter2Checkpoint1,
		[Description("Chapter 2 - Awake (Checkpoint 2)")]
		Chapter2Checkpoint2,
		[Description("Chapter 2 - Old Site (Complete)")]
		Chapter2,
		[Description("Chapter 3 - Huge Mess (Checkpoint 1)")]
		Chapter3Checkpoint1,
		[Description("Chapter 3 - Elevator Shaft (Checkpoint 2)")]
		Chapter3Checkpoint2,
		[Description("Chapter 3 - Presidential Suite (Checkpoint 3)")]
		Chapter3Checkpoint3,
		[Description("Chapter 3 - Celestial Resort (Complete)")]
		Chapter3,
		[Description("Chapter 4 - Shrine (Checkpoint 1)")]
		Chapter4Checkpoint1,
		[Description("Chapter 4 - Old Trail (Checkpoint 2)")]
		Chapter4Checkpoint2,
		[Description("Chapter 4 - Cliff Face (Checkpoint 3)")]
		Chapter4Checkpoint3,
		[Description("Chapter 4 - Golden Ridge (Complete)")]
		Chapter4,
		[Description("Chapter 5 - Depths (Checkpoint 1)")]
		Chapter5Checkpoint1,
		[Description("Chapter 5 - Unravelling (Checkpoint 2)")]
		Chapter5Checkpoint2,
		[Description("Chapter 5 - Search (Checkpoint 3)")]
		Chapter5Checkpoint3,
		[Description("Chapter 5 - Rescue (Checkpoint 4)")]
		Chapter5Checkpoint4,
		[Description("Chapter 5 - Mirror Temple (Complete)")]
		Chapter5,
		[Description("Chapter 6 - Lake (Checkpoint 1)")]
		Chapter6Checkpoint1,
		[Description("Chapter 6 - Hollows (Checkpoint 2)")]
		Chapter6Checkpoint2,
		[Description("Chapter 6 - Reflection (Checkpoint 3)")]
		Chapter6Checkpoint3,
		[Description("Chapter 6 - Rock Bottom (Checkpoint 4)")]
		Chapter6Checkpoint4,
		[Description("Chapter 6 - Resolution (Checkpoint 5)")]
		Chapter6Checkpoint5,
		[Description("Chapter 6 - Reflection (Complete)")]
		Chapter6,
		[Description("Chapter 7 - 500M (Checkpoint 1)")]
		Chapter7Checkpoint1,
		[Description("Chapter 7 - 1000M (Checkpoint 2)")]
		Chapter7Checkpoint2,
		[Description("Chapter 7 - 1500M (Checkpoint 3)")]
		Chapter7Checkpoint3,
		[Description("Chapter 7 - 2000M (Checkpoint 4)")]
		Chapter7Checkpoint4,
		[Description("Chapter 7 - 2500M (Checkpoint 5)")]
		Chapter7Checkpoint5,
		[Description("Chapter 7 - 3000M (Checkpoint 6)")]
		Chapter7Checkpoint6,
		[Description("Chapter 7 - The Summit (Complete)")]
		Chapter7,
		[Description("Epilogue (Complete)")]
		Epilogue,
		[Description("Chapter 8 - Into The Core (Checkpoint 1)")]
		Chapter8Checkpoint1,
		[Description("Chapter 8 - Hot And Cold (Checkpoint 2)")]
		Chapter8Checkpoint2,
		[Description("Chapter 8 - Heart Of The Mountain (Checkpoint 3)")]
		Chapter8Checkpoint3,
		[Description("Chapter 8 - Core (Complete)")]
		Chapter8,
		[Description("Chapter 1 - Cassette (Pickup)")]
		Chapter1Cassette,
		[Description("Chapter 1 - Heart Gem (Pickup)")]
		Chapter1HeartGem,
		[Description("Chapter 2 - Cassette (Pickup)")]
		Chapter2Cassette,
		[Description("Chapter 2 - Heart Gem (Pickup)")]
		Chapter2HeartGem,
		[Description("Chapter 3 - Cassette (Pickup)")]
		Chapter3Cassette,
		[Description("Chapter 3 - Heart Gem (Pickup)")]
		Chapter3HeartGem,
		[Description("Chapter 4 - Cassette (Pickup)")]
		Chapter4Cassette,
		[Description("Chapter 4 - Heart Gem (Pickup)")]
		Chapter4HeartGem,
		[Description("Chapter 5 - Cassette (Pickup)")]
		Chapter5Cassette,
		[Description("Chapter 5 - Heart Gem (Pickup)")]
		Chapter5HeartGem,
		[Description("Chapter 6 - Cassette (Pickup)")]
		Chapter6Cassette,
		[Description("Chapter 6 - Heart Gem (Pickup)")]
		Chapter6HeartGem,
		[Description("Chapter 7 - Cassette (Pickup)")]
		Chapter7Cassette,
		[Description("Chapter 7 - Heart Gem (Pickup)")]
		Chapter7HeartGem,
		[Description("Chapter 8 - Cassette (Pickup)")]
		Chapter8Cassette,
		[Description("Chapter 8 - Heart Gem (Pickup)")]
		Chapter8HeartGem,
	}
	public class SplitInfo {
		public SplitType Type { get; set; }
		public SplitInfo() { }
		public SplitInfo(string copy) {
			string[] info = copy.Split(',');
			if (info.Length > 0) {
				SplitType temp;
				if (Enum.TryParse(info[0], out temp)) {
					Type = temp;
				}
			}
		}
		public override string ToString() {
			return Type.ToString();
		}
	}
}