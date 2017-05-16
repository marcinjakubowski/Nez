﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Nez.Shadows
{
	/// <summary>    
	/// The end-point of a segment    
	/// </summary>
	internal class EndPoint
	{
		/// <summary>
		/// Position of the segment
		/// </summary>
		internal Vector2 Position;

		/// <summary>
		/// If this end-point is a begin or end end-point
		/// of a segment (each segment has only one begin and one end end-point
		/// </summary>
		internal bool Begin;

		/// <summary>
		/// The segment this end-point belongs to
		/// </summary>
		internal Segment Segment;

		/// <summary>
		/// The angle of the end-point relative to the location of the visibility test
		/// </summary>
		internal float Angle;


		internal EndPoint()
		{
			Position = Vector2.Zero;
			Begin = false;
			Segment = null;
			Angle = 0;
		}


		public override bool Equals( object obj )
		{
			if( obj is EndPoint )
			{
				var other = obj as EndPoint;
				return Position.Equals( other.Position ) && Begin.Equals( other.Begin ) && Angle.Equals( other.Angle );
				// We do not care about the segment being the same since that would create a circular reference
			}

			return false;
		}


		public override int GetHashCode()
		{
			return Position.GetHashCode() + Begin.GetHashCode() + Angle.GetHashCode();
		}


		public override string ToString()
		{
			return "{ p:" + Position.ToString() + "a: " + Angle + " in " + Segment.ToString() + "}";
		}
	}


	internal class EndPointComparer : IComparer<EndPoint>
	{
		internal EndPointComparer()
		{ }


		// Helper: comparison function for sorting points by angle
		public int Compare( EndPoint a, EndPoint b )
		{
			// Traverse in angle order
			if( a.Angle > b.Angle )
				return 1;

			if( a.Angle < b.Angle )
				return -1;

			// But for ties we want Begin nodes before End nodes
			if( !a.Begin && b.Begin )
				return 1;

			if( a.Begin && !b.Begin )
				return -1;

			return 0;
		}
	}

}
