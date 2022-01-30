using System.Collections.Generic;
using Godot;
using System;

public class StateMachine<T> where T : Enum
{
	private Dictionary<T, State> states = new Dictionary<T, State>();
	private T currentState = default(T);

	private AnimatedSprite3D animation;

	public StateMachine(AnimatedSprite3D animation)
	{
		this.animation = animation;
	}

	public void AddState(T state, string animation, Action onEnter, Action onUpdate)
	{
		states.Add(state, new State(onEnter, onUpdate, animation));
	}

	public void ChangeState(T newState)
	{
		if(Enum.Equals(currentState, newState))
			return;

		var newStateObject = states[newState];
		newStateObject.OnEnterState();

		currentState = newState;

		animation.Play(newStateObject.Animation);
	}

	public void UpdateCurrentState()
	{
		if(states.ContainsKey(currentState))
			states[currentState].UpdateState();
	}
}

public class State
{
	private Action onEnter;
	private Action update;

	public string Animation {get;}
	public State(Action onEnter, Action onUpdate, string animation)
	{
		this.onEnter = onEnter;
		this.update = onUpdate;
		Animation = animation;
	}

	public void UpdateState() 
	{
		update?.Invoke();
	}

	public void OnEnterState(){
		onEnter?.Invoke();
	}
}
