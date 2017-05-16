using System.Collections.Generic;
#if !FNA
using Microsoft.Xna.Framework.Input.Touch;
#endif


namespace Nez
{
	/// <summary>
	/// to enable touch input you must first call enableTouchSupport()
	/// </summary>
	public class TouchInput
	{
		#if !FNA
		public bool IsConnected
		{
			get { return _isConnected; }
		}

		public TouchCollection CurrentTouches
		{
			get { return _currentTouches; }
		}

		public TouchCollection PreviousTouches
		{
			get { return _previousTouches; }
		}

		public List<GestureSample> PreviousGestures
		{
			get { return _previousGestures; }
		}

		public List<GestureSample> CurrentGestures
		{
			get { return _currentGestures; }
		}

		TouchCollection _previousTouches;
		TouchCollection _currentTouches;
		List<GestureSample> _previousGestures = new List<GestureSample>();
		List<GestureSample> _currentGestures = new List<GestureSample>();
		#endif

		#pragma warning disable 0649
		bool _isConnected;
		#pragma warning restore 0649


		void OnGraphicsDeviceReset()
		{
			#if !FNA
			TouchPanel.DisplayWidth = Core.CoreGraphicsDevice.Viewport.Width;
			TouchPanel.DisplayHeight = Core.CoreGraphicsDevice.Viewport.Height;
			TouchPanel.DisplayOrientation = Core.CoreGraphicsDevice.PresentationParameters.DisplayOrientation;
			#endif
		}


		internal void Update()
		{
			if( !_isConnected )
				return;
			
			#if !FNA
			_previousTouches = _currentTouches;
			_currentTouches = TouchPanel.GetState();

			_previousGestures = _currentGestures;
			_currentGestures.Clear();
			while( TouchPanel.IsGestureAvailable )
				_currentGestures.Add( TouchPanel.ReadGesture() );
			#endif
		}


		public void EnableTouchSupport()
		{
			#if !FNA
            _isConnected = TouchPanel.GetCapabilities().IsConnected;
			#endif

			if( _isConnected )
			{
				Core.Emitter.AddObserver( CoreEvents.GraphicsDeviceReset, OnGraphicsDeviceReset );
				Core.Emitter.AddObserver( CoreEvents.OrientationChanged, OnGraphicsDeviceReset );
				OnGraphicsDeviceReset();
			}
		}

	}
}

