using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Nez.Tiled
{
	public class TiledObject
	{
		public enum TiledObjectShapeType
		{
			None,
			Ellipse,
			Image,
			Polygon,
			Polyline
		}

		public int Gid;
		public string Name;
		public string Type;
		public int X;
		public int Y;
		public int Width;
		public int Height;
		public int Rotation;
		public bool Visible;
		public TiledObjectShapeType TiledObjectType;
		public string ObjectType;
		public Vector2[] PolyPoints;
		public Dictionary<string,string> Properties = new Dictionary<string,string>();
		
		/// <summary>
		/// wraps the x/y fields in a Vector
		/// </summary>
	        public Vector2 Position
	        {
	            get { return new Vector2( X, Y ); }
	            set { X = (int)value.X; Y = (int)value.Y; }
	        }
	}
}

