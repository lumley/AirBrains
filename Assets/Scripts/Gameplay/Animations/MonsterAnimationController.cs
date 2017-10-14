using UnityEngine;

public sealed class MonsterAnimationController : CharacterAnimationController
{
    [SerializeField] 
    private StickerAnimation _stickerAnimation;

    [SerializeField] 
    private CharacterType _character;

    private HumanAnimationController _activeTarget;

    public void ApplyCurrentTarget(HumanAnimationController human)
    {
        _activeTarget = human;
    }

    protected override void OnStateChange(StateType oldState, StateType newState)
    {
        if (newState == StateType.Sticker && _activeTarget != null)
        {
            _activeTarget.ApplySticker(_stickerAnimation);
            _stickerAnimation.PlayAnimation(_character);

            _activeTarget = null;
        }
    }
}
