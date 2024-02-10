using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BEngineCore
{
	internal class Camera
	{
		public Vector3 position = Vector3.Zero;

		public Vector3 forward { get; private set; } = Vector3.UnitZ * -1f;
		public Vector3 up { get; } = Vector3.UnitY;

		public float x = -90.0f;
		public float y = 0.0f;
		public float z = -90.0f;

		public float fov = 45.0f;

		public void Recalculate()
		{
			if (y > 89.0f)
				y = 89.0f;
			if (y < -89.0f)
				y = -89.0f;

			Vector3 front = Vector3.Zero;
			front.X = (float)(Math.Cos(float.DegreesToRadians(x)) * Math.Cos(float.DegreesToRadians(y)));
			front.Y = (float)Math.Sin(float.DegreesToRadians(y));
			front.Z = (float)(Math.Sin(float.DegreesToRadians(x)) * Math.Cos(float.DegreesToRadians(y)));
			forward = front;
		}

		public Matrix4x4 CalculateViewMatrix()
		{
			return Matrix4x4.CreateLookAt(position, position + forward, up);
		}

		public Matrix4x4 CalculateProjectionMatrix(float width, float height)
		{
			return Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(fov),
					width / height, 0.1f, 100.0f);
		}
	}
}
