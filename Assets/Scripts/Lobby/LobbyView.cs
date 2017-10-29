using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyView : MonoBehaviour
{
    [SerializeField] private CharacterVisualConfig Config;

    [SerializeField] private GameObject _playerPortraitPrefab;

    [SerializeField] private Transform _portraitGrid;

    [SerializeField] private GameObject _readyToStartGame;

    private List<LobbyPlayerView> _players = new List<LobbyPlayerView>();

    public LobbyModel Model;

    private Coroutine _startGameCoroutine;

    public void ApplyModel(LobbyModel model)
    {
        if (Model != null)
        {
            Model.OnChanged -= OnModelChanged;
            Model = null;

            RemoveCoroutine();
        }

        Model = model;

        if (Model != null)
        {
            Model.OnChanged += OnModelChanged;
            OnModelChanged();
        }
    }

    private void OnDestoy()
    {
        ApplyModel(null);
    }

    private void RemoveCoroutine()
    {
        if (_startGameCoroutine != null)
        {
            if (_readyToStartGame != null)
            {
                _readyToStartGame.SetActive(false);
            }
            StopCoroutine(_startGameCoroutine);
            _startGameCoroutine = null;
        }
    }

    private IEnumerator StartGameCoroutine()
    {
        while (true)
        {
            _readyToStartGame.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _readyToStartGame.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnModelChanged()
    {
        //refresh player portraits and state if smth changed
        for (int i = 0; i < Model.Players.Count; i++)
        {
            var playerModel = Model.Players[i];
            var player = GetPlayer(i);
            var configuration = playerModel.Character != CharacterType.None
                ? Config.GetConfiguration(playerModel.Character)
                : null;

            player.ApplyModel(playerModel, configuration);
        }

        //remove unused player portraits if disconnected
        if (Model.Players.Count < _players.Count)
        {
            for (int i = Model.Players.Count; i < _players.Count; i++)
            {
                Destroy(_players[i].gameObject);
            }

            _players.RemoveRange(Model.Players.Count, _players.Count - Model.Players.Count);
        }

        if (Model.IsGameReadyToStart)
        {
            if (_startGameCoroutine == null)
            {
                _startGameCoroutine = StartCoroutine("StartGameCoroutine");
            }
        }
        else
        {
            RemoveCoroutine();
        }
    }

    private LobbyPlayerView GetPlayer(int index)
    {
        LobbyPlayerView player = _players.Count > index ? _players[index] : null;
        if (player == null)
        {
            var playerObject = Instantiate(_playerPortraitPrefab);
            playerObject.transform.SetParent(_portraitGrid);
            player = playerObject.GetComponent<LobbyPlayerView>();

            _players.Add(player);
        }

        return player;
    }
}