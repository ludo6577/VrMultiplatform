using UnityEngine;
using System.Collections;

public class CubeScript : MonoBehaviour {

	public void SetColorRed(){
		var renderer = GetComponent<Renderer> ();
		renderer.material.color = Color.red;
	}

	public void SetColorBlue(){
		var renderer = GetComponent<Renderer> ();
		renderer.material.color = Color.blue;
	}
}
