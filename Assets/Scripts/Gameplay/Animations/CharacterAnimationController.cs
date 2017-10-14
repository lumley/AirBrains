using UnityEngine;

public enum StateType
{
	None = 0,
	Idle,
	Walk,
	Fight,
	Sticker
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class CharacterAnimationController : MonoBehaviour
{
	[SerializeField] private Animator _animator;
	
	[SerializeField]
	private Vector3 _tileSize = new Vector3(1f, 1f, 0f);
	
	[SerializeField]
	private Vector3 _reverseLocalScale = new Vector3(-1, 1, 1);
	
	[SerializeField]
	private Vector3 _normalLocalScale = new Vector3(1, 1, 1);
	 
	[SerializeField] 
	private Vector3 _moveOffset = new Vector3(0, 0, 0);
	
	private StateType _currentState = StateType.None;
	private Vector3 _endPosition;
	private bool _isMoving = false;
	
	protected virtual void Start()
	{
		if (_animator == null)
		{
			_animator = GetComponent<Animator>();
		}
		
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

	protected abstract void OnStateChange(StateType oldState, StateType newState, params object[] args);


	
	private void OnWalk(params object[] args)
	{
		if (args.Length > 0)
		{		
			Direction direction = (Direction)args[0];
			UpdateDirection(direction);
			
			switch (direction)
			{
				case Direction.SOUTH:
					_endPosition = gameObject.transform.position - new Vector3(0, _tileSize.y, 0) + _moveOffset;
					break;
				case Direction.WEST:
					_endPosition = gameObject.transform.position - new Vector3(_tileSize.x, 0, 0) + _moveOffset;
					break;
				case Direction.EAST:
					_endPosition = gameObject.transform.position + new Vector3(_tileSize.x, 0, 0) + _moveOffset;
					break;
				case Direction.NORTH:
					_endPosition = gameObject.transform.position + new Vector3(0, _tileSize.y, 0) + _moveOffset;
					break;
			}

			_endPosition = (Vector3) args[1] + _moveOffset;
			
			_isMoving = true;
		}
	}

	protected virtual void UpdateDirection(Direction direction)
	{
		switch (direction)
		{
			case Direction.WEST:
				gameObject.transform.localScale = _reverseLocalScale;
				break;
			case Direction.EAST:
				gameObject.transform.localScale = _normalLocalScale;
				break;
			default:
				break;
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
