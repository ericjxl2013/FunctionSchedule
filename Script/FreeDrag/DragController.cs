/// <summary>
/// Filename: DragController
/// Author: Jiang Xiaolong
/// Created: 2014.01.13
/// Version:
/// Company: Sunnytech
/// Function: 移动方式总控制脚本
///
/// Changed By:
/// Modification Time:
/// Description:
/// </summary>

using UnityEngine;
using System.Collections;

public class DragController : MonoBehaviour {
	
	//拖动方式选择
	public bool modeSelect = false;  // 移动方式：false，自由移动；true：沿某一轴移动
	private string modeStr = "自由移动方式";
	public int movingAxis = 0; //0：X轴  1：Y轴  2：Z轴
	private string[] axisArray;
	public bool cooSelect = false;  // 移动方式：false，局部坐标系；true：世界坐标系
	private string cooStr = "局部坐标系";
	
	public Transform coo3D;
	
	private float yValue = 0;
	
	// Use this for initialization
	void Start () {
		axisArray = new string[] {"X", "Y", "Z"};
		coo3D = GameObject.Find(ConstClass.cooName).transform;
		HideRecursion(coo3D, false);  //开始先隐藏坐标系
		yValue = Screen.width - 230f; 
	}
	
	void OnGUI () 
	{
		if(GUI.Button(new Rect(10, 10, 100, 30), modeStr)){
			modeSelect = !modeSelect;
			if(modeSelect){
				modeStr = "沿某一轴移动";
				HideRecursion(coo3D, true);  //显示坐标系
			}else{
				modeStr = "自由移动方式";
				HideRecursion(coo3D, false);  //隐藏坐标系
			}
		}
		
		if(GUI.Button(new Rect(yValue, 10, 200, 30), "加载平面移动场景")){
			Application.LoadLevel("MoveOnPlane");
		}
		
		//坐标轴选择
		if(modeSelect){
			movingAxis = GUI.Toolbar(new Rect(10, 50, 210, 30), movingAxis, axisArray);
			
			if(GUI.Button(new Rect(10, 90, 100, 30), cooStr)){
				cooSelect = !cooSelect;
				if(cooSelect){
					cooStr = "世界坐标系";
					coo3D.eulerAngles = new Vector3(0,0,0);
				}else{
					cooStr = "局部坐标系";
					coo3D.localEulerAngles = new Vector3(0,0,0);
				}
			}
		}
		
	}
	
	//Transform或有父子关系的Transform组隐藏或显示控制
	void HideRecursion(Transform motherTrans, bool display_flag)
	{
		if(motherTrans.renderer != null)
			motherTrans.renderer.enabled = display_flag;
		foreach(Transform childClass in motherTrans)
		{
			if(childClass.childCount > 0)
			{
				HideRecursion(childClass, display_flag);
				if(childClass.renderer != null)
					childClass.renderer.enabled = display_flag;
			}
			else
			{
				if(childClass.renderer != null)
					childClass.renderer.enabled = display_flag;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
