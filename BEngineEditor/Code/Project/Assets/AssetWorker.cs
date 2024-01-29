namespace BEngineEditor
{
	public class AssetWorker
	{
		private Project _project;
	
		public AssetWorker(Project project)
		{
			_project = project;

			if (Directory.Exists(project.AssetsDirectory) == false)
				Directory.CreateDirectory(project.AssetsDirectory);
		}
	}
}