using UnityEngine;

public sealed class MonsterAnimationController : CharacterAnimationController
{
    [SerializeField] 
    private StickerAnimation _stickerAnimation;

    [SerializeField] 
    private CharacterType _character;

    protected override void OnStateChange(StateType oldState, StateType newState, params object[] args)
    {
        if (newState == StateType.Sticker && args.Length > 0)
        {
            var target = (HumanAnimationController) args[0];
            
            target.ApplySticker(_stickerAnimation);
            _stickerAnimation.PlayAnimation(_character);
        }
    }
}
