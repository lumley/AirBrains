using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAlreadyExistException: System.Exception
{
	public CharacterType Type;
	public CharacterAlreadyExistException(CharacterType type)
	{
		Type = type;
	}

	public override string ToString ()
	{
		return string.Format ("[CharacterAlreadyExistException: {0} already exist]", Type);
	}
}