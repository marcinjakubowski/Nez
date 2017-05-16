using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Nez.UI
{
	public class ProgressBar : Element
	{
		public event Action<float> OnChanged;

		public bool Disabled;

		public override float PreferredWidth
		{
			get
			{
				if( _vertical )
					return Math.Max( _style.Knob == null ? 0 : _style.Knob.MinWidth, _style.Background != null ? _style.Background.MinWidth : 0 );
				else
					return 140;
			}
		}

		public override float PreferredHeight
		{
			get
			{
				if( _vertical )
					return 140;
				else
					return Math.Max( _style.Knob == null ? 0 : _style.Knob.MinHeight, _style.Background != null ? _style.Background.MinHeight : 0 );
			}
		}

		public float[] SnapValues;
		public float SnapThreshold;
		public bool ShiftIgnoresSnap;

		protected float _value;
		protected float _min, _max, _stepSize;
		protected bool _vertical;
		protected float Position;
		ProgressBarStyle _style;


		public ProgressBar( float min, float max, float stepSize, bool vertical, ProgressBarStyle style )
		{
			Assert.IsTrue( min < max, "min must be less than max" );
			Assert.IsTrue( stepSize > 0, "stepSize must be greater than 0" );

			SetStyle( style );
			_min = min;
			_max = max;
			_stepSize = stepSize;
			_vertical = vertical;
			_value = _min;

			SetSize( PreferredWidth, PreferredHeight );
		}

		public ProgressBar( float min, float max, float stepSize, bool vertical,  Skin skin, string styleName = null ) : this( min, max, stepSize, vertical, skin.Get<ProgressBarStyle>( styleName ) )
		{}

		public ProgressBar( Skin skin, string styleName = null ) : this( 0, 1, 0.01f, false, skin )
		{}


		public virtual void SetStyle( ProgressBarStyle style )
		{
			this._style = style;
			InvalidateHierarchy();
		}


		/// <summary>
		/// Returns the progress bar's style. Modifying the returned style may not have an effect until
		/// {@link #setStyle(ProgressBarStyle)} is called.
		/// </summary>
		/// <returns>The style.</returns>
		public ProgressBarStyle GetStyle()
		{
			return _style;
		}


		/// <summary>
		/// Sets the progress bar position, rounded to the nearest step size and clamped to the minimum and maximum values.
		/// </summary>
		/// <param name="value">Value.</param>
		public ProgressBar SetValue( float value )
		{
			if( !ShiftIgnoresSnap || !InputUtils.IsShiftDown() )
			{
				value = Mathf.Clamp( Mathf.Round( value / _stepSize ) * _stepSize, _min, _max );
				value = Snap( value );
			}
			else
			{
				Mathf.Clamp( value, _min, _max );
			}

			if( value == _value )
				return this;

			_value = value;

			// fire changed event
			if( OnChanged != null )
				OnChanged( _value );

			return this;
		}


		public ProgressBar SetStepSize( float stepSize )
		{
			_stepSize = stepSize;
			return this;
		}


		protected virtual IDrawable GetKnobDrawable()
		{
			return ( Disabled && _style.DisabledKnob != null ) ? _style.DisabledKnob : _style.Knob;
		}


		public override void Draw( Graphics graphics, float parentAlpha )
		{
			var knob = GetKnobDrawable();
			var bg = ( Disabled && _style.DisabledBackground != null ) ? _style.DisabledBackground : _style.Background;
			var knobBefore = ( Disabled && _style.DisabledKnobBefore != null ) ? _style.DisabledKnobBefore : _style.KnobBefore;
			var knobAfter = ( Disabled && _style.DisabledKnobAfter != null ) ? _style.DisabledKnobAfter : _style.KnobAfter;

			var x = this.X;
			var y = this.Y;
			var width = this.Width;
			var height = this.Height;
			var knobHeight = knob == null ? 0 : knob.MinHeight;
			var knobWidth = knob == null ? 0 : knob.MinWidth;
			var percent = GetVisualPercent();
			var color = new Color( this.Color, this.Color.A * parentAlpha );

			if( _vertical )
			{
				var positionHeight = height;

				float bgTopHeight = 0;
				if( bg != null )
				{
					bg.Draw( graphics, x + (int)( ( width - bg.MinWidth ) * 0.5f ), y, bg.MinWidth, height, color );
					bgTopHeight = bg.TopHeight;
					positionHeight -= bgTopHeight + bg.BottomHeight;
				}

				float knobHeightHalf = 0;
				if( _min != _max )
				{
					if( knob == null )
					{
						knobHeightHalf = knobBefore == null ? 0 : knobBefore.MinHeight * 0.5f;
						Position = ( positionHeight - knobHeightHalf ) * percent;
						Position = Math.Min( positionHeight - knobHeightHalf, Position );
					}
					else
					{
						var bgBottomHeight = bg != null ? bg.BottomHeight : 0;
						knobHeightHalf = knobHeight * 0.5f;
						Position = ( positionHeight - knobHeight ) * percent;
						Position = Math.Min( positionHeight - knobHeight, Position ) + bgBottomHeight;
					}
					Position = Math.Max( 0, Position );
				}

				if( knobBefore != null )
				{
					float offset = 0;
					if( bg != null )
						offset = bgTopHeight;
					knobBefore.Draw( graphics, x + ( ( width - knobBefore.MinWidth ) * 0.5f ), y + offset, knobBefore.MinWidth,
						(int)( Position + knobHeightHalf ), color );
				}

				if( knobAfter != null )
				{
					knobAfter.Draw( graphics, x + ( ( width - knobAfter.MinWidth ) * 0.5f ), y + Position + knobHeightHalf,
						knobAfter.MinWidth, height - Position - knobHeightHalf, color );
				}

				if( knob != null )
					knob.Draw( graphics, x + (int)( ( width - knobWidth ) * 0.5f ), (int)( y + Position ), knobWidth, knobHeight, color );
			}
			else
			{
				float positionWidth = width;

				float bgLeftWidth = 0;
				if( bg != null )
				{
					bg.Draw( graphics, x, y + (int)( ( height - bg.MinHeight ) * 0.5f ), width, bg.MinHeight, color );
					bgLeftWidth = bg.LeftWidth;
					positionWidth -= bgLeftWidth + bg.RightWidth;
				}

				float knobWidthHalf = 0;
				if( _min != _max )
				{
					if( knob == null )
					{
						knobWidthHalf = knobBefore == null ? 0 : knobBefore.MinWidth * 0.5f;
						Position = ( positionWidth - knobWidthHalf ) * percent;
						Position = Math.Min( positionWidth - knobWidthHalf, Position );
					}
					else
					{
						knobWidthHalf = knobWidth * 0.5f;
						Position = ( positionWidth - knobWidth ) * percent;
						Position = Math.Min( positionWidth - knobWidth, Position ) + bgLeftWidth;
					}
					Position = Math.Max( 0, Position );
				}

				if( knobBefore != null )
				{
					float offset = 0;
					if( bg != null )
						offset = bgLeftWidth;
					knobBefore.Draw( graphics, x + offset, y + (int)( ( height - knobBefore.MinHeight ) * 0.5f ),
						(int)( Position + knobWidthHalf ), knobBefore.MinHeight, color );
				}

				if( knobAfter != null )
				{
					knobAfter.Draw( graphics, x + (int)( Position + knobWidthHalf ), y + (int)( ( height - knobAfter.MinHeight ) * 0.5f ),
						width - (int)( Position + knobWidthHalf ), knobAfter.MinHeight, color );
				}

				if( knob != null )
					knob.Draw( graphics, (int)( x + Position ), (int)( y + ( height - knobHeight ) * 0.5f ), knobWidth, knobHeight, color );
			}
		}


		public float GetVisualPercent()
		{
			return ( _value - _min ) / ( _max - _min );
		}


		/// <summary>
		/// Returns a snapped value
		/// </summary>
		/// <param name="value">Value.</param>
		float Snap( float value )
		{
			if( SnapValues == null )
				return value;
			
			for( var i = 0; i < SnapValues.Length; i++ )
			{
				if( Math.Abs( value - SnapValues[i] ) <= SnapThreshold )
					return SnapValues[i];
			}
			return value;
		}

	}


	/// <summary>
	/// The style for a progress bar
	/// </summary>
	public class ProgressBarStyle
	{
		/// <summary>
		/// The progress bar background, stretched only in one direction. Optional.
		/// </summary>
		public IDrawable Background;

		/// <summary>
		/// Optional
		/// </summary>
		public IDrawable DisabledBackground;

		/// <summary>
		/// Optional, centered on the background.
		/// </summary>
		public IDrawable Knob, DisabledKnob;

		/// <summary>
		/// Optional
		/// </summary>
		public IDrawable KnobBefore, KnobAfter, DisabledKnobBefore, DisabledKnobAfter;


		public ProgressBarStyle()
		{
		}


		public ProgressBarStyle( IDrawable background, IDrawable knob )
		{
			this.Background = background;
			this.Knob = knob;
		}


		public static ProgressBarStyle Create( Color knobBeforeColor, Color knobAfterColor )
		{
			var knobBefore = new PrimitiveDrawable( knobBeforeColor );
			knobBefore.MinWidth = knobBefore.MinHeight = 10;

			var knobAfter = new PrimitiveDrawable( knobAfterColor );
			knobAfter.MinWidth = knobAfter.MinHeight = 10;

			return new ProgressBarStyle {
				KnobBefore = knobBefore,
				KnobAfter = knobAfter
			};
		}


		public static ProgressBarStyle CreateWithKnob( Color backgroundColor, Color knobColor )
		{
			var background = new PrimitiveDrawable( backgroundColor );
			background.MinWidth = background.MinHeight = 10;

			var knob = new PrimitiveDrawable( knobColor );
			knob.MinWidth = knob.MinHeight = 20;

			return new ProgressBarStyle {
				Background = background,
				Knob = knob
			};
		}


		public ProgressBarStyle Clone()
		{
			return new ProgressBarStyle {
				Background = Background,
				DisabledBackground = DisabledBackground,
				Knob = Knob,
				DisabledKnob = DisabledKnob,
				KnobBefore = KnobBefore,
				KnobAfter = KnobAfter,
				DisabledKnobBefore = DisabledKnobBefore,
				DisabledKnobAfter = DisabledKnobAfter
			};
		}
	}
}

