using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class MonsterAnimationController : CharacterAnimationController
{
    [SerializeField] 
    private GameObject _stickerAnimationPrefab;

    [SerializeField] 
    private CharacterType _character;
    
    [SerializeField] 
    private Vector3 _stickerAnimationOffset = new Vector3(0.33f, 0.155f, -0.0001f);
    
    [SerializeField] 
    private Vector3 _stickerAnimationLocalScale = new Vector3(0.6f, 0.6f, 1f);

    private StickerAnimation _stickerAnimation;

	public List<CharacterType> characterAnimatorLinks;
	public List<RuntimeAnimatorController> animatorCharacterLinks;
    
    protected override void Start()
    {
        base.Start();

        GameObject monsterSticker = Instantiate(_stickerAnimationPrefab);
        monsterSticker.transform.SetParent(transform);
        monsterSticker.transform.localPosition = _stickerAnimationOffset;
        monsterSticker.transform.localScale = _stickerAnimationLocalScale;

        _stickerAnimation = monsterSticker.GetComponent<StickerAnimation>();
    }

    protected override void OnStateChange(StateType oldState, StateType newState, params object[] args)
    {
        if (newState == StateType.Sticker && args.Length > 0)
        {
			UpdateDirection(Direction.EAST);
            var target = (HumanAnimationController) args[0];
            target.ApplySticker(_stickerAnimation);
            _stickerAnimation.PlayAnimation(_character);
        }
    }

	public void SetCharacter(CharacterType newType) {
		this._character = newType;
		gameObject.GetComponent<Animator> ().runtimeAnimatorController = animatorCharacterLinks [characterAnimatorLinks.IndexOf (newType)];
	}
}
