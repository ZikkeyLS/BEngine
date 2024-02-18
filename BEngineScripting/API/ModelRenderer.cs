
namespace BEngine
{
	public class ModelRenderer : Script
	{
		public Transform Transform { get; set; }

		public override void OnStart()
		{
			Transform = GetScript<Transform>();
			if (Transform == null)
				Logger.LogMessage("AWFUL START");
			else
				Logger.LogMessage("GOOD START");
		}

		public override void OnUpdate()
		{
			if (Transform == null)
			{
				Transform = GetScript<Transform>();
				return;
			}

			Logger.LogMessage("Message from Model Renderer");
			Logger.LogMessage(Transform.Position.ToString());
			Logger.LogMessage(Transform.Rotation.ToString());
			Logger.LogMessage(Transform.Scale.ToString());
		}

		public override void OnEditorStart()
		{
			Transform = GetScript<Transform>();
			Logger.LogMessage("AWFUL START");
			if (Transform == null)
				Logger.LogMessage("AWFUL START");
			else
				Logger.LogMessage("GOOD START");
		}

		public override void OnEditorUpdate()
		{
			if (Transform == null)
			{
				Transform = GetScript<Transform>();
				return;
			}

			Logger.LogMessage("Message from Model Renderer");
			Logger.LogMessage(Transform.Position.ToString());
			Logger.LogMessage(Transform.Rotation.ToString());
			Logger.LogMessage(Transform.Scale.ToString());

		}
	}
}
