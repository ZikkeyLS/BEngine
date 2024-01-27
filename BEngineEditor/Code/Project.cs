using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	internal class Project
	{
		public string Name { get; private set; }
		public string Path { get; private set; }

		public string SolutionPath => $"{Path}/{Name}.sln";
		
		public Project(string name, string path)
		{
			Name = name;
			Path = path;
		}
	}
}
