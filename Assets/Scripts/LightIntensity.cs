using UnityEngine;
using System.Collections;
using Leap;

public class LightIntensity : MonoBehaviour {

	Leap.Controller controller;
	Frame frame;
	HandList hands;

	public float duration;
	public Light lt;
	public float maxIntensity;
	private float minIntensity = 0;
	private float timeLeft = 60.0f;

	void Start() {
		controller = new Controller();
		lt = GetComponent<Light>();
		lt.intensity = 0.0f;
	}

	void Update() {
		frame = controller.Frame();
		hands = frame.Hands;

		if (hands.Count != 0) {
			timeLeft = 60.0f;
			changeIntensity(maxIntensity);
		}
		else {
			timeLeft -= Time.deltaTime;
			if (timeLeft < 0)
				changeIntensity(minIntensity);
		}
	}

	public void changeIntensity(float intensity) {
		if (lt.intensity != intensity) { //if the light's intensity does not match that passed through parameters
//			if (lt.intensity > intensity) //decrease amount of light used (when hands don't exist in scene)
//				lt.intensity -= Time.deltaTime*duration;
//			else if (lt.intensity < intensity) //increase amount of light used (when hands exist in scene)
//				lt.intensity += Time.deltaTime*duration;
//			else
				lt.intensity = intensity;
		}
	}
}
