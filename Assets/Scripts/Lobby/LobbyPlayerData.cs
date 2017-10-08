public class LobbyPlayerData
{
	public int Id;
	public CharacterType Character;
	public bool IsReady = false;

	public override string ToString ()
	{
		return string.Format ("[LobbyPlayerData: Id:{0} Character:{1} IsReady:{2}]", Id, Character, IsReady);
	}

	public LobbyPlayerData Set(int id, CharacterType character)
	{
		Id = id;
		Character = character;
		return this;
	}
}

