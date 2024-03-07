
namespace BEngine
{
	public class Transform : Script
	{
		public Vector3 Position = new();
		[EditorIgnore] public Quaternion Rotation = new();

		[EditorName("Rotation")][EditorShowAt(1)] 
		public Vector3 EulerRotation 
		{
			get 
			{
				Vector3 vector3 = Quaternion.ToEuler(Rotation);
				return vector3; 
			}
			set 
			{ 
				Rotation = Quaternion.FromEuler(value.x, value.y, value.z);
			}
		}

		public Vector3 Scale = Vector3.one;
	}
}
