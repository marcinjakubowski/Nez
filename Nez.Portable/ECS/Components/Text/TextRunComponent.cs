﻿namespace Nez
{
	/// <summary>
	/// very basic wrapper around TextRun. Note that the TextRunComponent.compile method should be used not TextRun.compile so that
	/// the Component data can be passed off to the TextRun.
	/// </summary>
	public class TextRunComponent : RenderableComponent
	{
		public override float Width { get { return TextRun.Width; } }
		public override float Height { get { return TextRun.Height; } }
		public TextRun TextRun;


		public TextRunComponent( TextRun textRun )
		{
			this.TextRun = textRun;
		}


		/// <summary>
		/// calls through to TextRun.compile and handles marshalling some data between this Component and the underlying TextRun
		/// </summary>
		public void Compile()
		{
			TextRun.Position = Transform.Position;
			TextRun.Rotation = Transform.Rotation;
			TextRun.Compile();
		}


		public override void Render( Graphics graphics, Camera camera )
		{
			TextRun.Render( graphics );
		}
	}
}

