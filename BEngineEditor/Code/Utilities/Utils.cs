using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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

		public static string CreateFile(string path, int current = 1)
		{
			string resultPath = current <= 1 ? path :
				Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + $" ({current})" + Path.GetExtension(path);

			if (File.Exists(resultPath) == false)
			{
				File.Create(resultPath).Close();
			}
			else
			{
				current += 1;
				return CreateFile(path, current);
			}

			return resultPath;
		}

		public static void CreateDirectory(string directory, int current = 1)
		{
			string resultDirectory = current <= 1 ? directory : directory + $" ({current})";

			if (Directory.Exists(resultDirectory) == false)
			{
				Directory.CreateDirectory(resultDirectory);
			}
			else
			{
				current += 1;
				CreateDirectory(directory, current);
			}
		}
	}

	public static class EnumerableExtensions
	{
		public static IEnumerable<TSource> ExceptByProperty<TSource, TProperty>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TProperty> keySelector)
		{
			return first.ExceptBy(second, x => x, GenericComparer<TSource, TProperty>.Comparer(keySelector));
		}

	}

	public sealed class GenericComparer<T, TProperty> : IEqualityComparer<T>
	{
		public static IEqualityComparer<T> Comparer(Func<T, TProperty> selector)
		{
			return new GenericComparer<T, TProperty>(selector);
		}

		private readonly Func<T, TProperty> selector;

		public GenericComparer(Func<T, TProperty> selector)
		{
			this.selector = selector;
		}

		public bool Equals(T? x, T? y)
		{
			if (x == null || y == null) return false;

			return Equals(selector(x), selector(y));
		}

		public int GetHashCode([DisallowNull] T obj)
		{
			object? value = selector(obj);

			if (value == null) return obj.GetHashCode();

			return value.GetHashCode();
		}
	}
}
