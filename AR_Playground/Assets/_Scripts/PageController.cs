using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour {

	public CubeManager manager;

	[Header("Page Textures")]
	public bool fetchPages = true;
	public string url = "https://tihlde-api.herokuapp.com/v1/items/";
	public Texture2D[] pages;

	[Header("GameObjects")]
	public Transform pageHolder;

	[Header("Attributes")]
	[Range(0.3f, 2f)]
	public float pageScale = 1f;

	int currentPage = 0;

	void Start() {
		InitializeActions();
		if(fetchPages) {
			Client.url = url;
			StartCoroutine(Client.FetchImages((Texture2D[] textures) => {
				pages = textures;
				foreach(Texture2D t in pages) {
					Debug.Log("Width: " + t.width + ", Height: " + t.height);
				}
				if(pages.Length > 0) {
					SetPage(0);
				}
			}));
		}
		else {
			SetPage(0);
		}
	}

	private void InitializeActions() {
		CubeManager.Left += () => {
			if(manager.sideCam == CubeManager.Cubes) {
				ChangeIndex(-1);
				SetPage(currentPage);
			}
		};

		CubeManager.Right += () => {
			if(manager.sideCam == CubeManager.Rocket) {
				ChangeIndex(1);
				SetPage(currentPage);
			}
		};
	}

	private void ChangeIndex(int value) {
		int newValue = currentPage + value;
		currentPage = (newValue >= pages.Length)? pages.Length - 1 : (newValue < 0)? 0 : newValue;
	}

	private void SetPage(int index) {
		pageHolder.GetComponent<Renderer>().material.mainTexture = pages[index];
		float ratio = (float)pages[index].height/(float)pages[index].width;
		float height = 1f*ratio; // Wanted width is 1
		pageHolder.localScale = new Vector3(1f, height, 1f)*pageScale;
	}
}
