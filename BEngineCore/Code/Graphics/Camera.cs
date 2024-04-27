using System.Numerics;

namespace BEngineCore
{
	public struct CameraHandler
	{
		public string GUID;
		public uint Priority;

		public CameraHandler()
		{
			GUID = string.Empty;
			Priority = 0;
		}
	}

	public struct CameraHandlerRequest()
	{
		public string GUID;
		public uint Priority;
		public Vector3 Position;
		public Quaternion Rotation;
	}

	internal class Camera
	{
		// This looks like base for transform sort of things

		public Vector3 editorPosition = Vector3.Zero;
		public Quaternion editorRotation = Quaternion.Identity;

		public Vector3 position = Vector3.Zero;
		public Quaternion rotation = Quaternion.Identity;

		public Vector3 editorForward { get; private set; }
		public Vector3 editorUp { get; private set; }
		public Vector3 editorRight { get; private set; }

		public Vector3 forward { get; private set; }
		public Vector3 up { get; private set; }
		public Vector3 right { get; private set; }

		public float fov = 60.0f;
		public float minRange = 0.01f;
		public float maxRange = 100f;

		private CameraHandler _handler = new();
		public List<CameraHandlerRequest> _requests = new();

		public bool EditorCamera => _handler.GUID == string.Empty;

		public void Recalculate()
		{
			RecalculateCameraHandlers();

			editorForward = Vector3.Transform(Vector3.UnitZ, editorRotation);
			editorUp = Vector3.Transform(Vector3.UnitY, editorRotation);
			editorRight = Vector3.Transform(Vector3.UnitX, editorRotation);

			forward = Vector3.Transform(Vector3.UnitZ, rotation);
			up = Vector3.Transform(Vector3.UnitY, rotation);
			right = Vector3.Transform(Vector3.UnitX, rotation);
		}

		public Matrix4x4 CalculateViewMatrix(bool editor)
		{
			if (editor)
			{
				return Matrix4x4.CreateLookAt(editorPosition, editorPosition + editorForward, editorUp);
			}
			else
			{
				return Matrix4x4.CreateLookAt(position, position + forward, up);
			}
		}

		public Matrix4x4 CalculateProjectionMatrix(float width, float height)
		{
			return Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(fov),
					width / height, minRange, maxRange);
		}

		public void AddRequest(CameraHandlerRequest request)
		{
			_requests.Add(request);
		}

		public void ResetCameraHandler()
		{
			_handler.GUID = string.Empty;
			_handler.Priority = 0;
		}

		private void RecalculateCameraHandlers()
		{
			foreach (CameraHandlerRequest request in _requests)
			{
				if (_handler.GUID != string.Empty)
				{
					if (_handler.GUID == request.GUID)
					{
						UpdateHandlerData(request);
					}
					else if (_handler.Priority < request.Priority)
					{
						UpdateHandlerData(request);
					}

					continue;
				}

				UpdateHandlerData(request);
			}
			_requests.Clear();
		}

		private void UpdateHandlerData(CameraHandlerRequest request)
		{
			_handler.GUID = request.GUID;
			_handler.Priority = request.Priority;

			position = request.Position;
			rotation = request.Rotation;
		}
	}
}
