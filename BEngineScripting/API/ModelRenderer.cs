
namespace BEngine
{
	public class ModelRenderer : Script
	{
		public Transform Transform { get; set; }

		public override void OnStart()
		{
			Transform = GetScript<Transform>();
		}

		public override void OnUpdate()
		{
			if (Transform == null)
				return;

			Logger.LogMessage("BUILD COOOL");
		}

		public override void OnEditorStart()
		{
			Transform = GetScript<Transform>();
		}

		public override void OnEditorUpdate()
		{
			if (Transform == null)
			{
				Transform = GetScript<Transform>();
				return;
			}

			Logger.LogMessage("COOOL");
		}
	}
}
