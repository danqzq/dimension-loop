using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace DimensionLoop.Level1
{
	public class Menu : MonoBehaviour
	{
		[FormerlySerializedAs("PressSpace")] [SerializeField] private GameObject _pressSpace;
		
		private static readonly int StartAnimHash = Animator.StringToHash("start");

		private IEnumerator Start()
		{
			var anim = GetComponent<Animator>();
			var audioSource = FindObjectOfType<Canvas>().GetComponent<AudioSource>();
			
			yield return new WaitUntil(() => Input.GetKey(KeyCode.Space));
			
			audioSource.Play();
			_pressSpace.SetActive(false);
			anim.SetTrigger(StartAnimHash);
			yield return new WaitForSeconds(2.5f);
			anim.enabled = false;
		}
	}
}
