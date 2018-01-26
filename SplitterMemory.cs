using LiveSplit.Memory;
using System;
using System.Diagnostics;
namespace LiveSplit.Celeste {
	//.load C:\Windows\Microsoft.NET\Framework\v4.0.30319\SOS.dll
	public partial class SplitterMemory {
		private static ProgramPointer Celeste = new ProgramPointer(AutoDeref.Single, new ProgramSignature(PointerVersion.V1, "83C604F30F7E06660FD6078BCBFF15????????8D15", 21));
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public SplitterMemory() {
			lastHooked = DateTime.MinValue;
		}
		public string CelestePointer() {
			return Celeste.GetPointer(Program).ToString("X");
		}
		public double GameTime() {
			//Celeste.scene.MethodTable.TypeSize
			int size = Celeste.Read<int>(Program, 0x0, 0x98, 0x0, 0x4);
			if (size == 340) {
				//((Level)Celeste.scene).session.Time
				return (double)Celeste.Read<long>(Program, 0x0, 0x98, 0x2c, 0x4) / (double)10000000;
			}
			return 0;
		}
		public bool LevelCompleted() {
			//Celeste.scene.MethodTable.TypeSize
			int size = Celeste.Read<int>(Program, 0x0, 0x98, 0x0, 0x4);
			if (size == 340) {
				//((Level)Celeste.scene).Completed
				return Celeste.Read<bool>(Program, 0x0, 0x98, 0x2a);
			}
			return false;
		}
		public int Deaths() {
			//Celeste.scene.MethodTable.TypeSize
			int size = Celeste.Read<int>(Program, 0x0, 0x98, 0x0, 0x4);
			if (size == 340) {
				//((Level)Celeste.scene).session.Deaths
				return Celeste.Read<int>(Program, 0x0, 0x98, 0x2c, 0x40);
			}
			return 0;
		}
		public bool ShowInputUI() {
			//Celeste.scene.MethodTable.TypeSize
			int size = Celeste.Read<int>(Program, 0x0, 0x98, 0x0, 0x4);
			if (size == 100) {
				//((Overworld)Celeste.scene).showInputUI
				return Celeste.Read<bool>(Program, 0x0, 0x98, 0x2b);
			}
			return false;
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
	public enum PointerVersion {
		V1
	}
	public enum AutoDeref {
		None,
		Single,
		Double
	}
	public class ProgramSignature {
		public PointerVersion Version { get; set; }
		public string Signature { get; set; }
		public int Offset { get; set; }
		public ProgramSignature(PointerVersion version, string signature, int offset) {
			Version = version;
			Signature = signature;
			Offset = offset;
		}
		public override string ToString() {
			return Version.ToString() + " - " + Signature;
		}
	}
	public class ProgramPointer {
		private int lastID;
		private DateTime lastTry;
		private ProgramSignature[] signatures;
		private int[] offsets;
		public IntPtr Pointer { get; private set; }
		public PointerVersion Version { get; private set; }
		public AutoDeref AutoDeref { get; private set; }

		public ProgramPointer(AutoDeref autoDeref, params ProgramSignature[] signatures) {
			AutoDeref = autoDeref;
			this.signatures = signatures;
			lastID = -1;
			lastTry = DateTime.MinValue;
		}
		public ProgramPointer(AutoDeref autoDeref, params int[] offsets) {
			AutoDeref = autoDeref;
			this.offsets = offsets;
			lastID = -1;
			lastTry = DateTime.MinValue;
		}

		public T Read<T>(Process program, params int[] offsets) where T : struct {
			GetPointer(program);
			return program.Read<T>(Pointer, offsets);
		}
		public string Read(Process program, params int[] offsets) {
			GetPointer(program);
			return program.Read(Pointer, offsets);
		}
		public byte[] ReadBytes(Process program, int length, params int[] offsets) {
			GetPointer(program);
			return program.Read(Pointer, length, offsets);
		}
		public void Write<T>(Process program, T value, params int[] offsets) where T : struct {
			GetPointer(program);
			program.Write<T>(Pointer, value, offsets);
		}
		public void Write(Process program, byte[] value, params int[] offsets) {
			GetPointer(program);
			program.Write(Pointer, value, offsets);
		}
		public void ClearPointer() {
			Pointer = IntPtr.Zero;
		}
		public IntPtr GetPointer(Process program) {
			if (program == null) {
				Pointer = IntPtr.Zero;
				lastID = -1;
				return Pointer;
			} else if (program.Id != lastID) {
				Pointer = IntPtr.Zero;
				lastID = program.Id;
			}

			if (Pointer == IntPtr.Zero && DateTime.Now > lastTry.AddSeconds(1)) {
				lastTry = DateTime.Now;

				Pointer = GetVersionedFunctionPointer(program);
				if (Pointer != IntPtr.Zero) {
					if (AutoDeref != AutoDeref.None) {
						if (MemoryReader.is64Bit) {
							Pointer = (IntPtr)program.Read<ulong>(Pointer);
						} else {
							Pointer = (IntPtr)program.Read<uint>(Pointer);
						}
						if (AutoDeref == AutoDeref.Double) {
							if (MemoryReader.is64Bit) {
								Pointer = (IntPtr)program.Read<ulong>(Pointer);
							} else {
								Pointer = (IntPtr)program.Read<uint>(Pointer);
							}
						}
					}
				}
			}
			return Pointer;
		}
		private IntPtr GetVersionedFunctionPointer(Process program) {
			if (signatures != null) {
				MemorySearcher searcher = new MemorySearcher();
				searcher.MemoryFilter = delegate (MemInfo info) {
					return (info.State & 0x1000) != 0 && (info.Protect & 0x40) != 0 && (info.Protect & 0x100) == 0;
				};
				for (int i = 0; i < signatures.Length; i++) {
					ProgramSignature signature = signatures[i];

					IntPtr ptr = searcher.FindSignature(program, signature.Signature);
					if (ptr != IntPtr.Zero) {
						Version = signature.Version;
						return ptr + signature.Offset;
					}
				}
				return IntPtr.Zero;
			}

			if (MemoryReader.is64Bit) {
				return (IntPtr)program.Read<ulong>(program.MainModule.BaseAddress, offsets);
			} else {
				return (IntPtr)program.Read<uint>(program.MainModule.BaseAddress, offsets);
			}
		}
	}
}