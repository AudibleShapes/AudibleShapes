using UnityEngine;
using System.Collections;

public class CounterPlane : MonoBehaviour {

	public Material counter_1;
	public Material counter_2;
	public Material counter_3;
	public Material counter_4;


	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer>().material = counter_1;
	}

	public void setCounter1() {
		this.GetComponent<Renderer>().material = counter_1;
	}

	public void setCounter2() {
		this.GetComponent<Renderer>().material = counter_2;
	}

	public void setCounter3() {
		this.GetComponent<Renderer>().material = counter_3;
	}

	public void setCounter4() {
		this.GetComponent<Renderer>().material = counter_4;
	}


}
