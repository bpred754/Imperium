using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
	private Camera camera;
	private float guiScreenWidth = 0;
	private float guiWorldWidth = 0;
	private float maxWorldX = 0;
	private float maxWorldZ = 0;
	private float minWorldX = 0;
	private float minWorldZ = 0;
	
	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	void Awake() {
		this.camera = GetComponent<Camera> ();
	}
	
	void Update ()
	{
		moveCamera();
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	public void setCameraBoundaries(float maxX, float minX, float maxZ, float minZ) {
		this.maxWorldX = maxX;
		this.minWorldX = minX;
		this.maxWorldZ = maxZ;
		this.minWorldZ = minZ;
	}

	public void setGUIWidths(float _guiScreenWidth) {
		this.guiScreenWidth = _guiScreenWidth;

		// Set GUI world width variable
		float left = camera.ScreenToWorldPoint (new Vector3(0,0, 27.66f)).x;
		float right = camera.ScreenToWorldPoint (new Vector3(90,0, 27.66f)).x;
		this.guiWorldWidth = right - left;
	}

	/*********************************************************************************/
	/*	Private Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	// Executes logic when the user moves the camera
	private void moveCamera() {
		int screenScrollLimit = Screen.height / 7;
		float scrollRate = 0.4f;
		Vector3 movementVectorX;
		Vector3 movementVectorZ;
		
		float cameraTop = camera.ScreenToWorldPoint (new Vector3 (0, camera.pixelHeight, 27.66f)).z;
		float cameraBottom = camera.ScreenToWorldPoint (new Vector3 (0, 0, 27.66f)).z;
		float cameraRight = camera.ScreenToWorldPoint (new Vector3 (camera.pixelWidth, 0, 27.66f)).x;
		float cameraLeft = camera.ScreenToWorldPoint (new Vector3 (0, 0, 27.66f)).x;
		
		// Scrolling with the mouse as it enters edges of screen
		if (Input.mousePosition.y < screenScrollLimit && Input.mousePosition.y >= 0 && Input.mousePosition.x <= Screen.width - this.guiScreenWidth && cameraBottom > this.minWorldZ) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		} else if (Input.mousePosition.y > Screen.height - screenScrollLimit && Input.mousePosition.y <= Screen.height && Input.mousePosition.x <= Screen.width - this.guiScreenWidth && cameraTop < this.maxWorldZ) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		}
		if (Input.mousePosition.x < screenScrollLimit && Input.mousePosition.x >= 0  && cameraLeft > this.minWorldX) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.mousePosition.x > Screen.width - screenScrollLimit - this.guiScreenWidth && Input.mousePosition.x <= Screen.width - this.guiScreenWidth && cameraRight - guiWorldWidth < this.maxWorldX) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}
		
		// Scrolling with the arrow keys 
		if (Input.GetKey ("up") && !Input.GetKey ("down") && cameraTop < this.maxWorldZ) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position += movementVectorZ;
		} else if (Input.GetKey ("down") && !Input.GetKey ("up") && cameraBottom > this.minWorldZ) {
			movementVectorZ = new Vector3 (0f,0f,scrollRate);
			transform.position -= movementVectorZ;
		}
		if (Input.GetKey ("left") && !Input.GetKey ("right") && cameraLeft > this.minWorldX) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position -= movementVectorX;
		} else if (Input.GetKey ("right") && !Input.GetKey ("left") && cameraRight - guiWorldWidth < this.maxWorldX) {
			movementVectorX = new Vector3 (scrollRate,0f,0f);
			transform.position += movementVectorX;
		}
	}
}