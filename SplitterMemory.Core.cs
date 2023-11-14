using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiveSplit.Celeste {
    public partial class SplitterMemoryCore : ISplitterMemory {
        private static readonly byte[] CoreAutoSplitterInfoMagic =
            Encoding.ASCII.GetBytes("EVERESTAUTOSPLIT").Concat(new byte[] { 0xf0, 0xf1, 0xf2, 0xf3, }).ToArray();

        private const int CoreAutoSplitterInfoMinVersion = 3;

        [Flags]
        private enum AutoSplitterChapterFlags : uint {
            ChapterStarted = 1U << 0,
            ChapterComplete = 1U << 1,
            ChapterCassette = 1U << 2,
            ChapterHeart = 1U << 3,
            GrabbedGolden = 1U << 4,

            TimerActive = 1U << 31
        }

        [Flags]
        private enum AutoSplitterFileFlags : uint {
            IsDebug = 1U << 0,
            AssistMode = 1U << 1,
            VariantsMode = 1U << 2,

            StartingNewFile = 1U << 30,
            FileActive = 1U << 31,
        }

        public Process Program { get; private set; }
        public bool IsHooked { get; private set; } = false;
        public DateTime LastHooked { get; private set; } = DateTime.MinValue;

        private bool? highPriority;
        private IntPtr infoPointer;

        public void Dispose() {
            Program?.Dispose();
        }

        public bool HookProcess() {
            IsHooked = Program != null && !Program.HasExited;
            if (IsHooked || DateTime.Now < LastHooked.AddSeconds(1)) return IsHooked;

            LastHooked = DateTime.Now;
            Program = null;

            foreach (Process proc in Process.GetProcessesByName("Celeste")) {
                if (proc.HasExited) continue;
                MemoryReader.Update64Bit(proc);
                foreach (MemInfo procMem in proc.MemRegions()) {
                    if (!procMem.IsCommited || procMem.IsGuardPage) continue;

                    if (proc.Read(procMem.BaseAddress, 20).SequenceEqual(CoreAutoSplitterInfoMagic)) {
                        if (proc.Read<byte>(procMem.BaseAddress, 0x17) < CoreAutoSplitterInfoMinVersion) continue;

                        Program = proc;
                        highPriority = null;
                        infoPointer = procMem.BaseAddress;
                        return IsHooked = true;
                    }
                }
            }

            return false;
        }

        public string RAMPointers() => infoPointer.ToString("X");
        public string RAMPointerVersion() => "EverestCore";

        public int CelesteFieldOffs() => -1;

        public bool? IsHighPriority() {
            return highPriority;
        }
        public void SetHighPriority(bool isHighPriority) {
            if (!IsHooked || (highPriority.HasValue && highPriority.Value == isHighPriority)) return;
            Program.PriorityClass = isHighPriority ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            highPriority = isHighPriority;
        }

        public bool ChapterCompleted() {
            //CoreAutoSplitterInfo.ChapterFlags
            return (Program.Read<AutoSplitterChapterFlags>(infoPointer, 0x4c) & AutoSplitterChapterFlags.ChapterComplete) != 0;

        }
        public string LevelName() {
            //CoreAutoSplitterInfo.RoomName
            IntPtr levelNameStr = Program.Read<IntPtr>(infoPointer, 0x38);
            ushort nameLen = Program.Read<ushort>(levelNameStr - 2);
            return Encoding.UTF8.GetString(Program.Read(levelNameStr, nameLen));
        }
        public Area AreaID() {
            //CoreAutoSplitterInfo.ChapterID
            return (Area) Program.Read<int>(infoPointer, 0x30);
        }
        public AreaMode AreaDifficulty() {
            //CoreAutoSplitterInfo.ChapterMode
            return (AreaMode)Program.Read<int>(infoPointer, 0x34);
        }
        public bool ChapterStarted() {
            //CoreAutoSplitterInfo.ChapterFlags
            return (Program.Read<AutoSplitterChapterFlags>(infoPointer, 0x4c) & AutoSplitterChapterFlags.ChapterStarted) != 0;
        }
        public double GameTime() {
            //CoreAutoSplitterInfo.FileTime
            return (double) Program.Read<long>(infoPointer, 0x50) / (double) TimeSpan.TicksPerSecond;
        }
        public double LevelTime() {
            //CoreAutoSplitterInfo.ChapterTime
            return (double) Program.Read<long>(infoPointer, 0x40) / (double)TimeSpan.TicksPerSecond;
        }
        public int Strawberries() {
            //CoreAutoSplitterInfo.FileStrawberries
            return Program.Read<int>(infoPointer, 0x58);
        }
        public int Cassettes() {
            //CoreAutoSplitterInfo.FileCassettes
            return Program.Read<int>(infoPointer, 0x60);
        }
        public bool ChapterCassetteCollected() {
            //CoreAutoSplitterInfo.ChapterFlags
            return (Program.Read<AutoSplitterChapterFlags>(infoPointer, 0x4c) & AutoSplitterChapterFlags.ChapterCassette) != 0;
        }
        public int HeartGems() {
            //CoreAutoSplitterInfo.FileHearts
            return Program.Read<int>(infoPointer, 0x64);
        }
        public bool ChapterHeartCollected() {
            //CoreAutoSplitterInfo.ChapterFlags
            return (Program.Read<AutoSplitterChapterFlags>(infoPointer, 0x4c) & AutoSplitterChapterFlags.ChapterHeart) != 0;
        }
        public bool StartingNewFile() {
            //CoreAutoSplitterInfo.FileFlags
            return (Program.Read<AutoSplitterFileFlags>(infoPointer, 0x68) & AutoSplitterFileFlags.StartingNewFile) != 0;
        }
    }
}