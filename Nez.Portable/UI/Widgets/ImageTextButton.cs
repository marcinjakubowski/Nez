﻿using System;
using Nez.BitmapFonts;
using Microsoft.Xna.Framework;


namespace Nez.UI
{
	public class ImageTextButton : Button
	{
		Image _image;
		Label _label;
		ImageTextButtonStyle _style;


		public ImageTextButton( string text, ImageTextButtonStyle style ) : base( style )
		{
			this._style = style;

			Defaults().Space( 3 );

			_image = new Image();
			_image.SetScaling( Scaling.Fit );

			_label = new Label( text, style.Font, style.FontColor );
			_label.SetAlignment( UI.Align.Center );

			Add( _image );
			Add( _label );

			SetStyle( style );

			SetSize( PreferredWidth, PreferredHeight );
		}


		public ImageTextButton( string text, Skin skin, string styleName = null ) : this( text, skin.Get<ImageTextButtonStyle>( styleName ) )
		{}


		public void SetStyle( ImageTextButtonStyle style )
		{
			Assert.IsTrue( style is ImageTextButtonStyle, "style must be a ImageTextButtonStyle" );

			base.SetStyle( style );

			if( _image != null )
				UpdateImage();
			
			if( _label != null )
			{
				var labelStyle = _label.GetStyle();
				labelStyle.Font = style.Font;
				labelStyle.FontColor = style.FontColor;
				_label.SetStyle( labelStyle );
			}
		}


		public new ImageTextButtonStyle GetStyle()
		{
			return _style;
		}


		private void UpdateImage()
		{
			IDrawable drawable = null;
			if( _isDisabled && _style.ImageDisabled != null )
				drawable = _style.ImageDisabled;
			else if( _mouseDown && _style.ImageDown != null )
				drawable = _style.ImageDown;
			else if( IsChecked && _style.ImageChecked != null )
				drawable = ( _style.ImageCheckedOver != null && _mouseOver ) ? _style.ImageCheckedOver : _style.ImageChecked;
			else if( _mouseOver && _style.ImageOver != null )
				drawable = _style.ImageOver;
			else if( _style.ImageUp != null ) //
				drawable = _style.ImageUp;
			_image.SetDrawable( drawable );
		}


		public override void Draw( Graphics graphics, float parentAlpha )
		{
			UpdateImage();

			Color? fontColor;
			if( _isDisabled && _style.DisabledFontColor.HasValue )
				fontColor = _style.DisabledFontColor;
			else if( _mouseDown && _style.DownFontColor.HasValue )
				fontColor = _style.DownFontColor;
			else if( IsChecked && _style.CheckedFontColor.HasValue )
				fontColor = ( _mouseOver && _style.CheckedOverFontColor.HasValue ) ? _style.CheckedOverFontColor : _style.CheckedFontColor;
			else if( _mouseOver && _style.OverFontColor.HasValue )
				fontColor = _style.OverFontColor;
			else
				fontColor = _style.FontColor;
			
			if( fontColor.HasValue )
				_label.GetStyle().FontColor = fontColor.Value;
			
			base.Draw( graphics, parentAlpha );
		}


		public Image GetImage()
		{
			return _image;
		}


		public Cell GetImageCell()
		{
			return GetCell( _image );
		}


		public Label GetLabel()
		{
			return _label;
		}


		public Cell GetLabelCell()
		{
			return GetCell( _label );
		}


		public void SetText( string text )
		{
			_label.SetText( text );
		}


		public string GetText()
		{
			return _label.GetText();
		}
	}


	public class ImageTextButtonStyle : TextButtonStyle
	{
		/** Optional. */
		public IDrawable ImageUp, ImageDown, ImageOver, ImageChecked, ImageCheckedOver, ImageDisabled;


		public ImageTextButtonStyle()
		{
			Font = Graphics.Instance.BitmapFont;
		}


		public ImageTextButtonStyle( IDrawable up, IDrawable down, IDrawable over, BitmapFont font ) : base( up, down, over, font )
		{
		}


		public new ImageTextButtonStyle Clone()
		{
			return new ImageTextButtonStyle {
				Up = Up,
				Down = Down,
				Over = Over,
				Checkked = Checkked,
				CheckedOver = CheckedOver,
				Disabled = Disabled,
					
				Font = Font,
				FontColor = FontColor,
				DownFontColor = DownFontColor,
				OverFontColor = OverFontColor,
				CheckedFontColor = CheckedFontColor,
				CheckedOverFontColor = CheckedOverFontColor,
				DisabledFontColor = DisabledFontColor,

				ImageUp = ImageUp,
				ImageDown = ImageDown,
				ImageOver = ImageOver,
				ImageChecked = ImageChecked,
				ImageCheckedOver = ImageCheckedOver,
				ImageDisabled = ImageDisabled
			};
		}
	}
}

