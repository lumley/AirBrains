using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoesntExistException: System.Exception
{
	public int PlayerId;
	public PlayerDoesntExistException(int playerId)
	{
		PlayerId = playerId;
	}

	public override string ToString ()
	{
		return string.Format ("[PlayerDoesntExistException: {0} doesn't exist]", PlayerId);
	}
}