using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Nez
{
	public static class Screen
	{
		static internal GraphicsDeviceManager GraphicsManager;


		internal static void Initialize( GraphicsDeviceManager graphicsManager )
		{
			GraphicsManager = graphicsManager;
		}


		/// <summary>
		/// width of the CoreGraphicsDevice back buffer
		/// </summary>
		/// <value>The width.</value>
		public static int Width
		{
			get { return GraphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth; }
			set { GraphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth = value; }
		}


		/// <summary>
		/// height of the CoreGraphicsDevice back buffer
		/// </summary>
		/// <value>The height.</value>
		public static int Height
		{
			get { return GraphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight; }
			set { GraphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight = value; }
		}


		/// <summary>
		/// gets the Screen's center
		/// </summary>
		/// <value>The center.</value>
		public static Vector2 Center { get { return new Vector2( Width / 2, Height / 2 ); } }


		public static int PreferredBackBufferWidth
		{
			get { return GraphicsManager.PreferredBackBufferWidth; }
			set { GraphicsManager.PreferredBackBufferWidth = value; }
		}


		public static int PreferredBackBufferHeight
		{
			get { return GraphicsManager.PreferredBackBufferHeight; }
			set { GraphicsManager.PreferredBackBufferHeight = value; }
		}


		public static int MonitorWidth
		{
			get { return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; }
		}


		public static int MonitorHeight
		{
			get { return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; }
		}


		public static SurfaceFormat BackBufferFormat
		{
			get { return GraphicsManager.GraphicsDevice.PresentationParameters.BackBufferFormat; }
		}


		public static SurfaceFormat PreferredBackBufferFormat
		{
			get { return GraphicsManager.PreferredBackBufferFormat; }
			set { GraphicsManager.PreferredBackBufferFormat = value; }
		}


		public static bool SynchronizeWithVerticalRetrace
		{
			get { return GraphicsManager.SynchronizeWithVerticalRetrace; }
			set { GraphicsManager.SynchronizeWithVerticalRetrace = value; }
		}


		// defaults to Depth24Stencil8
		public static DepthFormat PreferredDepthStencilFormat
		{
			get { return GraphicsManager.PreferredDepthStencilFormat; }
			set { GraphicsManager.PreferredDepthStencilFormat = value;	}
		}


		public static bool IsFullscreen
		{
			get { return GraphicsManager.IsFullScreen; }
			set { GraphicsManager.IsFullScreen = value; }
		}

		public static DisplayOrientation SupportedOrientations
		{
			get { return GraphicsManager.SupportedOrientations; }
			set { GraphicsManager.SupportedOrientations = value; }
		}

		public static void ApplyChanges()
		{
			GraphicsManager.ApplyChanges();
		}


		/// <summary>
		/// sets the preferredBackBuffer then applies the changes
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public static void SetSize( int width, int height )
		{
			PreferredBackBufferWidth = width;
			PreferredBackBufferHeight = height;
			ApplyChanges();
		}

	}
}

