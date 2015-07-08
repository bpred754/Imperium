using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public PersonPlayer player; // Prefab (Camera)

	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	// Use this for initialization
	void Start () {
		this.player = (PersonPlayer)Instantiate (player, new Vector3 (0, 27.66f, 0), Quaternion.Euler (90, 0, 0));
		this.player.setTeam (Team.Allies);
		this.player.createBuilding (new Vector3 (40f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 40f)); // Create base
		this.player.createBuilding (new Vector3 (45f, 1f, 45f)); // Create base
		this.player.createBuilding (new Vector3 (40f, 1f, 40f)); // Create base
	}
	
	// Update is called once per frame
	void Update () {}
}
