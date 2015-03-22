using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using System.Drawing;

namespace DeadXDown
{
	public static class Input
	{
		static List<Key> keysDown;
		static List<Key> keysDownLast;
		static List<MouseButton> buttonsDown;
		static List<MouseButton> buttonsDownLast;
		public static Point LastMousePosition { get; private set; }

		public static void Initialize(GameWindow gw) {
			keysDown = new List<Key>();
			keysDownLast = new List<Key>();
			buttonsDown = new List<MouseButton>();
			buttonsDownLast = new List<MouseButton>();

			gw.KeyDown += HandleKeyDown;
			gw.KeyUp += HandleKeyUp;
			gw.MouseDown += HandleMouseDown;
			gw.MouseUp += HandleMouseUp;
			gw.MouseMove += HandleMouseMove;
		}

		static void HandleMouseMove (object sender, MouseMoveEventArgs e)
		{
			LastMousePosition = e.Position;
		}

		static void HandleMouseUp (object sender, MouseButtonEventArgs e)
		{
			while (buttonsDown.Contains(e.Button)) {
				buttonsDown.Remove(e.Button);
			}
		}

		static void HandleMouseDown (object sender, MouseButtonEventArgs e)
		{
			buttonsDown.Add(e.Button);
		}

		static void HandleKeyUp (object sender, KeyboardKeyEventArgs e)
		{
			while (keysDown.Contains(e.Key)) {
				keysDown.Remove(e.Key);
			}
		}

		static void HandleKeyDown (object sender, KeyboardKeyEventArgs e)
		{
			keysDown.Add(e.Key);
		}

		public static void Update() {
			keysDownLast = new List<Key>(keysDown);
			buttonsDownLast = new List<MouseButton>(buttonsDown);
		} 
		public static bool KeyPress(Key key) {
			return (keysDown.Contains(key) && !keysDownLast.Contains(key));
		}
		public static bool KeyRelease(Key key) {
			return (!keysDown.Contains(key) && keysDownLast.Contains(key));
		}
		public static bool KeyDown(Key key) {
			return keysDown.Contains(key);
		}

		public static bool MousePress(MouseButton button) {
			return (buttonsDown.Contains(button) && !buttonsDownLast.Contains(button));
		}
		public static bool MouseRelease(MouseButton button) {
			return (!buttonsDown.Contains(button) && buttonsDownLast.Contains(button));
		}
		public static bool MouseDown(MouseButton button) {
			return buttonsDown.Contains(button);
		}
	}
}

