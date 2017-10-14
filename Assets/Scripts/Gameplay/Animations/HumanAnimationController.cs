using UnityEngine;

public class HumanAnimationController : CharacterAnimationController
{
	[SerializeField]
	public SpriteRenderer _stickerSprite;

	private Vector3 _startSpriteLocalScale;

	private StickerAnimation _stickerAnimation;

	protected override void Start()
	{
		base.Start();

		_startSpriteLocalScale = _stickerSprite.gameObject.transform.localScale;
	}

	private void SetSticker(Sprite sprite)
	{
		_stickerSprite.sprite = sprite;

		ApplySticker(null);
	}

	public void ApplySticker(StickerAnimation sticker)
	{
		if (_stickerAnimation != null)
		{
			_stickerAnimation.OnSticker = null;
			_stickerAnimation = null;
		}

		_stickerAnimation = sticker;

		if (_stickerAnimation != null)
		{
			_stickerAnimation.OnSticker = SetSticker;
		}
	}

	public void OnCleanSticker()
	{
		SetSticker(null);
	}

	protected override void OnStateChange(StateType oldState, StateType newState, params object[] args)
	{
		
	}

	protected override void UpdateDirection(Direction direction)
	{
		base.UpdateDirection(direction);

		var newLocalScale = _startSpriteLocalScale;
		newLocalScale.x = newLocalScale.x * transform.localScale.x;
		_stickerSprite.transform.localScale = newLocalScale;
	}
}
