using UnityEngine;
using System.Collections;

public class SelectPresetShapeLeft : MonoBehaviour {

	public Material cube_select;
	public Material sphere_select;
	public Material none_select;

	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer>().material = cube_select;
	}

	public void setSphere() {
		this.GetComponent<Renderer>().material = sphere_select;
	}

	public void setCube() {
		this.GetComponent<Renderer>().material = cube_select;
	}

	public void setNoneLeft() {
		this.GetComponent<Renderer>().material = none_select;
	}
}
