using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterVisualData
{
	public CharacterType Character;
	public Sprite Portrait;
}

[CreateAssetMenu(fileName = "CharacterVisualConfig", menuName = "Config/CharacterVisualConfig", order = 1)]
public sealed class CharacterVisualConfig : ScriptableObject 
{
	[SerializeField]
	private List<CharacterVisualData> _config;

	private Dictionary<CharacterType, CharacterVisualData> _dictionary = new Dictionary<CharacterType, CharacterVisualData> ();

	public CharacterVisualData GetConfiguration(CharacterType character)
	{
		CharacterVisualData result;
		_dictionary.TryGetValue (character, out result);

		if (result == null) 
		{
			result = _config.Find (temp => temp.Character == character);

			if (result == null) 
			{
				throw new System.NullReferenceException (string.Format ("{0} visuals isn't defined", character));
			}

			_dictionary.Add (character, result);
		}

		return result;
	}

	public List<CharacterVisualData> Characters
	{
		get {
			return _config;
		}
	}
}
