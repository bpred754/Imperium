using UnityEngine;
using System.Collections;

public class MapObject : MonoBehaviour
{
	void Start ()
	{
		GameObject mapBounds = GameObject.CreatePrimitive (PrimitiveType.Cube);
		mapBounds.name = "MapBounds";
		mapBounds.layer = 12;
		Destroy (mapBounds.GetComponent<Collider>());
		mapBounds.transform.parent = transform;
		mapBounds.transform.localScale = new Vector3 (2,2,2);
		mapBounds.transform.localPosition = new Vector3 (0,0,0);
		mapBounds.GetComponent<Renderer> ().material.color = Color.green;
	}
}

