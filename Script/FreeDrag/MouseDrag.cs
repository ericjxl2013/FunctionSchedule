/// <summary>
/// Filename: MouseDrag
/// Author: Jiang Xiaolong
/// Created: 2014.01.13
/// Version:
/// Company: Sunnytech
/// Function: 控制每一个脚本所在的Transform自由移动
///
/// Changed By:
/// Modification Time:
/// Description:
/// </summary>

using UnityEngine;
using System.Collections;

public class MouseDrag : MonoBehaviour {
	
	DragController DragController_Script;  //拖拽控制脚本

	//鼠标经过时改变物体颜色  
    private Color mouseOverColor = Color.red;//声明变量为蓝色  
    private Color originalColor;//声明变量来存储本来颜色  
	
	
    void Start()  
    {  
        originalColor = renderer.sharedMaterial.color;//开始时得到物体着色  
		DragController_Script = GameObject.Find(ConstClass.MainScript).GetComponent<DragController>();
    }  
  
    void OnMouseEnter()  
    {  
        renderer.material.color = mouseOverColor;//当鼠标滑过时改变物体颜色为蓝色  
    }  
  
    void OnMouseExit()  
    {  
        renderer.material.color = originalColor;//当鼠标滑出时恢复物体本来颜色  
    } 

    IEnumerator OnMouseDown()  
    {  
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);//三维物体坐标转屏幕坐标  
        //将鼠标屏幕坐标转为三维坐标，再计算物体位置与鼠标之间的距离  
        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));  
        print("down");  
		//设定三维坐标位置和角度
		DragController_Script.coo3D.parent = gameObject.transform;
		DragController_Script.coo3D.position = gameObject.transform.position;
		if(DragController_Script.cooSelect){
			DragController_Script.coo3D.eulerAngles = Vector3.zero;
		}else{
			DragController_Script.coo3D.eulerAngles = gameObject.transform.eulerAngles;
		}
		
		if(DragController_Script.modeSelect){  //坐标轴移动
			if(DragController_Script.cooSelect){  //世界坐标轴
				while (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary))  
		        {  
		            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);  
					Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace);
					if(DragController_Script.movingAxis == 0){	//X轴
						transform.position = new Vector3(curPosition.x, transform.position.y, transform.position.z);
					}else if(DragController_Script.movingAxis == 1){	//Y轴
						transform.position = new Vector3(transform.position.x, curPosition.y, transform.position.z);
					}else{	//Z轴
						transform.position = new Vector3(transform.position.x, transform.position.y, curPosition.z);
					}
		            yield return new WaitForFixedUpdate();  
		        } 
			}else{  //局部坐标轴
				while (Input.GetMouseButton(0) ||  (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary))  
		        {  
		            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);  
					Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace);
					Vector3 localPosition = transform.InverseTransformPoint(curPosition);
					if(DragController_Script.movingAxis == 0){	//X轴
						transform.position = transform.TransformPoint(new Vector3(localPosition.x, 0, 0));	
					}else if(DragController_Script.movingAxis == 1){	//Y轴
						transform.position = transform.TransformPoint(new Vector3(0, localPosition.y, 0));	
					}else{	//Z轴
						transform.position = transform.TransformPoint(new Vector3(0, 0, localPosition.z));
					}
		            yield return new WaitForFixedUpdate();  
		        }
			}
		}else{  //自由移动
			while (Input.GetMouseButton(0) ||  (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary))  
	        {  
	            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);  
	            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;  
	            transform.position = curPosition;  
	            yield return new WaitForFixedUpdate();  
	        }  
		}
       
    } 

}
