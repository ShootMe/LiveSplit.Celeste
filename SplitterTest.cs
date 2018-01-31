using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace LiveSplit.Celeste {
	public class SplitterTest {
		private static SplitterComponent comp = new SplitterComponent(null);
		public static void Main(string[] args) {
			Thread test = new Thread(GetVals);
			test.IsBackground = true;
			test.Start();
			System.Windows.Forms.Application.Run();
		}
		private static void GetVals() {
			while (true) {
				try {
					comp.GetValues();

					Thread.Sleep(12);
				} catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
#if DebugInfo
	public class SplitterComponent {
		private static string LOGFILE = "_Celeste.log";
		private Dictionary<LogObject, string> currentValues = new Dictionary<LogObject, string>();
		private SplitterMemory mem;
		private SplitterSettings settings;
		private int currentSplit = -1, lastLogCheck;
		private bool hasLog = false;

		public SplitterComponent(object model) {
			mem = new SplitterMemory();
			settings = new SplitterSettings();
			foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
				currentValues[key] = "";
			}
		}

		public void GetValues() {
			if (!mem.HookProcess()) { return; }

			LogValues();
		}
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				string prev = string.Empty, curr = string.Empty;
				foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
					prev = currentValues[key];

					switch (key) {
						case LogObject.CurrentSplit: curr = currentSplit.ToString(); break;
						case LogObject.Pointers: curr = mem.RAMPointers(); break;
						//case LogObject.GameTime: curr = mem.GameTime().ToString("0"); break;
						//case LogObject.LevelTime: curr = mem.LevelTime().ToString("0"); break;
						case LogObject.ShowInputUI: curr = mem.ShowInputUI().ToString(); break;
						case LogObject.Started: curr = mem.ChapterStarted().ToString(); break;
						case LogObject.Completed: curr = mem.LevelCompleted().ToString(); break;
						case LogObject.Deaths: curr = mem.Deaths().ToString(); break;
						case LogObject.AreaID: curr = mem.AreaID().ToString(); break;
						case LogObject.AreaMode: curr = mem.AreaDifficulty().ToString(); break;
						case LogObject.LevelName: curr = mem.LevelName(); break;
						case LogObject.Menu: curr = mem.MenuType().ToString(); break;
						case LogObject.Strawberries: curr = mem.Strawberries().ToString(); break;
						case LogObject.Cassettes: curr = mem.Cassettes().ToString(); break;
						case LogObject.HeartGems: curr = mem.HeartGems().ToString(); break;
						default: curr = string.Empty; break;
					}

					if (string.IsNullOrEmpty(prev)) { prev = string.Empty; }
					if (string.IsNullOrEmpty(curr)) { curr = string.Empty; }
					if (!prev.Equals(curr)) {
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + ": " + key.ToString() + ": ".PadRight(16 - key.ToString().Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}
		private void WriteLog(string data) {
			if (hasLog || !Console.IsOutputRedirected) {
				if (Console.IsOutputRedirected) {
					using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
						wr.WriteLine(data);
					}
				} else {
					Console.WriteLine(data);
				}
			}
		}
	}
#endif
}