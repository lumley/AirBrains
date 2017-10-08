using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAlreadyUsedException: System.Exception
{
	public CharacterType Type;
	public CharacterAlreadyUsedException(CharacterType type)
	{
		Type = type;
	}

	public override string ToString ()
	{
		return string.Format ("[CharacterAlreadyUsedException: {0} already used]", Type);
	}
}