using NDream.AirConsole;
using UnityEngine;
using UnityEngine.UI;

public sealed class LobbyPlayerView : MonoBehaviour
{
    [SerializeField] private Image _portrait;

    [SerializeField] private Text _playerIdText;
    [SerializeField] private Image _playerIdTextBackground;

    [SerializeField] private Text _isReadyText;

    [SerializeField] private GameObject _isReadyContainer;

    [SerializeField] private Image _isReadyBackground;

    [SerializeField] private GameObject _emptySprite;

    [SerializeField] private Color _readyColor;
    [SerializeField] private Color _notReadyColor;
    [SerializeField] private Color _defaultTextBackgroundColor;

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
        _playerIdText.color = isEmpty ? Color.black : _visualData.TextColor;
        _playerIdTextBackground.color = isEmpty ? _defaultTextBackgroundColor : _visualData.TextBackgroundColor;
        
        //_isReadyText.text = _model.IsReady ? "Ready" : "Waiting";
        // _isReadyContainer.SetActive(!isEmpty);
        //_isReadyBackground.color = _model.IsReady ? _readyColor : _notReadyColor;
#if !DISABLE_AIRCONSOLE
        string nickname = null;
        if (_model.Id != 0)
        {
            nickname = AirConsole.instance.GetNickname(_model.Id);
        }

        _playerIdText.text = string.Format("{0}", nickname ?? string.Empty);
#else
		_playerIdText.text = string.Format ("PLAYER {0}", _model.Id + 1);
#endif
    }
}