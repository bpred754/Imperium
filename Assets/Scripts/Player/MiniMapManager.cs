using UnityEngine;
using System.Collections;

public class MiniMapManager : MonoBehaviour
{
	private CameraManager cameraManager;
	private Vector3 groundSize;
	private Camera minimapCamera;
	private PersonPlayer player;
	private string playerName;
	private Texture2D viewingArea;

	// Constants
	private float MINIMAP_CAMERA_HEIGHT = 260f;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	private void Awake() {
		this.viewingArea = Resources.Load<Texture2D> ("2x2");
	}

	// Use this for initialization
	private void Start () {

		this.minimapCamera = transform.GetComponent<Camera> ();
		this.minimapCamera.transform.position = new Vector3 (0,MINIMAP_CAMERA_HEIGHT,0);
		this.minimapCamera.transform.rotation = Quaternion.Euler (90, 0, 0);
		this.minimapCamera.depth = 21;
		this.minimapCamera.orthographic = true;
		this.minimapCamera.orthographicSize = 75;
		this.minimapCamera.pixelRect = (new Rect(Screen.width - GUIManager.GUI_WIDTH, Screen.height - GUIManager.GUI_WIDTH, GUIManager.GUI_WIDTH, GUIManager.GUI_WIDTH));
	}

	// Called several times per frame when needed
	private void OnGUI()
	{
		float worldGuiWidth = cameraManager.getGUIWorldWidth ();
		
		float cameraTop = cameraManager.getCameraTop ();
		float cameraBottom = cameraManager.getCameraBottom();
		float cameraRight = cameraManager.getCameraRight();
		float cameraLeft = cameraManager.getCameraLeft();

		float height = cameraTop - cameraBottom;
		float width = cameraRight - cameraLeft - worldGuiWidth;

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
			if (Input.mousePosition.x > Screen.width - this.cameraManager.getGUIScreenWidth () && Input.mousePosition.y > Screen.height - this.cameraManager.getGUIScreenWidth()) {
				
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

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/
	
	public void setPlayer(string _playerName) {
		this.playerName = _playerName;
		
		// Get main camera's view in world coordinates
		GameObject game = GameObject.Find ("Game");
		Renderer groundRenderer = game.GetComponent<Renderer>();
		this.groundSize = groundRenderer.bounds.size;
		
		GameObject playerObject = GameObject.Find (_playerName);
		this.cameraManager = playerObject.GetComponent<CameraManager> ();
		this.player = playerObject.GetComponent<PersonPlayer> ();
	}
}

