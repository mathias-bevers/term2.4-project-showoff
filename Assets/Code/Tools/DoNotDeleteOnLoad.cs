using UnityEngine;

namespace Code.Tools
{
	public class DoNotDeleteOnLoad : MonoBehaviour
	{
		private void Awake() { DontDestroyOnLoad(gameObject); }
	}
}