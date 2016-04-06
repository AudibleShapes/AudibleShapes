/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/** 
 * Detects pinches and grabs the closest rigidbody if it's within a given range.
 * 
 * Attach this script to the physics hand object assinged to the HandController in a scene.
 */
[RequireComponent(typeof(AudioSource))]
public class MagneticPinch : MonoBehaviour {

  public const float TRIGGER_DISTANCE_RATIO = 0.7f;

  /** The stiffness of the spring force used to move the object toward the hand. */
  public float forceSpringConstant = 100.0f;
  /** The maximum range at which an object can be picked up.*/
  public float magnetDistance = 2.0f;
	
	protected bool pinching_;
	protected Collider grabbed_;

	public static int count;

	public ChangeMaterialTest _cmt;
	public float timeLeft = 2.0f;
	public bool isPinched = false;

  	void Start() {
    	pinching_ = false;
    	grabbed_ = null;
		count = 0;
  	}

  	/** Finds an object to grab and grabs it. */
  	void OnPinch(Vector3 pinch_position) {
	    pinching_ = true;
		//_cmt.selectMoveBlock(pinch_position[0], pinch_position[1], pinch_position[2]);
 	}

  /** Clears the pinch state. */
  void OnRelease() {
    	grabbed_ = null;
    	pinching_ = false;
  }

  /**
   * Checks whether the hand is pinching and updates the position of the pinched object.
   */
  void Update() {
		
	bool trigger_pinch = false;
    HandModel hand_model = GetComponent<HandModel>();
    Hand leap_hand = hand_model.GetLeapHand();

    if (leap_hand == null)
			return;

	//	Debug.Log("ROTATION ANGLE : " + leap_hand.RotationAngle(frame, Vector.YAxis));

    // Scale trigger distance by thumb proximal bone length.
    Vector leap_thumb_tip = leap_hand.Fingers[0].TipPosition;
    float proximal_length = leap_hand.Fingers[0].Bone(Bone.BoneType.TYPE_PROXIMAL).Length;
    float trigger_distance = proximal_length * TRIGGER_DISTANCE_RATIO;

    // Check thumb tip distance to joints on all other fingers.
    // If it's close enough, start pinching.
    for (int i = 1; i < HandModel.NUM_FINGERS && !trigger_pinch; ++i) {
      Finger finger = leap_hand.Fingers[i];

      for (int j = 0; j < FingerModel.NUM_BONES && !trigger_pinch; ++j) {
        Vector leap_joint_position = finger.Bone((Bone.BoneType)j).NextJoint;
        if (leap_joint_position.DistanceTo(leap_thumb_tip) < trigger_distance)
          trigger_pinch = true;
      }
    }

    Vector3 pinch_position = hand_model.fingers[0].GetTipPosition();

    // Only change state if it's different.
    if (trigger_pinch && !pinching_) {
			OnPinch(pinch_position);

//			if (leap_hand.IsLeft) {
//				string hand = leap_hand.IsLeft ? "LEFT HAND" : "NOT LEFT HAND";
//				Debug.Log("PINCHING HAND " + hand);
//			} else if (leap_hand.IsRight) {
//				string hand = leap_hand.IsRight ? "RIGHT HAND" : "NOT RIGHT HAND";
//				Debug.Log("GESTURE HAND " + hand);
//			}
	}
    else if (!trigger_pinch && pinching_) {
      OnRelease();
	}

    // Accelerate what we are grabbing toward the pinch.
    if (grabbed_ != null) {
      Vector3 distance = pinch_position - grabbed_.transform.position;
      grabbed_.GetComponent<Rigidbody>().AddForce(forceSpringConstant * distance);
    }

		//Debug.Log("isHelpEnabled = " + isHelpEnabled);
  }
}
