using ICSharpCode.SharpZipLib.Zip;

namespace BEngineCore
{
	public class Packer
	{
		public void Pack(string[] directories, string[] files, string outputFile)
		{
			using (ZipOutputStream compression = new ZipOutputStream(File.Create(outputFile)))
			{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				directories.Select(dir => new DirectoryInfo(dir))
				            .Where(dirInfo => Directory.GetParent(dirInfo.FullName) != null)
				            .ToList()
				            .ForEach(dirInfo => ZipFolder(Directory.GetParent(dirInfo.FullName).FullName, dirInfo.FullName, compression));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

				for (int i = 0; i < files.Length; i++)
				{
					AddFileToZip(compression, "", files[i]);
				}
			}
		}

		public void ReadAllFiles(string archivePath, Action<string> onFileFound)
		{
			using (var fs = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
			{
				using (var zf = new ZipFile(fs))
				{
					foreach (ZipEntry ze in zf)
					{
						if (ze.IsDirectory)
							continue;

						onFileFound.Invoke(ze.Name);
					}
				}
			}
		}

		public Stream? ReadFile(string archivePath, string fileRelativePath)
		{
			archivePath = archivePath.Replace("\\", "/");
			fileRelativePath = fileRelativePath.Replace("\\", "/");

			var fs = new FileStream(archivePath, FileMode.Open, FileAccess.Read);
			var zf = new ZipFile(fs);

			var ze = zf.GetEntry(fileRelativePath);
			if (ze == null)
			{
				return null;
			}

			var s = zf.GetInputStream(ze);
			return s;
		}

		private void ZipFolder(string RootFolder, string CurrentFolder, ZipOutputStream zStream)
		{
			string[] SubFolders = Directory.GetDirectories(CurrentFolder);

			foreach (string Folder in SubFolders)
				ZipFolder(RootFolder, Folder, zStream);

			string relativePath = CurrentFolder.Substring(RootFolder.Length) + "/";

			if (relativePath.Length > 1)
			{
				ZipEntry dirEntry;

				dirEntry = new ZipEntry(relativePath);
				dirEntry.DateTime = DateTime.Now;
			}

			foreach (string file in Directory.GetFiles(CurrentFolder))
			{
				if (file.EndsWith(".cs") == false && file.EndsWith(".cs.meta") == false)
					AddFileToZip(zStream, relativePath, file);
			}
		}

		private static void AddFileToZip(ZipOutputStream zStream, string relativePath, string file)
		{
			byte[] buffer = new byte[4096];
			string fileRelativePath = (relativePath.Length > 1 ? relativePath : string.Empty) + Path.GetFileName(file);
			ZipEntry entry = new ZipEntry(fileRelativePath);

			entry.DateTime = DateTime.Now;
			zStream.PutNextEntry(entry);

			using (FileStream fs = File.OpenRead(file))
			{
				int sourceBytes;

				do
				{
					sourceBytes = fs.Read(buffer, 0, buffer.Length);
					zStream.Write(buffer, 0, sourceBytes);
				} while (sourceBytes > 0);
			}
		}
	}
}
