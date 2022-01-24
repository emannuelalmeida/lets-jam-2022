using System.Collections.Generic;
using Godot;
using System;

public class InputManager
{
	public Dictionary<string, string> keyMap;
	public List<ActionExecution> actions;
	public int ticks = 0;

	public InputManager(Dictionary<string, string> keyMap, ICollection<InputAction> actions)
	{
		this.keyMap = keyMap;
		this.actions = new List<ActionExecution>();
		foreach (var action in actions)
		{
			if(action.eventType == EventType.INPUT)
			{
				this.actions.Add(new ActionExecution(action));
			}
		}
	}

	public void update(Player actor)
	{
		ticks++;

		List<string> input = new List<String>();
		foreach (var key in keyMap.Keys)
		{
			if (Input.IsActionPressed(keyMap[key]))
			{
				input.Add(key);
			}
		}

		InputAction releaseMatch = null;
		InputAction bestMatch = null;
		foreach (var exec in actions)
		{
			InputAction action = exec.action;

			if (exec.updateState(input, ticks) && action.startStateMatches(actor.currentState))
			{
				if (bestMatch == null || action.priority > bestMatch.priority)
				{
					bestMatch = action;
				}
			}
			else if (action.releaseState != null && action.changeState == actor.currentState.name)
			{
				releaseMatch = action;
			}
		}

		if (bestMatch != null)
		{
			actor.submit(new ActionEvent(bestMatch, false));
		}
		else if (releaseMatch != null)
		{
			actor.submit(new ActionEvent(releaseMatch, true));
		}
	}
}
