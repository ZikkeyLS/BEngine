
namespace BEngine
{
	public struct RenderModel
	{
		public Model Model;
		public Transform Transform;
	}

	public class ModelRenderer : Script
	{
		private Transform _transform;

		public Model Model;

		public override void OnStart()
		{
			_transform = GetScript<Transform>();
		}

		public override void OnUpdate()
		{
			if (_transform == null)
			{
				_transform = GetScript<Transform>();
				return;
			}

			if (Model != null && Model.GUID != string.Empty)
				InternalCalls.AddRenderModel(new RenderModel() { Model = Model, Transform = _transform });
		}

		public override void OnEditorStart()
		{
			_transform = GetScript<Transform>();
		}

		public override void OnEditorUpdate()
		{
			if (_transform == null)
			{
				_transform = GetScript<Transform>();
				return;
			}

			if (Model != null && Model.GUID != string.Empty)
				InternalCalls.AddRenderModel(new RenderModel() { Model = Model, Transform = _transform });
		}
	}
}
