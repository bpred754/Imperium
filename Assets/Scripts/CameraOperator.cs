using UnityEngine;
using System.Collections;

public class CameraOperator : MonoBehaviour {

	public Texture2D selectionHighlight = null;
	public static Rect selection = new Rect(0,0,0,0);

	private Vector3 startClick = -Vector3.one;

	// Update is called once per frame
	void Update () {
		CheckCamera ();
		MoveCamera ();
	}

	private void CheckCamera()
	{
		if (Input.GetMouseButtonDown (0)) 
			startClick = Input.mousePosition;
		else if (Input.GetMouseButtonUp (0)) {
			startClick = -Vector3.one;
		}
		if (Input.GetMouseButton (0)) 
		{
			selection = new Rect(startClick.x, InvertMouseY(startClick.y),Input.mousePosition.x - startClick.x, InvertMouseY(Input.mousePosition.y)-InvertMouseY(startClick.y));
			if (selection.width < 0) {
				selection.x += selection.width;
				selection.width = -selection.width;
			}
			if (selection.height < 0) {
				selection.y += selection.height;
				selection.height = -selection.height;
			}
		}
	}

	private void MoveCamera()
	{
		int screenScrollLimit = 100;
		//bool inMiddle = true;
		float scrollRate = 0.2f;
		Vector3 movementVectorX;// = new Vector3 (scrollRate,0f,0f);
		Vector3 movementVectorZ;// = new Vector3 (0f,0f,scrollRate);


		/*********************************************************/
		/* Scrolling with the mouse as it enters edges of screen */
		/*********************************************************/
		if (Input.mousePosition.y < screenScrollLimit) {
			//print ("Bottom of the screen? " + Input.mousePosition.y);
			//inMiddle = false;
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		} else if (Input.mousePosition.y > Screen.height - screenScrollLimit) {
			//print ("Top of the screen? " + Input.mousePosition.y);
			//inMiddle = false;
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		}
		if (Input.mousePosition.x < screenScrollLimit) {
			//print ("Left of the screen? " + Input.mousePosition.x);
			//inMiddle = false;
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.mousePosition.x > Screen.width - screenScrollLimit) {
			//print ("Right of the screen? " + Input.mousePosition.x);
			//inMiddle = false;
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}
		//if (inMiddle)
			//print ("Middle of the screen?");

		/*********************************/
		/* Scrolling with the arrow keys */
		/*********************************/
		if (Input.GetKey ("up") && !Input.GetKey ("down")) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		} else if (Input.GetKey ("down") && !Input.GetKey ("up")) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		}
		if (Input.GetKey ("left") && !Input.GetKey ("right")) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.GetKey ("right") && !Input.GetKey ("left")) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}

	}

	private void OnGUI()
	{
		if (startClick != -Vector3.one) {
			GUI.color = new Color(1,1,1,0.5f);
			GUI.DrawTexture(selection,selectionHighlight);
		}
	}
	public static float InvertMouseY(float y)
	{
		return Screen.height - y;
	}
}
