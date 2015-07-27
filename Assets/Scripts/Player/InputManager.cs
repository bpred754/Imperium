using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
	// Resources
	public Texture2D selectionHighlight;

	private CameraManager camera;
	private GUIManager guiManager;
	private PersonPlayer player;
	private Rect selection = new Rect (0,0,0,0);
	private Vector3 startClick = -Vector3.one; 

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	private void Awake() {
		this.selectionHighlight = Resources.Load<Texture2D> ("2x2");
		
		GameObject playerGameObject = GetComponent<Transform> ().gameObject;
		this.player = playerGameObject.GetComponent<PersonPlayer>();
		this.camera = playerGameObject.GetComponent<CameraManager> ();
		this.guiManager = playerGameObject.GetComponent<GUIManager> (); 
	}

	// Update is called once per frame
	void Update ()
	{
		checkPlayerClick ();
		checkPlayerSelection ();
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
	/*	Private Functions - Order: Alphabetic										 */		
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
				string objectName = hit.collider.name;
				if (this.player.hasBuilding(objectName)) {
					this.player.createUnit(objectName);
				}

				// If a unit is clicked unselect other units
				if (this.player.hasUnit(objectName)) {
					this.player.setUnitsSelection(objectName, isControlPressed);
				}

				// If the ground is clicked unselect units
				if (objectName == "Ground") {
					if(!isControlPressed){
						this.player.unselectUnits();
					}
				}
			}
		} //If right click is pressed down, i.e. move order is made
		else if (Input.GetMouseButtonDown (1)) {
			this.player.moveSelectedUnitsInFormation(Input.mousePosition);
		}
	}

	// Executes logic when user clicks and drags (creates selection box)
	private void checkPlayerSelection() {
		
		// Detect mouse click
		if(Input.GetMouseButtonDown(0)) {
			this.startClick = Input.mousePosition;
		} else if (Input.GetMouseButtonUp(0)) {
			this.startClick = -Vector3.one;
		}
		
		// Determine selection box area
		if (Input.GetMouseButton (0)) {
			this.selection = new Rect (this.startClick.x, InvertMouseY (this.startClick.y), Input.mousePosition.x - this.startClick.x, InvertMouseY (Input.mousePosition.y) - InvertMouseY (this.startClick.y));
			if (this.selection.width < 0) {
				this.selection.x += this.selection.width;
				this.selection.width = -this.selection.width;
			}
			if (this.selection.height < 0) {
				this.selection.y += this.selection.height;
				this.selection.height = -this.selection.height;
			}
			
			// Only execute logic when the user creates a selection box, not when they click
			if (this.selection.height > 1) {
				this.player.setUnitsSelection(this.selection);
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
	public static float InvertMouseY(float y)
	{
		return Screen.height - y;
	}
}

