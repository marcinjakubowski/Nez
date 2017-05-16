using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.BitmapFonts;


namespace Nez
{
	/// <summary>
	/// IMGUI is a very simple class with only static methods designed to make sticking buttons, checkboxes, sliders and progress bars on screen
	/// in quick and dirty fashion. It is not designed to be a full and proper UI system.
	/// </summary>
	public class Imgui
	{
		enum TextAlign
		{
			Left,
			Center,
			Right
		}
			
		static SpriteBatch _spriteBatch;
		static BitmapFont _font;

		// constants
		const float FontLineHeight = 10;
		const float ElementHeight = 20;
		const float ShortElementHeight = 15;
		const float ElementPadding = 10;
		static Vector2 _fontScale;

		// colors
		static Color _fontColor = new Color( 255, 255, 255 );
		static Color _windowColor = new Color( 17, 17, 17 );
		static Color _buttonColor = new Color( 78, 91, 98 );
		static Color _buttonColorActive = new Color( 168, 207, 115 );
		static Color _buttonColorDown = new Color( 244, 23, 135 );
		static Color _toggleBg = new Color( 63, 63, 63 );
		static Color _toggleBgActive = new Color( 130, 130, 130 );
		static Color _toggleOn = new Color( 168, 207, 115 );
		static Color _toggleOnActive = new Color( 244, 23, 135 );
		static Color _sliderBg = new Color( 78, 91, 98 );
		static Color _sliderThumbBg = new Color( 25, 144, 188 );
		static Color _sliderThumbBgActive = new Color( 168, 207, 115 );
		static Color _sliderThumbBgDown = new Color( 244, 23, 135 );
		static Color _headerBg = new Color( 40, 46, 50 );

		// state
		static float _lastY;
		static float _elementX;
		static float _windowWidth;
		#pragma warning disable 0414
		static float _windowHeight;
		static float _elementWidth;
		static Point _mouseInWorldCoords;


		static Imgui()
		{
			_spriteBatch = new SpriteBatch( Core.CoreGraphicsDevice );
			_font = Graphics.Instance.BitmapFont;

			var scale = FontLineHeight / _font.LineHeight;
			_fontScale = new Vector2( scale, scale );
		}


		#region Helpers

		static void DrawString( string text, Color color, TextAlign align = TextAlign.Center, float elementHeight = ElementHeight )
		{
			// center align the text
			var textSize = _font.MeasureString( text ) * _fontScale.Y;
			float x = _elementX;
			switch( align )
			{
				case TextAlign.Center:
					x += ( _elementWidth - textSize.X ) * 0.5f;
					break;
				case TextAlign.Right:
					x = _elementX + _elementWidth - textSize.X;
					break;
			}

			var y = _lastY + ElementPadding + ( elementHeight - FontLineHeight ) * 0.5f;

			_spriteBatch.DrawString( _font, text, new Vector2( x, y ), color, 0, Vector2.Zero, _fontScale, SpriteEffects.None, 0 );
		}


		static bool IsMouseOverElement()
		{
			var rect = new Rectangle( (int)_elementX, (int)_lastY + (int)ElementPadding, (int)_elementWidth, (int)ElementHeight );
			return rect.Contains( _mouseInWorldCoords );
		}


		static bool IsMouseBetween( float left, float right )
		{
			var rect = new Rectangle( (int)left, (int)_lastY + (int)ElementPadding, (int)right - (int)left, (int)ElementHeight );
			return rect.Contains( _mouseInWorldCoords );
		}


		static void EndElement( float elementHeight = ElementHeight )
		{
			_lastY += elementHeight + ElementPadding;
		}

		#endregion


		/// <summary>
		/// begins an IMGUI window specifying where and how large it should be. If you are not using IMGUI in world space (for example, inside
		/// a Scene with a scaled resolution policy) passing false for useRawMousePosition will use the Input.scaledMousePosition.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="useRawMousePosition">If set to <c>true</c> use raw mouse position.</param>
		public static void BeginWindow( float x, float y, float width, float height, bool useRawMousePosition = true )
		{
			_spriteBatch.Begin();

			_spriteBatch.DrawRect( x, y, width, height, _windowColor );

			_elementX = x + ElementPadding;
			_lastY = y;
			_windowWidth = width;
			_windowHeight = height;
			_elementWidth = _windowWidth - 2f * ElementPadding;

			var mousePos = useRawMousePosition ? Input.RawMousePosition : Input.ScaledMousePosition.ToPoint();
			_mouseInWorldCoords = mousePos - new Point( Core.CoreGraphicsDevice.Viewport.X, Core.CoreGraphicsDevice.Viewport.Y );
		}


		public static void EndWindow()
		{
			_spriteBatch.End();
		}


		public static bool Button( string text )
		{
			var ret = false;

			var color = _buttonColor;
			if( IsMouseOverElement() )
			{
				ret = Input.LeftMouseButtonReleased;
				color = Input.LeftMouseButtonDown ? _buttonColorDown : _buttonColorActive;
			}

			_spriteBatch.DrawRect( _elementX, _lastY + ElementPadding, _elementWidth, ElementHeight, color );
			DrawString( text, _fontColor );
			EndElement();

			return ret;
		}


		/// <summary>
		/// creates a checkbox/toggle
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="isChecked">If set to <c>true</c> is checked.</param>
		public static bool Toggle( string text, bool isChecked )
		{
			var toggleX = _elementX + _elementWidth - ElementHeight;
			var color = _toggleBg;
			var toggleCheckColor = _toggleOn;
			var isToggleActive = false;

			if( IsMouseBetween( toggleX, toggleX + ElementHeight ) )
			{
				color = _toggleBgActive;
				if( Input.LeftMouseButtonDown )
				{
					isToggleActive = true;
					toggleCheckColor = _toggleOnActive;
				}

				if( Input.LeftMouseButtonReleased )
					isChecked = !isChecked;
			}

			DrawString( text, _fontColor, TextAlign.Left );
			_spriteBatch.DrawRect( toggleX, _lastY + ElementPadding, ElementHeight, ElementHeight, color );

			if( isChecked || isToggleActive )
				_spriteBatch.DrawRect( toggleX + 3, _lastY + ElementPadding + 3, ElementHeight - 6, ElementHeight - 6, toggleCheckColor );

			EndElement();

			return isChecked;
		}


		/// <summary>
		/// value should be between 0 and 1
		/// </summary>
		/// <param name="value">Value.</param>
		public static float Slider( float value, string name = "" )
		{
			var workingWidth = _elementWidth - ShortElementHeight;
			var thumbPos = workingWidth * value;
			var color = _sliderThumbBg;

			if( IsMouseOverElement() )
			{
				if( Input.LeftMouseButtonDown )
				{
					var localMouseX = _mouseInWorldCoords.X - _elementX - ShortElementHeight * 0.5f;
					value = MathHelper.Clamp( localMouseX / workingWidth, 0, 1 );
					thumbPos = workingWidth * value;
					color = _sliderThumbBgDown;
				}
				else
				{
					color = _sliderThumbBgActive;
				}
			}
				
			_spriteBatch.DrawRect( _elementX, _lastY + ElementPadding, _elementWidth, ShortElementHeight, _sliderBg );
			_spriteBatch.DrawRect( _elementX + thumbPos, _lastY + ElementPadding, ShortElementHeight, ShortElementHeight, color );
			DrawString( name + value.ToString( "F" ), _fontColor, TextAlign.Center, ShortElementHeight );
			EndElement();

			return value;
		}


		/// <summary>
		/// value should be between 0 and 1
		/// </summary>
		/// <returns>The bar.</returns>
		/// <param name="value">Value.</param>
		public static float ProgressBar( float value )
		{
			var thumbPos = _elementWidth * value;
			var color = _sliderThumbBg;

			if( IsMouseOverElement() )
			{
				if( Input.LeftMouseButtonDown )
				{
					var localMouseX = _mouseInWorldCoords.X - _elementX;
					value = MathHelper.Clamp( localMouseX / _elementWidth, 0, 1 );
					thumbPos = _elementWidth * value;
					color = _sliderThumbBgDown;
				}
				else
				{
					color = _sliderThumbBgActive;
				}
			}

			_spriteBatch.DrawRect( _elementX, _lastY + ElementPadding, _elementWidth, ElementHeight, _sliderBg );
			_spriteBatch.DrawRect( _elementX, _lastY + ElementPadding, thumbPos, ElementHeight, color );
			DrawString( value.ToString( "F" ), _fontColor );
			EndElement();

			return value;
		}


		/// <summary>
		/// creates a full width header with text
		/// </summary>
		/// <param name="text">Text.</param>
		public static void Header( string text )
		{
			// expand the header to full width and use a shorter element height
			_spriteBatch.DrawRect( _elementX - ElementPadding, _lastY + ElementPadding, _elementWidth + ElementPadding * 2, ShortElementHeight, _headerBg );
			DrawString( text, _fontColor, TextAlign.Center, ShortElementHeight );
			EndElement( ShortElementHeight );
		}


		/// <summary>
		/// adds some vertical space
		/// </summary>
		/// <param name="verticalSpace">Vertical space.</param>
		public static void Space( float verticalSpace )
		{
			_lastY += verticalSpace;
		}
	
	}
}

