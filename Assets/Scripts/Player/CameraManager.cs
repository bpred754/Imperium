using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
	public static float CAMERA_HEIGHT = 27.66f;

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

	void Start() {
		GameObject game = GameObject.Find ("Game");
		Renderer groundRenderer = game.GetComponent<Renderer>();
		Transform ground = game.transform;

		Vector3 groundPosition = ground.position;
		Vector3 groundSize = groundRenderer.bounds.size;
		this.maxWorldX = groundPosition.x + groundSize.x/2;
		this.minWorldX = groundPosition.x - groundSize.x/2;
		this.maxWorldZ = groundPosition.z + groundSize.z/2;
		this.minWorldZ = groundPosition.z - groundSize.z/2;
	}
	
	void Update ()
	{
		moveCamera();
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	public void setGUIWidths(float _guiScreenWidth) {
		this.guiScreenWidth = _guiScreenWidth;

		// Set GUI world width variable
		float left = camera.ScreenToWorldPoint (new Vector3(0,0, CAMERA_HEIGHT)).x;
		float right = camera.ScreenToWorldPoint (new Vector3(_guiScreenWidth,0, CAMERA_HEIGHT)).x;
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
		
		float cameraTop = getCameraTop ();
		float cameraBottom = getCameraBottom();
		float cameraRight = getCameraRight();
		float cameraLeft = getCameraLeft();
		
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

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	public void moveCameraToVector(Vector3 moveVector) {

		transform.position = new Vector3 (moveVector.x, CAMERA_HEIGHT, moveVector.z);

		float cameraTop = getCameraTop();
		float cameraBottom = getCameraBottom();
		float cameraRight = getCameraRight();
		float cameraLeft = getCameraLeft();
		
		// Prevent camera from going outside of game  boundaries
		if (cameraRight - guiWorldWidth > this.maxWorldX) {
			moveVector.x = maxWorldX - (((cameraRight) - cameraLeft)/2) + guiWorldWidth;
			transform.position = new Vector3 (moveVector.x, CAMERA_HEIGHT, moveVector.z);;
		}

		if (cameraLeft < this.minWorldX) {
			moveVector.x = minWorldX + (cameraRight - cameraLeft)/2;
			transform.position = new Vector3 (moveVector.x, CAMERA_HEIGHT, moveVector.z);
		}

		if (cameraTop > this.maxWorldZ) {
			moveVector.z = maxWorldZ - (cameraTop - cameraBottom)/2;
			transform.position = new Vector3 (moveVector.x, CAMERA_HEIGHT, moveVector.z);
		}

		if (cameraBottom < this.minWorldZ) {
			moveVector.z = minWorldZ + (cameraTop - cameraBottom)/2;
			transform.position = new Vector3 (moveVector.x, CAMERA_HEIGHT, moveVector.z);
		}
	}

	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic								 */		
	/*********************************************************************************/

	public float getCameraBottom() {
		return this.camera.ScreenToWorldPoint (new Vector3 (0, 0, CAMERA_HEIGHT)).z;
	}

	public float getCameraLeft() {
		return this.camera.ScreenToWorldPoint (new Vector3 (0, 0, CAMERA_HEIGHT)).x;
	}

	public float getCameraRight() {
		return this.camera.ScreenToWorldPoint (new Vector3 (this.camera.pixelWidth, 0, CAMERA_HEIGHT)).x;
	}

	public float getCameraTop() {
		return this.camera.ScreenToWorldPoint (new Vector3 (0, this.camera.pixelHeight, CAMERA_HEIGHT)).z;
	}

	public float getGUIScreenWidth() {
		return this.guiScreenWidth;
	}

	public float getGUIWorldWidth() {
		return this.guiWorldWidth;
	}
}