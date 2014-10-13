using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	private float speedValue = 4f;

	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.A) && transform.position.x > -12f) 
		{
			transform.Translate(Vector3.left * speedValue * Time.deltaTime, Space.Self);
		}
		else if (Input.GetKey(KeyCode.D) && transform.position.x < 12f)
		{
			transform.Translate(-Vector3.left * speedValue * Time.deltaTime, Space.Self);
		}
		else if (Input.GetKey(KeyCode.W) && transform.position.z < 12f)
		{
			transform.Translate(Vector3.forward * speedValue * Time.deltaTime, Space.Self);
		}
		else if (Input.GetKey(KeyCode.S) && transform.position.z > -12f)
		{
			transform.Translate(-Vector3.forward * speedValue * Time.deltaTime, Space.Self);
		}
	}
}
