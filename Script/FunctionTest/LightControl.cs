using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour {

	private Light directionLight;

	private bool isLightOn = true;
	private string lightStateStr = "灯亮着";

	// Use this for initialization
	void Start () {
		directionLight = GameObject.Find("Directional light1").GetComponent<Light>();
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(20, 20, 100, 30), lightStateStr))
		{
			isLightOn = !isLightOn;
			if (isLightOn)
			{
				lightStateStr = "灯亮着";
				directionLight.enabled = true;
			}
			else 
			{
				lightStateStr = "灯关了";
				directionLight.enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
