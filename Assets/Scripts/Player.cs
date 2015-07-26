using System;
using UnityEngine;
	
public class Player : MonoBehaviour
{
	protected Team team;

	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic							 	 */		
	/*********************************************************************************/
	
	public void setTeam(Team inTeam) {
		this.team = inTeam;
	}
}

