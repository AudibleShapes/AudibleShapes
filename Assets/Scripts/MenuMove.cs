using UnityEngine;
using System.Collections;
using Leap;

public class MenuMove : MonoBehaviour {

	public MainMenuPanel _mmp;
	public ScaleNameText _snt;
	public ChangeMaterialTest _cmt;

	private readonly float MAX_HEIGHT = 6.67f;
	private readonly float MIN_HEIGHT = 5.04f;

	public Material mat_pos;
	public Material mat_sca;
	public Material mat_rot;
	public Material mat_tempo;

	public Material mat_pos_def;
	public Material mat_sca_def;
	public Material mat_rot_def;
	public Material mat_tempo_def;

	public static bool isMenuEnabled = false;
	public static bool isPositionMode = false;
	public static bool isScaleMode = false;
	public static bool isRotateMode = false;
	public static bool isTempoMode = false;

	private static int scaleCount = -1;
	private static int saved_scaleCount = -1;
	private static readonly int PENTATONIC_COUNT = 3;
	private static readonly int MAJOR_COUNT = 0;
	private static readonly int MINOR_COUNT = 1;
	private static readonly int WHOLE_COUNT = 2;
	private static float timer = 1.0f;
	private bool isSelectedItem = false;

	private static readonly string SCALE_NAME = "Scale";
	private static readonly string POSITION_NAME = "Position";
	private static readonly string SCALING_NAME = "Scaling";
	private static readonly string ROTATION_NAME = "Rotation";
	private static readonly string TEMPO_NAME = "Tempo";

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform)
			child.position = new Vector3(child.position.x, MAX_HEIGHT, child.position.z);
		OSCHandler.Instance.SendMessageToClient("MaxMSP", "/musicScale", PENTATONIC_COUNT);
	}
	
	// Update is called once per frame
	void Update () {
//		foreach (Transform child in transform)
//			Debug.Log(child.transform.position);
		if (isSelectedItem) {
			timer -= Time.deltaTime;
			if (timer < 0.0f) {
				timer = 1.0f;
				isSelectedItem = false;
			}
		}
	}

	public void moveUp() {
		int count = transform.childCount;
		for (int i = 0; i < count; i++)
			if (transform.GetChild(i).transform.position.y != MAX_HEIGHT)
				transform.GetChild(i).transform.position = new Vector3(transform.GetChild(i).transform.position.x,
					MAX_HEIGHT, transform.GetChild(i).transform.position.z);
		isMenuEnabled = false;
		_mmp.moveUp();
	}

	public void moveDown() {
		int count = transform.childCount;
		for (int i = 0; i < count; i++)
			if (transform.GetChild(i).transform.position.y != MIN_HEIGHT)
				transform.GetChild(i).transform.position = new Vector3(transform.GetChild(i).transform.position.x,
					MIN_HEIGHT, transform.GetChild(i).transform.position.z);
		isMenuEnabled = true;
		_mmp.moveDown();
	}

	public bool triggerPoke(float x, float y, float z) {
		int count = transform.childCount;
		for (int i = 0; i < count; i++) { //each coordinate of the pinch is matched to each coordinate of each grid space; bounds.size gets the actual size of each grid space as to get its edges when pinching
			if (x >= (transform.GetChild(i).transform.position.x - (transform.GetChild(i).GetComponent<Renderer>().bounds.size.x/2)) && x < (transform.GetChild(i).transform.position.x + (transform.GetChild(i).GetComponent<Renderer>().bounds.size.x/2))) {
				if (y >= (transform.GetChild(i).transform.position.y - (transform.GetChild(i).GetComponent<Renderer>().bounds.size.y/2)) && y < (transform.GetChild(i).transform.position.y + (transform.GetChild(i).GetComponent<Renderer>().bounds.size.y/2))) {
					if (z >= (transform.GetChild(i).transform.position.z - (transform.GetChild(i).GetComponent<Renderer>().bounds.size.z/2)) && z < (transform.GetChild(i).transform.position.z + 3f)) {
						return true;
					}
				}
			}
		}

		return false;
	}

	public void changeMaterial(float x, float y, float z) {
		if (!isSelectedItem) {
			isSelectedItem = true;
			int count = transform.childCount;
			for (int i = 0; i < count; i++) { //each coordinate of the poke is matched to each coordinate of each grid space; bounds.size gets the actual size of each grid space as to get its edges when pinching
				if (x >= (transform.GetChild (i).transform.position.x - (transform.GetChild (i).GetComponent<Renderer> ().bounds.size.x / 2)) && x < (transform.GetChild (i).transform.position.x + (transform.GetChild (i).GetComponent<Renderer> ().bounds.size.x / 2))) {
					if (y >= (transform.GetChild (i).transform.position.y - (transform.GetChild (i).GetComponent<Renderer> ().bounds.size.y / 2)) && y < (transform.GetChild (i).transform.position.y + (transform.GetChild (i).GetComponent<Renderer> ().bounds.size.y / 2))) {
						if (z >= (transform.GetChild (i).transform.position.z - (transform.GetChild (i).GetComponent<Renderer> ().bounds.size.z / 2)) && z < (transform.GetChild (i).transform.position.z + 3f)) {
							if (transform.GetChild (i).name.Equals (SCALE_NAME)) {
								scaleCount++;
								if (scaleCount == MAJOR_COUNT) { //scaleCount is 0
									_snt.majorText ();
									OSCHandler.Instance.SendMessageToClient ("MaxMSP", "/musicScale", scaleCount);
								} else if (scaleCount == MINOR_COUNT) { //scaleCount is 1
									_snt.minorText ();
									OSCHandler.Instance.SendMessageToClient ("MaxMSP", "/musicScale", scaleCount);
								} else if (scaleCount == WHOLE_COUNT) { //scaleCount is 2
									_snt.wholeText ();
									OSCHandler.Instance.SendMessageToClient ("MaxMSP", "/musicScale", scaleCount);
								} else if (scaleCount == PENTATONIC_COUNT) { //scaleCount is 3
									_snt.pentatonicText ();
									OSCHandler.Instance.SendMessageToClient ("MaxMSP", "/musicScale", scaleCount);
									scaleCount = -1;
								}

								saved_scaleCount = scaleCount;
							} else if (transform.GetChild (i).name.Equals (POSITION_NAME)) {
								if (!isPositionMode) {
									transform.GetChild (i).transform.GetComponent<Renderer> ().material = mat_pos;
									isPositionMode = true;
									turnOffOtherMode (SCALING_NAME);
									turnOffOtherMode (ROTATION_NAME);
									turnOffOtherMode (TEMPO_NAME);
								} else if (isPositionMode) {
									transform.GetChild (i).transform.GetComponent<Renderer> ().material = mat_pos_def;
									isPositionMode = false;
								}
							} else if (transform.GetChild (i).name.Equals (SCALING_NAME)) {
								if (!isScaleMode) {
									transform.GetChild (i).transform.GetComponent<Renderer> ().material = mat_sca;
									isScaleMode = true;
									turnOffOtherMode (POSITION_NAME);
									turnOffOtherMode (ROTATION_NAME);
									turnOffOtherMode (TEMPO_NAME);
								} else if (isScaleMode) {
									transform.GetChild (i).transform.GetComponent<Renderer> ().material = mat_sca_def;
									isScaleMode = false;
								}
							} else if (transform.GetChild (i).name.Equals (ROTATION_NAME)) {
								if (!isRotateMode) {
									transform.GetChild (i).transform.GetComponent<Renderer> ().material = mat_rot;
									isRotateMode = true;
									turnOffOtherMode (POSITION_NAME);
									turnOffOtherMode (SCALING_NAME);
									turnOffOtherMode (TEMPO_NAME);
								} else if (isRotateMode) {
									transform.GetChild (i).transform.GetComponent<Renderer> ().material = mat_rot_def;
									isRotateMode = false;
								}
							} else if (transform.GetChild (i).name.Equals (TEMPO_NAME)) {
								if (!isTempoMode) {
									transform.GetChild (i).transform.GetComponent<Renderer> ().material = mat_tempo;
									isTempoMode = true;
									turnOffOtherMode (POSITION_NAME);
									turnOffOtherMode (SCALING_NAME);
									turnOffOtherMode (ROTATION_NAME);
								} else if (isTempoMode) {
									transform.GetChild (i).transform.GetComponent<Renderer> ().material = mat_tempo_def;
									isTempoMode = false;
								}
							}
						}
					}
				}
			}
		}
	}

	public void turnOffOtherMode(string m) {
		int count = transform.childCount;
		for (int i = 0; i < count; i++) {
			if (transform.GetChild(i).name.Equals(m)) {
				if (m.Equals(POSITION_NAME) && isPositionMode) {
					transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_pos_def;
					isPositionMode = false;
				}

				if (m.Equals(SCALING_NAME) && isScaleMode) {
					transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_sca_def;
					isScaleMode = false;
				}

				if (m.Equals(ROTATION_NAME) && isRotateMode) {
					transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_rot_def;
					isRotateMode = false;
				}

				if (m.Equals(TEMPO_NAME) && isTempoMode) {
					transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_tempo_def;
					isTempoMode = false;
				}
			}
		}
	}


	private void enableMode(string m) {
		int count = transform.childCount;
		for (int i = 0; i < count; i++) {
			if (transform.GetChild(i).name.Equals(m)) {

				if (m.Equals(POSITION_NAME))
					transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_pos_def;

				if (m.Equals(SCALING_NAME))
					transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_sca_def;

				if (m.Equals(ROTATION_NAME))
					transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_rot_def;

				if (m.Equals(TEMPO_NAME))
					transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_tempo_def;
			}
		}
	}

	public void disableTempoChange() {
		int count = transform.childCount;
		for (int i = 0; i < count; i++) {
			if (transform.GetChild(i).name.Equals(TEMPO_NAME)) {
				transform.GetChild(i).transform.GetComponent<Renderer>().material = mat_tempo_def;
				isTempoMode = false;
			}
		}
	}

	public void resetScale() {
		scaleCount = -1;
		OSCHandler.Instance.SendMessageToClient("MaxMSP", "/musicScale", PENTATONIC_COUNT);
		_snt.pentatonicText();
	}

	public void resetSavedScale() {
		resetScale();
		saved_scaleCount = scaleCount;
	}
		
	public void getSavedScale() {
		if (saved_scaleCount == MAJOR_COUNT) {
			OSCHandler.Instance.SendMessageToClient("MaxMSP", "/musicScale", MAJOR_COUNT);
			_snt.majorText();
		}
		else if (saved_scaleCount == MINOR_COUNT) {
			OSCHandler.Instance.SendMessageToClient("MaxMSP", "/musicScale", MINOR_COUNT);
			_snt.minorText();
		}
		else if (saved_scaleCount == WHOLE_COUNT) {
			OSCHandler.Instance.SendMessageToClient("MaxMSP", "/musicScale", WHOLE_COUNT);
			_snt.wholeText();
		}
		else {
			OSCHandler.Instance.SendMessageToClient("MaxMSP", "/musicScale", PENTATONIC_COUNT);
			_snt.pentatonicText();
		}

		scaleCount = saved_scaleCount;
	}
}
