using UnityEngine;

public enum StateType
{
	None = 0,
	Idle,
	Walk,
	Fight,
	Sticker
}

public class CharacterAnimationController : MonoBehaviour
{
	[SerializeField] private Animator _animator;

	private StateType _currentState = StateType.None;
	
	private void Start()
	{
		ApplyState(StateType.Idle);
	}

	public void ApplyState(StateType state)
	{
		if (_currentState != state)
		{
			OnStateChange(_currentState, state);
			_currentState = state;
			_animator.Play(_currentState.ToString().ToLower(), -1, 0f);
		}
	}

	protected virtual void OnStateChange(StateType oldState, StateType newState)
	{
		
	}

	//called from animator
	private void OnAnimationFinished()
	{
		if (_currentState != StateType.Idle)
		{
			ApplyState(StateType.Idle);
		}
	}
}
