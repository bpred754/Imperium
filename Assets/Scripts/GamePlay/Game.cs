using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	
	private GameObject camera;
	private CameraManager cameraManager;
	private GameObject cameraPrefab;
	private GameObject game;
	private Grid grid;
	private Transform ground;
	private Renderer groundRenderer;
	private Vector3 groundSize;
	private GUIManager guiManager;
	private Camera minimapCamera;
	private GameObject minimapCameraObject;
	private GameObject minimapCameraPrefab;
	private PathRequestManager pathRequestManager;
	private PersonPlayer player;

	private Unit unit;
	private Camera myCamera;
	private Texture2D viewingArea;
	private float viewingAreaWidth;
	private float viewingAreaHeight;
	private Vector3 viewPort;

	private float GUI_WIDTH = 150f;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/
	
	private void Awake() {
		// Load Resources
		this.cameraPrefab = Resources.Load<GameObject> ("Camera");
		this.minimapCameraPrefab = Resources.Load<GameObject> ("MiniMapCamera");
		
		// Get components
		this.ground = GetComponent<Transform> ();
		this.groundRenderer = GetComponent<Renderer> ();
		this.game = ground.gameObject;

		// Create reference to grid
		this.grid = game.AddComponent<Grid>();

		// Instantiate players
		this.camera = (GameObject)Instantiate(cameraPrefab, new Vector3 (0, CameraManager.CAMERA_HEIGHT, 0), Quaternion.Euler (90, 0, 0));
		this.player = camera.GetComponent<PersonPlayer> ();

		// Create references to managers
		this.cameraManager = camera.GetComponent<CameraManager> ();
		this.guiManager = camera.GetComponent<GUIManager> ();

		this.viewingArea = Resources.Load<Texture2D> ("2x2");
		this.unit = Resources.Load<Unit> ("Unit");
	}
	
	void Start () {

		// Set players team
		this.player.setTeam (Team.Allies);

		// Set players camera boundaries
		Vector3 groundPosition = ground.position;
		this.groundSize = groundRenderer.bounds.size;
		float maxWorldX = groundPosition.x + this.groundSize.x/2;
		float minWorldX = groundPosition.x - this.groundSize.x/2;
		float maxWorldZ = groundPosition.z + this.groundSize.z/2;
		float minWorldZ = groundPosition.z - this.groundSize.z/2;
		this.cameraManager.setCameraBoundaries (maxWorldX, minWorldX, maxWorldZ, minWorldZ);

		// Set the players grid
		this.game.layer = LayerMask.NameToLayer("Ground");
		this.grid.setGridWorldSize (new Vector2(this.groundSize.x, this.groundSize.z));
		this.grid.setNodeRadius (.75f);
		this.grid.setUnwalkableMask(LayerMask.GetMask("Unwalkable"));
		this.grid.setFloor(LayerMask.GetMask("Floor"));
		this.grid.setGround(LayerMask.GetMask("Ground"));
		this.grid.setRamp(LayerMask.GetMask("Ramp"));
		this.grid.CreateGrid ();
		this.guiManager.initializeMiniMap (this.grid);

		// Create the game's PathRequestManager(singleton)
		this.pathRequestManager = new PathRequestManager (this.grid);

		// Add high level camera for minimap
		minimapCameraObject = (GameObject)Instantiate(minimapCameraPrefab, new Vector3(0, 250, 0), Quaternion.Euler (90, 0, 0));
		minimapCamera = minimapCameraObject.GetComponent<Camera> ();
		minimapCamera.transform.position = new Vector3 (0,260,0);
		minimapCamera.transform.rotation = Quaternion.Euler (90, 0, 0);
		minimapCamera.depth = 21;
		minimapCamera.orthographic = true;
		minimapCamera.orthographicSize = 75;
		minimapCamera.pixelRect = (new Rect(Screen.width - GUI_WIDTH, Screen.height - GUI_WIDTH, GUI_WIDTH, GUI_WIDTH));

		// Get main camera's view in world coordinates
		myCamera = camera.GetComponent<Camera> ();

		// DEBUG - Add buildings to map
		this.player.createBuilding (new Vector3 (40f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 40f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (40f, 1f, 40f)); // Create base
	}

	// Called several times per frame when needed
	private void OnGUI()
	{
		Vector3 worldGUIStart = myCamera.ScreenToWorldPoint (new Vector3 (0, 0, CameraManager.CAMERA_HEIGHT));
		Vector3 worldGUIEnd = myCamera.ScreenToWorldPoint (new Vector3 (150f, 0, CameraManager.CAMERA_HEIGHT));
		float worldGuiSize = worldGUIEnd.x - worldGUIStart.x;
		
		float cameraTop = myCamera.ScreenToWorldPoint (new Vector3 (0, myCamera.pixelHeight, CameraManager.CAMERA_HEIGHT)).z;
		float cameraBottom = myCamera.ScreenToWorldPoint (new Vector3 (0, 0, CameraManager.CAMERA_HEIGHT)).z;
		float cameraRight = myCamera.ScreenToWorldPoint (new Vector3 (myCamera.pixelWidth, 0, CameraManager.CAMERA_HEIGHT)).x - worldGuiSize;
		float cameraLeft = myCamera.ScreenToWorldPoint (new Vector3 (0, 0, CameraManager.CAMERA_HEIGHT)).x;
		
		float width;
		if (cameraRight >= cameraLeft) {
			width = cameraRight - cameraLeft;
		} else {
			width = cameraLeft - cameraRight - worldGuiSize;
		}
		
		float height;
		if (cameraTop >= cameraBottom) {
			height = cameraTop - cameraBottom;
		} else {
			height = cameraBottom - cameraTop;
		}

		float percentX = width / groundSize.x;
		float percentZ = height / groundSize.z;

		float viewingAreaWidth = percentX * minimapCamera.pixelWidth;
		float viewingAreaHeight = percentZ * minimapCamera.pixelHeight;

		float zeroBaseX = groundSize.x/2 + cameraLeft;
		float zeroBaseZ = groundSize.z / 2 + cameraBottom;

		float xCoordPercent = zeroBaseX / groundSize.x;
		float zCoordPercent = zeroBaseZ / groundSize.z;

		float miniMapX = minimapCamera.pixelWidth * xCoordPercent;
		float miniMapZ = minimapCamera.pixelHeight * zCoordPercent;

		float viewingAreaX = Screen.width - viewingAreaWidth;
		float left = (minimapCamera.pixelWidth - viewingAreaWidth) - miniMapX;
		if (left < 0) {
			left = 0;
		}

		GUI.color = new Color (1, 1, 1, 0.5f);
		GUI.DrawTexture (new Rect(viewingAreaX - left,minimapCamera.pixelHeight - viewingAreaHeight - miniMapZ, viewingAreaWidth, viewingAreaHeight), viewingArea);
	}

	private void Update() {

		// Move camera to world position when minimap is clicked
		if (Input.GetMouseButtonDown (0)) {
			if (Input.mousePosition.x > Screen.width - this.cameraManager.getGUIScreenWidth ()) {

				this.cameraManager.moveCameraToVector(minimapCamera.ScreenToWorldPoint(Input.mousePosition));
			}
		} else if (Input.GetMouseButtonDown (1)) { 

			// Give move order to selected units when minimap is right clicked
			if (Input.mousePosition.x > Screen.width - this.cameraManager.getGUIScreenWidth ()) {
				Ray ray = minimapCamera.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					Physics.Raycast (ray, out hit);
					this.player.moveSelectedUnitsInFormation (hit.point);
				}
			}
		}
	}
}
