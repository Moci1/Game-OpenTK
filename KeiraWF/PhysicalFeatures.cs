using System;
using System.Collections.Generic;
using Geometry.Shapes;

namespace InternalSection {
	public enum CollisionState {
		Collosed, // nem tudunk mozogni, de ugrani lehet h lehet
		NearlyCollision, // ugrani lehet, mozogni lehet
		NotCollision // se ugrani, se mozogni (csak esni)
	}
	public struct PhysicalFeatures
	{
		public List<IShape> Collosed;
		public CollisionState CollisionMode;
		public float Weight;
		public Vector2 Direction { get; internal set; }
		public int Material; // ez vmi számkod lesz mint anyagi jellemző
		public bool IsDownLine;
		public PhysicMode Mode;
		public float Speed { get; internal set; } // Velocity megadja a sebességet, de ha csak írányt adunk meg vele akkor ezzel állíthatjuk
		// pl ha normalizáljuk a Velocity-t

		public PhysicalFeatures(Vector2 velocity) : this() {
			this.IsDownLine = false;
			this.Collosed = new List<IShape>();
			this.CollisionMode = CollisionState.NotCollision;
			this.Weight = 1f;
			this.Direction = velocity;
			this.Material = 0;
			this.Mode = PhysicMode.On;
			this.Speed = 0f;
//			this.SurfaceDirection = Vector2.Zero;
		}
		public PhysicalFeatures(float weight, Vector2 velocity, int material) : this() {
			this.IsDownLine = false;
			this.Collosed = new List<IShape>();
			this.CollisionMode = CollisionState.NotCollision;
			this.Weight = weight;
			this.Direction = velocity;
			this.Material = material;
			this.Mode = PhysicMode.On;
			this.Speed = 0f;
//			this.SurfaceDirection = Vector2.Zero;
		}
		public PhysicalFeatures(Vector2 velocity, PhysicMode mode) : this() {
			this.IsDownLine = false;
			this.Collosed = new List<IShape>();
			this.CollisionMode = CollisionState.NotCollision;
			this.Weight = 1f;
			this.Direction = velocity;
			this.Material = 0;
			this.Mode = mode;
			this.Speed = 0f;
//			this.SurfaceDirection = Vector2.Zero;
		}
		public PhysicalFeatures(Vector2 velocity, Vector2 surfaceDirection) : this() {
			this.IsDownLine = false;
			this.Collosed = new List<IShape>();
			this.CollisionMode = CollisionState.NotCollision;
			this.Weight = 1f;
			this.Direction = velocity;
			this.Material = 0;
			this.Mode = PhysicMode.On;
			this.Speed = 0f;
//			this.SurfaceDirection = surfaceDirection;
		}
	}
	public enum PhysicMode {
		On, Off
	}
}

