using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	
	private Grid grid;
	private GameObject minimapCameraObject;
	private PersonPlayer player;
	private GameObject playerObject;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/
	
	private void Awake() {
		// Load Resources
		GameObject playerPrefab = Resources.Load<GameObject> ("Camera");
		GameObject minimapCameraPrefab = Resources.Load<GameObject> ("MiniMapCamera");
		
		// Get components
		Transform ground = GetComponent<Transform> ();
		ground.name = "Game";
		GameObject game = ground.gameObject;
		game.layer = LayerMask.NameToLayer("Ground");

		// Create reference to grid
		this.grid = game.AddComponent<Grid>();

		// Instantiate players
		this.playerObject = (GameObject)Instantiate(playerPrefab, new Vector3 (0, CameraManager.CAMERA_HEIGHT, 0), Quaternion.Euler (90, 0, 0));
		playerObject.name = "Player0";
		this.player = playerObject.GetComponent<PersonPlayer> ();
		this.minimapCameraObject = (GameObject)Instantiate(minimapCameraPrefab, new Vector3(0, 250, 0), Quaternion.Euler (90, 0, 0));
	}
	
	void Start () {

		// TODO: For each player..
			// Set player's team
			this.player.setTeam (Team.Allies);

			// Set player's MiniMap
			MiniMapManager miniMapManager = minimapCameraObject.GetComponent<MiniMapManager> ();
			miniMapManager.setPlayer(this.playerObject.name);

		// Create the game's PathRequestManager(singleton)
		PathRequestManager pathRequestManager = new PathRequestManager (this.grid);

		// DEBUG - Add buildings to map
		this.player.createBuilding (new Vector3 (40f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 40f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (40f, 1f, 40f)); // Create base
	}
}
