using UnityEngine;
using System.Collections;

public class MainMenuPanel : MonoBehaviour {
	public Material menu_dis;
	public Material menu_en;
	private readonly float MAX_HEIGHT = 5.94f;
	private readonly float MIN_HEIGHT = 4.76f;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3(transform.position.x, MAX_HEIGHT, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (MenuMove.isMenuEnabled)
			GetComponent<Renderer>().material = menu_en;
		else
			GetComponent<Renderer>().material = menu_dis;
	}

	public void moveDown() {
		if(MenuMove.isMenuEnabled)
			transform.position = new Vector3(transform.position.x, MIN_HEIGHT, transform.position.z);
	}

	public void moveUp() {
		if (!MenuMove.isMenuEnabled)
			transform.position = new Vector3(transform.position.x, MAX_HEIGHT, transform.position.z);
	}
}
