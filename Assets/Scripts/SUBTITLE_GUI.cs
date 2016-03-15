using UnityEngine;
using System.Collections;
using Leap;

public class SUBTITLE_GUI : MonoBehaviour {

	Leap.Controller controller;
	Frame frame;
	HandList hands;

	private bool isHandsInScene = false;
	private float timeLeft = 10.0f;
	private float timeHand = 2.0f;
	private bool isPinched = false;

	public GUIStyle style;
	public Texture hand;
	public Texture help;

	void Start () {
		controller = new Controller();
		
	}

	void Update() {
		frame = controller.Frame();
		hands = frame.Hands;
		if (hands.Count > 0) {
			timeLeft = 10.0f;
			isHandsInScene = true;
		}
		else {
			timeLeft -= Time.deltaTime;
			if (timeLeft < 0)
				isHandsInScene = false;
		}
	}

	void OnGUI() {
		if (!isHandsInScene) {

			timeHand -= Time.deltaTime;
			GUI.skin.box = style;

			if (timeHand < 0  && !isPinched) {
				isPinched = true;
				hand = (Texture) Resources.Load("Images/Pinching_2");
				timeHand = 2.0f;
				timeHand -= Time.deltaTime;
			} else if (timeHand < 0 && isPinched){
				isPinched = false;
				hand = (Texture) Resources.Load("Images/Pinching_1");
				timeHand = 2.0f;
				timeHand -= Time.deltaTime;
			}

			GUI.BeginGroup(new Rect(450, 300, 800, 200));
			GUILayout.BeginHorizontal("BOX");
			GUI.DrawTexture(new Rect(50, 30, 150, 150), hand, ScaleMode.ScaleToFit, true, 0);
			GUI.Box(new Rect(327, 30, 50, 150), "ON");
			GUI.DrawTexture(new Rect(225, 30, 150, 150), help, ScaleMode.ScaleToFit, true, 0);
			GUI.Box(new Rect(480, 30, 440, 150), "FOR HELP");
			GUILayout.EndHorizontal();
			GUI.EndGroup();
		}
	}
}
