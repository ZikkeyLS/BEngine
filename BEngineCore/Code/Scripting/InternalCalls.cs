
using BEngine;



namespace BEngineCore
{


	public class InternalCalls
	{
		#region Logger

		public static void LogMessage(string message)
		{
			if (Logger.Main != null)
				Logger.Main.LogMessage(message);
		}

		public static void LogWarning(string warning)
		{
			if (Logger.Main != null)
				Logger.Main.LogWarning(warning);
		}

		public static void LogError(string error)
		{
			if (Logger.Main != null)
				Logger.Main.LogError(error);
		}
		#endregion

		#region Graphics
		public static void AddRenderModel(RenderModel renderModel)
		{
			ProjectAbstraction? loadedProject = ProjectAbstraction.LoadedProject;

			if (loadedProject != null)
			{
				Model? model = loadedProject.AssetsReader.ModelContext.GetModel(renderModel.Model.GUID);
				if (model != null)
					loadedProject.Graphics.ModelsToRender.Add(new ModelRenderContext { Model = model, Transform = renderModel.Transform });
			}
		}
		#endregion
	}
}
