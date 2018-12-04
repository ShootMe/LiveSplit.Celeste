#if !DebugInfo
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.Celeste {
	public class SplitterComponent : IComponent {
		public string ComponentName { get { return "Celeste Autosplitter"; } }
		public TimerModel Model { get; set; }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		private static string LOGFILE = "_Celeste.txt";
		private Dictionary<LogObject, string> currentValues = new Dictionary<LogObject, string>();
		private SplitterMemory mem;
		private SplitterSettings settings;
		private int currentSplit = -1, lastLogCheck, lastHeartGems, lastCassettes;
		private bool hasLog = false, lastShowInputUI, lastCompleted, exitingChapter;
		private double lastElapsed, levelTimer;
		private string lastLevelName, levelStarted;

		public SplitterComponent(LiveSplitState state) {
			mem = new SplitterMemory();
			settings = new SplitterSettings();
			foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
				currentValues[key] = "";
			}

			if (state != null) {
				Model = new TimerModel() { CurrentState = state };
				Model.InitializeGameTime();
				Model.CurrentState.IsGameTimePaused = true;
				state.OnReset += OnReset;
				state.OnPause += OnPause;
				state.OnResume += OnResume;
				state.OnStart += OnStart;
				state.OnSplit += OnSplit;
				state.OnUndoSplit += OnUndoSplit;
				state.OnSkipSplit += OnSkipSplit;
			}
		}

		public void GetValues() {
			if (!mem.HookProcess()) { return; }

			if (Model != null) {
				HandleSplits();
			}

			LogValues();
		}
		private void HandleSplits() {
			bool shouldSplit = false;

			if (currentSplit == -1 && (settings.Splits.Count == 0 || settings.ChapterSplits)) {
				if (settings.Splits.Count == 0) {
					string levelName = mem.LevelName();

					shouldSplit = !string.IsNullOrEmpty(levelName) && !string.IsNullOrEmpty(lastLevelName) && levelName != lastLevelName;

					if (shouldSplit) {
						levelStarted = lastLevelName;
						levelTimer = mem.LevelTime();
					}
					lastLevelName = levelName;
				} else if (!settings.ILSplits) {
					bool showInputUI = mem.ShowInputUI();

					shouldSplit = !showInputUI && lastShowInputUI && mem.MenuType() == Menu.FileSelect;

					lastShowInputUI = showInputUI;
				} else {
					bool chapterStarted = mem.ChapterStarted();

					shouldSplit = chapterStarted && !lastShowInputUI && DateTime.Now > mem.LastHooked.AddSeconds(3);

					lastShowInputUI = chapterStarted;
				}
			} else {
				double elapsed = settings.ILSplits ? mem.LevelTime() : mem.GameTime();
				bool completed = mem.ChapterCompleted();
				Area areaID = mem.AreaID();
				int addAmount = settings.Splits.Count > 0 && !settings.ChapterSplits ? 1 : 0;
				SplitInfo split = currentSplit + addAmount < settings.Splits.Count ? settings.Splits[currentSplit + addAmount] : null;
				string levelName = mem.LevelName();

				if (split != null && split.Type != SplitType.Manual) {
					switch (split.Type) {
						case SplitType.LevelEnter: shouldSplit = !string.IsNullOrEmpty(levelName) && !string.IsNullOrEmpty(lastLevelName) && levelName != lastLevelName && levelName.Equals(split.Value, StringComparison.OrdinalIgnoreCase); break;
						case SplitType.LevelExit: shouldSplit = !string.IsNullOrEmpty(levelName) && !string.IsNullOrEmpty(lastLevelName) && levelName != lastLevelName && lastLevelName.Equals(split.Value, StringComparison.OrdinalIgnoreCase); break;
						case SplitType.ChapterA: shouldSplit = ChapterSplit(Area.Prologue, Area.Prologue, levelName, completed, elapsed); break;
						case SplitType.Prologue: shouldSplit = ChapterSplit(areaID, Area.Prologue, levelName, completed, elapsed); break;
						case SplitType.Chapter1: shouldSplit = ChapterSplit(areaID, Area.ForsakenCity, levelName, completed, elapsed); break;
						case SplitType.Chapter2: shouldSplit = ChapterSplit(areaID, Area.OldSite, levelName, completed, elapsed); break;
						case SplitType.Chapter3: shouldSplit = ChapterSplit(areaID, Area.CelestialResort, levelName, completed, elapsed); break;
						case SplitType.Chapter4: shouldSplit = ChapterSplit(areaID, Area.GoldenRidge, levelName, completed, elapsed); break;
						case SplitType.Chapter5: shouldSplit = ChapterSplit(areaID, Area.MirrorTemple, levelName, completed, elapsed); break;
						case SplitType.Chapter6: shouldSplit = ChapterSplit(areaID, Area.Reflection, levelName, completed, elapsed); break;
						case SplitType.Chapter7: shouldSplit = ChapterSplit(areaID, Area.TheSummit, levelName, completed, elapsed); break;
						case SplitType.Epilogue: shouldSplit = ChapterSplit(areaID, Area.Epilogue, levelName, completed, elapsed); break;
						case SplitType.Chapter8: shouldSplit = ChapterSplit(areaID, Area.Core, levelName, completed, elapsed); break;
						case SplitType.Chapter1Checkpoint1: shouldSplit = areaID == Area.ForsakenCity && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "6" : "04"); break;
						case SplitType.Chapter1Checkpoint2: shouldSplit = areaID == Area.ForsakenCity && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "9b" : "08"); break;
						case SplitType.Chapter2Checkpoint1: shouldSplit = areaID == Area.OldSite && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "3" : "03"); break;
						case SplitType.Chapter2Checkpoint2: shouldSplit = areaID == Area.OldSite && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "end_3" : "08b"); break;
						case SplitType.Chapter3Checkpoint1: shouldSplit = areaID == Area.CelestialResort && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "08-a" : "06"); break;
						case SplitType.Chapter3Checkpoint2: shouldSplit = areaID == Area.CelestialResort && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "09-d" : "11"); break;
						case SplitType.Chapter3Checkpoint3: shouldSplit = areaID == Area.CelestialResort && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "00-d" : "16"); break;
						case SplitType.Chapter4Checkpoint1: shouldSplit = areaID == Area.GoldenRidge && levelName == "b-00"; break;
						case SplitType.Chapter4Checkpoint2: shouldSplit = areaID == Area.GoldenRidge && levelName == "c-00"; break;
						case SplitType.Chapter4Checkpoint3: shouldSplit = areaID == Area.GoldenRidge && levelName == "d-00"; break;
						case SplitType.Chapter5Checkpoint1: shouldSplit = areaID == Area.MirrorTemple && levelName == "b-00"; break;
						case SplitType.Chapter5Checkpoint2: shouldSplit = areaID == Area.MirrorTemple && levelName == "c-00"; break;
						case SplitType.Chapter5Checkpoint3: shouldSplit = areaID == Area.MirrorTemple && levelName == "d-00"; break;
						case SplitType.Chapter5Checkpoint4: shouldSplit = areaID == Area.MirrorTemple && levelName == "e-00"; break;
						case SplitType.Chapter6Checkpoint1: shouldSplit = areaID == Area.Reflection && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "00" : "b-00"); break;
						case SplitType.Chapter6Checkpoint2: shouldSplit = areaID == Area.Reflection && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "04" : "c-00"); break;
						case SplitType.Chapter6Checkpoint3: shouldSplit = areaID == Area.Reflection && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "b-00" : "d-00"); break;
						case SplitType.Chapter6Checkpoint4: shouldSplit = areaID == Area.Reflection && levelName == "boss-00"; break;
						case SplitType.Chapter6Checkpoint5: shouldSplit = areaID == Area.Reflection && levelName == "after-00"; break;
						case SplitType.Chapter7Checkpoint1: shouldSplit = areaID == Area.TheSummit && levelName == "b-00"; break;
						case SplitType.Chapter7Checkpoint2: shouldSplit = areaID == Area.TheSummit && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "c-00" : "c-01"); break;
						case SplitType.Chapter7Checkpoint3: shouldSplit = areaID == Area.TheSummit && levelName == "d-00"; break;
						case SplitType.Chapter7Checkpoint4: shouldSplit = areaID == Area.TheSummit && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "e-00b" : "e-00"); break;
						case SplitType.Chapter7Checkpoint5: shouldSplit = areaID == Area.TheSummit && levelName == "f-00"; break;
						case SplitType.Chapter7Checkpoint6: shouldSplit = areaID == Area.TheSummit && levelName == "g-00"; break;
						case SplitType.Chapter8Checkpoint1: shouldSplit = areaID == Area.Core && levelName == "a-00"; break;
						case SplitType.Chapter8Checkpoint2: shouldSplit = areaID == Area.Core && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "c-00" : "b-00"); break;
						case SplitType.Chapter8Checkpoint3: shouldSplit = areaID == Area.Core && levelName == (mem.AreaDifficulty() == AreaMode.ASide ? "d-00" : "c-01"); break;
						default:
							int cassettes = mem.Cassettes();
							int heartGems = mem.HeartGems();
							switch (split.Type) {
								case SplitType.Chapter1Cassette: shouldSplit = areaID == Area.ForsakenCity && cassettes == lastCassettes + 1; break;
								case SplitType.Chapter1HeartGem: shouldSplit = areaID == Area.ForsakenCity && heartGems == lastHeartGems + 1; break;
								case SplitType.Chapter2Cassette: shouldSplit = areaID == Area.OldSite && cassettes == lastCassettes + 1; break;
								case SplitType.Chapter2HeartGem: shouldSplit = areaID == Area.OldSite && heartGems == lastHeartGems + 1; break;
								case SplitType.Chapter3Cassette: shouldSplit = areaID == Area.CelestialResort && cassettes == lastCassettes + 1; break;
								case SplitType.Chapter3HeartGem: shouldSplit = areaID == Area.CelestialResort && heartGems == lastHeartGems + 1; break;
								case SplitType.Chapter4Cassette: shouldSplit = areaID == Area.GoldenRidge && cassettes == lastCassettes + 1; break;
								case SplitType.Chapter4HeartGem: shouldSplit = areaID == Area.GoldenRidge && heartGems == lastHeartGems + 1; break;
								case SplitType.Chapter5Cassette: shouldSplit = areaID == Area.MirrorTemple && cassettes == lastCassettes + 1; break;
								case SplitType.Chapter5HeartGem: shouldSplit = areaID == Area.MirrorTemple && heartGems == lastHeartGems + 1; break;
								case SplitType.Chapter6Cassette: shouldSplit = areaID == Area.Reflection && cassettes == lastCassettes + 1; break;
								case SplitType.Chapter6HeartGem: shouldSplit = areaID == Area.Reflection && heartGems == lastHeartGems + 1; break;
								case SplitType.Chapter7Cassette: shouldSplit = areaID == Area.TheSummit && cassettes == lastCassettes + 1; break;
								case SplitType.Chapter7HeartGem: shouldSplit = areaID == Area.TheSummit && heartGems == lastHeartGems + 1; break;
								case SplitType.Chapter8Cassette: shouldSplit = areaID == Area.Core && cassettes == lastCassettes + 1; break;
								case SplitType.Chapter8HeartGem: shouldSplit = areaID == Area.Core && heartGems == lastHeartGems + 1; break;
							}
							lastCassettes = cassettes;
							lastHeartGems = heartGems;
							break;
					}
				} else if (split == null && Model.CurrentState.Run.Count == 1) {
					if (levelName == levelStarted && elapsed - levelTimer < 2.5) {
						levelStarted = lastLevelName;
						levelTimer = elapsed;
					} else {
						shouldSplit = !string.IsNullOrEmpty(levelName) && !string.IsNullOrEmpty(lastLevelName) && levelName != lastLevelName;
					}
				}

				if (shouldSplit && addAmount > 0 && currentSplit < 0) {
					levelTimer = mem.LevelTime();
				}

				lastCompleted = completed;
				lastLevelName = levelName;

				if (elapsed > 0 || lastElapsed == elapsed) {
					Model.CurrentState.SetGameTime(TimeSpan.FromSeconds(settings.Splits.Count == 0 || addAmount > 0 ? elapsed - levelTimer : elapsed));
				}

				Model.CurrentState.IsGameTimePaused = Model.CurrentState.CurrentPhase != TimerPhase.Running || elapsed == lastElapsed;

				lastElapsed = elapsed;
			}

			HandleSplit(shouldSplit, settings.ILSplits && mem.AreaID() == Area.Menu);
		}
		private bool ChapterSplit(Area areaID, Area chapterArea, string level, bool completed, double elapsed) {
			if (!exitingChapter) {
				string levelName = chapterArea == Area.TheSummit ? level : null;
				exitingChapter = areaID == chapterArea && completed && !lastCompleted && (chapterArea != Area.TheSummit || (!string.IsNullOrEmpty(levelName) && !levelName.StartsWith("credits", StringComparison.OrdinalIgnoreCase)));
				return exitingChapter && settings.ILSplits;
			}
			return !completed && lastCompleted;
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (shouldReset) {
				if (currentSplit >= 0) {
					Model.Reset();
				}
			} else if (shouldSplit) {
				if (currentSplit == -1) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
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
						case LogObject.PointerVersion: curr = mem.RAMPointerVersion(); break;
						case LogObject.GameTime: curr = mem.GameTime().ToString("0"); break;
						case LogObject.LevelTime: curr = mem.LevelTime().ToString("0"); break;
						case LogObject.ShowInputUI: curr = mem.ShowInputUI().ToString(); break;
						case LogObject.Started: curr = mem.ChapterStarted().ToString(); break;
						case LogObject.Completed: curr = mem.ChapterCompleted().ToString(); break;
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
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + key.ToString() + ": ".PadRight(16 - key.ToString().Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}

		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			GetValues();
		}
		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			exitingChapter = false;
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------New Game " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3) + "-------------------------");
			SplitInfo split = currentSplit < settings.Splits.Count ? settings.Splits[currentSplit] : null;
			if (split != null) {
				WriteLog("---------" + split.Type.ToString());
			}
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			exitingChapter = false;
			WriteLog("---------Undo-----------------------------------");
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			exitingChapter = false;
			WriteLog("---------Skip-----------------------------------");
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			exitingChapter = false;
			WriteLog("---------Split-----------------------------------");
			if (currentSplit == Model.CurrentState.Run.Count) {
				ISegment segment = Model.CurrentState.Run[currentSplit - 1];
				segment.SplitTime = new Time(segment.SplitTime.RealTime, TimeSpan.FromSeconds(settings.Splits.Count == 0 ? lastElapsed - levelTimer : lastElapsed));
			} else {
				SplitInfo split = currentSplit < settings.Splits.Count ? settings.Splits[currentSplit] : null;
				if (split != null) {
					WriteLog("---------" + split.Type.ToString());
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

		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() {
			if (Model != null) {
				Model.CurrentState.OnReset -= OnReset;
				Model.CurrentState.OnPause -= OnPause;
				Model.CurrentState.OnResume -= OnResume;
				Model.CurrentState.OnStart -= OnStart;
				Model.CurrentState.OnSplit -= OnSplit;
				Model.CurrentState.OnUndoSplit -= OnUndoSplit;
				Model.CurrentState.OnSkipSplit -= OnSkipSplit;
			}
		}
	}
}
#endif