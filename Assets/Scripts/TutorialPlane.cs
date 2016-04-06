using UnityEngine;
using System.Collections;

public class TutorialPlane : MonoBehaviour {
	public bool isTutorialEnabled;
	public HelpPlane _hp;
	public Material step_one;
	public Material step_two;
	public Material step_three;
	public Material step_four;
	public Material step_five;
	public Material step_six;
	public Material step_seven;
	public Material step_eight;
	private readonly float MIN_Y = -5.2f;
	private readonly float MAX_Y = 1.95f;
	private readonly int FIRST = 1;
	private readonly int SECOND = 2;
	private readonly int THIRD = 3;
	private readonly int FOURTH = 4;
	private readonly int FIFTH = 5;
	private readonly int SIXTH = 6;
	private readonly int SEVENTH = 7;
	private readonly int EIGHTH = 8;
	private int count = 0;
	private float timerRight = 0.2f;
	private float timerLeft = 0.2f;
	public bool rightSwipe = false;
	public bool leftSwipe = false;

	// Use this for initialization
	void Start () {
		isTutorialEnabled = false;
		//Debug.Log(this.transform.position.y);
		this.transform.position = new Vector3(this.transform.position.x, MIN_Y, this.transform.position.z);
	}

	public void moveUp() {
		isTutorialEnabled = true;
		OSCHandler.Instance.SendMessageToClient("MaxMSP", "/play", 0);
		this.transform.position = new Vector3(this.transform.position.x, MAX_Y, this.transform.position.z);
		_hp.questionUpMat();
		count++;
		if (count == FIRST)
			this.GetComponent<Renderer>().material = step_one;
	}

	public void moveDown() {
		isTutorialEnabled = false;
		OSCHandler.Instance.SendMessageToClient("MaxMSP", "/play", 1);
		this.transform.position = new Vector3(this.transform.position.x, MIN_Y, this.transform.position.z);
		_hp.questionDownMat();
		count = 0;
	}

	public void goRight() {
		if (count > 0 && count < EIGHTH) {
			rightSwipe = true;
			count++;
			if (count == SECOND)
				this.GetComponent<Renderer>().material = step_two;
			else if (count == THIRD)
				this.GetComponent<Renderer>().material = step_three;
			else if (count == FOURTH)
				this.GetComponent<Renderer>().material = step_four;
			else if (count == FIFTH)
				this.GetComponent<Renderer>().material = step_five;
			else if  (count == SIXTH)
				this.GetComponent<Renderer>().material = step_six;
			else if (count == SEVENTH)
				this.GetComponent<Renderer>().material = step_seven;
			else if (count == EIGHTH)
				this.GetComponent<Renderer>().material = step_eight;
		}
//		else {
//			count = 1;
//			this.GetComponent<Renderer>().material = step_one;
//		}
	}

	public void goLeft() {
		if (count > FIRST && count <= EIGHTH) {
			leftSwipe = true;
			count--;
			if (count == FIRST)
				this.GetComponent<Renderer>().material = step_one;
			if (count == SECOND)
				this.GetComponent<Renderer>().material = step_two;
			else if (count == THIRD)
				this.GetComponent<Renderer>().material = step_three;
			else if (count == FOURTH)
				this.GetComponent<Renderer>().material = step_four;
			else if (count == FIFTH)
				this.GetComponent<Renderer>().material = step_five;
			else if  (count == SIXTH)
				this.GetComponent<Renderer>().material = step_six;
			else if (count == SEVENTH)
				this.GetComponent<Renderer>().material = step_seven;
//		else {
//			count = 4;
//			this.GetComponent<Renderer>().material = step_four;
//		}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (rightSwipe) {
			leftSwipe = false;
			timerRight -= Time.deltaTime;
			timerLeft = 0.2f;
		}

		if (leftSwipe) {
			rightSwipe = false;
			timerLeft -= Time.deltaTime;
			timerRight = 0.2f;
		}

		if (timerRight < 0) {
			timerRight = 0.2f;
			rightSwipe = false;
			Debug.Log("READY TO SWIPE RIGHT");
		}

		if (timerLeft < 0) {
			timerLeft = 0.2f;
			leftSwipe = false;
			Debug.Log("READY TO SWIPE LEFT");
		}
	}
}
