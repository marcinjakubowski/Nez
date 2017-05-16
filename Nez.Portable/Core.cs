using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Nez.Systems;
using Nez.Console;
using Nez.Tweens;
using Nez.Timers;
using Nez.BitmapFonts;
using Nez.Analysis;
using Nez.Textures;


namespace Nez
{
	public class Core : Game
	{
		/// <summary>
		/// core emitter. emits only Core level events.
		/// </summary>
		public static Emitter<CoreEvents> Emitter;

		/// <summary>
		/// enables/disables if we should quit the app when escape is pressed
		/// </summary>
		public static bool ExitOnEscapeKeypress = true;

		/// <summary>
		/// enables/disables pausing when focus is lost. No update or render methods will be called if true when not in focus.
		/// </summary>
		public static bool PauseOnFocusLost = true;

		/// <summary>
		/// enables/disables debug rendering
		/// </summary>
		public static bool DebugRenderEnabled = false;

		/// <summary>
		/// global access to the CoreGraphicsDevice
		/// </summary>
		public static GraphicsDevice CoreGraphicsDevice;

		/// <summary>
		/// global GlobalContent manager for loading any assets that should stick around between scenes
		/// </summary>
		public static NezContentManager GlobalContent;

		/// <summary>
		/// default SamplerState used by Materials. Note that this must be set at launch! Changing it after that time will result in only
		/// Materials created after it was set having the new SamplerState
		/// </summary>
		public static SamplerState DefaultSamplerState = SamplerState.PointClamp;

		/// <summary>
		/// default wrapped SamplerState. Determined by the Filter of the defaultSamplerState.
		/// </summary>
		/// <value>The default state of the wraped sampler.</value>
		public static SamplerState DefaultWrappedSamplerState { get { return DefaultSamplerState.Filter == TextureFilter.Point ? SamplerState.PointWrap : SamplerState.LinearWrap; } }

		/// <summary>
		/// default GameServiceContainer access
		/// </summary>
		/// <value>The GameServices.</value>
		public static GameServiceContainer GameServices { get { return Instance.Services; } }

		/// <summary>
		/// internal flag used to determine if EntitySystems should be used or not
		/// </summary>
		internal static bool EntitySystemsEnabled;

		/// <summary>
		/// facilitates easy access to the global Content instance for internal classes
		/// </summary>
		internal static Core Instance;

		#if DEBUG
		internal static ulong DrawCalls;
		TimeSpan _frameCounterElapsedTime = TimeSpan.Zero;
		int _frameCounter = 0;
		string _windowTitle;
		#endif

		Scene _scene;
		Scene _nextScene;
		internal SceneTransition SceneTransition;

		/// <summary>
		/// used to coalesce GraphicsDeviceReset events
		/// </summary>
		ITimer _graphicsDeviceChangeTimer;

		// globally accessible systems
		FastList<IUpdatableManager> _globalManagers = new FastList<IUpdatableManager>();
		CoroutineManager _coroutineManager = new CoroutineManager();
		TimerManager _timerManager = new TimerManager();


		/// <summary>
		/// The currently active Scene. Note that if set, the Scene will not actually change until the end of the Update
		/// </summary>
		public static Scene Scene
		{
			get { return Instance._scene; }
			set { Instance._nextScene = value; }
		}


		public Core( int width = 1280, int height = 720, bool isFullScreen = false, bool enableEntitySystems = true, string windowTitle = "Nez", string contentDirectory = "Content" )
		{
			#if DEBUG
			_windowTitle = windowTitle;
			#endif

			Instance = this;
			Emitter = new Emitter<CoreEvents>( new CoreEventsComparer() );

			var graphicsManager = new GraphicsDeviceManager( this );
			graphicsManager.PreferredBackBufferWidth = width;
			graphicsManager.PreferredBackBufferHeight = height;
			graphicsManager.IsFullScreen = isFullScreen;
			graphicsManager.SynchronizeWithVerticalRetrace = true;
			graphicsManager.DeviceReset += OnGraphicsDeviceReset;
			graphicsManager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

			Screen.Initialize( graphicsManager );
			Window.ClientSizeChanged += OnGraphicsDeviceReset;
			Window.OrientationChanged += OnOrientationChanged;

			Content.RootDirectory = contentDirectory;
			GlobalContent = new NezGlobalContentManager( Services, Content.RootDirectory );
			IsMouseVisible = true;
			IsFixedTimeStep = false;

			EntitySystemsEnabled = enableEntitySystems;

			// setup systems
			_globalManagers.Add( _coroutineManager );
			_globalManagers.Add( new TweenManager() );
			_globalManagers.Add( _timerManager );
			_globalManagers.Add( new RenderTarget() );
		}


		void OnOrientationChanged( object sender, EventArgs e )
		{
			Emitter.Emit( CoreEvents.OrientationChanged );
		}


		/// <summary>
		/// this gets called whenever the screen size changes
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnGraphicsDeviceReset( object sender, EventArgs e )
		{
			// we coalese these to avoid spamming events
			if( _graphicsDeviceChangeTimer != null )
			{
				_graphicsDeviceChangeTimer.Reset();
			}
			else
			{
				_graphicsDeviceChangeTimer = Schedule( 0.05f, false, this, t =>
				{
					( t.Context as Core )._graphicsDeviceChangeTimer = null;
					Emitter.Emit( CoreEvents.GraphicsDeviceReset );
				} );
			}
		}


		#region Passthroughs to Game

		public static void exit()
		{
			Instance.Exit();
		}

		#endregion


		#region Game overides

		protected override void Initialize()
		{
			base.Initialize();

			// prep the default Graphics system
			CoreGraphicsDevice = GraphicsDevice;
			var font = GlobalContent.Load<BitmapFont>( "nez://Nez.Content.NezDefaultBMFont.xnb" );
			Graphics.Instance = new Graphics( font );
		}


		protected override void Update( GameTime gameTime )
		{
			if( PauseOnFocusLost && !IsActive )
			{
				SuppressDraw();
				return;
			}

			#if DEBUG
			TimeRuler.Instance.StartFrame();
			TimeRuler.Instance.BeginMark( "update", Color.Green );
			#endif

			// update all our systems and global managers
			Time.Update( (float)gameTime.ElapsedGameTime.TotalSeconds );
			Input.Update();

			for( var i = _globalManagers.Length - 1; i >= 0; i-- )
				_globalManagers.Buffer[i].Update();

			if( ExitOnEscapeKeypress && ( Input.IsKeyDown( Keys.Escape ) || Input.GamePads[0].IsButtonReleased( Buttons.Back ) ) )
			{
				Exit();
				return;
			}

			if( _scene != null )
				_scene.Update();

			if( _scene != _nextScene )
			{
				if( _scene != null )
					_scene.End();

				_scene = _nextScene;
				OnSceneChanged();

				if( _scene != null )
					_scene.Begin();
			}

			#if DEBUG
			TimeRuler.Instance.EndMark( "update" );
			DebugConsole.Instance.Update();
			DrawCalls = 0;
			#endif

			#if FNA
			// MonoGame only updates old-school XNA Components in Update which we dont care about. FNA's core FrameworkDispatcher needs
			// Update called though so we do so here.
			FrameworkDispatcher.Update();
			#endif
		}


		protected override void Draw( GameTime gameTime )
		{
			if( PauseOnFocusLost && !IsActive )
				return;

			#if DEBUG
			TimeRuler.Instance.BeginMark( "draw", Color.Gold );

			// fps counter
			_frameCounter++;
			_frameCounterElapsedTime += gameTime.ElapsedGameTime;
			if( _frameCounterElapsedTime >= TimeSpan.FromSeconds( 1 ) )
			{
				var totalMemory = ( GC.GetTotalMemory( false ) / 1048576f ).ToString( "F" );
				Window.Title = string.Format( "{0} {1} fps - {2} MB", _windowTitle, _frameCounter, totalMemory );
				_frameCounter = 0;
				_frameCounterElapsedTime -= TimeSpan.FromSeconds( 1 );
			}
			#endif

			if( SceneTransition != null )
				SceneTransition.PreRender( Graphics.Instance );

			if( _scene != null )
			{
				_scene.Render();

				#if DEBUG
				if( DebugRenderEnabled )
					Debug.Render();
				#endif

				// render as usual if we dont have an active SceneTransition
				if( SceneTransition == null )
					_scene.PostRender();
			}

			// special handling of SceneTransition if we have one
			if( SceneTransition != null )
			{
				if( _scene != null && SceneTransition.WantsPreviousSceneRender && !SceneTransition.HasPreviousSceneRender )
				{
					_scene.PostRender( SceneTransition.PreviousSceneRender );
					if( SceneTransition.LoadsNewScene )
						Scene = null;
					StartCoroutine( SceneTransition.OnBeginTransition() );
				}
				else if( _scene != null )
				{
					_scene.PostRender();
				}

				SceneTransition.Render( Graphics.Instance );
			}

			#if DEBUG
			TimeRuler.Instance.EndMark( "draw" );
			DebugConsole.Instance.Render();

			// the TimeRuler only needs to render when the DebugConsole is not open
			if( !DebugConsole.Instance.IsOpen )
				TimeRuler.Instance.Render();

			#if !FNA
			DrawCalls = CoreGraphicsDevice.Metrics.DrawCount;
			#endif
			#endif
		}

		#endregion


		/// <summary>
		/// Called after a Scene ends, before the next Scene begins
		/// </summary>
		void OnSceneChanged()
		{
			Emitter.Emit( CoreEvents.SceneChanged );
			Time.SceneChanged();
			GC.Collect();
		}


		/// <summary>
		/// temporarily runs SceneTransition allowing one Scene to transition to another smoothly with custom effects.
		/// </summary>
		/// <param name="sceneTransition">Scene transition.</param>
		public static T StartSceneTransition<T>( T sceneTransition ) where T : SceneTransition
		{
			Assert.IsNull( Instance.SceneTransition, "You cannot start a new SceneTransition until the previous one has completed" );
			Instance.SceneTransition = sceneTransition;
			return sceneTransition;
		}


		#region Global Managers

		/// <summary>
		/// adds a global manager object that will have its update method called each frame before Scene.update is called
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <param name="manager">Manager.</param>
		public static void RegisterGlobalManager( IUpdatableManager manager )
		{
			Instance._globalManagers.Add( manager );
		}


		/// <summary>
		/// removes the global manager object
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <param name="manager">Manager.</param>
		public static void UnregisterGlobalManager( IUpdatableManager manager )
		{
			Instance._globalManagers.Remove( manager );
		}


		/// <summary>
		/// gets the global manager of type T
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetGlobalManager<T>() where T : class, IUpdatableManager
		{
			for( var i = 0; i < Instance._globalManagers.Length; i++ )
			{
				if( Instance._globalManagers.Buffer[i] is T )
					return Instance._globalManagers.Buffer[i] as T;
			}
			return null;
		}

		#endregion


		#region Systems access

		/// <summary>
		/// starts a coroutine. Coroutines can yeild ints/floats to delay for seconds or yeild to other calls to startCoroutine.
		/// Yielding null will make the coroutine get ticked the next frame.
		/// </summary>
		/// <returns>The coroutine.</returns>
		/// <param name="enumerator">Enumerator.</param>
		public static ICoroutine StartCoroutine( IEnumerator enumerator )
		{
			return Instance._coroutineManager.StartCoroutine( enumerator );
		}


		/// <summary>
		/// schedules a one-time or repeating timer that will call the passed in Action
		/// </summary>
		/// <param name="timeInSeconds">Time in seconds.</param>
		/// <param name="repeats">If set to <c>true</c> repeats.</param>
		/// <param name="context">Context.</param>
		/// <param name="onTime">On time.</param>
		public static ITimer Schedule( float timeInSeconds, bool repeats, object context, Action<ITimer> onTime )
		{
			return Instance._timerManager.Schedule( timeInSeconds, repeats, context, onTime );
		}


		/// <summary>
		/// schedules a one-time timer that will call the passed in Action after timeInSeconds
		/// </summary>
		/// <param name="timeInSeconds">Time in seconds.</param>
		/// <param name="context">Context.</param>
		/// <param name="onTime">On time.</param>
		public static ITimer Schedule( float timeInSeconds, object context, Action<ITimer> onTime )
		{
			return Instance._timerManager.Schedule( timeInSeconds, false, context, onTime );
		}


		/// <summary>
		/// schedules a one-time or repeating timer that will call the passed in Action
		/// </summary>
		/// <param name="timeInSeconds">Time in seconds.</param>
		/// <param name="repeats">If set to <c>true</c> repeats.</param>
		/// <param name="onTime">On time.</param>
		public static ITimer Schedule( float timeInSeconds, bool repeats, Action<ITimer> onTime )
		{
			return Instance._timerManager.Schedule( timeInSeconds, repeats, null, onTime );
		}


		/// <summary>
		/// schedules a one-time timer that will call the passed in Action after timeInSeconds
		/// </summary>
		/// <param name="timeInSeconds">Time in seconds.</param>
		/// <param name="onTime">On time.</param>
		public static ITimer Schedule( float timeInSeconds, Action<ITimer> onTime )
		{
			return Instance._timerManager.Schedule( timeInSeconds, false, null, onTime );
		}

		#endregion

	}
}

