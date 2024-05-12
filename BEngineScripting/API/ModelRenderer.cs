
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

		[RunInAny]
		public override void OnStart()
		{
			_transform = GetScript<Transform>();
		}

		[RunInAny]
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
	}
}
