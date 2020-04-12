using System;
using System.Diagnostics;
namespace LiveSplit.Celeste {
    public enum PointerVersion {
        XNA,
        OpenGL,
        Itch
    }
    public enum AutoDeref {
        None,
        Single,
        Double
    }
    public class ProgramPointer {
        private int lastID;
        private DateTime lastTry;
        private IFindPointer currentFinder;
        public IntPtr Pointer { get; private set; }
        public IFindPointer[] Finders { get; private set; }
        public string AsmName { get; private set; }
        public PointerVersion Version {
            get {
                return (currentFinder?.Version).GetValueOrDefault(PointerVersion.XNA);
            }
        }

        public ProgramPointer(params IFindPointer[] finders) : this(string.Empty, finders) { }
        public ProgramPointer(string asmName, params IFindPointer[] finders) {
            AsmName = asmName;
            Finders = finders;
            lastID = -1;
            lastTry = DateTime.MinValue;
        }

        public T Read<T>(Process program, params int[] offsets) where T : unmanaged {
            GetPointer(program);
            return program.Read<T>(Pointer, offsets);
        }
        public string Read(Process program, params int[] offsets) {
            GetPointer(program);
            return program.ReadString(Pointer, offsets);
        }
        public byte[] ReadBytes(Process program, int length, params int[] offsets) {
            GetPointer(program);
            return program.Read(Pointer, length, offsets);
        }
        public void Write<T>(Process program, T value, params int[] offsets) where T : unmanaged {
            GetPointer(program);
            program.Write<T>(Pointer, value, offsets);
        }
        public void Write(Process program, byte[] value, params int[] offsets) {
            GetPointer(program);
            program.Write(Pointer, value, offsets);
        }
        public IntPtr GetPointer(Process program) {
            if (program == null) {
                Pointer = IntPtr.Zero;
                lastID = -1;
                return Pointer;
            } else if (program.Id != lastID) {
                Pointer = IntPtr.Zero;
                lastID = program.Id;
            } else if (Pointer != IntPtr.Zero && !currentFinder.VerifyPointer(program)) {
                Pointer = IntPtr.Zero;
            }

            if (Pointer == IntPtr.Zero && DateTime.Now > lastTry) {
                lastTry = DateTime.Now.AddSeconds(1);

                for (int i = 0; i < Finders.Length; i++) {
                    IFindPointer finder = Finders[i];
                    try {
                        Pointer = finder.FindPointer(program, AsmName);
                        if (Pointer != IntPtr.Zero || finder.FoundBaseAddress()) {
                            currentFinder = finder;
                            break;
                        }
                    } catch { }
                }
            }
            return Pointer;
        }
        public static IntPtr DerefPointer(Process program, IntPtr pointer, AutoDeref autoDeref) {
            if (pointer != IntPtr.Zero) {
                if (autoDeref != AutoDeref.None) {
                    pointer = program.Read<IntPtr>(pointer);

                    if (autoDeref == AutoDeref.Double) {
                        pointer = program.Read<IntPtr>(pointer);
                    }
                }
            }
            return pointer;
        }
        public static Tuple<IntPtr, IntPtr> GetAddressRange(Process program, string asmName) {
            Module64 module = program.Module64(asmName);
            if (module != null) {
                return new Tuple<IntPtr, IntPtr>(module.BaseAddress, module.BaseAddress + module.MemorySize);
            }
            return new Tuple<IntPtr, IntPtr>(IntPtr.Zero, IntPtr.Zero);
        }
    }
    public interface IFindPointer {
        IntPtr FindPointer(Process program, string asmName);
        bool FoundBaseAddress();
        bool VerifyPointer(Process program);
        PointerVersion Version { get; }
    }
    public class FindPointerSignature : IFindPointer {
        public PointerVersion Version { get; private set; }
        private AutoDeref AutoDeref;
        private string Signature;
        private IntPtr BasePtr;
        private MemorySearcher Searcher;
        private DateTime LastVerified;
        private int[] Relative;

        public FindPointerSignature(PointerVersion version, AutoDeref autoDeref, string signature, params int[] relative) {
            Version = version;
            AutoDeref = autoDeref;
            Signature = signature;
            BasePtr = IntPtr.Zero;
            Searcher = new MemorySearcher();
            LastVerified = DateTime.MaxValue;
            Relative = relative;
        }

        public bool FoundBaseAddress() {
            return BasePtr != IntPtr.Zero;
        }
        public bool VerifyPointer(Process program) {
            DateTime now = DateTime.Now;
            if (now > LastVerified) {
                bool isValid = Searcher.VerifySignature(program, BasePtr, Signature);
                LastVerified = now.AddSeconds(5);
                if (!isValid) {
                    BasePtr = IntPtr.Zero;
                    return false;
                }
            }
            return true;
        }
        public IntPtr FindPointer(Process program, string asmName) {
            return ProgramPointer.DerefPointer(program, GetPointer(program, asmName), AutoDeref);
        }
        private IntPtr GetPointer(Process program, string asmName) {
            if (string.IsNullOrEmpty(asmName)) {
                Searcher.MemoryFilter = delegate (MemInfo info) {
                    return (info.State & 0x1000) != 0 && (info.Protect & 0x40) != 0 && (info.Protect & 0x100) == 0;
                };
            } else {
                Tuple<IntPtr, IntPtr> range = ProgramPointer.GetAddressRange(program, asmName);
                Searcher.MemoryFilter = delegate (MemInfo info) {
                    return (ulong)info.BaseAddress >= (ulong)range.Item1 && (ulong)info.BaseAddress <= (ulong)range.Item2 && (info.State & 0x1000) != 0 && (info.Protect & 0x20) != 0 && (info.Protect & 0x100) == 0;
                };
            }

            BasePtr = Searcher.FindSignature(program, Signature);
            if (BasePtr != IntPtr.Zero) {
                LastVerified = DateTime.Now.AddSeconds(5);
                int offset = CalculateRelative(program);
                return BasePtr + offset;
            }
            return BasePtr;
        }
        private int CalculateRelative(Process program) {
            int maxIndex = Relative.Length - 1;
            if (Relative == null || maxIndex < 0) { return 0; }

            int offset = 0;
            for (int i = 0; i < maxIndex; i++) {
                offset += Relative[i];
                offset += program.Read<int>(BasePtr + offset) + 4;
            }
            return offset + Relative[maxIndex];
        }
    }
    public class FindOffset : IFindPointer {
        private int[] Offsets;
        private IntPtr BasePtr;
        private AutoDeref AutoDeref;
        private DateTime LastVerified;
        public PointerVersion Version { get; private set; }

        public FindOffset(PointerVersion version, AutoDeref autoDeref, params int[] offsets) {
            Version = version;
            AutoDeref = autoDeref;
            Offsets = offsets;
            LastVerified = DateTime.MaxValue;
        }

        public bool FoundBaseAddress() {
            return BasePtr != IntPtr.Zero;
        }
        public bool VerifyPointer(Process program) {
            return DateTime.Now < LastVerified;
        }
        public IntPtr FindPointer(Process program, string asmName) {
            if (string.IsNullOrEmpty(asmName)) {
                BasePtr = program.MainModule.BaseAddress;
            } else {
                Tuple<IntPtr, IntPtr> range = ProgramPointer.GetAddressRange(program, asmName);
                BasePtr = range.Item1;
            }

            if (Offsets.Length > 1) {
                LastVerified = DateTime.Now.AddSeconds(5);
                return ProgramPointer.DerefPointer(program, program.Read<IntPtr>(BasePtr, Offsets), AutoDeref);
            } else {
                LastVerified = DateTime.MaxValue;
                BasePtr += Offsets[0];
                return ProgramPointer.DerefPointer(program, BasePtr, AutoDeref);
            }
        }
    }
}