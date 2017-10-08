using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class LobbyPlayerView : MonoBehaviour 
{
	[SerializeField]
	private Image _portrait;

	[SerializeField]
	private Text _playerIdText;

	[SerializeField]
	private Text _isReadyText;

	[SerializeField]
	private GameObject _isReadyContainer;

	[SerializeField]
	private Image _isReadyBackground;

	[SerializeField]
	private GameObject _emptySprite;
	
	private LobbyPlayerData _model;
	private CharacterVisualData _visualData;

	public void ApplyModel(LobbyPlayerData playerData, CharacterVisualData visualData)
	{
		_model = playerData;
		_visualData = visualData;

		RefreshView ();
	}

	private void RefreshView()
	{
		bool isEmpty = _model.Character == CharacterType.None;
		_emptySprite.SetActive (isEmpty);
		_portrait.sprite = isEmpty ? null :  _visualData.Portrait;
		_isReadyText.text = _model.IsReady ? "Ready" : "Waiting";
		_isReadyContainer.SetActive (!isEmpty);
		_playerIdText.text = string.Format ("PLAYER {0}", _model.Id + 1);

		_isReadyBackground.color = _model.IsReady ? Color.green : Color.red;
	}
}
