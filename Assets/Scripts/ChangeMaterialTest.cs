using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ChangeMaterialTest : MonoBehaviour
{

	public FingerTempo _ft;
	public TempoText _tt;
	public MenuMove _mm;
	public ScaleNameText _snt;
	public SelectPresetShapeLeft shape_left;
	public SelectPresetShapeLeft no_shape_left;
	public SelectShapePresetRight shape_right;
	public SelectShapePresetRight no_shape_right;

	public GameObject[] cubes;
	public GameObject[] spheres;
	public GameObject[] cylinders;
	public GameObject[] tetras;
	private int count_c;
	private int count_s;
	private int count_y;
	private int count_t;
	private int temp_count_cube = 16;
	private int temp_count_sphere = 0;
	private int temp_count_tetra = 0;
	private int temp_count_cyl = 0;

	//used for each GO in grid when selecting preset to change to
	public bool[] isCubePreset;
	public bool[] isSpherePreset;
	public bool[] isCylinderPreset;
	public bool[] isTetraPreset;

	//used for changing materials on walls; in conjunction with gesture to select presets
	public bool isCubePresetSelected;
	public bool isSpherePresetSelected;
	public bool isTetraPresetSelected;
	public bool isCylinderPresetSelected;

	public Material mat_on;
	public Material mat_off;
	public Material mat_sel_move;
	public Material mat_sel_size;
	public Material mat_sel_rotate;
	public Material mat_play;
	public bool[] isBlockOn;
	public bool[] isMoveBlockSelect;
	public bool[] isSizeBlockSelect;
	public bool[] isRotateBlockSelect;
	private int clear = 0;
	private readonly float Z_COORDINATE = 1.5f;
	private float[] timeLeftMV;
	private float[] timeLeftSZ;
	private float[] timeLeftRT;
	public static readonly float Z_MIN = -2.0f;
	public static readonly float Z_MAX = 5.0f;
	private readonly float SIZE = 1.2f;
	private readonly float SIZE_DEFAULT_FOR_MAX_MSP = 0.78f;
	public static readonly float SIZE_MIN = 0.4f;
	public static readonly float SIZE_MAX = 1.2f;
	private readonly Quaternion ROTATE = new Quaternion (0, 0, 0, 1);
	private readonly float ROTATE_VAL = 0.0f;
	public static readonly float ROT_MIN = -1.0f;
	public static readonly float ROT_MAX = 1.0f;
	public static readonly float TEMPO_MAX = 241.0f;
	public static readonly float TEMPO_MIN = 60.0f;
	private static readonly int TEMPO_DEF = 120;
	private readonly float TIME = 10.0f;
	private readonly int PRESET_CUBE = 1;
	private readonly int PRESET_SPHERE = 2;
	private readonly int PRESET_TETRA = 3;
	private readonly int PRESET_CYL = 4;

	//saved arrays used for restoring and saving values for current session played
	private bool[] saved_isBlockOn;
	private float[] saved_zcoors;
	private float[] saved_sizes;
	private Quaternion[] saved_rotate;
	private bool[] saved_isCubePreset;
	private bool[] saved_isSpherePreset;
	private bool[] saved_isTetraPreset;
	private bool[] saved_isCylinderPreset;

	//important to distinguish between shapes that have size altered and shapes that have remained the same
	//key to sending out OSC messages for the size of shapes (whether altered or not)
	private bool[] temp_isSizeSelect;

	//stores three values for each shape: -1, 0, or 1, depending on its rotation
	//saved during the rotation method
	private int[] temp_RotateMaxMSP;

	public bool isGridCleared;
	public bool isSaved;
	public int count_move = 0;
	public int count_scale = 0;
	public int count_rotate = 0;

	void Start ()
	{
		clear = 1;
		OSCHandler.Instance.SendMessageToClient ("MaxMSP", "/clear", clear);
		WithForeachLoop ();
	}

	//intialize everything in Audible Shapes and send out the default OSC messages to Max MSP
	void WithForeachLoop ()
	{
		foreach (Transform child in transform)
			child.position = new Vector3(child.position.x, child.position.y, Z_COORDINATE);

		cubes = GameObject.FindGameObjectsWithTag ("Cube").OrderBy (go => go.name.Substring (1)).ToArray ();
		spheres = GameObject.FindGameObjectsWithTag ("Sphere").OrderBy (go => go.name.Substring (1)).ToArray ();
		tetras = GameObject.FindGameObjectsWithTag ("Tetra").OrderBy (go => go.transform.GetChild (0).name.Substring (1)).ToArray ();
		cylinders = GameObject.FindGameObjectsWithTag ("Cylinder").OrderBy (go => go.transform.GetChild (0).name.Substring (1)).ToArray ();
		
		isGridCleared = true;
		isSaved = false;
		isCubePresetSelected = true;
		isSpherePresetSelected = false;
		isTetraPresetSelected = false;
		isCylinderPresetSelected = false;

		count_c = cubes.Length;
		count_s = spheres.Length;
		count_t = tetras.Length;
		count_y = cylinders.Length;

		isBlockOn = new bool[count_c];
		isMoveBlockSelect = new bool[count_c];
		isSizeBlockSelect = new bool[count_c];
		isRotateBlockSelect = new bool[count_c];
		timeLeftMV = new float[count_c];
		timeLeftSZ = new float[count_c];
		timeLeftRT = new float[count_c];
		isCubePreset = new bool[count_c];
		isSpherePreset = new bool[count_c];
		isTetraPreset = new bool[count_c];
		isCylinderPreset = new bool[count_c];

		saved_isBlockOn = new bool[count_c];
		saved_zcoors = new float[count_c];
		saved_sizes = new float[count_c];
		saved_rotate = new Quaternion[count_c];
		saved_isCubePreset = new bool[count_c];
		saved_isSpherePreset = new bool[count_c];
		saved_isTetraPreset = new bool[count_c];
		saved_isCylinderPreset = new bool[count_c];
		temp_isSizeSelect = new bool[count_c];
		temp_RotateMaxMSP = new int[count_c];

		for (int i = 0; i < count_c; i++) {

			spheres [i].SetActive (false);
			tetras [i].SetActive (false);
			cylinders [i].SetActive (false);

			isBlockOn [i] = false;
			isMoveBlockSelect [i] = false;
			isSizeBlockSelect [i] = false;
			temp_isSizeSelect[i] = isSizeBlockSelect[i];
			temp_RotateMaxMSP[i] = 0;
			isRotateBlockSelect [i] = false;
			timeLeftMV [i] = TIME;
			timeLeftSZ [i] = TIME;
			timeLeftRT [i] = TIME;

			saved_isBlockOn [i] = isBlockOn [i];
			saved_zcoors [i] = Z_COORDINATE;
			saved_sizes [i] = SIZE;
			saved_rotate [i] = ROTATE;
			isCubePreset[i] = true;
			isSpherePreset[i] = false;
			isTetraPreset[i] = false;
			isCylinderPreset[i] = false;
			saved_isCubePreset[i] = isCubePreset[i];
			saved_isSpherePreset[i] = isSpherePreset[i];
			saved_isTetraPreset[i] = isTetraPreset[i];
			saved_isCylinderPreset[i] = isCylinderPreset[i];

			string message = "/move" + cubes [i].name.Substring (1);
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, Z_COORDINATE);
			string message2 = "/size" + cubes [i].name.Substring (1);
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message2, SIZE_DEFAULT_FOR_MAX_MSP);
			string message3 = "/rotate" + cubes [i].name.Substring (1);
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message3, ROTATE_VAL);
			string messageOSC = "/shape" + cubes[i].name.Substring(1);
			OSCHandler.Instance.SendMessageToClient("MaxMSP", messageOSC, PRESET_CUBE);
		}

		string message4 = "/tempo";
		OSCHandler.Instance.SendMessageToClient ("MaxMSP", message4, TEMPO_DEF);
	}

	public void changeMaterial (float x, float y, float z)
	{ //only used when poking any shape in the grid
		float on = 0.0f;
		selectMaterialChild (x, y, z, on);
	}

	public bool triggerPoke (float x, float y, float z)
	{
		return triggerPokeChild(x, y, z);
	}

	public void selectMoveBlock (float x, float y, float z)
	{
		selectMoveChild(x, y, z);
	}

	public void selectSizeBlock (float x, float y, float z)
	{
		selectSizeChild (x, y, z);
	}

	public void selectRotateBlock (float x, float y, float z)
	{
		selectRotateChild (x, y, z);
	}

	public void moveBlocks (float z)
	{
		moveChilds(z);
		isSaved = true;
	}

	public void deselectMoveBlocks ()
	{
		deselectMoveChilds();
	}

	public void resizeBlocks (float z)
	{
		resizeChilds (z);
		isSaved = true;
	}

	public void deselectResizeBlocks ()
	{
		deselectResizeChilds ();
	}

	public void rotateBlocks (float y)
	{
		rotateChilds (y);
		isSaved = true;
	}

	public void deselectRotateBlocks ()
	{
		deselectRotateChilds ();
	}

	public void clearGrid ()
	{ //clear the entire grid, change material to off, and change the blocks' bool values to false, and reset to default locations
		resetPresetChild ();

		//turn off any mode currently on in the menu
		//so as to default selection to turning shapes on or off
		_mm.turnOffOtherMode("Position");
		_mm.turnOffOtherMode("Scaling");
		_mm.turnOffOtherMode("Rotation");
		_mm.turnOffOtherMode("Tempo");

		for (int i = 0; i < count_c; i++) {

			if (isBlockOn [i])
				isBlockOn [i] = false;

			if (isMoveBlockSelect [i])
				isMoveBlockSelect [i] = false;

			if (isSizeBlockSelect [i])
				isSizeBlockSelect [i] = false;

			if (isRotateBlockSelect [i])
				isRotateBlockSelect [i] = false;

			float on = 0.0f;
			string name = activeShapeMessageName(i);
			string message = "/on" + name;
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, on);
			string message1 = "/move" + name;
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message1, Z_COORDINATE);
			string message2 = "/size" + name;
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message2, SIZE_DEFAULT_FOR_MAX_MSP);
			string message3 = "/rotate" + name;
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message3, ROTATE_VAL);
		}

		_mm.resetScale ();
		_ft.setMapped (TEMPO_DEF);

		string message4 = "/tempo";
		OSCHandler.Instance.SendMessageToClient ("MaxMSP", message4, TEMPO_DEF);

		isGridCleared = true;
		temp_count_cyl = 0;
		temp_count_sphere = 0;
		temp_count_tetra = 0;
		temp_count_cube = count_c;
	}

	public void savedScene ()
	{ //using left swipe, reset last session of Audible Shapes, including all changes to cubes (z position, on states, sizes)
		savedSceneChild();
	}

	void Update () //only use this method to change the time left values whenever a shape is selected for manipulation purposes
	{
		for (int i = 0; i < count_c; i++) {
			if (isMoveBlockSelect [i])
				timeLeftMV [i] -= Time.deltaTime;

			if (isSizeBlockSelect [i])
				timeLeftSZ [i] -= Time.deltaTime;

			if (isRotateBlockSelect [i])
				timeLeftRT [i] -= Time.deltaTime;
		}
	}

	/**
 	*  PRIVATE METHODS FOR SELECTION AND MANIPULATION ON ALL GAME OBJECTS
 	*/

	private void selectMaterialChild (float x, float y, float z, float on)
	{
		//checks if any block is turned on, which switches isGridCleared
		//to its opposite value (important for resetting save states)
		Vector3 size = new Vector3 (1, 1, 1);
		Vector3 position = new Vector3 (0, 0, 0);

		for (int i = 0; i < count_t; i++) { //each coordinate of the poke is matched to each coordinate of each grid space; bounds.size gets the actual size of each grid space as to get its edges when poking
			size = checkActiveShapesSize(i); //check what the current active shape is in spot and use its size and position
			position = checkActiveShapesPosition(i);

			if (x >= (position.x - (size.x / 2)) && x < (position.x + (size.x / 2))) {
				if (y >= (position.y - (size.y / 2)) && y < (position.y + (size.y / 2))) {
					if (z >= (position.z - (size.z / 2)) && z < (position.z + (size.z / 2))) {
						if (isBlockOn [i] && !isMoveBlockSelect [i] && !isRotateBlockSelect [i] && !isSizeBlockSelect [i]) { //if the block is active or on (without having its manipulation enabled), change it to off
							//if the shape getting selected is active, on, and belongs to the same preset group, then turn it off and send OSC message
							if ((isCubePresetSelected && cubes[i].activeSelf) || (isSpherePresetSelected && spheres[i].activeSelf)
								|| (isTetraPresetSelected && tetras[i].activeSelf) || (isCylinderPresetSelected && cylinders[i].activeSelf)) {
								on = 0.0f;
								string name = activeShapeMessageName(i);
								string message = "/on" + name;
								OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, on);
								checkActiveShapesMat(i, mat_off);
								isBlockOn [i] = false;
								saved_isBlockOn [i] = isBlockOn [i];
							} else //change the shape and its properties to newly-selected shape
								checkActiveShapes(i);
						} else if (!isBlockOn [i]) { //if the button is inactive or off, change it to on
							if (isGridCleared && temp_count_cube == count_c) {
								//clear any saved values if the grid is initially cleared and only contains cubes (default preset)
								clearSavedValues(count_c);
								//Debug.Log("THIS IS GETTING CALLED " + temp_count_cube);
							}
							checkActiveShapes(i);
							on = 1.0f;
							string name = activeShapeMessageName(i);
							string message = "/on" + name;
							OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, on);
							checkActiveShapesMat(i, mat_on);
							isBlockOn [i] = true;
							saved_isBlockOn [i] = isBlockOn [i];
							if (isGridCleared)
								isGridCleared = false;
							if (!isSaved)
								isSaved = true;
						}
					}
				}
			}
		}

		int count_off = 0;
		for (int i = 0; i < count_c; i++) {
			if (!isBlockOn[i])
				count_off++;
		}
			
		if (count_off == count_c && temp_count_sphere == 0 && temp_count_tetra == 0 && temp_count_cyl == 0 && temp_count_cube == count_c) {
			clearSavedValues(count_c);
			//Debug.Log("SAVED VALUES DESTROYED");
		}
		//Debug.Log((count_off == count_c && temp_count_sphere == 0 && temp_count_tetra == 0 && temp_count_cyl == 0 && temp_count_cube == count_c));
		checkGridCleared(count_t);
	}

	//enable a certain shape at any grid space whenever the user changes preset
	//and disables any other shape in that space if/when selected; as well,
	//inherit any properties previously-active shape to newly-active shape
	//as well as its material if the block is currently in the on-state
	private void checkActiveShapes(int i) {
		if (!isCubePresetSelected && cubes[i].activeSelf) {
			isCubePreset[i]  = false;
			saved_isCubePreset[i] = isCubePreset[i];
			cubes[i].SetActive(false);
			temp_count_cube--;
			if (!spheres[i].activeSelf && isSpherePresetSelected) {
				isSpherePreset[i] = true;
				saved_isSpherePreset[i] = isSpherePreset[i];
				spheres[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_sphere++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_SPHERE);
			} else if (!tetras[i].activeSelf && isTetraPresetSelected) {
				isTetraPreset[i] = true;
				saved_isTetraPreset[i] = isTetraPreset[i];
				tetras[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_tetra++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_TETRA);
			} else if (!cylinders[i].activeSelf && isCylinderPresetSelected) {
				isCylinderPreset[i] = true;
				saved_isCylinderPreset[i] = isCylinderPreset[i];
				cylinders[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_cyl++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_CYL);
			}
		}
			
		if (!isSpherePresetSelected && spheres[i].activeSelf) {
			isSpherePreset[i] = false;
			saved_isSpherePreset[i] = isSpherePreset[i];
			spheres[i].SetActive(false);
			temp_count_sphere--;
			if (!cubes[i].activeSelf && isCubePresetSelected) {
				isCubePreset[i] = true;
				saved_isCubePreset[i] = isCubePreset[i];
				cubes[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_cube++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_CUBE);
			} else if (!tetras[i].activeSelf && isTetraPresetSelected) {
				isTetraPreset[i] = true;
				saved_isTetraPreset[i] = isTetraPreset[i];
				tetras[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_tetra++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_TETRA);
			} else if (!cylinders[i].activeSelf && isCylinderPresetSelected) {
				isCylinderPreset[i] = true;
				saved_isCylinderPreset[i] = isCylinderPreset[i];
				cylinders[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_cyl++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_CYL);
			}
		}

		if (!isTetraPresetSelected && tetras[i].activeSelf) {
			isTetraPreset[i] = false;
			saved_isTetraPreset[i] = isTetraPreset[i];
			tetras[i].SetActive(false);
			temp_count_tetra--;
			if (!spheres[i].activeSelf && isSpherePresetSelected) {
				isSpherePreset[i] = true;
				saved_isSpherePreset[i] = isSpherePreset[i];
				spheres[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_sphere++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_SPHERE);
			} else if (!cubes[i].activeSelf && isCubePresetSelected) {
				isCubePreset[i] = true;
				saved_isCubePreset[i] = isCubePreset[i];
				cubes[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_cube++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_CUBE);
			} else if (!cylinders[i].activeSelf && isCylinderPresetSelected) {
				isCylinderPreset[i] = true;
				saved_isCylinderPreset[i] = isCylinderPreset[i];
				cylinders[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_cyl++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_CYL);
			}
		}

		if (!isCylinderPresetSelected && cylinders[i].activeSelf) {
			isCylinderPreset[i] = false;
			saved_isCylinderPreset[i] = isCylinderPreset[i];
			cylinders[i].SetActive(false); 
			temp_count_cyl--;
			if (!spheres[i].activeSelf && isSpherePresetSelected) {
				isSpherePreset[i] = true;
				saved_isSpherePreset[i] = isSpherePreset[i];
				spheres[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_sphere++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_SPHERE);
			} else if (!cubes[i].activeSelf && isCubePresetSelected) {
				isCubePreset[i] = true;
				saved_isCubePreset[i] = isCubePreset[i];
				cubes[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_cube++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_CUBE);
			} else if (!tetras[i].activeSelf && isTetraPresetSelected) {
				isTetraPreset[i] = true;
				saved_isTetraPreset[i] = isTetraPreset[i];
				tetras[i].SetActive(true);
				if (isBlockOn[i])
					checkActiveShapesMat(i, mat_on);
				temp_count_tetra++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_TETRA);
			}
		}

		//Debug.Log(isCubePreset[i] + "\t" + isSpherePreset[i] + "\t" + isTetraPreset[i] + "\t" + isCylinderPreset[i]);
	}

	private bool triggerPokeChild (float x, float y, float z)
	{
		Vector3 size = new Vector3 (0, 0, 0);
		Vector3 position = new Vector3 (0, 0, 0);
		for (int i = 0; i < count_c; i++) { //each coordinate of the pinch is matched to each coordinate of each grid space; bounds.size gets the actual size of each grid space as to get its edges when pinching
			size = checkActiveShapesSize(i);
			position = checkActiveShapesPosition(i);
			if (x >= (position.x - (size.x / 2)) && x < (position.x + (size.x / 2))) {
				if (y >= (position.y - (size.y / 2)) && y < (position.y + (size.y / 2))) {
					if (z >= (position.z - (size.z / 2)) && z < (position.z + (size.z / 2))) {
						return true;
					}
				}
			}
		}

		return false;
	}

	//check which shape is currently active in each grid space, and assign the size corresponding to the active shape 
	private Vector3 checkActiveShapesSize(int i) {
		Vector3 s = new Vector3(0,0,0);
		if (cubes[i].activeSelf) {
			s = cubes[i].GetComponent<Renderer>().bounds.size;
		} else if (spheres[i].activeSelf) {
			s = spheres[i].GetComponent<Renderer>().bounds.size;
		} else if (tetras[i].activeSelf) {
			s = tetras[i].GetComponent<Renderer>().bounds.size;
		} else if (cylinders[i].activeSelf) {
			s = cylinders[i].GetComponent<Renderer>().bounds.size;
		}

		return s;
	}

	//check which shape is currently active in each grid space, and assign the position corresponding to the active shape 
	private Vector3 checkActiveShapesPosition(int i) {
		Vector3 p = new Vector3(0,0,0);
		if (cubes[i].activeSelf) {
			p = cubes[i].transform.position;
		} else if (spheres[i].activeSelf) {
			p = cubes[i].transform.position;
		} else if (tetras[i].activeSelf) {
			p = cubes[i].transform.position;
		} else if (cylinders[i].activeSelf) {
			p = cubes[i].transform.position;
		}

		return p;
	}

	private void selectMoveChild (float x, float y, float z)
	{
		Vector3 size = new Vector3 (1, 1, 1);
		Vector3 position = new Vector3 (0, 0, 0);
		for (int i = 0; i < count_s; i++) { //each coordinate of the pinch is matched to each coordinate of each grid space; bounds.size gets the actual size of each grid space as to get its edges when pinching
			size = checkActiveShapesSize(i);
			position = checkActiveShapesPosition(i);

			if (x >= (position.x - (size.x / 2)) && x < (position.x + (size.x / 2))) {
				if (y >= (position.y - (size.y / 2)) && y < (position.y + (size.y / 2))) {
					if (z >= (position.z - (size.z / 2)) && z < (position.z + (size.z / 2))) {
						if (isBlockOn [i] && !isMoveBlockSelect [i]) {
							if (isSizeBlockSelect [i]) { //deselect the cube from resize function to move function and reset its timer
								isSizeBlockSelect [i] = false;
								timeLeftSZ [i] = TIME;
							} else if (isRotateBlockSelect [i]) { //deselect cube from rotate function to move function and reset its timer
								isRotateBlockSelect [i] = false;
								timeLeftRT [i] = TIME;
							}

							count_move++;
							checkActiveShapesMat(i, mat_sel_move);
							isMoveBlockSelect [i] = true;
						} 
					}
				}
			}
		}
	}

	//check which shape is currently active and change its material
	//important to distinguish between cubes/spheres and tetras/cylinders
	//since tetra/cylinders are child GOs while spheres/cubes are parent GOs
	private void checkActiveShapesMat(int i, Material mat) {
		if (cubes[i].activeSelf) {
			cubes[i].transform.GetComponent<Renderer>().material = mat;
		} else if (spheres[i].activeSelf) {
			spheres[i].transform.GetComponent<Renderer>().material = mat;
		} else if (tetras[i].activeSelf) {
			tetras[i].transform.GetChild (0).GetComponent<Renderer> ().material = mat;
		} else if (cylinders[i].activeSelf) {
			cylinders[i].transform.GetChild (0).GetComponent<Renderer> ().material = mat;
		}
	}

	private void selectSizeChild (float x, float y, float z)
	{
		Vector3 size = new Vector3 (0, 0, 0);
		Vector3 position = new Vector3 (0, 0, 0);

		for (int i = 0; i < count_s; i++) { //each coordinate of the poke is matched to each coordinate of each grid space; bounds.size gets the actual size of each grid space as to get its edges when pinching
			position = checkActiveShapesPosition(i);
			size = checkActiveShapesSize(i);
			
			if (x >= (position.x - (size.x / 2)) && x < (position.x + (size.x / 2))) {
				if (y >= (position.y - (size.y / 2)) && y < (position.y + (size.y / 2))) {
					if (z >= (position.z - (size.z / 2)) && z < (position.z + (size.z / 2))) {
						if (isBlockOn [i] && !isSizeBlockSelect [i]) {
							if (isMoveBlockSelect [i]) { //deselect the cube from the move function to resize function and reset its timer
								isMoveBlockSelect [i] = false;
								timeLeftMV [i] = TIME;
							} else if (isRotateBlockSelect [i]) { //deselect cube from rotate function to resize function and reset its timer
								isRotateBlockSelect [i] = false;
								timeLeftRT [i] = TIME;
							}
							count_scale++;
							checkActiveShapesMat(i, mat_sel_size);
							isSizeBlockSelect [i] = true;
							temp_isSizeSelect[i] = isSizeBlockSelect[i];
						} 
					}
				}
			}
		}
	}

	private void selectRotateChild (float x, float y, float z)
	{
		Vector3 size = new Vector3 (0, 0, 0);
		Vector3 position = new Vector3 (0, 0, 0);

		for (int i = 0; i < count_c; i++) { //each coordinate of the poke is matched to each coordinate of each grid space; bounds.size gets the actual size of each grid space as to get its edges when pinching

			position = checkActiveShapesPosition(i);
			size = checkActiveShapesSize(i);
			
			if (x >= (position.x - (size.x / 2)) && x < (position.x + (size.x / 2))) {
				if (y >= (position.y - (size.y / 2)) && y < (position.y + (size.y / 2))) {
					if (z >= (position.z - (size.z / 2)) && z < (position.z + (size.z / 2))) {
						if (isBlockOn [i] && !isRotateBlockSelect [i]) {
							if (isMoveBlockSelect [i]) { //deselect the cube from the move function to rotate function and reset its timer
								isMoveBlockSelect [i] = false;
								timeLeftMV [i] = TIME;
							} else if (isSizeBlockSelect [i]) { //deselect cube from resize function to rotate function and reset its timer
								isSizeBlockSelect [i] = false;
								timeLeftRT [i] = TIME;
							}
							count_rotate++;
							checkActiveShapesMat(i, mat_sel_rotate);
							isRotateBlockSelect [i] = true;
						} 
					}
				}
			}
		}
	}

	private void moveChilds(float z)
	{
		string name = "";
		for (int i = 0; i < count_y; i++) { //each coordinate of the poke is matched to each coordinate of each grid space; bounds.size gets the actual size of each grid space as to get its edges when pinching
			if (isMoveBlockSelect [i]) { //move selected blocks to new z-coordinate
				if (MenuMove.isPositionMode) {
					if (z >= Z_MIN && z < Z_MAX) {
						name = activeShapeMessageName(i);
						string message = "/move" + name;
						OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, z);
						moveActiveShapes(i, z);
						saved_zcoors [i] = z;
					}

					if (timeLeftMV [i] < 0) {
						count_move--;
						checkActiveShapesMat(i, mat_on);
						isMoveBlockSelect [i] = false;
						timeLeftMV [i] = TIME;
					}
				}
			}
		}

		if (count_move == 0)
			_mm.turnOffOtherMode("Position");
	}

	//check what shape is active and change its position, while inheriting that position to inactive shapes in that coordinate
	private void moveActiveShapes(int i, float z) {
		if (cubes[i].activeSelf) {
			cubes[i].transform.position = new Vector3 (cubes[i].transform.position.x, cubes[i].transform.position.y, z);
			inheritShapePosition(cubes[i], spheres[i], tetras[i], cylinders[i]);
		} else if (spheres[i].activeSelf) {
			spheres[i].transform.position = new Vector3 (spheres[i].transform.position.x, spheres[i].transform.position.y, z);
			inheritShapePosition(spheres[i], cubes[i], tetras[i], cylinders[i]);
		} else if (tetras[i].activeSelf) {
			tetras[i].transform.position = new Vector3 (tetras[i].transform.position.x, tetras[i].transform.position.y, z);
			inheritShapePosition(tetras[i], spheres[i], cubes[i], cylinders[i]);
		} else if (cylinders[i].activeSelf) {
			cylinders[i].transform.position = new Vector3 (cylinders[i].transform.position.x, cylinders[i].transform.position.y, z);
			inheritShapePosition(cylinders[i], spheres[i], tetras[i], cubes[i]);
		}
	}

	//inherit position from GO one to other inactive GOs
	private void inheritShapePosition(GameObject one, GameObject two, GameObject three, GameObject four) {
		two.transform.position = new Vector3(one.transform.position.x, one.transform.position.y, one.transform.position.z);
		three.transform.position = new Vector3(one.transform.position.x, one.transform.position.y, one.transform.position.z);
		four.transform.position = new Vector3(one.transform.position.x, one.transform.position.y, one.transform.position.z);
	}

	//get the active shape's substring name and use it for sending OSC messages
	private string activeShapeMessageName(int i) {
		string name = "";
		if (cubes[i].activeSelf) {
			name = cubes[i].name.Substring(1);
		} else if (spheres[i].activeSelf) {
			name = spheres[i].name.Substring(1);
		} else if (tetras[i].activeSelf) {
			name = tetras[i].transform.GetChild(0).name.Substring(1);
		} else if (cylinders[i].activeSelf) {
			name = cylinders[i].transform.GetChild(0).name.Substring(1);
		}

		return name;
	}
		
	private void deselectMoveChilds ()
	{
		for (int i = 0; i < count_s; i++) {
			if (!MenuMove.isPositionMode && isMoveBlockSelect [i]) {
				count_move--;
				checkActiveShapesMat(i, mat_on);
				isMoveBlockSelect [i] = false;
				timeLeftMV [i] = TIME;
			}
		}
	}

	private void resizeChilds (float z)
	{
		string name = "";
		for (int i = 0; i < count_c; i++) {
			if (isSizeBlockSelect [i]) {
				if (MenuMove.isScaleMode) {
					if (z >= SIZE_MIN && z < SIZE_MAX) {
						name = activeShapeMessageName(i);
						string message = "/size" + name;
						OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, z);
						resizeActiveShapes(i, z);
						saved_sizes [i] = z;
					}

					if (timeLeftSZ [i] < 0) {
						count_scale--;
						checkActiveShapesMat(i, mat_on);
						isSizeBlockSelect [i] = false;
						timeLeftSZ [i] = TIME;
					}
				}
			}
		}

		if (count_scale == 0)
			_mm.turnOffOtherMode("Scaling");
	}

	private void resizeActiveShapes (int i, float z) {

		if (cubes[i].activeSelf) {
			cubes[i].transform.localScale = new Vector3(z, z, z);
			inheritShapeSize(cubes[i], spheres[i], tetras[i], cylinders[i]);
		} else if (spheres[i].activeSelf) {
			spheres[i].transform.localScale = new Vector3(z, z, z);
			inheritShapeSize(spheres[i], cubes[i], tetras[i], cylinders[i]);
		} else if (tetras[i].activeSelf) {
			tetras[i].transform.localScale = new Vector3(z, z, z);
			inheritShapeSize(tetras[i], spheres[i], cubes[i], cylinders[i]);
		} else if (cylinders[i].activeSelf) {
			cylinders[i].transform.localScale = new Vector3(z, z, z);
			inheritShapeSize(cylinders[i], spheres[i], tetras[i], cubes[i]);
		}
	}

	private void inheritShapeSize(GameObject one, GameObject two, GameObject three, GameObject four) {
		two.transform.localScale = new Vector3(one.transform.localScale.x, one.transform.localScale.y, one.transform.localScale.z);
		three.transform.localScale = new Vector3(one.transform.localScale.x, one.transform.localScale.y, one.transform.localScale.z);
		four.transform.localScale = new Vector3(one.transform.localScale.x, one.transform.localScale.y, one.transform.localScale.z);
	}

	private void deselectResizeChilds ()
	{
		for (int i = 0; i < count_c; i++) {
			if (!MenuMove.isScaleMode && isSizeBlockSelect [i]) {
				count_scale--;
				checkActiveShapesMat(i, mat_on);
				isSizeBlockSelect [i] = false;
				timeLeftMV [i] = TIME;
			}
		}
	}

	private void rotateChilds (float y)
	{
		string name = "";
		for (int i = 0; i < count_s; i++) {

			if (isRotateBlockSelect [i]) {
				if (MenuMove.isRotateMode) {
					if (y >= ROT_MIN && y < ROT_MAX) {
						rotateActiveShapes(i, y);
						float y_ = getActiveShapeRotation(i);
						int ii = 0;
						if (y_ >= -1.0f && y_ < -0.3f)
							ii = -1;
						else if (y_ >= -0.3f && y_ < 0.3f)
							ii = 0;
						else if (y_ >= 0.3f && y_ < 1.0f)
							ii = 1;
						name = activeShapeMessageName(i);
						string message = "/rotate" + name;
						OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, ii);
						temp_RotateMaxMSP[i] = ii;
					}

					if (timeLeftRT [i] < 0) {
						count_rotate--;
						checkActiveShapesMat(i, mat_on);
						isRotateBlockSelect [i] = false;
						timeLeftRT [i] = TIME;
					}
				}
			}
		}

		if (count_rotate == 0)
			_mm.turnOffOtherMode("Rotation");
	}

	private void rotateActiveShapes(int i, float y) {
		if (cubes[i].activeSelf) {
			cubes[i].transform.Rotate(0, y, 0);
			saved_rotate[i] = cubes[i].transform.rotation;
			inheritShapeRotation(cubes[i], spheres[i], tetras[i], cylinders[i]);
		} else if (spheres[i].activeSelf) {
			spheres[i].transform.Rotate(0, y, 0);
			saved_rotate[i] = spheres[i].transform.rotation;
			inheritShapeRotation(spheres[i], cubes[i], tetras[i], cylinders[i]);
		} else if (tetras[i].activeSelf) {
			tetras[i].transform.Rotate(0, y, 0);
			saved_rotate[i] = tetras[i].transform.rotation;
			inheritShapeRotation(tetras[i], spheres[i], cubes[i], cylinders[i]);
		} else if (cylinders[i].activeSelf) {
			cylinders[i].transform.Rotate(0, y, 0);
			saved_rotate[i] = cylinders[i].transform.rotation;
			inheritShapeRotation(cylinders[i], spheres[i], tetras[i], cubes[i]);
		}
	}

	private void inheritShapeRotation(GameObject one, GameObject two, GameObject three, GameObject four) {
		two.transform.rotation = Quaternion.Slerp(two.transform.rotation, one.transform.rotation, 1.0f);
		three.transform.rotation = Quaternion.Slerp(three.transform.rotation, one.transform.rotation, 1.0f);
		four.transform.rotation = Quaternion.Slerp(four.transform.rotation, one.transform.rotation, 1.0f);
	}

	private float getActiveShapeRotation(int i) {
		float f = 0.0f;
		if (cubes[i].activeSelf)
			f = cubes[i].transform.rotation.y;
		else if (spheres[i].activeSelf)
			f = spheres[i].transform.rotation.y;
		else if (tetras[i].activeSelf)
			f = tetras[i].transform.rotation.y;
		else if (cylinders[i].activeSelf)
			f = cylinders[i].transform.rotation.y;

		return f;
	}

	private void deselectRotateChilds ()
	{
		for (int i = 0; i < count_y; i++) {
			if (!MenuMove.isRotateMode && isRotateBlockSelect [i]) {
				count_rotate--;
				checkActiveShapesMat(i, mat_on);
				isRotateBlockSelect [i] = false;
				timeLeftMV [i] = TIME;
			}
		}
	}

	private void resetPresetChild ()
	{
		if (isSpherePresetSelected)
			isSpherePresetSelected = false;
		else if (isTetraPresetSelected)
			isTetraPresetSelected = false;
		else if (isCylinderPresetSelected)
			isCylinderPresetSelected = false;

		if (!isCubePresetSelected)
			isCubePresetSelected = true;

		shape_left.setCube();
		no_shape_left.setCube();
		shape_right.setNoneRight();
		no_shape_right.setNoneRight();
		
		for (int i = 0; i < count_c; i++) {
			if (spheres[i].activeSelf) {
				spheres[i].SetActive (false);
				if (isSpherePreset[i])
					isSpherePreset[i] = false;
			} else if (tetras[i].activeSelf) {
				tetras[i].SetActive(false);
				if (isTetraPreset[i])
					isTetraPreset[i] = false;
			} else if (cylinders[i].activeSelf) {
				cylinders[i].SetActive(false);
				if (isCylinderPreset[i])
					isCylinderPreset[i] = false;
			}

			if (!cubes[i].activeSelf) {
				cubes[i].SetActive (true);
				if (!isCubePreset[i])
					isCubePreset[i] = false;
			}

			checkActiveShapesMat(i, mat_off);
			cubes[i].transform.position = new Vector3 (cubes[i].transform.position.x, cubes[i].transform.position.y, Z_COORDINATE);
			inheritShapePosition(cubes[i], spheres[i], tetras[i], cylinders[i]);
			cubes[i].transform.localScale = new Vector3 (SIZE, SIZE, SIZE);
			inheritShapeSize(cubes[i], spheres[i], tetras[i], cylinders[i]);
			cubes[i].transform.rotation = Quaternion.Slerp (transform.rotation, ROTATE, 1.0f);
			inheritShapeRotation(cubes[i], spheres[i], tetras[i], cylinders[i]);
			string msg = "/shape" + activeShapeMessageName(i);
			OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_CUBE);
		}
	}
		
	private void savedSceneChild()
	{
		string name = "";
		isGridCleared = false;
		for (int i = 0; i < count_c; i++) {

//			Debug.Log("saved_cubes[" + i + "] " + saved_isCubePreset[i]);
//			Debug.Log("saved_spheres[" + i + "] " + saved_isSpherePreset[i]);
//			Debug.Log("saved_tetras[" + i + "] " + saved_isTetraPreset[i]);
//			Debug.Log("saved_cylinders[" + i + "] " + saved_isCylinderPreset[i]);

			if (saved_isSpherePreset[i]) {
				isSpherePreset[i] = saved_isSpherePreset[i];
				spheres[i].SetActive(true);
				temp_count_sphere++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_SPHERE);
			} else if (saved_isTetraPreset[i]) {
				isTetraPreset[i] = saved_isTetraPreset[i];
				tetras[i].SetActive(true);
				temp_count_tetra++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_TETRA);
			} else if (saved_isCylinderPreset[i]) {
				isCylinderPreset[i] = saved_isCylinderPreset[i];
				cylinders[i].SetActive(true);
				temp_count_cyl++;
				string msg = "/shape" + activeShapeMessageName(i);
				OSCHandler.Instance.SendMessageToClient("MaxMSP", msg, PRESET_CYL);
			}

			if (!saved_isCubePreset[i]) {
				isCubePreset[i] = saved_isCubePreset[i];
				cubes[i].SetActive(false);
				temp_count_cube--;
			}

			name = activeShapeMessageName(i);

			if (saved_isBlockOn [i]) { //send OSC message to Max if cube was on in last session
				float on = 1.0f;
				string message = "/on" + name;
				OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, on);
				checkActiveShapesMat(i, mat_on);
			}

			isBlockOn [i] = saved_isBlockOn [i];
			retrieveSavedPositions(i);
			retrieveSavedSizes(i);
			retrieveSavedRotations(i);
			string message1 = "/move" + name;
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message1, saved_zcoors [i]);
			string message2 = "/size" + name;
			if (temp_isSizeSelect[i])
				OSCHandler.Instance.SendMessageToClient ("MaxMSP", message2, saved_sizes [i]);
			else if (!temp_isSizeSelect[i])
				OSCHandler.Instance.SendMessageToClient ("MaxMSP", message2, SIZE_DEFAULT_FOR_MAX_MSP);
			string message3 = "/rotate" + name;
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message3, temp_RotateMaxMSP[i]);
			//send OSC message for preset
		}

		//_mm.moveDown();
		int saved_tempo = _ft.getSavedMapped ();
		_tt.changeTempoText (saved_tempo.ToString ());
		string message4 = "/tempo";
		OSCHandler.Instance.SendMessageToClient ("MaxMSP", message4, saved_tempo);
		_mm.getSavedScale ();
	}

	private void retrieveSavedPositions(int i) { //get saved positions and assign them to active shapes, along with inactive shapes inheriting them
		if (cubes[i].activeSelf) {
			cubes[i].transform.position = new Vector3 (cubes[i].transform.position.x, cubes[i].transform.position.y, saved_zcoors [i]);
			inheritShapePosition(cubes[i], spheres[i], tetras[i], cylinders[i]);
		} else if (spheres[i].activeSelf) {
			spheres[i].transform.position = new Vector3 (spheres[i].transform.position.x, spheres[i].transform.position.y, saved_zcoors [i]);
			inheritShapePosition(spheres[i], cubes[i], tetras[i], cylinders[i]);
		} else if (tetras[i].activeSelf) {
			tetras[i].transform.position = new Vector3 (tetras[i].transform.position.x, tetras[i].transform.position.y, saved_zcoors [i]);
			inheritShapePosition(tetras[i], spheres[i], cubes[i], cylinders[i]);
		} else if (cylinders[i].activeSelf) {
			cylinders[i].transform.position = new Vector3 (cylinders[i].transform.position.x, cylinders[i].transform.position.y, saved_zcoors [i]);
			inheritShapePosition(cylinders[i], spheres[i], tetras[i], cubes[i]);
		}
	}

	private void retrieveSavedSizes(int i) { //get saved sizes and assign them to active shapes, along with inactive shapes inheriting them
		if (cubes[i].activeSelf) {
			cubes[i].transform.localScale = new Vector3 (saved_sizes [i], saved_sizes [i], saved_sizes [i]);
			inheritShapeSize(cubes[i], spheres[i], tetras[i], cylinders[i]);
		} else if (spheres[i].activeSelf) {
			spheres[i].transform.localScale = new Vector3 (saved_sizes [i], saved_sizes [i], saved_sizes [i]);
			inheritShapeSize(spheres[i], cubes[i], tetras[i], cylinders[i]);
		} else if (tetras[i].activeSelf) {
			tetras[i].transform.localScale = new Vector3 (saved_sizes [i], saved_sizes [i], saved_sizes [i]);
			inheritShapeSize(tetras[i], spheres[i], cubes[i], cylinders[i]);
		} else if (cylinders[i].activeSelf) {
			cylinders[i].transform.localScale = new Vector3 (saved_sizes [i], saved_sizes [i], saved_sizes [i]);
			inheritShapeSize(cylinders[i], spheres[i], tetras[i], cubes[i]);
		}
	}

	private void retrieveSavedRotations(int i) { //get saved rotations and assign them to active shapes, along with inactive shapes inheriting them
		if (cubes[i].activeSelf) {
			cubes[i].transform.rotation = Quaternion.Slerp (cubes[i].transform.rotation, saved_rotate [i], 1.0f);
			inheritShapeRotation(cubes[i], spheres[i], tetras[i], cylinders[i]);
		} else if (spheres[i].activeSelf) {
			spheres[i].transform.rotation = Quaternion.Slerp (spheres[i].transform.rotation, saved_rotate [i], 1.0f);
			inheritShapeRotation(spheres[i], cubes[i], tetras[i], cylinders[i]);
		} else if (tetras[i].activeSelf) {
			tetras[i].transform.rotation = Quaternion.Slerp (tetras[i].transform.rotation, saved_rotate [i], 1.0f);
			inheritShapeRotation(tetras[i], spheres[i], cubes[i], cylinders[i]);
		} else if (cylinders[i].activeSelf) {
			cylinders[i].transform.rotation = Quaternion.Slerp (cylinders[i].transform.rotation, saved_rotate [i], 1.0f);
			inheritShapeRotation(cylinders[i], spheres[i], tetras[i], cubes[i]);
		}
	}

	private void checkGridCleared(int length) {
		//check if the grid is cleared

		int count_true = 0;
		int count_z = 0;
		int count_s = 0;
		int count_r = 0;

		for (int i = 0; i < length; i++) {
			if (isBlockOn[i])
				count_true++;

			if (cubes[i].activeSelf) {
				if (cubes[i].transform.position.z == Z_COORDINATE)
					count_z++;

				if (cubes[i].transform.localScale.z == SIZE)
					count_s++;

				if (cubes[i].transform.rotation.y == ROTATE.y)
					count_r++;
			} else if (spheres[i].activeSelf) {
				if (spheres[i].transform.position.z == Z_COORDINATE)
					count_z++;

				if (spheres[i].transform.localScale.z == SIZE)
					count_s++;

				if (spheres[i].transform.rotation.y == ROTATE.y)
					count_r++;
			} else if (tetras[i].activeSelf) {
				if (tetras[i].transform.position.z == Z_COORDINATE)
					count_z++;

				if (tetras[i].transform.localScale.z == SIZE)
					count_s++;

				if (tetras[i].transform.rotation.y == ROTATE.y)
					count_r++;
			} else if (cylinders[i].activeSelf) {
				if (cylinders[i].transform.position.z == Z_COORDINATE)
					count_z++;

				if (cylinders[i].transform.localScale.z == SIZE)
					count_s++;

				if (cylinders[i].transform.rotation.y == ROTATE.y)
					count_r++;
			}
		}

//		Debug.Log(count_s + "\t" + count_z + "\t" + count_r);
//		Debug.Log((count_s == count_c && count_z == count_c && count_r == count_c));

		//if all the active shapes are cubes and they are all in their default positions, sizes, and rotations,
		//then the grid is considered cleared; otherwise, it is not
		if (temp_count_sphere == 0 && temp_count_tetra == 0 && temp_count_cyl == 0 && temp_count_cube == count_c
				&& count_true == 0 && (count_s == count_c && count_z == count_c && count_r == count_c)) { 
			clearSavedValues(length);
		} else {
			isGridCleared = false;
		}
	}

	private void clearSavedValues(int length) {
		//clear all saved values to default values
		//whenever someone starts a new session or if checkGridCleared calls it
		isGridCleared = true;
		isSaved = false;
		//_mm.moveUp();
		for (int i = 0; i < length; i++) {
			saved_isBlockOn [i] = false;
			saved_zcoors [i] = Z_COORDINATE;
			saved_sizes [i] = SIZE;
			saved_rotate [i] = ROTATE;
			temp_RotateMaxMSP[i] = 0;
			_ft.setMapped(TEMPO_DEF);
			_ft.setSavedMapped(TEMPO_DEF);
			string message = "/tempo";
			OSCHandler.Instance.SendMessageToClient ("MaxMSP", message, TEMPO_DEF);
			_mm.resetSavedScale();

			if (!saved_isCubePreset[i]) {
				isCubePreset[i] = true;
				saved_isCubePreset[i] = isCubePreset[i];
			}

			if (saved_isSpherePreset[i]) {
				isSpherePreset[i] = false;
				saved_isSpherePreset[i] = isSpherePreset[i];
			}

			if (saved_isTetraPreset[i]) {
				isTetraPreset[i] = false;
				saved_isTetraPreset[i] = isTetraPreset[i];
			}

			if (saved_isCylinderPreset[i]) {
				isCylinderPreset[i] = false;
				saved_isCylinderPreset[i] = isCylinderPreset[i];
			}

			if (temp_isSizeSelect[i])
				temp_isSizeSelect[i] = !temp_isSizeSelect[i];
		}
	}
}
