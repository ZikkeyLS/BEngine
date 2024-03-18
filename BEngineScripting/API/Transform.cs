
namespace BEngine
{
	public class Transform : Script
	{
		public Vector3 Position = new();
		[EditorIgnore] public Quaternion Rotation = Quaternion.identity;

		[EditorName("Rotation")] [EditorShowAt(1)]
		public Vector3 EulerRotation 
		{
			get 
			{
				return Quaternion.ToEuler(Rotation);
			}
			set 
			{
				Rotation = Quaternion.FromEuler(value.x, value.y, value.z);
			}
		}

		public Vector3 Scale = Vector3.one;
	}
}
