
namespace BEngine
{
	public struct RenderModel
	{
		public Model Model;
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale;
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
			{
				RenderModel renderModel = new RenderModel() { Model = Model, Position = _transform.Position, Rotation = _transform.Rotation, Scale = _transform.Scale };
				GetInheritedTransform(Entity.Parent, ref renderModel);
				InternalCalls.AddRenderModel(renderModel);
			}
		}

		private void GetInheritedTransform(Entity entity, ref RenderModel model)
		{
			if (entity == null)
				return;

			Transform entityTransform = entity.GetScript<Transform>();

			if (entityTransform != null)
			{
				model.Position += entityTransform.Position;
				model.Rotation += entityTransform.Rotation;
				model.Scale *= entityTransform.Scale;
			}

			if (entity.Parent != null)
			{
				GetInheritedTransform(entity.Parent, ref model);
			}
		}
	}
}
