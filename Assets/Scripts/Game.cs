using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public PersonPlayer player; // Prefab (Camera)

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	// Use this for initialization
	void Start () {

		this.player = (PersonPlayer)Instantiate (player, new Vector3 (0, 27.66f, 0), Quaternion.Euler (60, 0, 0));
		this.player.setTeam (Team.Allies);
		this.player.createBuilding (new Vector3 (40f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 40f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (40f, 1f, 40f)); // Create base

		Transform ground = GetComponent<Transform> ();
		Renderer groundRenderer = GetComponent<Renderer> ();
		Vector3 groundPosition = ground.position;
		Vector3 groundSize = groundRenderer.bounds.size;

		float maxWorldX = groundPosition.x + groundSize.x/2;
		float minWorldX = groundPosition.x - groundSize.x/2;
		float maxWorldZ = groundPosition.z + groundSize.z/2;
		float minWorldZ = groundPosition.z - groundSize.z/2;

		this.player.setCameraBoundaries (maxWorldX, minWorldX, maxWorldZ, minWorldZ);

		Grid grid = GetComponentInParent<Grid> ();
		this.player.setGrid (grid);
	}
	
	// Update is called once per frame
	void Update () {}
}
