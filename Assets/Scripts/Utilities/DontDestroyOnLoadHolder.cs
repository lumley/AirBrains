using UnityEngine;

namespace Utilities
{
	public class DontDestroyOnLoadHolder : MonoBehaviour
	{
		[SerializeField] private GameObject[] _doNotDestroyOnLoad;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			for (var i = 0; i < _doNotDestroyOnLoad.Length; i++)
			{
				var target = _doNotDestroyOnLoad[i];
				DontDestroyOnLoad(target);
			}
		}
	}
}
