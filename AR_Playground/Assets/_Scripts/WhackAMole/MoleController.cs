using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleController : MonoBehaviour {

	public float lifeTime = 5f;
	public delegate void Callback(GameObject obj);
	private Callback onDeath = null;

	// Use this for initialization
	void Start () {
		Animator anim = GetComponent<Animator>();
		if(anim == null) {return;}

		anim.SetTrigger("Up");
		StartCoroutine(KillMole());
	}

	public void SetOnDeath(Callback callback) {
		onDeath = callback;
	}
	
	IEnumerator KillMole() {
		yield return new WaitForSeconds(lifeTime);

		Animator anim = GetComponent<Animator>();
		anim.SetTrigger("Down");

		if(onDeath != null) {
			onDeath(this.gameObject);
		}
		GameObject.Destroy(this.gameObject, 1.5f);
	}
}
