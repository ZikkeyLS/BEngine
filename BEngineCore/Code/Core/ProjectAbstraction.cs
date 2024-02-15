
namespace BEngineCore
{
	public class ProjectAbstraction
	{
		protected Scripting scripting = new();
		protected AssetReader reader;
		protected Logger logger = new(true);

		public Scripting Scripting => scripting;
		public AssetReader AssetsReader => reader;
		public Logger Logger => logger;

		public void LoadProject()
		{
			LoadProjectData();
		}

		public virtual void LoadProjectData()
		{

		}
	}
}
