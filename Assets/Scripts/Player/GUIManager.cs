using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GUIManager : MonoBehaviour
{	
	// Constants
	private float GUI_WIDTH = 150f;
	private float TAB_HEIGHT = 20f;
	
	private GameObject buildingList;
	private Button buildingsTab;
	private Canvas canvas;
	private CameraManager cameraManager;
	private Grid grid;
	private float guiButtonContainerHeight;
	private float guiScreenWidth = 0;
	private float guiWorldWidth = 0;
	private GameObject minimap;
	private PersonPlayer player;
	private float previousGuiButtonContainerHeight;
	private GameObject tabs;
	private Button unitButton0;
	private Button unitButton1;
	private GameObject unitList;
	private Button unitsTab;
	private Text unitTabText;
	
	// Prefabs
	private GameObject buildingListPrefab;
	private GameObject minimapPrefab;
	private GameObject tabsPrefab;
	private GameObject unitListPrefab;

	// Transforms
	private RectTransform[] buildingButtonTransforms;
	private RectTransform buildingListRectTransform;
	private RectTransform buildingTabTransform;
	private Text buildingTabText;
	private RectTransform canvasRectTransform;
	private RectTransform miniMapRectTransform;
	private RectTransform tabTransform;
	private RectTransform[] unitButtonTransforms;
	private RectTransform unitListRectTransform;
	private RectTransform unitTabTransform;

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
		this.player = playerGameObject.GetComponent<PersonPlayer> ();
		this.cameraManager = playerGameObject.GetComponent<CameraManager> ();
		
		// Add Event System to scene
		if (FindObjectOfType<EventSystem> () == null) {
			GameObject es = new GameObject ("EventSystem", typeof(EventSystem));
			es.AddComponent<StandaloneInputModule> ();
		}
		
		// Add canvas to scene
		GameObject canvasObject = new GameObject ("Canvas");
		this.canvas = canvasObject.AddComponent<Canvas> ();
		this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasObject.AddComponent<GraphicRaycaster> ();
		
		// Add minimap to canvas
		this.minimap = (GameObject)Instantiate (minimapPrefab);
		this.minimap.transform.SetParent (canvas.transform, false);
		this.miniMapRectTransform = this.minimap.GetComponent<RectTransform> ();
		
		// Add tabs to canvas
		this.tabs = (GameObject)Instantiate (tabsPrefab);
		this.tabs.transform.SetParent (canvas.transform, false);
		this.tabTransform = tabs.GetComponent<RectTransform> ();
		this.buildingsTab = GameObject.Find ("BuildingsTab").GetComponentInChildren<Button> ();
		this.buildingTabTransform = buildingsTab.GetComponent<RectTransform> ();
		this.buildingTabText = buildingsTab.GetComponentInChildren<Text> ();

		this.unitsTab = GameObject.Find ("UnitsTab").GetComponentInChildren<Button> ();
		this.unitTabTransform = unitsTab.GetComponent<RectTransform> ();
		this.unitTabText = unitsTab.GetComponentInChildren<Text> ();

		// Add buildingList to prefab
		this.buildingList = (GameObject)Instantiate (buildingListPrefab);
		this.buildingList.transform.SetParent (canvas.transform, false);
		Button[] buildingListButtons = buildingList.GetComponentsInChildren<Button> ();
		this.buildingListRectTransform = this.buildingList.GetComponent<RectTransform> ();
		buildingButtonTransforms = new RectTransform[buildingListButtons.Length];
		for (int i = 0; i < buildingListButtons.Length; i++) {
			buildingButtonTransforms[i] = buildingListButtons[i].GetComponent<RectTransform>();
		}

		// Add unit List to prefab
		this.unitList = (GameObject)Instantiate(unitListPrefab);
		this.unitList.transform.SetParent(canvas.transform, false);
		Button[] unitListButtons = unitList.GetComponentsInChildren<Button>();
		this.unitListRectTransform = this.unitList.GetComponent<RectTransform> ();
		unitButtonTransforms = new RectTransform[unitListButtons.Length];
		for (int i = 0; i < unitListButtons.Length; i++) {
			unitButtonTransforms[i] = unitListButtons[i].GetComponent<RectTransform>();
		}
		this.unitButton0 = unitListButtons [0];
		this.unitButton1 = unitListButtons[1];
	}
	
	private void Start() {

		// Set initial sizes of GUI elements
		this.canvasRectTransform = this.canvas.GetComponent<RectTransform> ();

		this.miniMapRectTransform.sizeDelta = new Vector2(GUI_WIDTH,GUI_WIDTH);
		this.miniMapRectTransform.anchoredPosition = new Vector3 (-1*GUI_WIDTH/2, -1*GUI_WIDTH/2);

		this.tabTransform.sizeDelta = new Vector2 (GUI_WIDTH, TAB_HEIGHT);
		this.tabTransform.anchoredPosition = new Vector3 (-1*GUI_WIDTH/2, -1*TAB_HEIGHT/2 - miniMapRectTransform.rect.height);

		this.buildingTabTransform.sizeDelta = new Vector2 (GUI_WIDTH/2, TAB_HEIGHT*1.5f);
		this.buildingTabTransform.anchoredPosition = new Vector2 (buildingTabTransform.sizeDelta.x/2, 0);

		this.unitTabTransform.sizeDelta = new Vector2 (GUI_WIDTH/2, TAB_HEIGHT*1.5f);
		this.unitTabTransform.anchoredPosition = new Vector2 (unitTabTransform.sizeDelta.x/2, 0);

		this.buildingTabText.fontSize = 12;
		this.unitTabText.fontSize = 12;

		this.previousGuiButtonContainerHeight = canvasRectTransform.rect.height - GUI_WIDTH - TAB_HEIGHT;
		this.guiButtonContainerHeight = canvasRectTransform.rect.height - GUI_WIDTH - TAB_HEIGHT;
		this.buildingListRectTransform.sizeDelta = new Vector2 (GUI_WIDTH, guiButtonContainerHeight);
		this.buildingListRectTransform.anchoredPosition = new Vector2 (-GUI_WIDTH/2, guiButtonContainerHeight/2);

		this.unitListRectTransform.sizeDelta = new Vector2 (GUI_WIDTH, guiButtonContainerHeight);
		this.unitListRectTransform.anchoredPosition = new Vector2 (-GUI_WIDTH/2, guiButtonContainerHeight/2);

		float startX = 40;
		float startY = -40;
		float buttonSize = GUI_WIDTH / 2 - 10;
		int buttonsPerRow = 2;

		int index = 0;
		int buildingRows = this.buildingButtonTransforms.Length / buttonsPerRow;
		for (int i = 0; i < buildingRows; i++) {
			for(int j = 0; j < buttonsPerRow; j++) {
				this.buildingButtonTransforms[index].sizeDelta = new Vector2 (buttonSize, buttonSize);
				this.buildingButtonTransforms[index].anchoredPosition = new Vector2 (startX+(j*(buttonSize+5)), startY+(-i*(buttonSize+5)));
				index++;
			}
		}

		index = 0;
		int unitRows = this.unitButtonTransforms.Length / buttonsPerRow;
		for (int i = 0; i < unitRows; i++) {
			for(int j = 0; j < buttonsPerRow; j++) {
				this.unitButtonTransforms[index].sizeDelta = new Vector2 (buttonSize, buttonSize);
				this.unitButtonTransforms[index].anchoredPosition = new Vector2 (startX+(j*(buttonSize+5)), startY+(-i*(buttonSize+5)));
				index++;
			}
		}
	
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

	private void Update() {

		// Update GUI when screen size changes
		this.guiButtonContainerHeight = this.canvasRectTransform.rect.height - GUI_WIDTH - TAB_HEIGHT;
		if (this.guiButtonContainerHeight != this.previousGuiButtonContainerHeight) {
			this.buildingListRectTransform.sizeDelta = new Vector2 (GUI_WIDTH, guiButtonContainerHeight);
			this.buildingListRectTransform.anchoredPosition = new Vector2 (-GUI_WIDTH / 2, guiButtonContainerHeight / 2);

			this.unitListRectTransform.sizeDelta = new Vector2 (GUI_WIDTH, guiButtonContainerHeight);
			this.unitListRectTransform.anchoredPosition = new Vector2 (-GUI_WIDTH / 2, guiButtonContainerHeight / 2);
		}
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