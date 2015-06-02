using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonPlayer : Player
{
	// Constants
	public const string BUILDING = "Building";

	// Variables linked from scene
	public Texture2D selectionHighlight;
	public Building building; // Prefab (Building)

	// Logic variables
	private Vector3 startClick = -Vector3.one; 
	private Rect selection = new Rect (0,0,0,0);
	private Dictionary<string, Unit> units = new Dictionary<string, Unit> ();
	private Dictionary<string, Building> buildings = new Dictionary<string, Building> ();
	private int unitCount = 0;
	private int buildingCount = 0;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	// Called when PersonPlayer is instantiated
	private void Start() {}

	// Called every frame
	private void Update() {
		checkPlayerClick();
		checkPlayerSelection();
		moveCamera();
		//moveUnits ();
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
		buildingObject.initialize (position);
		string buildingName = BUILDING + buildingCount;
		buildingObject.name = buildingName;
		buildings.Add(buildingName, buildingObject);
		buildingCount++;
	}

	/*********************************************************************************/
	/*	Private Functions - Order: Alphabetic							 			 */		
	/*********************************************************************************/

	// Executes logic when user clicks
	private void checkPlayerClick() {
		
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				/*Debug.Log ("Name = " + hit.collider.name);
				Debug.Log ("Tag = " + hit.collider.tag);
				Debug.Log ("Hit Point = " + hit.point);
				Debug.Log ("Object position = " + hit.collider.gameObject.transform.position);
				Debug.Log ("--------------");*/

				// If building is clicked create new unit
				if(buildings.ContainsKey(hit.collider.name)) {
					Unit newUnit = buildings[hit.collider.name].createUnit(this.team);
					if(newUnit != null) {
						units.Add(newUnit.name, newUnit);
						unitCount++;
					}
				}
				
				// If a unit is clicked unselect other units
				if(units.ContainsKey(hit.collider.name)) {
					
					foreach(KeyValuePair<string, Unit> entry in units) {
						Unit unit = entry.Value;
						if(entry.Key == hit.collider.name) {
							unit.setSelected(true);
						} else {
							unit.setSelected(false);
						}
					}
				}
				
				// If the ground is clicked unselect units
				if(hit.collider.name == "Ground") {
					foreach(KeyValuePair<string,Unit> entry in units) {
						Unit unit = entry.Value;
						unit.setSelected(false);
					}
				}
			}
		}
	}

	// Executes logic when user clicks and drags (creates selection box)
	private void checkPlayerSelection() {

		// Detect button press
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

	// Inverts the Y component of the mouse vector
	private static float InvertMouseY(float y)
	{
		return Screen.height - y;
	}
	
	// Executes logic when the user moves the camera
	private void moveCamera() {
		int screenScrollLimit = 100;
		float scrollRate = 0.2f;
		Vector3 movementVectorX;
		Vector3 movementVectorZ;
	
		// Scrolling with the mouse as it enters edges of screen
		if (Input.mousePosition.y < screenScrollLimit && Input.mousePosition.y >= 0) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		} else if (Input.mousePosition.y > Screen.height - screenScrollLimit && Input.mousePosition.y <= Screen.height) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		}
		if (Input.mousePosition.x < screenScrollLimit && Input.mousePosition.x >= 0) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.mousePosition.x > Screen.width - screenScrollLimit && Input.mousePosition.x <= Screen.width) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}

		//Scrolling with the arrow keys 
		if (Input.GetKey ("up") && !Input.GetKey ("down")) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		} else if (Input.GetKey ("down") && !Input.GetKey ("up")) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		}
		if (Input.GetKey ("left") && !Input.GetKey ("right")) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.GetKey ("right") && !Input.GetKey ("left")) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}
	}

	// Logic to move units
	/*private void moveUnits() {
		for (int i = 0; i < units.Length; i++) {
			Unit unit = units[i];
			if(unit.getSelected() && startClick.x > 0) {
				unit.transform.Translate(Vector3.forward * Time.deltaTime);
			}
		}
	}*/
}

