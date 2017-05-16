using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Nez.Systems;
using System.Runtime.CompilerServices;
#if !FNA
using Microsoft.Xna.Framework.Input.Touch;
#endif


namespace Nez
{
	public static class Input
	{
		public static Emitter<InputEventType, InputEvent> Emitter;

		public static GamePadData[] GamePads;
		public const float DefaultDeadzone = 0.1f;

		/// <summary>
		/// set by the Scene and used to scale mouse input
		/// </summary>
		internal static Vector2 ResolutionScale;
		/// <summary>
		/// set by the Scene and used to scale input
		/// </summary>
		internal static Point ResolutionOffset;
		static KeyboardState _previousKbState;
		static KeyboardState _currentKbState;
		static MouseState _previousMouseState;
		static MouseState _currentMouseState;
		static internal FastList<VirtualInput> VirtualInputs = new FastList<VirtualInput>();
		static int _maxSupportedGamePads;

		public static TouchInput Touch;


		public static int MaxSupportedGamePads
		{
			get { return _maxSupportedGamePads; }
			set
			{
				_maxSupportedGamePads = Mathf.Clamp( value, 1, 4 );
				GamePads = new GamePadData[_maxSupportedGamePads];
				for( var i = 0; i < _maxSupportedGamePads; i++ )
					GamePads[i] = new GamePadData( (PlayerIndex)i );
			}
		}


		static Input()
		{
			Emitter = new Emitter<InputEventType, InputEvent>();
			Touch = new TouchInput();

			_previousKbState = new KeyboardState();
			_currentKbState = Keyboard.GetState();

			_previousMouseState = new MouseState();
			_currentMouseState = Mouse.GetState();

			MaxSupportedGamePads = 1;
		}


		public static void Update()
		{
			Touch.Update();

			_previousKbState = _currentKbState;
			_currentKbState = Keyboard.GetState();

			_previousMouseState = _currentMouseState;
			_currentMouseState = Mouse.GetState();

			for( var i = 0; i < _maxSupportedGamePads; i++ )
				GamePads[i].Update();

			for( var i = 0; i < VirtualInputs.Length; i++ )
				VirtualInputs.Buffer[i].Update();
		}

		/// <summary>
		/// this takes into account the SceneResolutionPolicy and returns the value scaled to the RenderTargets coordinates
		/// </summary>
		/// <value>The scaled position.</value>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector2 ScaledPosition( Vector2 position )
		{
			var scaledPos = new Vector2( position.X - ResolutionOffset.X, position.Y - ResolutionOffset.Y );
			return scaledPos * ResolutionScale;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector2 ScaledPosition( Point position )
		{
			return ScaledPosition( new Vector2( position.X, position.Y ) );
		}

		#region Keyboard

		public static KeyboardState PreviousKeyboardState { get { return _previousKbState; } }

		public static KeyboardState CurrentKeyboardState { get { return _currentKbState; } }


		/// <summary>
		/// only true if down this frame
		/// </summary>
		/// <returns><c>true</c>, if key pressed was gotten, <c>false</c> otherwise.</returns>
		public static bool IsKeyPressed( Keys key )
		{
			return _currentKbState.IsKeyDown( key ) && !_previousKbState.IsKeyDown( key );
		}


		/// <summary>
		/// true the entire time the key is down
		/// </summary>
		/// <returns><c>true</c>, if key down was gotten, <c>false</c> otherwise.</returns>
		public static bool IsKeyDown( Keys key )
		{
			return _currentKbState.IsKeyDown( key );
		}


		/// <summary>
		/// true only the frame the key is released
		/// </summary>
		/// <returns><c>true</c>, if key up was gotten, <c>false</c> otherwise.</returns>
		public static bool IsKeyReleased( Keys key )
		{
			return !_currentKbState.IsKeyDown( key ) && _previousKbState.IsKeyDown( key );
		}


		/// <summary>
		/// only true if one of the keys is down this frame
		/// </summary>
		/// <returns><c>true</c>, if key pressed was gotten, <c>false</c> otherwise.</returns>
		public static bool IsKeyPressed( Keys keyA, Keys keyB )
		{
			return IsKeyPressed( keyA ) || IsKeyPressed( keyB );
		}


		/// <summary>
		/// true while either of the keys are down
		/// </summary>
		/// <returns><c>true</c>, if key down was gotten, <c>false</c> otherwise.</returns>
		public static bool IsKeyDown( Keys keyA, Keys keyB )
		{
			return IsKeyDown( keyA ) || IsKeyDown( keyB );
		}


		/// <summary>
		/// true only the frame one of the keys are released
		/// </summary>
		/// <returns><c>true</c>, if key up was gotten, <c>false</c> otherwise.</returns>
		public static bool IsKeyReleased( Keys keyA, Keys keyB )
		{
			return IsKeyReleased( keyA ) || IsKeyReleased( keyB );
		}

		#endregion


		#region Mouse

		/// <summary>
		/// returns the previous mouse state. Use with caution as it only contains raw data and does not take camera scaling into affect like
		/// Input.mousePosition does.
		/// </summary>
		/// <value>The state of the previous mouse.</value>
		public static MouseState PreviousMouseState { get { return _previousMouseState; } }

		/// <summary>
		/// only true if down this frame
		/// </summary>
		public static bool LeftMouseButtonPressed
		{
			get { return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released; }
		}

		/// <summary>
		/// true while the button is down
		/// </summary>
		public static bool LeftMouseButtonDown
		{
			get { return _currentMouseState.LeftButton == ButtonState.Pressed; }
		}

		/// <summary>
		/// true only the frame the button is released
		/// </summary>
		public static bool LeftMouseButtonReleased
		{
			get
			{
				return _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;
			}
		}

		/// <summary>
		/// only true if pressed this frame
		/// </summary>
		public static bool RightMouseButtonPressed
		{
			get
			{
				return _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released;
			}
		}

		/// <summary>
		/// true while the button is down
		/// </summary>
		public static bool RightMouseButtonDown
		{
			get { return _currentMouseState.RightButton == ButtonState.Pressed; }
		}

		/// <summary>
		/// true only the frame the button is released
		/// </summary>
		public static bool RightMouseButtonReleased
		{
			get
			{
				return _currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed;
			}
		}

		/// <summary>
		/// gets the raw ScrollWheelValue
		/// </summary>
		/// <value>The mouse wheel.</value>
		public static int MouseWheel
		{
			get { return _currentMouseState.ScrollWheelValue; }
		}

		/// <summary>
		/// gets the delta ScrollWheelValue
		/// </summary>
		/// <value>The mouse wheel delta.</value>
		public static int MouseWheelDelta
		{
			get { return _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue; }
		}

		/// <summary>
		/// unscaled mouse position. This is the actual screen space value
		/// </summary>
		/// <value>The raw mouse position.</value>
		public static Point RawMousePosition
		{
			get { return new Point( _currentMouseState.X, _currentMouseState.Y ); }
		}

		/// <summary>
		/// alias for scaledMousePosition
		/// </summary>
		/// <value>The mouse position.</value>
		public static Vector2 MousePosition { get { return ScaledMousePosition; } }

		/// <summary>
		/// this takes into account the SceneResolutionPolicy and returns the value scaled to the RenderTargets coordinates
		/// </summary>
		/// <value>The scaled mouse position.</value>
		public static Vector2 ScaledMousePosition { get { return ScaledPosition( new Vector2( _currentMouseState.X, _currentMouseState.Y ) ); } }

		public static Point MousePositionDelta
		{
			get { return new Point( _currentMouseState.X, _currentMouseState.Y ) - new Point( _previousMouseState.X, _previousMouseState.Y ); }
		}

		public static Vector2 ScaledMousePositionDelta
		{
			get
			{
				var pastPos = new Vector2( _previousMouseState.X - ResolutionOffset.X, _previousMouseState.Y - ResolutionOffset.Y );
				pastPos *= ResolutionScale;
				return ScaledMousePosition - pastPos;
			}
		}

		#endregion

	}


	public enum InputEventType
	{
		GamePadConnected,
		GamePadDisconnected
	}


	public struct InputEvent
	{
		public int GamePadIndex;
	}


	/// <summary>
	/// comparer that should be passed to a dictionary constructor to avoid boxing/unboxing when using an enum as a key
	/// on Mono
	/// </summary>
	public struct InputEventTypeComparer : IEqualityComparer<InputEventType>
	{
		public bool Equals( InputEventType x, InputEventType y )
		{
			return x == y;
		}


		public int GetHashCode( InputEventType obj )
		{
			return (int)obj;
		}
	}
}

