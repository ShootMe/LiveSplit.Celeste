using LiveSplit.Memory;
using System;
using System.Diagnostics;
namespace LiveSplit.Celeste {
	//.load C:\Windows\Microsoft.NET\Framework\v4.0.30319\SOS.dll
	public partial class SplitterMemory {
		private static ProgramPointer Celeste = new ProgramPointer(AutoDeref.Single, new ProgramSignature(PointerVersion.V1, "83C604F30F7E06660FD6078BCBFF15????????8D15", 21));
		//private static ProgramPointer AreaData = new ProgramPointer(AutoDeref.Single, new ProgramSignature(PointerVersion.V1, "8B3D????????3B770C720DB90D0000008D5109E8????????8B47043B70040F83", 2));
		private static ProgramPointer SaveData = new ProgramPointer(AutoDeref.Single, new ProgramSignature(PointerVersion.V1, "8B7C90088B432C8B40048D5B6C8B53043B500473368B7490088B15", 27));
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public SplitterMemory() {
			lastHooked = DateTime.MinValue;
		}
		public string RAMPointers() {
			return Celeste.GetPointer(Program).ToString("X") + " " + SaveData.GetPointer(Program).ToString("X");
		}
		public bool ChapterCompleted() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			//Celeste.Instance.AutosplitterInfo.ChapterComplete
			if (minor == 1 && build == 6) {
				return Celeste.Read<bool>(Program, 0x0, 0xac, 0x11);
			}
			return Celeste.Read<bool>(Program, 0x0, 0xac, 0x32);
		}
		public string LevelName() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			//Celeste.Instance.AutosplitterInfo.Level
			if (minor == 1 && build == 6) {
				return Celeste.Read(Program, 0x0, 0xac, 0x4, 0x0);
			}
			return Celeste.Read(Program, 0x0, 0xac, 0x14, 0x0);
		}
		public Area AreaID() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			//Celeste.Instance.AutosplitterInfo.Chapter
			if (minor == 1 && build == 6) {
				return (Area)Celeste.Read<int>(Program, 0x0, 0xac, 0x8);
			}
			return (Area)Celeste.Read<int>(Program, 0x0, 0xac, 0x18);
		}
		public AreaMode AreaDifficulty() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			//Celeste.Instance.AutosplitterInfo.Mode
			if (minor == 1 && build == 6) {
				return (AreaMode)SaveData.Read<int>(Program, 0x0, 0xac, 0xc);
			}
			return (AreaMode)SaveData.Read<int>(Program, 0x0, 0xac, 0x1c);
		}
		public bool ChapterStarted() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			//Celeste.Instance.AutosplitterInfo.ChapterStarted
			if (minor == 1 && build == 6) {
				return Celeste.Read<bool>(Program, 0x0, 0xac, 0x10);
			}
			return Celeste.Read<bool>(Program, 0x0, 0xac, 0x31);
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
		public double GameTime() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			if (minor == 1 && build == 6) {
				//SaveData.Instance.Time
				return (double)SaveData.Read<long>(Program, 0x0, 0x4) / (double)10000000;
			}
			//Celeste.Instance.AutosplitterInfo.FileTime
			return (double)Celeste.Read<long>(Program, 0x0, 0xac, 0xc) / (double)10000000;
		}
		public int Strawberries() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			if (minor == 1 && build == 6) {
				//SaveData.Instance.TotalStrawberries
				return SaveData.Read<int>(Program, 0x0, 0x34);
			}
			//Celeste.Instance.AutosplitterInfo.FileStrawberries
			return Celeste.Read<int>(Program, 0x0, 0xac, 0x24);
		}
		public int Deaths() {
			//SaveData.Instance.TotalDeaths
			return SaveData.Read<int>(Program, 0x0, 0x30);
		}
		public double LevelTime() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			if (minor == 1 && build == 6) {
				//SaveData.Instance.CurrentSession.Time
				return (double)SaveData.Read<long>(Program, 0x0, 0x24, 0x4) / (double)10000000;
			}
			//Celeste.Instance.AutosplitterInfo.ChapterTime
			return (double)Celeste.Read<long>(Program, 0x0, 0xac, 0x4) / (double)10000000;
		}
		public int Cassettes() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			if (minor == 1 && build == 6) {
				//SaveData.Instance.Areas.Size
				int size = SaveData.Read<int>(Program, 0x0, 0x28, 0xc);
				int count = 0;
				for (int i = 0; i < size; i++) {
					//SaveData.Instance.Areas[i].Cassette
					if (SaveData.Read<bool>(Program, 0x0, 0x28, 0x4, 0x8 + (i * 4), 0xc)) {
						count++;
					}
				}
				return count;
			}
			//Celeste.Instance.AutosplitterInfo.FileCassettes
			return Celeste.Read<int>(Program, 0x0, 0xac, 0x28);
		}
		public int HeartGems() {
			int minor = Celeste.Read<int>(Program, 0x0, 0x94, 0x8);
			int build = Celeste.Read<int>(Program, 0x0, 0x94, 0xc);
			if (minor == 1 && build == 6) {
				//SaveData.Instance.Areas.Size
				int size = SaveData.Read<int>(Program, 0x0, 0x28, 0xc);
				int count = 0;
				for (int i = 0; i < size; i++) {
					IntPtr area = (IntPtr)SaveData.Read<uint>(Program, 0x0, 0x28, 0x4, 0x8 + (i * 4), 0x4);
					for (int j = 0; j < 3; j++) {
						//SaveData.Instance.Areas[i].Modes[j].HeartGem
						if (Program.Read<bool>(area, 0x8 + (j * 4), 0x36)) {
							count++;
						}
					}
				}
				return count;
			}
			//Celeste.Instance.AutosplitterInfo.FileHearts
			return Celeste.Read<int>(Program, 0x0, 0xac, 0x2c);
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