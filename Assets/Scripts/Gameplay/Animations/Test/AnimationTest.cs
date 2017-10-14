using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public enum MoveDirection
{
	Left,
	Right,
	Top, 
	Down
}

public class AnimationTest : MonoBehaviour
{
	public MonsterAnimationController Monster;
	public HumanAnimationController Human;

	private void OnGUI()
	{
		// region monster
		if (GUI.Button(new Rect(0, 0, 400, 40), "Monster Idle"))
		{
			Monster.ApplyState(StateType.Idle);
		}
		
		if (GUI.Button(new Rect(0, 50, 400, 40), "Monster Fight"))
		{
			Monster.ApplyState(StateType.Fight);
		}
		
		if (GUI.Button(new Rect(0, 100, 400, 40), "Monster Walk Top"))
		{
			Monster.ApplyState(StateType.Walk, MoveDirection.Top);
		}
		
		if (GUI.Button(new Rect(0, 150, 400, 40), "Monster Walk Left"))
		{
			Monster.ApplyState(StateType.Walk, MoveDirection.Left);
		}
		
		if (GUI.Button(new Rect(0, 200, 400, 40), "Monster Walk Right"))
		{
			Monster.ApplyState(StateType.Walk, MoveDirection.Right);
		}
		
		if (GUI.Button(new Rect(0, 250, 400, 40), "Monster Walk Down"))
		{
			Monster.ApplyState(StateType.Walk, MoveDirection.Down);
		}
		
		if (GUI.Button(new Rect(0, 300, 400, 40), "Monster Sticker"))
		{
			Monster.ApplyState(StateType.Sticker, Human);
		}
		
		//region human
		if (GUI.Button(new Rect(0, 400, 400, 40), "Human Idle"))
		{
			Human.ApplyState(StateType.Idle);
		}
				
		if (GUI.Button(new Rect(0, 450, 400, 40), "Human Clean Sticker"))
		{
			Human.OnCleanSticker();
		}
		
		if (GUI.Button(new Rect(0, 500, 400, 40), "Human Walk Top"))
		{
			Human.ApplyState(StateType.Walk, MoveDirection.Top);
		}
		
		if (GUI.Button(new Rect(0, 550, 400, 40), "Human Walk Left"))
		{
			Human.ApplyState(StateType.Walk, MoveDirection.Left);
		}
		
		if (GUI.Button(new Rect(0, 600, 400, 40), "Human Walk Right"))
		{
			Human.ApplyState(StateType.Walk, MoveDirection.Right);
		}
		
		if (GUI.Button(new Rect(0, 650, 400, 40), "Human Walk Down"))
		{
			Human.ApplyState(StateType.Walk, MoveDirection.Down);
		}
	}
}
