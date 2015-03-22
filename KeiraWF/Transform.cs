using System;
using InternalSection;

namespace Entities {
	public class Transform {
		public Vector2 Translate;
		public Vector2 Rotate;
		public Vector2 Scale;
		public Vector2 Skew;
		public TransformMode Current { get; internal set; }

		public Transform() {

		}
//		public Transform(Matrix3x3 m) {
//
//		}
		
		public void SetPoistion(float x, float y) {
			Translate.X = x;
			Translate.Y = y;
		}
	}
	public static class TransformExtension
	{
		public static event EventHandler<EventArgs> TranslateEvent;
		public static event EventHandler<EventArgs> RotateEvent;
		public static event EventHandler<EventArgs> ScaleEvent;
		public static event EventHandler<EventArgs> SkewEvent;
		public static bool Translation(this Transform t, float x, float y) {
			if(t.Translate.X != x || t.Translate.Y != y) {
				t.Current = TransformMode.Translate;
				if(TranslateEvent != null)
					TranslateEvent(t, EventArgs.Empty);
				return true;
			}
			return false;
		}
	}
	public enum TransformMode {
		Translate, Rotate, Scale, Skew
	}
}

