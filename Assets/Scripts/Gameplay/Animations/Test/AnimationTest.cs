using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
	public MonsterAnimationController Monster;
	public HumanAnimationController Human;


	private void OnGUI()
	{
		// region monster
		if (GUI.Button(new Rect(0, 0, 100, 40), "Monster Idle"))
		{
			Monster.ApplyState(StateType.Idle);
		}
		
		if (GUI.Button(new Rect(0, 50, 100, 40), "Monster Fight"))
		{
			Monster.ApplyState(StateType.Fight);
		}
		
		if (GUI.Button(new Rect(0, 100, 100, 40), "Monster Walk"))
		{
			Monster.ApplyState(StateType.Walk);
		}
		
		if (GUI.Button(new Rect(0, 150, 100, 40), "Monster Sticker"))
		{
			Monster.ApplyCurrentTarget(Human);
			Monster.ApplyState(StateType.Sticker);
		}
		
		//region human
		if (GUI.Button(new Rect(0, 200, 100, 40), "Human Idle"))
		{
			Human.ApplyState(StateType.Idle);
		}
		
		if (GUI.Button(new Rect(0, 250, 100, 40), "Human Walk"))
		{
			Human.ApplyState(StateType.Walk);
		}
		
		if (GUI.Button(new Rect(0, 300, 100, 40), "Human Clean Sticker"))
		{
			Human.ApplySticker(null);
		}
	}
}
