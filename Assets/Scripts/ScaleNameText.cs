using UnityEngine;
using System.Collections;

public class ScaleNameText : MonoBehaviour {

	private static readonly string PENTATONIC = "Pentatonic";
	private static readonly string MAJOR = "Major";
	private static readonly string MINOR = "Minor";
	private static readonly string WHOLE = "Whole";

	public void majorText() {
		GetComponent<TextMesh>().text = MAJOR;
	}

	public void minorText() {
		GetComponent<TextMesh>().text = MINOR;
	}

	public void wholeText() {
		GetComponent<TextMesh>().text = WHOLE;
	}

	public void pentatonicText() {
		GetComponent<TextMesh>().text = PENTATONIC;
	}

	public string getText() {
		return GetComponent<TextMesh>().text;
	}
}
