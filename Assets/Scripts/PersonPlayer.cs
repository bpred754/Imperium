using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonPlayer : Player
{
	// Constants
	public const string BUILDING = "Building";

	// Variables linked from scene
	public Texture2D selectionHighlight;
	public Building building; // Prefab (Building)


	// Logic variables
	private Camera camera;
	private Vector3 startClick = -Vector3.one; 
	private Rect selection = new Rect (0,0,0,0);
	private Dictionary<string, Unit> units = new Dictionary<string, Unit> ();
	private Dictionary<string, Building> buildings = new Dictionary<string, Building> ();
	private int unitCount = 0;
	private int buildingCount = 0;
	private List<Unit> selectedUnitsList = new List<Unit>();
	private float maxWorldX = 0;
	private float minWorldX = 0;
	private float maxWorldZ = 0;
	private float minWorldZ = 0;
	private LayerMask layer;

	// GUI Prefabs
	private GameObject minimapPrefab;
	private GameObject tabsPrefab;
	private GameObject buildingListPrefab;
	private GameObject unitListPrefab;

	// GUI variables
	private float guiScreenWidth = 0;
	private float guiWorldWidth = 0;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	// Called when PersonPlayer is instantiated
	private void Start() {

		// Load Resources
		minimapPrefab = Resources.Load<GameObject> ("MiniMap");
		tabsPrefab = Resources.Load<GameObject> ("GUITabs");
		buildingListPrefab = Resources.Load<GameObject> ("GUIBuildingList");
		unitListPrefab = Resources.Load<GameObject> ("GUIUnitList");

		camera = GetComponent<Camera> ();

		// Add Event System to scene
		if (FindObjectOfType<EventSystem>() == null)
		{
			GameObject es = new GameObject("EventSystem", typeof(EventSystem));
			es.AddComponent<StandaloneInputModule>();
		}
		
		// Add canvas to scene
		var canvasObject = new GameObject("Canvas");
		var canvas = canvasObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		var graphicRayCaster = canvasObject.AddComponent<GraphicRaycaster> ();
		
		// Add minimap to canvas
		GameObject minimap = (GameObject)Instantiate(minimapPrefab);
		minimap.transform.SetParent(canvas.transform, false);
		
		// Add tabs to canvas
		GameObject tabs = (GameObject)Instantiate(tabsPrefab);
		tabs.transform.SetParent(canvas.transform, false);
		Button buildingsTab = GameObject.Find ("BuildingsTab").GetComponentInChildren<Button>();
		Button unitsTab = GameObject.Find ("UnitsTab").GetComponentInChildren<Button>();
		selectTab (buildingsTab);
		unselectTab (unitsTab);

		// Add buildingList to prefab
		GameObject buildingList = (GameObject)Instantiate(buildingListPrefab);
		buildingList.transform.SetParent(canvas.transform, false);
		buildingList.SetActive(true);

		// Add unit List to prefab
		GameObject unitList = (GameObject)Instantiate(unitListPrefab);
		unitList.transform.SetParent(canvas.transform, false);
		unitList.SetActive(false);
		
		// Add listeners to tabs
		buildingsTab.onClick.RemoveAllListeners ();
		buildingsTab.onClick.AddListener (() => buildingsTabListener(buildingsTab, unitsTab, buildingList, unitList));
		
		unitsTab.onClick.RemoveAllListeners();
		unitsTab.onClick.AddListener(() => unitTabListener (buildingsTab, unitsTab, buildingList, unitList));
		
		// TODO: Add building/unit listing button listeners

		// Set GUI width variable
		RectTransform tabTransform = tabs.GetComponent<RectTransform>();
		this.guiScreenWidth = tabTransform.rect.width;

		// Set GUI world width variable
		float left = camera.ScreenToWorldPoint (new Vector3(0,0, 27.66f)).x;
		float right = camera.ScreenToWorldPoint (new Vector3(90,0, 27.66f)).x;
		this.guiWorldWidth = right - left;
	}

	// Called every frame
	private void Update() {
		checkPlayerClick();
		checkPlayerSelection();
		moveCamera();
	}

	// Called several times per frame when needed
	private void OnGUI()
	{
		if (startClick != -Vector3.one) {
			GUI.color = new Color(1,1,1,0.5f);
			GUI.DrawTexture(selection,selectionHighlight);
		}
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	public void createBuilding(Vector3 position) {
		Building buildingObject = (Building)Instantiate(building, position, Quaternion.identity);
		buildingObject.initialize (position, buildingCount);
		string buildingName = BUILDING + buildingCount;
		buildingObject.name = buildingName;
		buildings.Add(buildingName, buildingObject);
		buildingCount++;
	}

	public void setCameraBoundaries(float maxX, float minX, float maxZ, float minZ) {
		this.maxWorldX = maxX;
		this.minWorldX = minX;
		this.maxWorldZ = maxZ;
		this.minWorldZ = minZ;
	}

	/*********************************************************************************/
	/*	Private Logic Functions - Order: Alphabetic							 	     */		
	/*********************************************************************************/

	// Executes logic when user clicks
	private void checkPlayerClick() {

		bool isControlPressed = false;
		if (Input.GetKey (KeyCode.LeftControl)) {
			isControlPressed = true;
		}
		
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				/*Debug.Log ("Name = " + hit.collider.name);
				Debug.Log ("Tag = " + hit.collider.tag);
				Debug.Log ("Hit Point = " + hit.point);
				Debug.Log ("Object position = " + hit.collider.gameObject.transform.position);
				Debug.Log ("Object bounds = " + hit.collider.bounds.size);
				Debug.Log ("--------------");*/

				// If building is clicked create new unit
				if (buildings.ContainsKey (hit.collider.name)) {
					Unit newUnit = buildings [hit.collider.name].createUnit (this.team);
					if (newUnit != null) {
						units.Add (newUnit.name, newUnit);
						unitCount++;
					}
				}
				
				// If a unit is clicked unselect other units
				if (units.ContainsKey (hit.collider.name)) {
					foreach (KeyValuePair<string, Unit> entry in units) {
						Unit unit = entry.Value;
						if (entry.Key == hit.collider.name) {
							unit.setSelected (true);
						} else {
							// Don't unselect units if the control key is pressed
							if(!isControlPressed) {
								unit.setSelected (false);
							}
						}
					}
				}
				
				// If the ground is clicked unselect units
				if (hit.collider.name == "Ground") {
					if(!isControlPressed){
						foreach (KeyValuePair<string,Unit> entry in units) {
							Unit unit = entry.Value;
							unit.setSelected (false);
						}
					}
				}
			}
		} //If right click is pressed down, i.e. move order is made
		else if (Input.GetMouseButtonDown (1)) {

			// Formations
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Casts the ray and get the first game object hit
			Physics.Raycast(ray, out hit);
			layer = hit.collider.gameObject.layer;
			createFormation("Shell", Input.mousePosition);
		}
	}

	// Executes logic when user clicks and drags (creates selection box)
	private void checkPlayerSelection() {

		// Detect mouse click
		if(Input.GetMouseButtonDown(0)) {
			startClick = Input.mousePosition;
		} else if (Input.GetMouseButtonUp(0)) {
			startClick = -Vector3.one;
		}
		
		// Determine selection box area
		if (Input.GetMouseButton (0)) {
			selection = new Rect (startClick.x, InvertMouseY (startClick.y), Input.mousePosition.x - startClick.x, InvertMouseY (Input.mousePosition.y) - InvertMouseY (startClick.y));
			if (selection.width < 0) {
				selection.x += selection.width;
				selection.width = -selection.width;
			}
			if (selection.height < 0) {
				selection.y += selection.height;
				selection.height = -selection.height;
			}

			// Only execute logic when the user creates a selection box, not when they click
			if (selection.height > 1) {
				foreach (KeyValuePair<string,Unit> entry in units) {
					Unit unit = entry.Value;
					if (unit.isVisible () && Input.GetMouseButton (0)) {
						Vector3 camPos = Camera.main.WorldToScreenPoint (unit.transform.position);
						camPos.y = InvertMouseY (camPos.y);
						unit.setSelected (selection.Contains (camPos));
					}			
				}
			}
		}

		// Loop through buildings and select the buildings that are in selection area
		/*foreach(KeyValuePair<string,Building> entry in buildings) {
			Building building = entry.Value;
			if(building.isVisible() && Input.GetMouseButton(0)) {
				Vector3 camPos = Camera.main.WorldToScreenPoint(building.transform.position);
				camPos.y = InvertMouseY(camPos.y);
				building.setSelected(selection.Contains(camPos));
			}			
		}*/
	}
	
	//Creates formation, takes string:formationName
	private void createFormation(string formationName, Vector3 mousePosition){
		if(getNumUnitsSelected() > 1){
			Vector3 movePosition = mousePosition;
			int numberUnits = getNumUnitsSelected ();
			int unitSpace = 15;
			if (formationName == "Square" || formationName == "Squared") {
				float side = Mathf.Sqrt (numberUnits);
				foreach (Unit unit in selectedUnitsList) {
					giveMoveOrder (movePosition, unit);
					movePosition.x += unitSpace;
					if (movePosition.x >= mousePosition.x + unitSpace * side) {
						movePosition.x = mousePosition.x;
						movePosition.y -= unitSpace;
					}
				}
			} else if (formationName == "Shell" || formationName == "Shelled") {
				int circumference = unitSpace * numberUnits * 2;
				float radius = circumference / (Mathf.PI * 2);
				float degreeOffset = 360 / numberUnits;
				float radOffset = (degreeOffset * Mathf.PI) / 180;
				float radianOffset = 0;
				
				foreach (Unit unit in selectedUnitsList) {
					movePosition = mousePosition;
					
					movePosition.x += radius * Mathf.Sin (radianOffset);
					movePosition.y += radius * Mathf.Cos (radianOffset);
					
					giveMoveOrder (movePosition, unit);
					
					radianOffset += radOffset;
				}
			}
		}else{ //If only one unit is selected
			foreach (Unit unit in selectedUnitsList) {
				giveMoveOrder (mousePosition, unit);
			}
		}
	}

	private int getNumUnitsSelected(){
		
		// Clears the list to make way for new units
		selectedUnitsList.Clear ();
		
		foreach (KeyValuePair<string,Unit> entry in units) {
			Unit unit = entry.Value;
			if(unit.getSelected()){
				selectedUnitsList.Add(unit);
			}
		}
		return selectedUnitsList.Count;
	}

	// Takes a unit and a move order and moves unit to that location
	private void giveMoveOrder(Vector3 moveOrder, Unit unit){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(moveOrder);

		// Casts the ray and get the first game object hit
		Physics.Raycast(ray, out hit);

		unit.makeMove (hit);
	}
	
	// Inverts the Y component of the mouse vector
	private static float InvertMouseY(float y)
	{
		return Screen.height - y;
	}
	
	// Executes logic when the user moves the camera
	private void moveCamera() {
		int screenScrollLimit = Screen.height / 7;
		float scrollRate = 0.4f;
		Vector3 movementVectorX;
		Vector3 movementVectorZ;

		float cameraTop = camera.ScreenToWorldPoint (new Vector3 (0, camera.pixelHeight, 27.66f)).z;
		float cameraBottom = camera.ScreenToWorldPoint (new Vector3 (0, 0, 27.66f)).z;
		float cameraRight = camera.ScreenToWorldPoint (new Vector3 (camera.pixelWidth, 0, 27.66f)).x;
		float cameraLeft = camera.ScreenToWorldPoint (new Vector3 (0, 0, 27.66f)).x;

		// Scrolling with the mouse as it enters edges of screen
		if (Input.mousePosition.y < screenScrollLimit && Input.mousePosition.y >= 0 && Input.mousePosition.x <= Screen.width - this.guiScreenWidth && cameraBottom > this.minWorldZ) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		} else if (Input.mousePosition.y > Screen.height - screenScrollLimit && Input.mousePosition.y <= Screen.height && Input.mousePosition.x <= Screen.width - this.guiScreenWidth && cameraTop < this.maxWorldZ) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		}
		if (Input.mousePosition.x < screenScrollLimit && Input.mousePosition.x >= 0  && cameraLeft > this.minWorldX) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.mousePosition.x > Screen.width - screenScrollLimit - this.guiScreenWidth && Input.mousePosition.x <= Screen.width - this.guiScreenWidth && cameraRight - guiWorldWidth < this.maxWorldX) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}

		// Scrolling with the arrow keys 
		if (Input.GetKey ("up") && !Input.GetKey ("down") && cameraTop < this.maxWorldZ) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		} else if (Input.GetKey ("down") && !Input.GetKey ("up") && cameraBottom > this.minWorldZ) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		}
		if (Input.GetKey ("left") && !Input.GetKey ("right") && cameraLeft > this.minWorldX) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.GetKey ("right") && !Input.GetKey ("left") && cameraRight - guiWorldWidth < this.maxWorldX) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}
	}

	/*********************************************************************************/
	/*	Private GUI Functions - Order: Alphabetic							 	     */		
	/*********************************************************************************/

	private void buildingsTabListener(Button buildingTab, Button unitTab, GameObject buildingList, GameObject unitList) {
		
		// Deactivate Unit listing prefab
		unitList.SetActive(false);
		unselectTab (unitTab);
		
		// Activate Buildings listing prefab
		buildingList.SetActive(true);
		selectTab (buildingTab);
		
	}

	private void selectTab(Button tab) {
		var selectedColor = tab.colors;
		selectedColor.normalColor =  new Color32(71, 71, 197, 255);
		selectedColor.pressedColor = new Color32(71, 71, 197, 255);
		selectedColor.highlightedColor = new Color32(71, 71, 197, 255);
		selectedColor.disabledColor = new Color32(71, 71, 197, 255);
		tab.colors = selectedColor;
		
		Text tabText = tab.GetComponentInChildren<Text> ();
		tabText.color = Color.white;
	}
	
	private void unitTabListener (Button buildingTab, Button unitTab, GameObject buildingList, GameObject unitList) {
		
		// Deactivate Buildings listing prefab
		buildingList.SetActive(false);
		unselectTab (buildingTab);
		
		// Activate Units listing prefab
		unitList.SetActive(true);
		selectTab (unitTab);
	}

	private void unselectTab(Button tab) {
		var selectedColor = tab.colors;
		selectedColor.normalColor = Color.grey;
		selectedColor.pressedColor = Color.grey;
		selectedColor.highlightedColor = Color.grey;
		selectedColor.disabledColor = Color.grey;
		tab.colors = selectedColor;
		
		Text tabText = tab.GetComponentInChildren<Text> ();
		tabText.color = Color.black;
	}
}

