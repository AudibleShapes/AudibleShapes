using UnityEngine;
using System.Collections;

public class SavePlane : MonoBehaviour {

	public Material transparent;
	public Material save_mat;
	public ChangeMaterialTest _cmt;

	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer>().material = transparent;
	}
	
	// Update is called once per frame
	void Update () {
		if (_cmt.isSaved && _cmt.isGridCleared)
			this.GetComponent<Renderer>().material = save_mat;
		else 
			this.GetComponent<Renderer>().material = transparent;
	}
}
