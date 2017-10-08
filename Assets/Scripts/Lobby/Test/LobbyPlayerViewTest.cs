using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyPlayerViewTest : MonoBehaviour 
{
	[SerializeField]
	private Image _portrait;

	[SerializeField]
	private Button _connectButton;

	[SerializeField]
	private Button _disconnectButton;

	[SerializeField]
	private Toggle _isReadyToggle;

	private bool _isAvailable = true;
	private bool _isReady = false;

	public Action<LobbyPlayerViewTest> OnConnected;
	public Action<LobbyPlayerViewTest> OnDisconnected;
	public Action<LobbyPlayerViewTest> OnReadyStateChanged;

	private void Awake()
	{
		_connectButton.onClick.AddListener (OnConnectClick);
		_disconnectButton.onClick.AddListener (OnDisconnectClick);
		_isReadyToggle.onValueChanged.AddListener (OnReadyStateCnaged);
	}

	private void OnConnectClick()
	{
		if (OnConnected != null) {
			OnConnected (this);
		}
	}

	private void OnDisconnectClick()
	{
		if (OnDisconnected != null) {
			OnDisconnected (this);
		}
	}

	private void OnReadyStateCnaged(bool state)
	{
		_isReady = state;
		if (OnReadyStateChanged != null) {
			OnReadyStateChanged (this);
		}
	}

	private void RefreshView()
	{
		_isReadyToggle.isOn = _isReady;
		_connectButton.gameObject.SetActive(_isAvailable);
		_disconnectButton.gameObject.SetActive(!_isAvailable);

		_isReadyToggle.gameObject.SetActive(!_isAvailable);
	}

	public void ApplyData(CharacterVisualData data, int playerId)
	{
		_portrait.sprite = data.Portrait;
		Character = data.Character;
		PlayerId = playerId;
	}

	public bool IsAvailable
	{
		set {
			if (_isAvailable != value) {
				_isAvailable = value;

				if (!_isAvailable) {
					_isReady = false;
				}
				RefreshView ();
			}
		}
	}

	public bool IsReady
	{
		set {
			if (_isReady != value) {
				_isReady = value;
				RefreshView ();
			}
		}
		get {
			return _isReady;
		}
	}

	public CharacterType Character {
		private set;
		get;
	}

	public int PlayerId {
		private set;
		get;
	}
}
