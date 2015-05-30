using System;
using UnityEngine;
	
public class Player : MonoBehaviour
{
	public Team team;

	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic							 	 */		
	/*********************************************************************************/
	
	public void setTeam(Team inTeam) {
		this.team = inTeam;
	}
}

