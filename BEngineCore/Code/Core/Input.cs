using BEngine;
using Silk.NET.Input;
using Key = BEngine.Key;
using MouseButton = BEngine.MouseButton;

namespace BEngineCore
{
	public class KeyStatus
	{
		public bool Down = false;
		public bool Up = false;

		public KeyStatus()
		{

		}
	}

	public class MouseButtonStatus
	{
		public bool Down = false;
		public bool Up = false;

		public MouseButtonStatus()
		{

		}
	}

	public class Input
	{
		private IInputContext _input;

		private Dictionary<Key, KeyStatus> _keys = new();
		private Dictionary<MouseButton, MouseButtonStatus> _buttons = new();

		private Vector2 _mousePosition = Vector2.zero;

		public Input(IInputContext input)
		{
			_input = input;

			if (IsKeyboardConnected())
			{
				foreach (var key in _input.Keyboards[0].SupportedKeys)
				{
					_keys.Add((Key)key, new KeyStatus());
				}

				_input.Keyboards[0].KeyDown += KeyDown;
				_input.Keyboards[0].KeyUp += KeyUp;
			}

			if (IsMouseConnected())
			{
				foreach (var button in _input.Mice[0].SupportedButtons)
				{
					_buttons.Add((MouseButton)button, new MouseButtonStatus());
				}

				_input.Mice[0].MouseDown += MouseDown;
				_input.Mice[0].MouseUp += MouseUp;
				_input.Mice[0].MouseMove += MouseMove;
			}
		}

		private void MouseMove(IMouse arg1, System.Numerics.Vector2 arg2)
		{
			_mousePosition = arg2;
		}

		private void KeyDown(IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3)
		{
			_keys[(Key)arg2].Down = true;
		}

		private void KeyUp(IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3)
		{
			_keys[(Key)arg2].Up = true;
		}

		private void MouseDown(IMouse arg1, Silk.NET.Input.MouseButton arg2)
		{
			_buttons[(MouseButton)arg2].Down = true;
		}

		private void MouseUp(IMouse arg1, Silk.NET.Input.MouseButton arg2)
		{
			_buttons[(MouseButton)arg2].Up = true;
		}

		public bool IsMouseConnected() => _input.Mice.Count != 0;
		public bool IsKeyboardConnected() => _input.Keyboards.Count != 0;
		public Vector2 GetMousePosition() => _mousePosition;

		public bool IsButtonPressed(MouseButton button)
		{
			if (IsMouseConnected() == false)
				return false;
			return _input.Mice[0].IsButtonPressed((Silk.NET.Input.MouseButton)button);
		}

		public bool IsButtonDown(MouseButton button)
		{
			return _buttons[button].Down;
		}

		public bool IsButtonUp(MouseButton button)
		{
			return _buttons[button].Up;
		}


		public bool IsKeyPressed(Key key)
		{
			if (IsKeyboardConnected() == false)
				return false;
			return _input.Keyboards[0].IsKeyPressed((Silk.NET.Input.Key)key);
		}

		public bool IsKeyDown(Key key)
		{
			return _keys[key].Down;
		}

		public bool IsKeyUp(Key key)
		{
			return _keys[key].Up;
		}

		public void Clean()
		{
			foreach (var status in _keys.Values)
			{
				status.Up = false;
				status.Down = false;
			}

			foreach (var status in _buttons.Values)
			{
				status.Up = false;
				status.Down = false;
			}
		}
	}
}