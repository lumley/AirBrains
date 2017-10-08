using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class LobbyPlayerView : MonoBehaviour 
{
	[SerializeField]
	private Image _portrait;

	[SerializeField]
	private Text _isReadyText;
	
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
		_portrait.sprite = _visualData.Portrait;
		_isReadyText.text = _model.IsReady ? "Ready" : "Not ready";
	}

	public int PlayerId
	{
		get 
		{
			return _model != null ? _model.Id : -1;
		}
	}
}
