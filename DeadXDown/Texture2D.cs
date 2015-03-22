using System;

namespace DeadXDown
{
	public struct Texture2D
	{
		int id, width, height;

		public int ID { get { return id; }}
		public int Width { get { return width; }}
		public int Height { get { return height; }}

		public Texture2D(int id, int width, int height) {
			this.id = id;
			this.width = width;
			this.height = height;
		}
	}
}

