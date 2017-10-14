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
	
	private Vector3 _tileSize = new Vector3(0.2f, 0.2f, 0f);
	
	private void Start()
	{
		ApplyState(StateType.Idle);
	}

	public void ApplyState(StateType state, params object[] args)
	{
		if (_currentState != state)
		{
			OnStateChange(_currentState, state, args);
			_currentState = state;
			_animator.Play(_currentState.ToString().ToLower(), -1, 0f);

			switch (_currentState)
			{
				case StateType.Fight:
				case StateType.Idle:
				case StateType.Sticker:
					break;
				case StateType.Walk:
					OnWalk(args);
					break;
				default:
					break;
			}
		}
	}

	protected virtual void OnStateChange(StateType oldState, StateType newState, params object[] args)
	{
		
	}

	private Vector3 _endPosition;
	private bool _isMoving = false;
	
	private Vector3 _reverseLocalScale = new Vector3(-1, 1, 1);
	private Vector3 _normalLocalScale = new Vector3(1, 1, 1);
	
	private void OnWalk(params object[] args)
	{
		if (args.Length > 0)
		{		
			MoveDirection direction = (MoveDirection)args[0];

			switch (direction)
			{
				case MoveDirection.Down:
					_endPosition = gameObject.transform.position - new Vector3(0, _tileSize.y, 0);
					break;
				case MoveDirection.Left:
					gameObject.transform.localScale = _reverseLocalScale;
					_endPosition = gameObject.transform.position - new Vector3(_tileSize.x, 0, 0);
					break;
				case MoveDirection.Right:
					gameObject.transform.localScale = _normalLocalScale;
					_endPosition = gameObject.transform.position + new Vector3(_tileSize.x, 0, 0);
					break;
				case MoveDirection.Top:
					_endPosition = gameObject.transform.position + new Vector3(0, _tileSize.y, 0);
					break;
			}
			
			_isMoving = true;
		}
	}

	private void Update()
	{
		if (_isMoving)
		{
			if (Vector3.Distance(transform.position, _endPosition) > 0.001f)
			{
				transform.position = Vector3.MoveTowards(transform.position, _endPosition, Time.deltaTime);
			}
			else
			{
				transform.position = _endPosition;
				_isMoving = false;
				
				ApplyState(StateType.Idle);
			}
		}
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
