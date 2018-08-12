using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {

	public CubeManager manager;

	void Start() {

		CubeManager.OnTurn += () => {
			if(manager.sideCam == CubeManager.MagicCube) {
				// Do something
			}
		};
		
	}

	void Update() {
		
	}
}
