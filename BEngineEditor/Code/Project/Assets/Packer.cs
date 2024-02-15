using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	internal class Packer
	{
		public void Pack()
		{
			ArchiveProvider compressor = new ArchiveProvider();
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					CompressorOption option = new CompressorOption()
					{
						Password = пароль_если_зашифровать,
						WithoutCompress = true_если_без_сжатия,
						RemoveSource = true_если_удалять_исходные_файлы,
						Output = sfd.FileName
					};
					//Списки файлов и каталогов для сжатия
					foreach (string line in lbIncludes.Items)
						option.IncludePath.Add(line);
					//Списки файлов и каталогов для исключения
					foreach (string line in lbExclude.Items)
						option.ExcludePath.Add(line);
					compressor.Compress(option);
				}
			}
		}
	}
}
