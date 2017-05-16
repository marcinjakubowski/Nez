using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;


namespace Nez
{
	/// <summary>
	/// skewable rectangle sprite for prototyping
	/// </summary>
	public class PrototypeSprite : Sprite
	{
		public override float Width { get { return _width; } }
		public override float Height { get { return _height; } }

		public override RectangleF Bounds
		{
			get
			{
				if( _areBoundsDirty )
				{
					_bounds.CalculateBounds( Entity.Transform.Position, _localOffset, _origin, Entity.Transform.Scale, Entity.Transform.Rotation, _width, _height );
					_areBoundsDirty = false;
				}

				return _bounds;
			}
		}

		public float SkewTopX { get { return _skewTopX; } }
		public float SkewBottomX { get { return _skewBottomX; } }
		public float SkewLeftY { get { return _skewLeftY; } }
		public float SkewRightY { get { return _skewRightY; } }

		float _width, _height;
		[Inspectable]
		float _skewTopX, _skewBottomX, _skewLeftY, _skewRightY;


		public PrototypeSprite( float width, float height ) : base( Graphics.Instance.PixelTexture )
		{
			_width = width;
			_height = height;
			OriginNormalized = new Vector2( 0.5f, 0.5f );
		}


		/// <summary>
		/// sets the width of the sprite
		/// </summary>
		/// <returns>The width.</returns>
		/// <param name="width">Width.</param>
		public PrototypeSprite SetWidth( float width )
		{
			_width = width;
			return this;
		}


		/// <summary>
		/// sets the height of the sprite
		/// </summary>
		/// <returns>The height.</returns>
		/// <param name="height">Height.</param>
		public PrototypeSprite SetHeight( float height )
		{
			_height = height;
			return this;
		}


		/// <summary>
		/// sets the skew values for the sprite
		/// </summary>
		/// <returns>The skew.</returns>
		/// <param name="skewTopX">Skew top x.</param>
		/// <param name="skewBottomX">Skew bottom x.</param>
		/// <param name="skewLeftY">Skew left y.</param>
		/// <param name="skewRightY">Skew right y.</param>
		public PrototypeSprite SetSkew( float skewTopX, float skewBottomX, float skewLeftY, float skewRightY )
		{
			_skewTopX = skewTopX;
			_skewBottomX = skewBottomX;
			_skewLeftY = skewLeftY;
			_skewRightY = skewRightY;
			return this;
		}


		public override void Render( Graphics graphics, Camera camera )
		{
			var pos = ( Entity.Transform.Position - ( Origin * Entity.Transform.LocalScale ) + LocalOffset );
			var size = new Point( (int)( _width * Entity.Transform.LocalScale.X ), (int)( _height * Entity.Transform.LocalScale.Y ) );
			var destRect = new Rectangle( (int)pos.X, (int)pos.Y, size.X, size.Y );
			graphics.Batcher.Draw( Subtexture, destRect, Subtexture.SourceRect, Color, Entity.Transform.Rotation, SpriteEffects.None, LayerDepth, _skewTopX, _skewBottomX, _skewLeftY, _skewRightY );
		}

	}
}

