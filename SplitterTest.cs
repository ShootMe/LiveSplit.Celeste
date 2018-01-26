using System;
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
}