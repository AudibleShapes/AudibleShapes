using UnityEngine;
using System.Collections;
using Leap;

public class ClosedFistGesture : MonoBehaviour {
	
	public SelectPresetShapeLeft shape_left;
	public SelectPresetShapeLeft no_shape_left;
	public SelectShapePresetRight shape_right;
	public SelectShapePresetRight no_shape_right;
	public ChangeMaterialTest _cmt;

	private float min_z_li = 10.0f;
	private float min_z_lm = 10.0f;
	private float min_z_lr = 10.0f;
	private float min_z_lp = 10.0f;
	private float min_z_ri = 10.0f;
	private float min_z_rm = 10.0f;
	private float min_z_rr = 10.0f;
	private float min_z_rp = 10.0f;
	private readonly float MAX_Z_POSS = 2f;
	private readonly float MIN_Z_POSS = -2f;
	private bool isClosedLeft = false;
	private bool triggerCloseLeft;
	private bool isClosedRight = false;
	private bool triggerCloseRight;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		HandModel hand_model = GetComponent<HandModel>();
		Hand leap_hand = hand_model.GetLeapHand();
		triggerCloseLeft = false;
		triggerCloseRight = false;

		if (leap_hand == null)
			return;

		if (!MenuMove.isPositionMode && !MenuMove.isScaleMode && !MenuMove.isRotateMode && !MenuMove.isTempoMode) {
			if (leap_hand.IsLeft) {
				//Vector3 leftThumb = hand_model.fingers[0].GetTipPosition();
				Vector3 leftIndex = hand_model.fingers[1].GetTipPosition();
				Vector3 leftMiddle = hand_model.fingers[2].GetTipPosition();
				Vector3 leftRing = hand_model.fingers[3].GetTipPosition();
				Vector3 leftPinky = hand_model.fingers[4].GetTipPosition();
				Vector3 palm = hand_model.palm.position;

				//float zThumb = leftThumb.z - palm.z;
				float zIndex = leftIndex.z - palm.z;
				float zMiddle = leftMiddle.z - palm.z;
				float zRing = leftRing.z - palm.z;
				float zPinky = leftPinky.z - palm.z;

				if (zIndex < min_z_li && zIndex > MIN_Z_POSS)
					min_z_li = zIndex;

				if (zMiddle < min_z_lm && zMiddle > MIN_Z_POSS)
					min_z_lm = zMiddle;

				if (zRing < min_z_lr && zRing > MIN_Z_POSS)
					min_z_lr = zRing;

				if (zPinky < min_z_lp && zPinky > MIN_Z_POSS)
					min_z_lp = zPinky;


				float liz = mapZ(zIndex, min_z_li, MAX_Z_POSS, 0, 1);
				float lmz = mapZ(zMiddle, min_z_lm, MAX_Z_POSS, 0, 1);
				float lrz = mapZ(zRing, min_z_lr, MAX_Z_POSS, 0, 1);
				float lpz = mapZ(zPinky, min_z_lp, MAX_Z_POSS, 0, 1);

				//Debug.Log("LEFT : " + liz +"\t" + lmz + "\t" + lrz + "\t" + lpz);

				if (liz < 0.5f && lmz < 0.5f && lrz < 0.5f && lpz < 0.5f &&
					(leftIndex.y <= palm.y + 0.2f && leftMiddle.y <= palm.y + 0.2f && leftRing.y <= palm.y + 0.2f && leftPinky.y <= palm.y + 0.2f) &&
					(leftIndex.y > palm.y - 0.2f && leftMiddle.y > palm.y - 0.2f && leftRing.y > palm.y - 0.2f && leftPinky.y > palm.y - 0.2f) &&
					(liz > MIN_Z_POSS && lmz > MIN_Z_POSS && lrz > MIN_Z_POSS && lpz > MIN_Z_POSS)) {
					triggerCloseLeft = true;
					if (triggerCloseLeft && !isClosedLeft)
						onCloseLeft();
				} else {
					triggerCloseLeft = false;
					if (!triggerCloseLeft && isClosedLeft)
						onOpenLeft();
				}

			}

			if (leap_hand.IsRight) {

				//Vector3 rightThumb = hand_model.fingers[0].GetTipPosition();
				Vector3 rightIndex = hand_model.fingers[1].GetTipPosition();
				Vector3 rightMiddle = hand_model.fingers[2].GetTipPosition();
				Vector3 rightRing = hand_model.fingers[3].GetTipPosition();
				Vector3 rightPinky = hand_model.fingers[4].GetTipPosition();
				Vector3 palm = hand_model.palm.position;

				//float zThumb = rightThumb.z - palm.z;
				float zIndex = rightIndex.z - palm.z;
				float zMiddle = rightMiddle.z - palm.z;
				float zRing = rightRing.z - palm.z;
				float zPinky = rightPinky.z - palm.z;

				if (zIndex < min_z_ri && zIndex > MIN_Z_POSS)
					min_z_ri = zIndex;

				if (zMiddle < min_z_rm && zMiddle > MIN_Z_POSS)
					min_z_rm = zMiddle;

				if (zRing < min_z_rr && zRing > MIN_Z_POSS)
					min_z_rr = zRing;

				if (zPinky < min_z_rp && zPinky > MIN_Z_POSS)
					min_z_rp = zPinky;


				float riz = mapZ(zIndex, min_z_ri, MAX_Z_POSS, 0, 1);
				float rmz = mapZ(zMiddle, min_z_rm, MAX_Z_POSS, 0, 1);
				float rrz = mapZ(zRing, min_z_rr, MAX_Z_POSS, 0, 1);
				float rpz = mapZ(zPinky, min_z_rp, MAX_Z_POSS, 0, 1);

				//Debug.Log("RIGHT : " + riz +"\t" + rmz + "\t" + rrz + "\t" + rpz);

				if (riz < 0.5f && rmz < 0.5f && rrz < 0.5f && rpz < 0.5f &&
					(rightIndex.y <= palm.y + 0.2f && rightMiddle.y <= palm.y + 0.2f && rightRing.y <= palm.y + 0.2f && rightPinky.y <= palm.y + 0.2f) &&
					(rightIndex.y > palm.y - 0.2f && rightMiddle.y > palm.y - 0.2f && rightRing.y > palm.y - 0.2f && rightPinky.y > palm.y - 0.2f) &&
					(riz > 0 && rmz > 0 && rrz > 0 && rpz > 0)) {
					triggerCloseRight = true;
					if (triggerCloseRight && !isClosedRight)
						//onCloseRight();
						Debug.Log("WHAT");
				} else {
					triggerCloseRight = false;
					if (!triggerCloseRight && isClosedRight)
						onOpenRight();
				}
			}
		}
	}

	//simple formula to recalculate diff's z-coordinate from the range of min and max diff's to min and max of z-coordinate in grid
	float mapZ(float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	private void onCloseLeft() {
		isClosedLeft = true;
		Debug.Log("CLOSED LEFT FIST");
		if (!_cmt.isSpherePresetSelected) {
			_cmt.isSpherePresetSelected = true;
			shape_left.setSphere();
			no_shape_left.setSphere();
			shape_right.setNoneRight();
			no_shape_right.setNoneRight();
			if (_cmt.isCubePresetSelected)
				_cmt.isCubePresetSelected = false;
			else if (_cmt.isTetraPresetSelected)
				_cmt.isTetraPresetSelected = false;
			else if (_cmt.isCylinderPresetSelected)
				_cmt.isCylinderPresetSelected = false;
			Debug.Log("SPHERE");
		} else if (!_cmt.isCubePresetSelected) {
			_cmt.isCubePresetSelected = true;
			shape_left.setCube();
			no_shape_left.setCube();
			shape_right.setNoneRight();
			no_shape_right.setNoneRight();
			if (_cmt.isSpherePresetSelected)
				_cmt.isSpherePresetSelected = false;
			else if (_cmt.isTetraPresetSelected)
				_cmt.isTetraPresetSelected = false;
			else if (_cmt.isCylinderPresetSelected)
				_cmt.isCylinderPresetSelected = false;
			Debug.Log("CUBE");
		}
	}

	private void onOpenLeft() {
		isClosedLeft = false;
		Debug.Log("OPEN LEFT FIST");
	}



	private void onOpenRight() {
		isClosedRight = false;
		Debug.Log("OPEN RIGHT FIST");
	}

	public void resetDiffs(float i) {
		min_z_li = i;
		min_z_lm = i;
		min_z_lr = i;
		min_z_lp = i;

		min_z_ri = i;
		min_z_rm = i;
		min_z_rr = i;
		min_z_rp = i;
	}
}

