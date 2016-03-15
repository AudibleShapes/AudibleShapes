using UnityEngine;
using System.Collections;
using Leap;

public class SwipeGestures : MonoBehaviour {

	//Leap components for gestures (to be built in the future)
	Leap.Controller controller;
	Frame frame;
	public ChangeMaterialTest _cmt;
	public MenuMove _mm;
	public TutorialPlane _tp;
	public SelectPresetShapeLeft shape_left;
	public SelectPresetShapeLeft no_shape_left;
	public SelectShapePresetRight shape_right;
	public SelectShapePresetRight no_shape_right;
	private bool flagQUp;
	private bool flagMUp;

	// Use this for initialization
	void Start () { //enable gestures you need to use here
		controller = new Controller();
		controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
		controller.Config.SetFloat ("Gesture.Swipe.MinVelocity", 2000f);
		controller.Config.Save();
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
				case(Gesture.GestureType.TYPESWIPE): //when performing swipe gesture, call the clearGrid method to clear the grid in Unity and Max
				{
					SwipeGesture swipe = new SwipeGesture (gesture);

					if (!MenuMove.isMenuEnabled && _tp.isTutorialEnabled) {
						if (swipe.Direction.y <= -0.8f && swipe.Direction.y > -1.0f) {
							_tp.moveDown();
							_mm.moveDown(); //swipe down to move menu down (enable it)
							Debug.Log("DOWN");
						}
					} else if (MenuMove.isMenuEnabled && !_tp.isTutorialEnabled) {
						if (swipe.Direction.y >= 0.8f && swipe.Direction.y < 1.0f) {
							if (!MenuMove.isPositionMode && !MenuMove.isScaleMode && !MenuMove.isRotateMode && !MenuMove.isTempoMode) {
								_mm.moveUp(); //swipe up to move menu up (disable it)
								_tp.moveUp();
								Debug.Log("UP");
							}
						}
					} 

					if (!_tp.isTutorialEnabled) {
						if (swipe.Direction.x >= 0.8f && swipe.Direction.x < 1.0f) {
								if (!_cmt.isGridCleared)
								_cmt.clearGrid(); //clear the grid with any hand swiping to the right
						}
							
							if (swipe.Direction.x <= -0.8f && swipe.Direction.x > -1.0f) {
								if (_cmt.isGridCleared && _cmt.isSaved) {
									_cmt.savedScene(); //retrieve saved scene with any hand swiping to the left
								}
							}
					} else if (_tp.isTutorialEnabled) { //swipes only during in tutorial mode
						if (swipe.Direction.x >= 0.8f && swipe.Direction.x < 1.0f && !_tp.rightSwipe) {
							_tp.goRight(); //advance to the next scene with swiping right
						}

						if (swipe.Direction.x <= -0.8f && swipe.Direction.x > -1.0f && !_tp.leftSwipe) {
							_tp.goLeft(); //go back to the last screen with swiping left
						}
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
}
