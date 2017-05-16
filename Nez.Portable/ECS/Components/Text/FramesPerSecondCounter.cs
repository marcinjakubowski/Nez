using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Nez.BitmapFonts;


namespace Nez
{
	public class FramesPerSecondCounter : Text, IUpdatable
	{
		public enum FpsDockPosition
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}

		public long TotalFrames;
		public float AverageFramesPerSecond;
		public float CurrentFramesPerSecond;

		/// <summary>
		/// total number of samples that should be stored and averaged for calculating the FPS
		/// </summary>
		public int MaximumSamples;


		/// <summary>
		/// position the FPS counter should be docked
		/// </summary>
		/// <value>The dock position.</value>
		public FpsDockPosition DockPosition
		{
			get { return _dockPosition; }
			set
			{
				_dockPosition = value;
				UpdateTextPosition();
			}
		}

		/// <summary>
		/// offset from dockPosition the FPS counter should be drawn
		/// </summary>
		/// <value>The dock offset.</value>
		public Vector2 DockOffset
		{
			get { return _dockOffset; }
			set
			{
				_dockOffset = value;
				UpdateTextPosition();
			}
		}

		FpsDockPosition _dockPosition;
		Vector2 _dockOffset;
		readonly Queue<float> _sampleBuffer = new Queue<float>();


		public FramesPerSecondCounter( BitmapFont font, Color color, FpsDockPosition dockPosition = FpsDockPosition.TopRight, int maximumSamples = 100 ) : base( font, string.Empty, Vector2.Zero, color )
		{
			this.MaximumSamples = maximumSamples;
			this.DockPosition = dockPosition;
			Init();
		}


		public FramesPerSecondCounter( NezSpriteFont font, Color color, FpsDockPosition dockPosition = FpsDockPosition.TopRight, int maximumSamples = 100 ) : base( font, string.Empty, Vector2.Zero, color )
		{
			this.MaximumSamples = maximumSamples;
			this.DockPosition = dockPosition;
			Init();
		}


		void Init()
		{
			UpdateTextPosition();
		}


		void UpdateTextPosition()
		{
			switch( DockPosition )
			{
				case FpsDockPosition.TopLeft:
					_horizontalAlign = HorizontalAlign.Left;
					_verticalAlign = VerticalAlign.Top;
					LocalOffset = DockOffset;
					break;
				case FpsDockPosition.TopRight:
					_horizontalAlign = HorizontalAlign.Right;
					_verticalAlign = VerticalAlign.Top;
					LocalOffset = new Vector2( Core.CoreGraphicsDevice.Viewport.Width - DockOffset.X, DockOffset.Y );
					break;
				case FpsDockPosition.BottomLeft:
					_horizontalAlign = HorizontalAlign.Left;
					_verticalAlign = VerticalAlign.Bottom;
					LocalOffset = new Vector2( DockOffset.X, Core.CoreGraphicsDevice.Viewport.Height - DockOffset.Y );
					break;
				case FpsDockPosition.BottomRight:
					_horizontalAlign = HorizontalAlign.Right;
					_verticalAlign = VerticalAlign.Bottom;
					LocalOffset = new Vector2( Core.CoreGraphicsDevice.Viewport.Width - DockOffset.X, Core.CoreGraphicsDevice.Viewport.Height - DockOffset.Y );
					break;
			}
		}


		public void Reset()
		{
			TotalFrames = 0;
			_sampleBuffer.Clear();
		}


		void IUpdatable.Update()
		{
			CurrentFramesPerSecond = 1.0f / Time.UnscaledDeltaTime;
			_sampleBuffer.Enqueue( CurrentFramesPerSecond );

			if( _sampleBuffer.Count > MaximumSamples )
			{
				_sampleBuffer.Dequeue();
				AverageFramesPerSecond = _sampleBuffer.Average( i => i );
			}
			else
			{
				AverageFramesPerSecond = CurrentFramesPerSecond;
			}

			TotalFrames++;

			text = string.Format( "FPS: {0:0.00}", AverageFramesPerSecond );
		}


		public override void Render( Graphics graphics, Camera camera )
		{
			// we override render and use position instead of entityPosition. this keeps the text in place even if the entity moves
			graphics.Batcher.DrawString( _font, _text, LocalOffset, Color, Entity.Transform.Rotation, Origin, Entity.Transform.Scale, SpriteEffects, LayerDepth );
		}


		public override void DebugRender( Graphics graphics )
		{
			// due to the override of position in render we have to do the same here
			var rect = Bounds;
			rect.Location = LocalOffset;
			graphics.Batcher.DrawHollowRect( rect, Color.Yellow );
		}


		#region Fluent setters

		/// <summary>
		/// Sets how far the fps text will appear from the edges of the screen.
		/// </summary>
		/// <param name="dockOffset">Offset from screen edges</param>
		public FramesPerSecondCounter SetDockOffset( Vector2 dockOffset )
		{
			this.DockOffset = dockOffset;
			return this;
		}


		/// <summary>
		/// Sets which corner of the screen the fps text will show.
		/// </summary>
		/// <param name="dockPosition">Corner of the screen</param>
		public FramesPerSecondCounter SetDockPosition( FpsDockPosition dockPosition )
		{
			this.DockPosition = dockPosition;
			return this;
		}

		#endregion

	}
}
