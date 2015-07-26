using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	private GameObject cameraPrefab;
	private PersonPlayer myPlayer;
	private Grid myGrid;
	private PathRequestManager pathRequestManager;

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/
	
	void Start () {

		// Load Resources
		cameraPrefab = Resources.Load<GameObject> ("Camera");

		// Get scene components
		Transform ground = GetComponent<Transform> ();
		Renderer groundRenderer = GetComponent<Renderer> ();
		GameObject game = ground.gameObject;

		// Set players
		GameObject camera = (GameObject)Instantiate(cameraPrefab, new Vector3 (0, 27.66f, 0), Quaternion.Euler (60, 0, 0));
		myPlayer = camera.GetComponent<PersonPlayer> ();
		this.myPlayer.setTeam (Team.Allies);

		// Set players camera boundaries
		Vector3 groundPosition = ground.position;
		Vector3 groundSize = groundRenderer.bounds.size;
		float maxWorldX = groundPosition.x + groundSize.x/2;
		float minWorldX = groundPosition.x - groundSize.x/2;
		float maxWorldZ = groundPosition.z + groundSize.z/2;
		float minWorldZ = groundPosition.z - groundSize.z/2;
		this.myPlayer.setCameraBoundaries (maxWorldX, minWorldX, maxWorldZ, minWorldZ);

		// Set the players grid
		game.layer = LayerMask.NameToLayer("Ground");
		myGrid = game.AddComponent<Grid>();
		myGrid.setGridWorldSize (new Vector2(groundSize.x, groundSize.z));
		myGrid.setNodeRadius (.75f);
		myGrid.setUnwalkableMask(LayerMask.GetMask("Unwalkable"));
		myGrid.setFloor(LayerMask.GetMask("Floor"));
		myGrid.setGround(LayerMask.GetMask("Ground"));
		myGrid.setRamp(LayerMask.GetMask("Ramp"));
		myGrid.CreateGrid ();
		this.myPlayer.setGrid (myGrid);

		// Create the game's PathRequestManager(singleton)
		this.pathRequestManager = new PathRequestManager (myGrid);

		// DEBUG - Add buildings to map
		this.myPlayer.createBuilding (new Vector3 (40f, 1f, 45f)); // Create base
		this.myPlayer.createBuilding (new Vector3 (45f, 1f, 40f)); // Create base
		this.myPlayer.createBuilding (new Vector3 (45f, 1f, 45f)); // Create base
		this.myPlayer.createBuilding (new Vector3 (40f, 1f, 40f)); // Create base
	}
	
	// Update is called once per frame
	void Update () {}
}
