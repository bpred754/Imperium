using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GUIManager : MonoBehaviour
{	
	private GameObject buildingList;
	private Button buildingsTab;
	private Canvas canvas;
	private CameraManager cameraManager;
	private Grid grid;
	private float guiScreenWidth = 0;
	private float guiWorldWidth = 0;
	private GameObject minimap;
	private PersonPlayer player;
	private GameObject tabs;
	private RectTransform tabTransform;
	private Button unitButton0;
	private Button unitButton1;
	private GameObject unitList;
	private Button unitsTab;
	
	// Prefabs
	private GameObject buildingListPrefab;
	private GameObject minimapPrefab;
	private GameObject tabsPrefab;
	private GameObject unitListPrefab;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	void Awake ()
	{
		// Load Resources
		this.minimapPrefab = Resources.Load<GameObject> ("MiniMap");
		this.tabsPrefab = Resources.Load<GameObject> ("GUITabs");
		this.buildingListPrefab = Resources.Load<GameObject> ("GUIBuildingList");
		this.unitListPrefab = Resources.Load<GameObject> ("GUIUnitList");

		GameObject playerGameObject = GetComponent<Transform> ().gameObject;
		this.player = playerGameObject.GetComponent<PersonPlayer>();
		this.cameraManager = playerGameObject.GetComponent<CameraManager> ();
		
		// Add Event System to scene
		if (FindObjectOfType<EventSystem>() == null)
		{
			GameObject es = new GameObject("EventSystem", typeof(EventSystem));
			es.AddComponent<StandaloneInputModule>();
		}
		
		// Add canvas to scene
		GameObject canvasObject = new GameObject("Canvas");
		this.canvas = canvasObject.AddComponent<Canvas>();
		this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasObject.AddComponent<GraphicRaycaster> ();
		
		// Add minimap to canvas
		this.minimap = (GameObject)Instantiate(minimapPrefab);
		this.minimap.transform.SetParent(canvas.transform, false);
		
		// Add tabs to canvas
		this.tabs = (GameObject)Instantiate(tabsPrefab);
		this.tabs.transform.SetParent(canvas.transform, false);
		this.tabTransform = tabs.GetComponent<RectTransform>();
		this.buildingsTab = GameObject.Find ("BuildingsTab").GetComponentInChildren<Button>();
		this.unitsTab = GameObject.Find ("UnitsTab").GetComponentInChildren<Button>();
		
		// Add buildingList to prefab
		this.buildingList = (GameObject)Instantiate(buildingListPrefab);
		this.buildingList.transform.SetParent(canvas.transform, false);
		
		// Add unit List to prefab
		this.unitList = (GameObject)Instantiate(unitListPrefab);
		this.unitList.transform.SetParent(canvas.transform, false);
		Button[] unitListButtons = unitList.GetComponentsInChildren<Button>();
		this.unitButton0 = unitListButtons [0];
		this.unitButton1 = unitListButtons[1];
	}
	
	private void Start() {

		// Set buildings tab as default selected tab
		selectTab (buildingsTab);
		unselectTab (unitsTab);
		this.buildingList.SetActive(true);
		this.unitList.SetActive(false);

		// Add listeners to tabs
		this.buildingsTab.onClick.RemoveAllListeners ();
		this.buildingsTab.onClick.AddListener (() => buildingsTabListener(this.buildingsTab, this.unitsTab, this.buildingList, this.unitList));
		this.unitsTab.onClick.RemoveAllListeners();
		this.unitsTab.onClick.AddListener(() => unitTabListener (this.buildingsTab, this.unitsTab, this.buildingList, this.unitList));

		// Add listener to U1 button to turn on grid gizmos
		this.unitButton0.onClick.RemoveAllListeners();
		this.unitButton0.onClick.AddListener (() => toggleGridGizmos(this.unitButton0));

		// Add listener to U2 button to turn on unit waypoints
		this.unitButton1.onClick.RemoveAllListeners();
		this.unitButton1.onClick.AddListener (() => {
			this.displayWayPointGizmos = !this.displayWayPointGizmos;
			this.player.setUnitsWayPointGizmos(this.displayWayPointGizmos);
			toggleWayPointGizmos(this.unitButton1);
		});

		// Give camera width of GUI
		this.guiScreenWidth = tabTransform.rect.width;
		this.cameraManager.setGUIWidths (this.guiScreenWidth);
	}

	/*********************************************************************************/
	/*	Private Functions - Order: Alphabetic										 */		
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

	/*********************************************************************************/
	/*	DEBUG Functions - Order: Alphabetic							 	             */		
	/*********************************************************************************/

	private bool displayWayPointGizmos = false;
	
	public void setGrid(Grid inGrid) {
		this.grid = inGrid;
	}

	private void selectButton(Button button) {
		var selectedColor = button.colors;
		selectedColor.normalColor =  Color.green;
		selectedColor.pressedColor = Color.green;
		selectedColor.highlightedColor = Color.green;
		selectedColor.disabledColor = Color.green;
		button.colors = selectedColor;
	}

	private void toggleGridGizmos(Button button) {
		
		if (this.grid.getDisplayGridGizmos()) {
			unselectButton(button);
			this.grid.setDisplayGridGizmos(false);
		} else {
			selectButton(button);
			this.grid.setDisplayGridGizmos(true);
		}
	}
	
	private void toggleWayPointGizmos(Button button) {
		if (this.displayWayPointGizmos) {
			selectButton (button);
		} else {
			unselectButton(button);
		}
	}
	
	private void unselectButton(Button button) {
		var selectedColor = button.colors;
		selectedColor.normalColor = Color.white;
		selectedColor.pressedColor = Color.white;
		selectedColor.highlightedColor = Color.white;
		selectedColor.disabledColor = Color.white;
		button.colors = selectedColor;
	}
}