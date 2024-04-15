using BEngine;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Text.Json;

namespace BEngineCore
{
	public class Packer
	{
		public void Pack(string directory, string outputFile)
		{
			using (ZipOutputStream compression = new ZipOutputStream(File.Create(outputFile)))
			{
				ZipFolder(directory, directory, compression);
			}
		}

		public void ReadFile(string archivePath, string fileRelativePath)
		{
			using (var fs = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
			using (var zf = new ZipFile(fs))
			{
			
				var ze = zf.GetEntry(fileRelativePath);
				if (ze == null)
				{
					throw new ArgumentException(fileRelativePath, "not found in Zip");
				}

				using (var s = zf.GetInputStream(ze))
				{
					JsonUtils.Deserialize<object>(s);
				}
			}
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
