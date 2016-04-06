﻿using UnityEngine;
using System.Collections;
using Leap;

public class FingerMovementRotation : MonoBehaviour {
	public ChangeMaterialTest _cmt;
	private float max_z = 0.0f; //used to establish max z between z coordinates of pinky finger and palm
	private float min_z = 10.0f; //used to establish min z between z coordinates of pinky finger and palm
	private float mapped = 0.0f;
	private readonly float MAX_Z_POSS = 0.8f;
	private readonly float MIN_Z_POSS = -1.4f;
	private bool poking = false;
	private bool trigger_poke;

	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		HandModel hand_model = GetComponent<HandModel>();
		Hand leap_hand = hand_model.GetLeapHand();

		if (leap_hand == null)
			return;
		
		if (MenuMove.isRotateMode) { //left hand pokes cube to be rotated
			Vector3 position = hand_model.fingers[1].GetTipPosition();
			trigger_poke = _cmt.triggerPoke(position.x, position.y, position.z);
			
			if (trigger_poke && !poking)
				OnPoke(position);
			else if (!trigger_poke && poking)
				NoPoke();
			if (leap_hand.IsRight) {
				float z = leap_hand.Direction.Yaw;

				//Debug.Log(position.z + "\t" + palm.z + "\t" + z);

				if (z > max_z && z < MAX_Z_POSS)
					max_z = z;
					
				if (z < min_z && z > MIN_Z_POSS)
					min_z = z;

			//Debug.Log(min_z + "\t" + max_z + "\t" + z);

			//map z from range of min_z - max_z to new range of Z_MIN and Z_MAX from ChangeMaterialTest script
			//then, rotate blocks in grid based on this mapped value
				mapped = mapZ(z, max_z, min_z, ChangeMaterialTest.ROT_MIN, ChangeMaterialTest.ROT_MAX);
				if (_cmt.count_rotate > 0)
					_cmt.rotateBlocks(mapped); 
			}
		} else
			_cmt.deselectRotateBlocks();
	}
	
	void OnPoke(Vector3 position) {
		poking = true;
		_cmt.selectRotateBlock(position.x, position.y, position.z);
	}
	
	void NoPoke() {
		poking = false;
	}
	
	//simple formula to recalculate diff's z-coordinate from the range of min and max diff's to min and max of z-coordinate in grid
	float mapZ(float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	public void resetDiffs(float i, float j) {
		min_z = i;
		max_z = j;
	}
}