using System.Diagnostics;

namespace BEnginePostCoreBuild
{
	internal class Program
	{
		private const string GlobalDirectory = "../../../../";
		private const string OutputDirectory = "/bin/Release/net8.0/";
		private const string EditorName = "BEngineEditor";
		private const string CoreName = "BEngineCore";

		static void Main(string[] args)
		{
			foreach(string file in Directory.GetFiles(GlobalDirectory + CoreName + OutputDirectory + "Binary"))
			{
				FileInfo info = new FileInfo(file);
				File.Copy(file, GlobalDirectory + EditorName + OutputDirectory + info.Name, true);
			}
		}
	}
}
