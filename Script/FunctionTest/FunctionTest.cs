using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FunctionTest : MonoBehaviour {

	private Transform activeCapsule;

	private bool isLightOn = true;
	private Light directionLight;
	private string lightStateStr = "灯亮着";

	private float speedValue = 4f;

	private Material defaultMa;
	private Material activeMa;

	private CubeStation cubeStation = new CubeStation();
	//LightGlobal lightGlobal;
	LightFunc lightIns;
	MoveFunc moveIns;
	Station1Func stationBackIns;
	Station2Func stationForwardIns;
	Station3Func stationLeftIns;
	Station4Func stationRightIns;

	// Use this for initialization
	void Start () {
		activeCapsule = GameObject.Find("ManCapsule").transform;
		cubeStation.autoCube = GameObject.Find("AutoCube");
		defaultMa = (Material)Resources.Load("Station");
		cubeStation.defaultMaterial = defaultMa;
		activeMa = (Material)Resources.Load("Green");
		cubeStation.activeMaterial = activeMa;
		cubeStation.stationBack = GameObject.Find("Station_back");
		cubeStation.stationForward = GameObject.Find("Station_forward");
		cubeStation.stationLeft = GameObject.Find("Station_left");
		cubeStation.stationRight = GameObject.Find("Station_right");
		directionLight = GameObject.Find("Directional light1").GetComponent<Light>();

		//IFunction cubeRun = new CubeRun("CUBE", true, IState.Wait);
		//IFunction sphereRun = new SphereRun("SPHERE", true, IState.Invalid);
        //FunctionMG.AddFunction(sphereRun, "SPHERE");
		//lightGlobal = new LightGlobal("LIGHT", true, IState.Run, directionLight);
        //FunctionMG.AddFunction(lightGlobal, "LIGHT");
        //FunctionMG.Run();
		//LogicParser abc = new LogicParser(Application.dataPath + "/FunctionManager.xls");
		//abc.ExpressionParse("!A&((!B|C)&D)&E)");
		//abc.ExpressionParse("A&(B|(C&D))&E");
		//abc.ExpressionParse("!A");
		FunctionDispatch.Init(Application.dataPath + "/FunctionManager.xls");
		
		lightIns = new LightFunc("LIGHT", true, IState.Run, directionLight);
		moveIns = new MoveFunc("MOVE", false, IState.Run, activeCapsule);
		stationBackIns = new Station1Func("STATION1", true, IState.Wait, activeCapsule, GameObject.Find("Station_back"), defaultMa, activeMa);
		stationForwardIns = new Station2Func("STATION2", true, IState.Wait, activeCapsule, GameObject.Find("Station_forward"), defaultMa, activeMa);
		stationLeftIns = new Station3Func("STATION3", true, IState.Wait, activeCapsule, GameObject.Find("Station_left"), defaultMa, activeMa);
		stationRightIns = new Station4Func("STATION4", true, IState.Wait, activeCapsule, GameObject.Find("Station_right"), defaultMa, activeMa);
		//FunctionDispatch.Run();
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(20, 20, 100, 30), lightStateStr))
		{
			isLightOn = !isLightOn;
			//if (isLightOn)
			//{
			//	lightStateStr = "灯亮着";
			//	directionLight.enabled = true;
			//}
			//else
			//{
			//	lightStateStr = "灯关了";
			//	directionLight.enabled = false;
			//}
			lightIns.Execute();
		}
	}
	
	// Update is called once per frame
	void Update () {
		moveIns.Execute();
		stationBackIns.Execute();
		stationForwardIns.Execute();
		stationLeftIns.Execute();
		stationRightIns.Execute();
		//if (isLightOn)
		//{
			//if (Input.GetKey(KeyCode.A) && activeCapsule.position.x > -12f)
			//{
			//	activeCapsule.Translate(Vector3.left * speedValue * Time.deltaTime, Space.Self);
			//}
			//else if (Input.GetKey(KeyCode.D) && activeCapsule.position.x < 12f)
			//{
			//	activeCapsule.Translate(-Vector3.left * speedValue * Time.deltaTime, Space.Self);
			//}
			//else if (Input.GetKey(KeyCode.W) && activeCapsule.position.z < 12f)
			//{
			//	activeCapsule.Translate(Vector3.forward * speedValue * Time.deltaTime, Space.Self);
			//}
			//else if (Input.GetKey(KeyCode.S) && activeCapsule.position.z > -12f)
			//{
			//	activeCapsule.Translate(-Vector3.forward * speedValue * Time.deltaTime, Space.Self);
			//}

		//	cubeStation.Contains(activeCapsule.position);

		//	cubeStation.CubeRun();
		//}

		//if (Input.GetKeyDown(KeyCode.Q))
		//{
			//FunctionDispatch.Run();
            //lightGlobal.Excute();
		//}
	}
}

public class CubeStation
{
	/// <summary>
	/// The default material.
	/// </summary>
	public Material defaultMaterial;
	/// <summary>
	/// The active material.
	/// </summary>
	public Material activeMaterial;
	/// <summary>
	/// The auto cube.
	/// </summary>
	public GameObject autoCube;
	/// <summary>
	/// The station back.
	/// </summary>
	public GameObject stationBack;
	/// <summary>
	/// The station forward.
	/// </summary>
	public GameObject stationForward;
	/// <summary>
	/// The station left.
	/// </summary>
	public GameObject stationLeft;
	/// <summary>
	/// The station right.
	/// </summary>
	public GameObject stationRight;

	private enum MoveState { Stop = 0, Back, Right, Forward, Left }
	private MoveState currentState = MoveState.Stop;
	private MoveState lockState = MoveState.Stop;
	private bool isMoving = false;
	private bool needChange = false;
	private float timeCounter = 10f;
	private const float STOPTIME = 3F;
	private Vector3 startPos = Vector3.zero;
	private Vector3 endPos = Vector3.zero;
	private int posFlag = 1;
	private Vector3[] positionArray = new Vector3[] { new Vector3(12f, -1.27852f, 12f), new Vector3(12f, -1.27852f, -12f), new Vector3(-12f, -1.27852f, -12f), new Vector3(-12f, -1.27852f, 12f) };

	/// <summary>
	/// Initializes a new instance of the <see cref="CubeStation"/> class.
	/// </summary>
	public CubeStation()
	{ 
	
	}

	/// <summary>
	/// Contains the specified position.
	/// </summary>
	/// <param name="position">Position.</param>
	public void Contains(Vector3 position)
	{
		if (position.x >= -1f && position.x <= 1f && position.z >= 3f && position.z <= 5f)
		{
			if (lockState != MoveState.Back) 
			{
				autoCube.renderer.material = activeMaterial;
				stationBack.renderer.material = activeMaterial;
				stationForward.renderer.material = defaultMaterial;
				stationLeft.renderer.material = defaultMaterial;
				stationRight.renderer.material = defaultMaterial;
				lockState = MoveState.Back;
				isMoving = true;
				if (currentState != lockState)
					needChange = true;
				else
					needChange = false;
			}
		}
		else if (position.x >= -1f && position.x <= 1f && position.z >= -5f && position.z <= -3f)
		{
			if (lockState != MoveState.Forward)
			{
				autoCube.renderer.material = activeMaterial;
				stationBack.renderer.material = defaultMaterial;
				stationForward.renderer.material = activeMaterial;
				stationLeft.renderer.material = defaultMaterial;
				stationRight.renderer.material = defaultMaterial;
				lockState = MoveState.Forward;
				isMoving = true;
				if (currentState != lockState)
					needChange = true;
				else
					needChange = false;
			}
		}
		else if (position.x >= -5f && position.x <= -3f && position.z >= -1f && position.z <= 1f)
		{
			if (lockState != MoveState.Left)
			{
				autoCube.renderer.material = activeMaterial;
				stationBack.renderer.material = defaultMaterial;
				stationForward.renderer.material = defaultMaterial;
				stationLeft.renderer.material = activeMaterial;
				stationRight.renderer.material = defaultMaterial;
				lockState = MoveState.Left;
				isMoving = true;
				if (currentState != lockState)
					needChange = true;
				else
					needChange = false;
			}
		}
		else if (position.x >= 3f && position.x <= 5f && position.z >= -1f && position.z <= 1f)
		{
			if (lockState != MoveState.Right)
			{
				autoCube.renderer.material = activeMaterial;
				stationBack.renderer.material = defaultMaterial;
				stationForward.renderer.material = defaultMaterial;
				stationLeft.renderer.material = defaultMaterial;
				stationRight.renderer.material = activeMaterial;
				lockState = MoveState.Right;
				isMoving = true;
				if (currentState != lockState)
					needChange = true;
				else
					needChange = false;
			}
		}
		else
		{
			if (lockState != MoveState.Stop)
			{
				autoCube.renderer.material = defaultMaterial;
				stationBack.renderer.material = defaultMaterial;
				stationForward.renderer.material = defaultMaterial;
				stationLeft.renderer.material = defaultMaterial;
				stationRight.renderer.material = defaultMaterial;
				isMoving = false;
				lockState = MoveState.Stop;
				if (currentState != lockState)
					needChange = true;
				else
					needChange = false;
			}
		}
	}

	/// <summary>
	/// Cubes the run.
	/// </summary>
	public void CubeRun()
	{
		if (isMoving)
		{
			if (timeCounter >= STOPTIME)
			{
				timeCounter = 0f;
				if (needChange)
				{
					int diffValue = (int)lockState - posFlag;
					if (diffValue == 0)
					{
						int startIndex = posFlag;
						int endIndex = posFlag - 1;
						if (endIndex == 0)
							endIndex = 4;
						startPos = positionArray[startIndex - 1];
						endPos = positionArray[endIndex - 1];
						posFlag = endIndex;
						currentState = lockState;
						needChange = false;
					}
                    else if (diffValue == 1 || diffValue == -3)
                    {
                        int startIndex = posFlag;
                        int endIndex = posFlag + 1;
                        if (endIndex == 5)
                            endIndex = 1;
                        startPos = positionArray[startIndex - 1];
                        endPos = positionArray[endIndex - 1];
                        posFlag = endIndex;
                        currentState = lockState;
                        needChange = false;
                    }
                    else if (diffValue == -1 || diffValue == 3)
                    {
                        int startIndex = posFlag;
                        int endIndex = posFlag - 1;
                        if (endIndex == 0)
                            endIndex = 4;
                        startPos = positionArray[startIndex - 1];
                        endPos = positionArray[endIndex - 1];
                        posFlag = endIndex;
                        currentState = (MoveState)startIndex;
                        needChange = true;
                    }
                    else if (diffValue == -2 || diffValue == 2)
                    {
                        int startIndex = posFlag;
                        int endIndex = posFlag + 1;
                        if (endIndex == 5)
                            endIndex = 1;
                        startPos = positionArray[startIndex - 1];
                        endPos = positionArray[endIndex - 1];
                        posFlag = endIndex;
                        currentState = (MoveState)endIndex;
                        needChange = true;
                    }
				}
				else
				{
					Vector3 tempPos = new Vector3(startPos.x, startPos.y, startPos.z);
					startPos = endPos;
					endPos = tempPos;
					if ((int)currentState - posFlag == 0)
					{
						posFlag--;
						if (posFlag == 0)
							posFlag = 4;
					}
					else 
					{
						posFlag++;
						if (posFlag == 5)
							posFlag = 1;
					}
				}
			}
			timeCounter += Time.deltaTime;
			autoCube.transform.position = Vector3.Lerp(startPos, endPos, timeCounter / STOPTIME);
		}
	}
}

