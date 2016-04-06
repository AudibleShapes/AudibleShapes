using UnityEngine;
using System;
using System.Collections;
using Leap;

public class TitleScreen : MonoBehaviour {

	Leap.Controller controller;
	Frame frame;
	HandList hands;

	public GameObject guiTextPrefab;
	GameObject go;

	public ChangeMaterialTest _cmt;
	public MenuMove _mm;
	public TempoText _tt;
	public ScaleNameText _snt;
	public FingerMovement _fm;
	public FingerMovementSize _fms;
	public FingerMovementRotation _fmr;
	public FingerTempo _ft;
	public TutorialPlane _tp;

	public float fadeTime;
	public float duration = 2.0f;
	private Color startColor;
	private Color endColor;
	private float timeLeft = 60.0f;

	// Use this for initialization
	void Start () {
		controller = new Controller();
		go = (GameObject) Instantiate(guiTextPrefab, new Vector3(0.25f, 0.8f, 0.0f), Quaternion.identity);
		startColor = go.GetComponent<GUIText>().color;
		endColor = startColor - new Color(0,0,0,1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		frame = controller.Frame();
		hands = frame.Hands;

		if (hands.Count != 0) {
			if (!_tp.isTutorialEnabled)
				_mm.moveDown();
			timeLeft = 60.0f;
			go.GetComponent<GUIText>().enabled = false;
			go.GetComponent<GUIText>().color = Color.Lerp(endColor, startColor, fadeTime);
			if (fadeTime < 1)
				fadeTime += Time.deltaTime/duration;
		} else if (hands.Count == 0) { //reset everything in the current scene except for saved values when retrieving save state
			timeLeft -= Time.deltaTime;
			if (timeLeft < 0) {
				float min = 10.0f;
				float max = 0.0f;
				_cmt.clearGrid();
				_mm.moveUp();
				Debug.Log("MENU HAS BEEN MOVED UP");
				_snt.pentatonicText();
				if (_tp.isTutorialEnabled)
					_tp.moveDown();

				_fm.resetDiffs(min, max);
				_fms.resetDiffs(min, max);
				_fmr.resetDiffs(min, max);
				_ft.resetDiffs(min, max);

				timeLeft = 60.0f;

				go.GetComponent<GUIText>().enabled = true;
				go.GetComponent<GUIText>().color = Color.Lerp(endColor, startColor, fadeTime);
				if (fadeTime < 1) {
					fadeTime += Time.deltaTime/duration;
				}
			}
		}
	}
}
