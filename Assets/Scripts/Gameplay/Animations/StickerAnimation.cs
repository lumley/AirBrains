using System;
using UnityEngine;

public class StickerAnimation : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;
	
	public Action OnStart;
	public Action<Sprite> OnSticker;

	public void PlayAnimation(CharacterType character)
	{
		_animator.Play(character.ToString().ToLower(), -1, 0f);
	}
	
	private void OnStartPlay()
	{
		if (OnStart != null)
		{
			OnStart();
		}
	}
	
	private void OnShowSticker(UnityEngine.Object stickerSprite)
	{
		if (OnSticker != null)
		{
			OnSticker(stickerSprite as Sprite);
		}
	}
}
