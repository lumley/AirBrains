using NDream.AirConsole;
using UnityEngine;
using UnityEngine.UI;

public sealed class LobbyPlayerView : MonoBehaviour
{
    [SerializeField] private Image _portrait;

    [SerializeField] private Text _playerIdText;

    [SerializeField] private Text _isReadyText;

    [SerializeField] private GameObject _isReadyContainer;

    [SerializeField] private Image _isReadyBackground;

    [SerializeField] private GameObject _emptySprite;

    private LobbyPlayerData _model;
    private CharacterVisualData _visualData;

    public void ApplyModel(LobbyPlayerData playerData, CharacterVisualData visualData)
    {
        _model = playerData;
        _visualData = visualData;

        RefreshView();
    }

    private void RefreshView()
    {
        bool isEmpty = _model.Character == CharacterType.None;
        _emptySprite.SetActive(isEmpty);
        _portrait.sprite = isEmpty ? null : _visualData.Portrait;
        _isReadyText.text = _model.IsReady ? "Ready" : "Waiting";
        _isReadyContainer.SetActive(!isEmpty);
#if !DISABLE_AIRCONSOLE
        string nickname = null;
        if (_model.Id != 0)
        {
            nickname = AirConsole.instance.GetNickname(_model.Id);
        }

        _playerIdText.text = string.Format("{0}", nickname ?? "EMPTY");
#else
		_playerIdText.text = string.Format ("PLAYER {0}", _model.Id + 1);
#endif

        _isReadyBackground.color = _model.IsReady ? Color.green : Color.red;
    }
}