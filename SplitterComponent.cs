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
		private static string LOGFILE = "_Celeste.log";
		private static string[] keys = { "CurrentSplit", "Pointers", "GameTime", "LevelTime", "ShowInputUI", "Menu", "Completed", "Deaths", "AreaID", "AreaMode", "LevelName" };
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private SplitterMemory mem;
		private SplitterSettings settings;
		private int currentSplit = -1, lastLogCheck, elapsedCounter;
		private bool hasLog = false, lastShowInputUI, lastCompleted, exitingChapter;
		private double lastElapsed;

		public SplitterComponent(LiveSplitState state) {
			mem = new SplitterMemory();
			settings = new SplitterSettings();
			foreach (string key in keys) {
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

			if (currentSplit == -1) {
				bool showInputUI = mem.ShowInputUI();

				shouldSplit = !showInputUI && lastShowInputUI && mem.MenuType() == Menu.FileSelect;

				lastShowInputUI = showInputUI;
			} else {
				double elapsed = settings.ILSplits ? mem.LevelTime() : mem.GameTime();

				if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
					bool completed = mem.LevelCompleted();
					Area areaID = mem.AreaID();
					SplitInfo split = currentSplit < settings.Splits.Count ? settings.Splits[currentSplit] : null;

					if (split != null && split.Type != SplitType.Manual) {
						switch (split.Type) {
							case SplitType.Prologue: shouldSplit = ChapterSplit(areaID, Area.Prologue, completed, elapsed); break;
							case SplitType.Chapter1: shouldSplit = ChapterSplit(areaID, Area.ForsakenCity, completed, elapsed); break;
							case SplitType.Chapter2: shouldSplit = ChapterSplit(areaID, Area.OldSite, completed, elapsed); break;
							case SplitType.Chapter3: shouldSplit = ChapterSplit(areaID, Area.CelestialResort, completed, elapsed); break;
							case SplitType.Chapter4: shouldSplit = ChapterSplit(areaID, Area.GoldenRidge, completed, elapsed); break;
							case SplitType.Chapter5: shouldSplit = ChapterSplit(areaID, Area.MirrorTemple, completed, elapsed); break;
							case SplitType.Chapter6: shouldSplit = ChapterSplit(areaID, Area.Reflection, completed, elapsed); break;
							case SplitType.Chapter7: shouldSplit = ChapterSplit(areaID, Area.TheSummit, completed, elapsed); break;
							case SplitType.Epilogue: shouldSplit = ChapterSplit(areaID, Area.Epilogue, completed, elapsed); break;
							case SplitType.Chapter8: shouldSplit = ChapterSplit(areaID, Area.Core, completed, elapsed); break;
							case SplitType.Chapter1Checkpoint1: shouldSplit = areaID == Area.ForsakenCity && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "6" : "04"); break;
							case SplitType.Chapter1Checkpoint2: shouldSplit = areaID == Area.ForsakenCity && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "9b" : "08"); break;
							case SplitType.Chapter2Checkpoint1: shouldSplit = areaID == Area.OldSite && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "3" : "03"); break;
							case SplitType.Chapter2Checkpoint2: shouldSplit = areaID == Area.OldSite && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "end_3" : "08b"); break;
							case SplitType.Chapter3Checkpoint1: shouldSplit = areaID == Area.CelestialResort && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "08-a" : "06"); break;
							case SplitType.Chapter3Checkpoint2: shouldSplit = areaID == Area.CelestialResort && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "09-d" : "11"); break;
							case SplitType.Chapter3Checkpoint3: shouldSplit = areaID == Area.CelestialResort && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "00-d" : "16"); break;
							case SplitType.Chapter4Checkpoint1: shouldSplit = areaID == Area.GoldenRidge && mem.LevelName() == "b-00"; break;
							case SplitType.Chapter4Checkpoint2: shouldSplit = areaID == Area.GoldenRidge && mem.LevelName() == "c-00"; break;
							case SplitType.Chapter4Checkpoint3: shouldSplit = areaID == Area.GoldenRidge && mem.LevelName() == "d-00"; break;
							case SplitType.Chapter5Checkpoint1: shouldSplit = areaID == Area.MirrorTemple && mem.LevelName() == "b-00"; break;
							case SplitType.Chapter5Checkpoint2: shouldSplit = areaID == Area.MirrorTemple && mem.LevelName() == "c-00"; break;
							case SplitType.Chapter5Checkpoint3: shouldSplit = areaID == Area.MirrorTemple && mem.LevelName() == "d-00"; break;
							case SplitType.Chapter5Checkpoint4: shouldSplit = areaID == Area.MirrorTemple && mem.LevelName() == "e-00"; break;
							case SplitType.Chapter6Checkpoint1: shouldSplit = areaID == Area.Reflection && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "00" : "b-00"); break;
							case SplitType.Chapter6Checkpoint2: shouldSplit = areaID == Area.Reflection && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "04" : "c-00"); break;
							case SplitType.Chapter6Checkpoint3: shouldSplit = areaID == Area.Reflection && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "b-00" : "d-00"); break;
							case SplitType.Chapter6Checkpoint4: shouldSplit = areaID == Area.Reflection && mem.LevelName() == "boss-00"; break;
							case SplitType.Chapter6Checkpoint5: shouldSplit = areaID == Area.Reflection && mem.LevelName() == "after-00"; break;
							case SplitType.Chapter7Checkpoint1: shouldSplit = areaID == Area.TheSummit && mem.LevelName() == "b-00"; break;
							case SplitType.Chapter7Checkpoint2: shouldSplit = areaID == Area.TheSummit && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "c-00" : "c-01"); break;
							case SplitType.Chapter7Checkpoint3: shouldSplit = areaID == Area.TheSummit && mem.LevelName() == "d-00"; break;
							case SplitType.Chapter7Checkpoint4: shouldSplit = areaID == Area.TheSummit && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "e-00b" : "e-00"); break;
							case SplitType.Chapter7Checkpoint5: shouldSplit = areaID == Area.TheSummit && mem.LevelName() == "f-00"; break;
							case SplitType.Chapter7Checkpoint6: shouldSplit = areaID == Area.TheSummit && mem.LevelName() == "g-00"; break;
							case SplitType.Chapter8Checkpoint1: shouldSplit = areaID == Area.Core && mem.LevelName() == "a-00"; break;
							case SplitType.Chapter8Checkpoint2: shouldSplit = areaID == Area.Core && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "c-00" : "b-00"); break;
							case SplitType.Chapter8Checkpoint3: shouldSplit = areaID == Area.Core && mem.LevelName() == (mem.AreaDifficulty() == AreaMode.ASide ? "d-00" : "c-01"); break;
						}
					}

					lastCompleted = completed;

					if (elapsed > 0 || lastElapsed == elapsed) {
						Model.CurrentState.SetGameTime(TimeSpan.FromSeconds(elapsed));
					}
				}

				Model.CurrentState.IsGameTimePaused = Model.CurrentState.CurrentPhase != TimerPhase.Running || elapsed == lastElapsed;

				lastElapsed = elapsed;
			}

			HandleSplit(shouldSplit, false);
		}
		private bool ChapterSplit(Area areaID, Area chapterArea, bool completed, double elapsed) {
			if (!exitingChapter) {
				exitingChapter = areaID == chapterArea && completed && !lastCompleted;
			} else if (elapsedCounter < 3) {
				if (elapsed == lastElapsed) {
					elapsedCounter++;
				} else {
					elapsedCounter = 0;
				}
			}
			return elapsedCounter >= 3;
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
				foreach (string key in keys) {
					prev = currentValues[key];

					switch (key) {
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "Pointers": curr = mem.RAMPointers(); break;
						case "GameTime": curr = mem.GameTime().ToString("0"); break;
						case "LevelTime": curr = mem.LevelTime().ToString("0"); break;
						case "ShowInputUI": curr = mem.ShowInputUI().ToString(); break;
						case "Completed": curr = mem.LevelCompleted().ToString(); break;
						case "Deaths": curr = mem.Deaths().ToString(); break;
						case "AreaID": curr = mem.AreaID().ToString(); break;
						case "AreaMode": curr = mem.AreaDifficulty().ToString(); break;
						case "LevelName": curr = mem.LevelName(); break;
						case "Menu": curr = mem.MenuType().ToString(); break;
						default: curr = string.Empty; break;
					}

					if (string.IsNullOrEmpty(prev)) { prev = string.Empty; }
					if (string.IsNullOrEmpty(curr)) { curr = string.Empty; }
					if (!prev.Equals(curr)) {
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}

		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			IList<ILayoutComponent> components = lvstate.Layout.LayoutComponents;
			for (int i = components.Count - 1; i >= 0; i--) {
				ILayoutComponent component = components[i];
				if (component.Component is SplitterComponent && invalidator == null && width == 0 && height == 0) {
					components.Remove(component);
				}
			}

			GetValues();
		}
		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			exitingChapter = false;
			elapsedCounter = 0;
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
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			exitingChapter = false;
			elapsedCounter = 0;
			WriteLog("---------Undo-----------------------------------");
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			exitingChapter = false;
			elapsedCounter = 0;
			WriteLog("---------Skip-----------------------------------");
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			exitingChapter = false;
			elapsedCounter = 0;
			WriteLog("---------Split-----------------------------------");
			if (currentSplit == Model.CurrentState.Run.Count) {
				ISegment segment = Model.CurrentState.Run[currentSplit - 1];
				segment.SplitTime = new Time(segment.SplitTime.RealTime, TimeSpan.FromSeconds(lastElapsed));
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
		public void Dispose() { }
	}
}