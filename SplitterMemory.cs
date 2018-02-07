using LiveSplit.Memory;
using System;
using System.Diagnostics;
namespace LiveSplit.Celeste {
	//.load C:\Windows\Microsoft.NET\Framework\v4.0.30319\SOS.dll
	public partial class SplitterMemory {
		private static ProgramPointer Celeste = new ProgramPointer(AutoDeref.Single, new ProgramSignature(PointerVersion.V1, "83C604F30F7E06660FD6078BCBFF15????????8D15", 21));
		//private static ProgramPointer AreaData = new ProgramPointer(AutoDeref.Single, new ProgramSignature(PointerVersion.V1, "8B3D????????3B770C720DB90D0000008D5109E8????????8B47043B70040F83", 2));
		//private static ProgramPointer SaveData = new ProgramPointer(AutoDeref.Single, new ProgramSignature(PointerVersion.V1, "8B7C90088B432C8B40048D5B6C8B53043B500473368B7490088B15", 27));
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public SplitterMemory() {
			lastHooked = DateTime.MinValue;
		}
		public string RAMPointers() {
			return Celeste.GetPointer(Program).ToString("X");
		}
		public bool ChapterCompleted() {
			//int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			//int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			//Celeste.Instance.AutosplitterInfo.ChapterComplete
			return Celeste.Read<bool>(Program, 0x0, 0xac, 0x32);
		}
		public string LevelName() {
			//Celeste.Instance.AutosplitterInfo.Level
			return Celeste.Read(Program, 0x0, 0xac, 0x14, 0x0);
		}
		public Area AreaID() {
			//Celeste.Instance.AutosplitterInfo.Chapter
			return (Area)Celeste.Read<int>(Program, 0x0, 0xac, 0x18);
		}
		public AreaMode AreaDifficulty() {
			//Celeste.Instance.AutosplitterInfo.Mode
			return (AreaMode)Celeste.Read<int>(Program, 0x0, 0xac, 0x1c);
		}
		public bool ChapterStarted() {
			//Celeste.Instance.AutosplitterInfo.ChapterStarted
			return Celeste.Read<bool>(Program, 0x0, 0xac, 0x31);
		}
		public double GameTime() {
			//Celeste.Instance.AutosplitterInfo.FileTime
			return (double)Celeste.Read<long>(Program, 0x0, 0xac, 0xc) / (double)10000000;
		}
		public double LevelTime() {
			//Celeste.Instance.AutosplitterInfo.ChapterTime
			return (double)Celeste.Read<long>(Program, 0x0, 0xac, 0x4) / (double)10000000;
		}
		public int Strawberries() {
			//Celeste.Instance.AutosplitterInfo.FileStrawberries
			return Celeste.Read<int>(Program, 0x0, 0xac, 0x24);
		}
		public int Cassettes() {
			//Celeste.Instance.AutosplitterInfo.FileCassettes
			return Celeste.Read<int>(Program, 0x0, 0xac, 0x28);
		}
		public int HeartGems() {
			//Celeste.Instance.AutosplitterInfo.FileHearts
			return Celeste.Read<int>(Program, 0x0, 0xac, 0x2c);
		}
		public bool ShowInputUI() {
			//Celeste.Instance.scene.MethodTable.TypeSize
			int size = Celeste.Read<int>(Program, 0x0, 0x98, 0x0, 0x4);
			if (size == 100) {
				//((Overworld)Celeste.Instance.scene).showInputUI
				return Celeste.Read<bool>(Program, 0x0, 0x98, 0x2b);
			}
			return false;
		}
		public Menu MenuType() {
			//Celeste.Instance.scene.MethodTable.TypeSize
			int size = Celeste.Read<int>(Program, 0x0, 0x98, 0x0, 0x4);
			if (size == 100) {
				//((Overworld)Celeste.Instance.scene).current.MethodTable.TypeSize
				return (Menu)Celeste.Read<int>(Program, 0x0, 0x98, 0x30, 0x0, 0x4);
			}
			return Menu.InGame;
		}
		public bool HookProcess() {
			IsHooked = Program != null && !Program.HasExited;
			if (!IsHooked && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("Celeste");
				Program = processes != null && processes.Length > 0 ? processes[0] : null;

				if (Program != null && !Program.HasExited) {
					MemoryReader.Update64Bit(Program);
					IsHooked = true;
				}
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
}