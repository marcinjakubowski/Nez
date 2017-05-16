using System;
using System.Collections.Generic;


namespace Nez.AI.FSM
{
	public class StateMachine<T>
	{
		public event Action OnStateChanged;

		public State<T> CurrentState { get { return _currentState; } }
		public State<T> PreviousState;
		public float ElapsedTimeInState = 0f;

		protected State<T> _currentState;
		protected T _context;
		Dictionary<Type, State<T>> _states = new Dictionary<Type, State<T>>();


		public StateMachine( T context, State<T> initialState )
		{
			_context = context;

			// setup our initial state
			AddState( initialState );
			_currentState = initialState;
			_currentState.Begin();
		}


		/// <summary>
		/// adds the state to the machine
		/// </summary>
		public void AddState( State<T> state )
		{
			state.SetMachineAndContext( this, _context );
			_states[state.GetType()] = state;
		}


		/// <summary>
		/// ticks the state machine with the provided delta time
		/// </summary>
		public virtual void Update( float deltaTime )
		{
			ElapsedTimeInState += deltaTime;
			_currentState.Reason();
			_currentState.Update( deltaTime );
		}


		/// <summary>
		/// changes the current state
		/// </summary>
		public TR ChangeState<TR>() where TR : State<T>
		{
			// avoid changing to the same state
			var newType = typeof( TR );
			if( _currentState.GetType() == newType )
				return _currentState as TR;

			// only call end if we have a currentState
			if( _currentState != null )
				_currentState.End();

			Assert.IsTrue( _states.ContainsKey( newType ), "{0}: state {1} does not exist. Did you forget to add it by calling addState?", GetType(), newType );

			// swap states and call begin
			ElapsedTimeInState = 0f;
			PreviousState = _currentState;
			_currentState = _states[newType];
			_currentState.Begin();

			// fire the changed event if we have a listener
			if( OnStateChanged != null )
				OnStateChanged();

			return _currentState as TR;
		}

	}
}

