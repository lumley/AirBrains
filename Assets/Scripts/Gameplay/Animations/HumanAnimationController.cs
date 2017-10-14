﻿using UnityEngine;

public class HumanAnimationController : CharacterAnimationController
{
	[SerializeField]
	public SpriteRenderer _stickerSprite;

	private StickerAnimation _stickerAnimation;

	private void SetSticker(Sprite sprite)
	{
		_stickerSprite.sprite = sprite;

		ApplySticker(null);
	}

	public void ApplySticker(StickerAnimation sticker)
	{
		if (_stickerAnimation != null)
		{
			_stickerAnimation.OnStart = null;
			_stickerAnimation.OnSticker = null;
			_stickerAnimation = null;
		}

		_stickerAnimation = sticker;

		if (_stickerAnimation != null)
		{
			_stickerAnimation.OnStart = OnCleanSticker;
			_stickerAnimation.OnSticker = SetSticker;
		}
		else
		{
			OnCleanSticker();
		}
	}

	private void OnCleanSticker()
	{
		//SetSticker(null);
	}
}
