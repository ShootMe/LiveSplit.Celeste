using System;
namespace LiveSplit.Celeste {
    public interface ISplitterMemory : IDisposable {
        bool IsHooked { get; }
        DateTime LastHooked { get; }

        bool HookProcess();

        string RAMPointers();
        string RAMPointerVersion();
        int CelesteFieldOffs();
        bool? IsHighPriority();
        void SetHighPriority(bool isHighPriority);

        bool ChapterCompleted();
        string LevelName();
        Area AreaID();
        AreaMode AreaDifficulty();
        bool ChapterStarted();
        double GameTime();
        double LevelTime();
        int Strawberries();
        int Cassettes();
        bool ChapterCassetteCollected();
        int HeartGems();
        bool ChapterHeartCollected();
        bool StartingNewFile();
    }

    public class SplitterMemoryDispatch : ISplitterMemory {
        private readonly ISplitterMemory vanillaMem = new SplitterMemoryVanilla();
        private readonly ISplitterMemory coreMem = new SplitterMemoryCore();

        public ISplitterMemory ActiveMemory { get; private set; }
        private ISplitterMemory ActiveMemoryFail => ActiveMemory ?? throw new InvalidOperationException();

        public bool IsHooked => ActiveMemory?.IsHooked ?? false;
        public DateTime LastHooked => ActiveMemory?.LastHooked ?? DateTime.MinValue;

        public void Dispose() {
            vanillaMem.Dispose();
            coreMem.Dispose();
        }

        public bool HookProcess() {
            if (ActiveMemory != null && ActiveMemory.HookProcess()) return true;

            if (coreMem.HookProcess()) {
                ActiveMemory = coreMem;
                return true;
            } else if (vanillaMem.HookProcess()) {
                ActiveMemory = vanillaMem;
                return true;
            } else {
                ActiveMemory = null;
                return false;
            }
        }

        public string RAMPointers() => ActiveMemoryFail.RAMPointers();
        public string RAMPointerVersion() => ActiveMemoryFail.RAMPointerVersion();
        public int CelesteFieldOffs() => ActiveMemoryFail.CelesteFieldOffs();

        public bool? IsHighPriority() => ActiveMemory?.IsHighPriority() ?? false;
        public void SetHighPriority(bool isHighPriority) => ActiveMemory?.SetHighPriority(isHighPriority);

        public bool ChapterCompleted() => ActiveMemoryFail.ChapterCompleted();
        public string LevelName() => ActiveMemoryFail.LevelName();
        public Area AreaID() => ActiveMemoryFail.AreaID();
        public AreaMode AreaDifficulty() => ActiveMemoryFail.AreaDifficulty();
        public bool ChapterStarted() => ActiveMemoryFail.ChapterStarted();
        public double GameTime() => ActiveMemoryFail.GameTime();
        public double LevelTime() => ActiveMemoryFail.LevelTime();
        public int Strawberries() => ActiveMemoryFail.Strawberries();
        public int Cassettes() => ActiveMemoryFail.Cassettes();
        public bool ChapterCassetteCollected() => ActiveMemoryFail.ChapterCassetteCollected();
        public int HeartGems() => ActiveMemoryFail.HeartGems();
        public bool ChapterHeartCollected() => ActiveMemoryFail.ChapterHeartCollected();
        public bool StartingNewFile() => ActiveMemoryFail.StartingNewFile();
    }
}