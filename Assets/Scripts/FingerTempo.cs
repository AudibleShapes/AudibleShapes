using UnityEngine;
using System.Collections;
using Leap;

public class FingerTempo : MonoBehaviour {
	public ChangeMaterialTest _cmt;
	public TempoText _tt;
	public MenuMove _mm;
	private float max_z = 0.0f; //used to establish max z between z coordinates of pinky finger and palm
	private float min_z = 10.0f; //used to establish min z between z coordinates of pinky finger and palm
	public static float mapped = 120.0f;
	private static int saved_mapped = 120;
	private float timeLeft = 10.0f;
	private readonly float MAX_Z_POSS = 2f;
	private readonly float MIN_Z_POSS = -2f;


	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		HandModel hand_model = GetComponent<HandModel>();
		Hand leap_hand = hand_model.GetLeapHand();

		if (leap_hand == null)
			return;

		if (MenuMove.isTempoMode) { //right hand rotates cube
			if (leap_hand.IsRight) {
				timeLeft -= Time.deltaTime;

				Vector3 position = hand_model.fingers[2].GetTipPosition();
				Vector3 index = hand_model.fingers[1].GetTipPosition();
				Vector3 palm = hand_model.palm.position;
				float y = (position.y + index.y)/2 - palm.y;

				//Debug.Log("position.y = " + position.y + "\tindex.y = " + index.y);
				//Debug.Log("\tpalm.y = " + palm.y + "\ty = " + y);

				if (y > max_z && y < MAX_Z_POSS)
					max_z = y;

				if (y < min_z && y > MIN_Z_POSS)
					min_z = y;

				//Debug.Log("RANGE : " + y + "\t" + min_z + "\t" + max_z);

				//map z from range of min_z - max_z to new range of Z_MIN and Z_MAX from ChangeMaterialTest script
				//then, rotate blocks in grid based on this mapped value
				mapped = mapZ(y, min_z, max_z, ChangeMaterialTest.TEMPO_MIN, ChangeMaterialTest.TEMPO_MAX);
				if (mapped >= ChangeMaterialTest.TEMPO_MIN && mapped < ChangeMaterialTest.TEMPO_MAX) {
					int temp = (int) mapped;
					saved_mapped = temp;
					_tt.changeTempoText(saved_mapped.ToString());
					string message = "/tempo";
					OSCHandler.Instance.SendMessageToClient("MaxMSP", message, saved_mapped);
				}
				//change tempo here in OSC
				if (timeLeft < 0) {
					_mm.disableTempoChange();
					timeLeft = 10.0f;
				}
			}
		}
	}

	//simple formula to recalculate diff's z-coordinate from the range of min and max diff's to min and max of z-coordinate in grid
	float mapZ(float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	public int getTempo() {
		return (int) mapped;
	}

	public void setMapped(int m) {
		mapped = m;
		_tt.defaultTempo(m);
	}

	public void setSavedMapped(int m) {
		saved_mapped = m;
	}

	public int getSavedMapped() {
		return saved_mapped;
	}

	public void resetDiffs(float i, float j) {
		min_z = i;
		max_z = j;
	}
}