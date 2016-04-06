using UnityEngine;
using System.Collections;
using Leap;

public class PokeMenu : MonoBehaviour {

	private bool poking = false;
	private bool trigger_poke;
	public MenuMove _mm;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		trigger_poke = false;

		//frame = controller.Frame();
		HandModel hand_model = GetComponent<HandModel>();
		Hand leap_hand = hand_model.GetLeapHand();

		if (leap_hand == null) 
			return;

		//get the position of index finger from either hand
		Vector3 position = hand_model.fingers[1].GetTipPosition();

		trigger_poke = _mm.triggerPoke(position[0], position[1], position[2]);

		if (trigger_poke && !poking)
			OnPokeMenu(position);
		else if (!trigger_poke && poking)
			NoPoke();
	
	}

	void OnPokeMenu(Vector3 position) {
		poking = true;
		_mm.changeMaterial(position[0], position[1], position[2]);
	}

	void NoPoke() {
		poking = false;
	}
}
