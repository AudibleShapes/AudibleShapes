using UnityEngine;
using System.Collections;

public class TempoText : MonoBehaviour {

	public void changeTempoText(string tempo) {
		GetComponent<TextMesh>().text = tempo;
	}

	public void defaultTempo(int m) {
		GetComponent<TextMesh>().text = m.ToString();
	}
}