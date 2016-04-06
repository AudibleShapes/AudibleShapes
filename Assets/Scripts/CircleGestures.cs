using UnityEngine;
using System.Collections;
using Leap;

public class CircleGestures : MonoBehaviour {
	Leap.Controller controller;
	Frame frame;
	public ChangeMaterialTest _cmt;
	public TutorialPlane _tp;
	public SelectPresetShapeLeft shape_left;
	public SelectPresetShapeLeft no_shape_left;
	public SelectShapePresetRight shape_right;
	public SelectShapePresetRight no_shape_right;

	// Use this for initialization
	void Start () {
		controller = new Controller();
		controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		controller.Config.SetFloat("Gesture.Circle.MinRadius", 20.0f);
		controller.Config.Save ();
	}

	// Update is called once per frame
	void Update () {
		Frame frame = controller.Frame();
		HandModel hand_model = GetComponent<HandModel>();
		Hand leap_hand = hand_model.GetLeapHand(); //use this to check which hand performs which gesture

		foreach (Gesture gesture in frame.Gestures())
		{
			switch(gesture.Type)
			{
			case(Gesture.GestureType.TYPECIRCLE):
				{
					//Debug.Log(gesture.Hands.Count);

					if (gesture.Hands[0].IsLeft && !_tp.isTutorialEnabled &&
						!MenuMove.isPositionMode && !MenuMove.isRotateMode && !MenuMove.isScaleMode && !MenuMove.isTempoMode) { //left hand circle gestures
						CircleGesture circle = new CircleGesture(gesture);
						Debug.Log("LEFT HAND : " + circle.Radius);
						if (circle.Pointable.Direction.AngleTo (circle.Normal) <= Mathf.PI / 2)
							onClockwiseLeft(); //clockwise gesture gets you Cube preset
						else
							onCounterClockwiseLeft(); //ccw gesture gets you Sphere preset
					} else if (gesture.Hands[0].IsRight && !_tp.isTutorialEnabled&&
						!MenuMove.isPositionMode && !MenuMove.isRotateMode && !MenuMove.isScaleMode && !MenuMove.isTempoMode) { //right hand circle gestures
						CircleGesture circle = new CircleGesture(gesture);
						Debug.Log("RIGHT HAND : " + circle.Radius);
						if (circle.Pointable.Direction.AngleTo (circle.Normal) <= Mathf.PI / 2)
							onClockwiseRight(); //clockwise gesture gets Tetra preset
						else
							onCounterClockwiseRight(); //ccw gesture gets you Cylinder preset
					}
					break;
				}
			default:
				{
					break;
				}
			}
		}
	}

	private void onClockwiseLeft() {
		if (!_cmt.isCubePresetSelected) {
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

	private void onCounterClockwiseLeft() {
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
		}
	}

	private void onClockwiseRight() {
		if (!_cmt.isTetraPresetSelected) {
			_cmt.isTetraPresetSelected = true;
			shape_left.setNoneLeft();
			no_shape_left.setNoneLeft();
			shape_right.setTetra();
			no_shape_right.setTetra();
			if (_cmt.isCubePresetSelected)
				_cmt.isCubePresetSelected = false;
			else if (_cmt.isSpherePresetSelected)
				_cmt.isSpherePresetSelected = false;
			else if (_cmt.isCylinderPresetSelected)
				_cmt.isCylinderPresetSelected = false;
			Debug.Log("TETRA");
		}
	}

	private void onCounterClockwiseRight() {
		if (!_cmt.isCylinderPresetSelected) {
			_cmt.isCylinderPresetSelected = true;
			shape_left.setNoneLeft();
			no_shape_left.setNoneLeft();
			shape_right.setCylinder();
			no_shape_right.setCylinder();
			if (_cmt.isCubePresetSelected)
				_cmt.isCubePresetSelected = false;
			else if (_cmt.isTetraPresetSelected)
				_cmt.isTetraPresetSelected = false;
			else if (_cmt.isSpherePresetSelected)
				_cmt.isSpherePresetSelected = false;
			Debug.Log("CYLINDER");
		}
	}

}