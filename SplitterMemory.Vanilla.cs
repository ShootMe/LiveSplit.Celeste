using System;
using System.Diagnostics;
using System.Linq;
namespace LiveSplit.Celeste {
    //.load C:\Windows\Microsoft.NET\Framework\v4.0.30319\SOS.dll
    public partial class SplitterMemoryVanilla : ISplitterMemory {
        //Celeste.Celeste::.ctor
        private static ProgramPointer Celeste = new ProgramPointer(
            new FindPointerSignature(PointerVersion.XNA, AutoDeref.Single, "83C604F30F7E06660FD6078BCBFF15????????8D15", 21),
            new FindPointerSignature(PointerVersion.OpenGL, AutoDeref.Single, "8B55F08B45E88D5274E8????????8B45F08D15", 19),
            new FindPointerSignature(PointerVersion.OpenGL14, AutoDeref.Single, "683804000068C0030000681C020000FF35", 99),
            new FindPointerSignature(PointerVersion.Itch, AutoDeref.Single, "8D5674E8????????8D15????????E8????????C605", 10));

        public Process Program { get; private set; }
        public bool IsHooked { get; private set; } = false;
        public DateTime LastHooked { get; private set; } = DateTime.MinValue;

        private bool? highPriority;
        private IntPtr LastCelesteOffsBase;
        private int LastCelesteOffsSize;
        private int LastCelesteOffs;

        private bool lastShowInputUI;

        public void Dispose() {
            Program?.Dispose();
        }

        public bool HookProcess() {
            IsHooked = Program != null && !Program.HasExited;
            if (!IsHooked && DateTime.Now > LastHooked.AddSeconds(1)) {
                LastHooked = DateTime.Now;
                Program = Process.GetProcessesByName("Celeste").FirstOrDefault(p => {
                    // Vanilla Celeste is a 32 bit managed .NET Framework process
                    // We check for the latter condition by checking for the clr.dll module which should be in C:\Windows\Microsoft.NET\Framework\...
                    return !p.Is64Bit() && p.Modules64().Any(m => m.Name == "clr.dll"&& m.FileName.ToLowerInvariant().Contains("framework"));
                });
                if (Program != null && !Program.HasExited) {
                    MemoryReader.Update64Bit(Program);
                    highPriority = null;
                    IsHooked = true;
                }
            }

            return IsHooked;
        }

        public string RAMPointers() {
            return Celeste.GetPointer(Program).ToString("X");
        }
        public string RAMPointerVersion() {
            return Celeste.Version.ToString();
        }

        public int CelesteFieldOffs() {
            IntPtr basePtr = Celeste.GetPointer(Program);
            if (LastCelesteOffsBase == basePtr)
                return LastCelesteOffs;
            LastCelesteOffsBase = basePtr;
            //Celeste.Instance.MethodTable.TypeSize
            int size = Celeste.Read<int>(Program, 0x0, 0x0, 0x4);
            if (LastCelesteOffsSize == size)
                return LastCelesteOffs;
            LastCelesteOffsSize = size;
            //Scan backwards for field Title that points to string "Celeste"
            //The Title field is the field that we could even care about after all the XNA / FNA Game fields.
            //All other fields are then read with offsets relative to Celeste.Instance.Title
            for (int offs = size - 4; offs >= 0; offs -= 4) {
                //Celeste.Instance.OFFS
                if (Celeste.Read(Program, 0x0, offs, 0x0) == "Celeste") {
                    return LastCelesteOffs = offs;
                }
            }
            //Fallback - retry scanning ASAP though.
            LastCelesteOffsBase = IntPtr.Zero;
            LastCelesteOffsSize = 0;
            if (Celeste.Version == PointerVersion.XNA) {
                return 0x90;
            } else if (Celeste.Version == PointerVersion.OpenGL14) {
                return 0x84;
            }
            return 0x70;
        }
        public bool? IsHighPriority() {
            return highPriority;
        }
        public void SetHighPriority(bool isHighPriority) {
            if (!IsHooked || (highPriority.HasValue && highPriority.Value == isHighPriority)) return;
            Program.PriorityClass = isHighPriority ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            highPriority = isHighPriority;
        }

        public bool ChapterCompleted() {
            //Celeste.Instance.AutosplitterInfo.ChapterComplete
            return Celeste.Read<bool>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x32);

        }
        public string LevelName() {
            //Celeste.Instance.AutosplitterInfo.Level
            return Celeste.Read(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x14, 0x0);
        }
        public Area AreaID() {
            //Celeste.Instance.AutosplitterInfo.Chapter
            return (Area)Celeste.Read<int>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x18);
        }
        public AreaMode AreaDifficulty() {
            //Celeste.Instance.AutosplitterInfo.Mode
            return (AreaMode)Celeste.Read<int>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x1c);
        }
        public bool ChapterStarted() {
            //Celeste.Instance.AutosplitterInfo.ChapterStarted
            return Celeste.Read<bool>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x31);
        }
        public double GameTime() {
            //Celeste.Instance.AutosplitterInfo.FileTime
            return (double)Celeste.Read<long>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0xc) / (double)10000000;
        }
        public double LevelTime() {
            //Celeste.Instance.AutosplitterInfo.ChapterTime
            return (double)Celeste.Read<long>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x4) / (double)10000000;
        }
        public int Strawberries() {
            //Celeste.Instance.AutosplitterInfo.FileStrawberries
            return Celeste.Read<int>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x24);
        }
        public int Cassettes() {
            //Celeste.Instance.AutosplitterInfo.FileCassettes
            return Celeste.Read<int>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x28);
        }
        public bool ChapterCassetteCollected() {
            //Celeste.Instance.AutosplitterInfo.ChapterCassette
            return Celeste.Read<bool>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x33);
        }
        public int HeartGems() {
            //Celeste.Instance.AutosplitterInfo.FileHearts
            return Celeste.Read<int>(Program, 0x0, CelesteFieldOffs() + 0x1c, 0x2c);
        }
        public bool ChapterHeartCollected() {
            //Celeste.Instance.AutosplitterInfo.ChapterHeart
            return Celeste.Read<bool>(Program, 0x0, CelesteFieldOffs()+ 0x1c, 0x34);
        }
        private bool ShowInputUI() {
            //Celeste.Instance.scene.MethodTable.TypeSize
            int size = Celeste.Read<int>(Program, 0x0, CelesteFieldOffs() + 0x8, 0x0, 0x4);
            if (size == 100) {
                //((Overworld)Celeste.Instance.scene).showInputUI
                return Celeste.Read<bool>(Program, 0x0, CelesteFieldOffs() + 0x8, 0x2b);
            }
            return false;
        }
        private bool EnteringFirstChapter() {
            //Celeste.Instance.scene.MethodTable.TypeSize
            int size = Celeste.Read<int>(Program, 0x0, CelesteFieldOffs() + 0x8, 0x0, 0x4);
            if (size == 100) {
                //((Overworld)Celeste.Instance.scene).Maddy.MethodTable.TypeSize
                int maddy3DSize = Celeste.Read<int>(Program, 0x0, CelesteFieldOffs() + 0x8, 0x4C, 0x0, 0x4);
                //((Overworld)Celeste.Instance.scene).Maddy.Disabled
                return Celeste.Read<bool>(Program, 0x0, CelesteFieldOffs() + 0x8, 0x4C, maddy3DSize - 0x1B);
            }
            return false;
        }
        public bool StartingNewFile() {
            bool showInputUI = ShowInputUI();

            bool startingNewFile = !showInputUI && lastShowInputUI && EnteringFirstChapter();

            lastShowInputUI = showInputUI;
            return startingNewFile;
        }
    }
}