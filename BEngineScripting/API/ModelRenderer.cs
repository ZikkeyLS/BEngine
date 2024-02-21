
namespace BEngine
{
	public struct RenderModel
	{
		public Model Model;
		public Transform Transform;
	}

	public class ModelRenderer : Script
	{
		public Transform Transform { get; set; }

		public Model Model;

		public override void OnStart()
		{
			Transform = GetScript<Transform>();
		}

		public override void OnUpdate()
		{
			if (Transform == null)
			{
				Transform = GetScript<Transform>();
				return;
			}

			if (Model != null && Model.GUID != string.Empty)
				InternalCalls.AddRenderModel(new RenderModel() { Model = Model, Transform = Transform });
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

			if (Model != null && Model.GUID != string.Empty)
				InternalCalls.AddRenderModel(new RenderModel() { Model = Model, Transform = Transform });
		}
	}
}
