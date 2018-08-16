using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[Header("Cube")]
	public CubeManager manager;

	[Header("GameObjects")]
	public GameObject molePrefab;
	public Transform[] spawnPoints;
	private List<GameObject> moles = new List<GameObject>();

	[Header("UI")]
	public Text scoreText;
	public Text timeText;
	public Text gameOverText;
	public Image timeFill;

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
	public float startTime = 100f;
	private float currentTime;

	// Game attributes
	[Header("Game Attributes")
	private bool isGameOver = false;
	private bool isResetting = false;
	private float elapsedResetTime = 0f;
	private const float resetTime = 4f;

	[Header("Game Values")]
	private int score = 0;

	void Start() {
		InitializeSpawnPlates();
		ResetGame();
		InitializeCube();
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

	void InitializeCube() {
		if(manager == null) {
			return;
		}

		CubeManager.OnFound += () => {
			isResetting =(manager.sideCam == CubeManager.MagicCube);
			if(!isResetting) {
				elapsedResetTime = 0f;
			}
		};

		CubeManager.OnTurn += () => {
			isResetting = manager.sideCam == CubeManager.MagicCube;
			if(!isResetting) {
				elapsedResetTime = 0f;
			}
		};

		CubeManager.OnLost += () => {
			isResetting = false;
			elapsedResetTime = 0f;
		};
	}

	void Update() {
		if(isGameOver) {
			if(isResetting) {
				elapsedResetTime += Time.deltaTime;
				if(elapsedResetTime >= resetTime) {
					elapsedResetTime = 0f;
					ResetGame();
				}
			}
		} else {

			// Spawn
			elapsedTime += Time.deltaTime;
			if(elapsedTime >= spawnRate) {
				elapsedTime = 0;
				spawnRate = Random.Range(minSpawnTime, maxSpawnTime);
				SpawnMole();
			}

			// Time
			currentTime -= Time.deltaTime;
			if(currentTime <= 0) {
				InitializeGameOver();
			}

			// Render
			RenderScore();
		}
		RenderBar();
	}

	void SpawnMole() {
		if(spawnPoints.Length == 0) {return;}

		int randomIndex = Random.Range(0, spawnPoints.Length);
		Vector3 spawnPos = spawnPoints[randomIndex].position;

		GameObject obj = GameObject.Instantiate(molePrefab);
		obj.transform.SetParent(spawnPoints[randomIndex]);
		obj.transform.localRotation = Quaternion.Euler(0f, 0f ,0f);
		obj.transform.localPosition = new Vector3(0f, obj.transform.localScale.y*0.5f, 0f);
		moles.Add(obj);

		Collision collider = obj.GetComponent<Collision>();
		if(collider == null) {obj.AddComponent<Collision>();}
		collider.onEnter = (Collider col, GameObject o) => {
			moles.Remove(o);
			GameObject.Destroy(o);
			score++;
		};

		MoleController controller = obj.GetComponent<MoleController>();
		if(controller != null) {
			controller.SetOnDeath((GameObject mole) => {
				moles.Remove(mole);
			});
		}
	}

	void RenderScore() {
		if(scoreText == null) {return;}
		scoreText.text = "Score: " + score;
	}

	void RenderBar() {
		if(timeFill == null) {return;}

		if(!isGameOver) {
			timeFill.fillAmount = currentTime/startTime;
			timeText.text = ((currentTime/60)).ToString("00") + ":" + (currentTime%60).ToString("00");
		} else {
			Debug.Log("Elapsed: " + elapsedResetTime/resetTime);
			timeFill.fillAmount = elapsedResetTime/resetTime;
		}
	}

	void ResetGame() {
		currentTime = startTime;
		gameOverText.gameObject.SetActive(false);
		isGameOver = false;
		isResetting = false;
		timeFill.color = secondaryColor;
		score = 0;
	}

	void InitializeGameOver() {
		isGameOver = true;
		isResetting = false;
		gameOverText.gameObject.SetActive(true);
		timeFill.color = primaryColor;
		ClearMoles();
	}

	void ClearMoles() {
		foreach(GameObject mole in moles) {
			if(mole != null) {
				GameObject.Destroy(mole);
			}
		}
		moles.Clear();
	}

}