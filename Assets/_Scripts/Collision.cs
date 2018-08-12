using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {

	public delegate void Callback(Collider col, GameObject g);
	public Callback onEnter;

	void OnTriggerEnter(Collider col)
	{
		if(onEnter != null) {
			onEnter(col, this.gameObject);
		}
	}
}
