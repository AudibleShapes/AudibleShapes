using UnityEngine;
using System.Collections;

public class SelectShapePresetRight : MonoBehaviour {
	
	public Material cylinder_select;
	public Material tetra_select;
	public Material none_select;
	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer>().material = none_select;
	}
	
	public void setTetra() {
		this.GetComponent<Renderer>().material = tetra_select;
	}

	public void setCylinder() {
		this.GetComponent<Renderer>().material = cylinder_select;
	}

	public void setNoneRight() {
		this.GetComponent<Renderer>().material = none_select;
	}
}
