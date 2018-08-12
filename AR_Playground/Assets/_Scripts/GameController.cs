using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[Header("GameObjects")]
	public GameObject molePrefab;
	public Transform[] spawnPoints;

	[Header("UI")]
	public Text scoreText;

	[Header("Colors")]
	public Color primaryColor;
	public Color secondaryColor;

	[Header("Attributes")]
	// Spawn Attributes
	[Range(1f, 10f)]
	public float minSpawnTime = 2f;
	[Range(1f, 10f)]
	public float maxSpawnTime = 4f;
	private float elapsedTime = 0;
	private float spawnRate = 2f;

	// Time attributes
	public float startTime = 10f;
	public float currentTime;

	[Header("Game Values")]
	private int score = 0;

	void Start() {
		InitializeSpawnPlates();
		currentTime = startTime;
	}

	void InitializeSpawnPlates() {
		foreach(Transform plate in spawnPoints) {
			Collision collision = plate.GetComponent<Collision>();
			collision.onEnter = (Collider col, GameObject obj) => {
				Color curColor = obj.GetComponent<Renderer>().material.color;
				obj.GetComponent<Renderer>().material.color = (curColor == primaryColor)? secondaryColor : primaryColor;
			};
		}
	}

	void Update() {
		// Spawn
		elapsedTime += Time.deltaTime;
		if(elapsedTime >= spawnRate) {
			elapsedTime = 0;
			spawnRate = Random.Range(minSpawnTime, maxSpawnTime);
			SpawnMole();
		}

		// Time
		currentTime -= Time.deltaTime;

		RenderScore();
	}

	void SpawnMole() {
		if(spawnPoints.Length == 0) {return;}

		int randomIndex = Random.Range(0, spawnPoints.Length);
		Vector3 spawnPos = spawnPoints[randomIndex].position;

		GameObject obj = GameObject.Instantiate(molePrefab);
		obj.transform.SetParent(spawnPoints[randomIndex]);
		obj.transform.localRotation = Quaternion.Euler(0f, 0f ,0f);
		obj.transform.localPosition = new Vector3(0f, obj.transform.localScale.y*0.5f, 0f);

		Collision collider = obj.GetComponent<Collision>();
		if(collider == null) {obj.AddComponent<Collision>();}
		collider.onEnter = (Collider col, GameObject o) => {
			GameObject.Destroy(o);
			score++;
		};
	}

	void RenderScore() {
		if(scoreText == null) {return;}
		scoreText.text = "Score: " + score;
	}

}