using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public bool selected = false;
	private Color startColor = new Color(.2f,.2f,.2f);

	// Update is called once per frame
	private void Update () {
		if (GetComponent<Renderer> ().isVisible && Input.GetMouseButton (0)) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = CameraOperator.InvertMouseY(camPos.y);
			selected = CameraOperator.selection.Contains(camPos);
		}
		if (selected)
			GetComponent<Renderer> ().material.color = Color.red;
		else
			GetComponent<Renderer> ().material.color = startColor;
	}
}
