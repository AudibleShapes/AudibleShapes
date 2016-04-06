using UnityEngine;
using System.Collections;

public class CenterGUIText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.GetComponent<GUIText>().pixelOffset = new Vector2(Screen.width/2,Screen.height/2);
	}
}
