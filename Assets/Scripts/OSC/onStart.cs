using UnityEngine;
using System.Collections;

public class onStart : MonoBehaviour {
    

    int clear = 0;
	// Use this for initialization
	void Start () {

        clear = 1;
        OSCHandler.Instance.SendMessageToClient("MaxMSP", "/clear", clear);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
