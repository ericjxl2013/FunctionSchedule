/// <summary>
/// Filename: PlaneDrag.cs
/// Author: Jiang Xiaolong
/// Created: 2014.01.15
/// Version:
/// Company: Sunnytech
/// Function: 基于GUI的平面元素随着鼠标在上下左右四个位置的平移.
///
/// Changed By:
/// Modification Time:
/// Description:
/// </summary>

using UnityEngine;
using System.Collections;

public class PlaneDrag : MonoBehaviour {
	
	private Rect winRect;
	private Rect btnRect;  //用此Rect参数控制你的按钮位置，根据要求修改
	
	private float distanceMax = 30;  //鼠标拖拽移动的最大距离，根据要求修改
	private Vector2 initialPosition = Vector2.zero;  //按钮的初始位置
	
	private float yValue = 0;
	
	// Use this for initialization
	void Start () 
	{
		winRect = new Rect(0, 0, 400, 400);
		winRect.x = (Screen.width - winRect.width) / 2;
		winRect.y = (Screen.height - winRect.height) / 2;
		btnRect = new Rect(0, 0, 150, 150);
		btnRect.x = (winRect.width - btnRect.width) / 2;
		btnRect.y = (winRect.height - btnRect.height) / 2;
		initialPosition = new Vector2(btnRect.x, btnRect.y);
		yValue = Screen.width - 230f; 
	}
	
	
	
	void OnGUI ()
	{
		winRect = GUI.Window(0, winRect, ControlWindow, "");
		
		if(GUI.Button(new Rect(yValue, 10, 200, 30), "加载自由移动场景")){
			Application.LoadLevel("SimpleMove");
		}
	}
	
	void ControlWindow(int WindowID)
	{
		GUI.Box(btnRect, "");  //假装是要移动的按钮
		
		if(btnRect.Contains(Event.current.mousePosition)){  //如果按钮区域包含了鼠标，切且鼠标被按下，则触发
			
			Event mouseE = Event.current;
			if(mouseE.isMouse && mouseE.type ==  EventType.mouseDown && mouseE.button == 0){
				print("Contains");
				StartCoroutine(BtnTranslation(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
			}
		}else
			GUI.DragWindow();
	}
	
	IEnumerator BtnTranslation(Vector2 initial_position)
	{
		//判断是沿X轴还是Y轴
		bool axisJudge = false;  //判断是X轴还是Y轴，false：X轴；true：Y轴
		Vector2 directionTarget = Vector2.zero;  //用于记录鼠标移动的方向
		float timeRecord = 0;
		while(Input.GetMouseButton(0)){  //用while循环对鼠标一直停留在原地和移动情况下进行判断，应该移动哪一个轴
			directionTarget = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - initial_position;
			if(directionTarget != Vector2.zero){  //零向量则默认X轴
				if(directionTarget.x == 0){  //Y轴上
					axisJudge = true;
				}else if(directionTarget.y == 0){  //X轴上
					axisJudge = false; 
				}else{  //需要判断
					//判断是否为钝角
					Vector3 crossVec = Vector3.Cross(new Vector3(0, 1, 0), new Vector3(directionTarget.x, directionTarget.y, 0));
					float angleVec = 0;  //方向向量与Y轴正方向夹角
					if(Vector3.Dot(crossVec, new Vector3(0, 0, 1)) > 0){ //小于180°角
						angleVec = Vector2.Angle(new Vector2(0, 1), directionTarget);
						if(angleVec > 0 && angleVec <= 45f)
							axisJudge = true;
						else if(angleVec >= 135f && angleVec <= 180f)
							axisJudge = true;
						else
							axisJudge = false;
					}else{  //大于180°角
						angleVec = 360 - Vector2.Angle(new Vector2(0, 1), directionTarget);
						if(angleVec >= 180f && angleVec <= 225f)
							axisJudge = true;
						else if(angleVec >= 315f && angleVec <= 360f)
							axisJudge = true;
						else
							axisJudge = false;
					}
//					Debug.Log(angleVec + ": " + axisJudge.ToString());
				}
				break;
			}else{  //鼠标在原地，下一帧再进行判断，如果时间超过2s，则默认为X轴移动
				timeRecord += Time.deltaTime;
				yield return new WaitForFixedUpdate();
				if(timeRecord > 2f){
					axisJudge = false;
					break;
				}
			}
		}
		//贴图移动
		while(Input.GetMouseButton(0)){
			if(axisJudge){  //沿Y轴
				btnRect.y = initialPosition.y + (initial_position.y - Input.mousePosition.y); //移动
				//最大位置判断
				if(btnRect.y > initialPosition.y + distanceMax)
					btnRect.y = initialPosition.y + distanceMax;
				else	if(btnRect.y < initialPosition.y - distanceMax)
					btnRect.y = initialPosition.y - distanceMax;
			}else{  //沿X轴
				btnRect.x = initialPosition.x + (Input.mousePosition.x - initial_position.x); //移动
				//最大位置判断
				if(btnRect.x > initialPosition.x + distanceMax)
					btnRect.x = initialPosition.x + distanceMax;
				else	if(btnRect.x < initialPosition.x - distanceMax)
					btnRect.x = initialPosition.x - distanceMax;
			}
			yield return new WaitForFixedUpdate();
		}
		btnRect.x = initialPosition.x;
		btnRect.y = initialPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
