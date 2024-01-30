using System.Diagnostics;

namespace BEngineEditor
{
	internal static class Utils
	{
		public static void Rename(this FileInfo fileInfo, string newName)
		{
			fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
		}

		public static void Rename(this DirectoryInfo directoryInfo, string newName)
		{
			directoryInfo.MoveTo(directoryInfo.Parent.FullName + "\\" + newName);
		}

		public static void OpenWithDefaultProgram(string path)
		{
			using Process fileopener = new Process();

			fileopener.StartInfo.FileName = "explorer";
			fileopener.StartInfo.Arguments = "\"" + path + "\"";
			fileopener.Start();
		}

		public static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool recursive = true)
		{
			// Get information about the source directory
			var dir = new DirectoryInfo(sourceDirectory);

			// Check if the source directory exists
			if (!dir.Exists)
				throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

			// Cache directories before we start copying
			DirectoryInfo[] dirs = dir.GetDirectories();

			// Create the destination directory
			Directory.CreateDirectory(destinationDirectory);

			// Get the files in the source directory and copy to the destination directory
			foreach (FileInfo file in dir.GetFiles())
			{
				string targetFilePath = Path.Combine(destinationDirectory, file.Name);
				file.CopyTo(targetFilePath, true);
			}

			// If recursive and copying subdirectories, recursively call this method
			if (recursive)
			{
				foreach (DirectoryInfo subDir in dirs)
				{
					string newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
					CopyDirectory(subDir.FullName, newDestinationDir, true);
				}
			}
		}

		public static void MoveDirectory(string sourceDirectory, string destinationDirectory, bool recursive = true)
		{
			// Get information about the source directory
			var dir = new DirectoryInfo(sourceDirectory);

			// Check if the source directory exists
			if (!dir.Exists)
				throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

			// Cache directories before we start copying
			DirectoryInfo[] dirs = dir.GetDirectories();

			// Create the destination directory
			Directory.CreateDirectory(destinationDirectory);

			// Get the files in the source directory and copy to the destination directory
			foreach (FileInfo file in dir.GetFiles())
			{
				string targetFilePath = Path.Combine(destinationDirectory, file.Name);
				file.MoveTo(targetFilePath, true);
			}

			// If recursive and copying subdirectories, recursively call this method
			if (recursive)
			{
				foreach (DirectoryInfo subDir in dirs)
				{
					string newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
					CopyDirectory(subDir.FullName, newDestinationDir, true);
				}
			}

			dir.Delete(true);
		}
	}
}
