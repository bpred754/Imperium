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
	private GUIManager guiManager;
	private PathRequestManager pathRequestManager;
	private PersonPlayer player;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/
	
	private void Awake() {
		// Load Resources
		this.cameraPrefab = Resources.Load<GameObject> ("Camera");
		
		// Get components
		this.ground = GetComponent<Transform> ();
		this.groundRenderer = GetComponent<Renderer> ();
		this.game = ground.gameObject;

		// Create reference to grid
		this.grid = game.AddComponent<Grid>();

		// Instantiate players
		this.camera = (GameObject)Instantiate(cameraPrefab, new Vector3 (0, 27.66f, 0), Quaternion.Euler (60, 0, 0));
		this.player = camera.GetComponent<PersonPlayer> ();

		// Create references to managers
		this.cameraManager = camera.GetComponent<CameraManager> ();
		this.guiManager = camera.GetComponent<GUIManager> ();
	}
	
	void Start () {

		// Set players team
		this.player.setTeam (Team.Allies);

		// Set players camera boundaries
		Vector3 groundPosition = ground.position;
		Vector3 groundSize = groundRenderer.bounds.size;
		float maxWorldX = groundPosition.x + groundSize.x/2;
		float minWorldX = groundPosition.x - groundSize.x/2;
		float maxWorldZ = groundPosition.z + groundSize.z/2;
		float minWorldZ = groundPosition.z - groundSize.z/2;
		this.cameraManager.setCameraBoundaries (maxWorldX, minWorldX, maxWorldZ, minWorldZ);

		// Set the players grid
		this.game.layer = LayerMask.NameToLayer("Ground");
		this.grid.setGridWorldSize (new Vector2(groundSize.x, groundSize.z));
		this.grid.setNodeRadius (.75f);
		this.grid.setUnwalkableMask(LayerMask.GetMask("Unwalkable"));
		this.grid.setFloor(LayerMask.GetMask("Floor"));
		this.grid.setGround(LayerMask.GetMask("Ground"));
		this.grid.setRamp(LayerMask.GetMask("Ramp"));
		this.grid.CreateGrid ();
		this.guiManager.setGrid (this.grid);

		// Create the game's PathRequestManager(singleton)
		this.pathRequestManager = new PathRequestManager (this.grid);

		// DEBUG - Add buildings to map
		this.player.createBuilding (new Vector3 (40f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 40f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (40f, 1f, 40f)); // Create base
	}
}
