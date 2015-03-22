using System;
using System.Drawing;


namespace Animation {
	public struct AnimBorder : IAnimBorder
	{
		#region IAnimObject implementation
		public int FrameWidth
        {
            get;
            private set;
        }

        public int FrameHeight
        {
            get;
            private set;
        }
        public Rectangle this[ushort index]
        {
            get 
            {
                return new Rectangle((index / FrameRowCount) * FrameHeight, (index % FrameRowCount) * FrameWidth, FrameWidth, FrameHeight);
            }
        }
        public Rectangle this[sbyte row, sbyte column]
        {
            get { return new Rectangle(FrameWidth * row, FrameHeight * column, FrameWidth, FrameHeight); }
        }
        public int FrameRowCount
        {
            get;
            private set;
        }
        public Rectangle FromPosition(sbyte x, sbyte y)
        {
            return new Rectangle(x * FrameWidth, y * FrameHeight, FrameWidth, FrameHeight);
        }
        public ushort FromRectangle(Rectangle? r)
        {
            if (r.HasValue)
                return (ushort)(r.Value.X / FrameHeight * FrameRowCount);
            else
                return 0;
        }
		#endregion
	}
}

