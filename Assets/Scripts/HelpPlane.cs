using UnityEngine;
using System.Collections;

public class HelpPlane : MonoBehaviour {

	public Material questionDown;
	public Material questionUp;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void questionUpMat() {
		this.GetComponent<Renderer>().material = questionUp;
	}

	public void questionDownMat() {
		this.GetComponent<Renderer>().material = questionDown;
	}
}
