using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class LightGlobal : IFunction
{ 
	//是否为单次触发
	public bool IsSingle
	{
		get { return _isSingle; }
	}
	private bool _isSingle = false;

	//当前状态
	public IState CurrentState
	{
		get { return _currentState; }
        set
        {
            if (_currentState != value)
            {
                Debug.Log("HAHA");
            }
            _currentState = value; 
        }
	}
	private IState _currentState = IState.Run;

    //当前功能ID
    public string FunctionID
    {
        get { return _id; }
        set { _id = value; }
    }
    private string _id;


    private Light _directionLight;

	//构造函数
	public LightGlobal(string ID, bool is_single, IState ini_state, Light light)
	{
		_isSingle = is_single;
		_currentState = ini_state;
        _directionLight = light;
        _id = ID;
		//FunctionDispatch.AddFunction(this, ID);
	}

	public void Invalid()
	{
		Debug.Log("LightInvalid");
	}

	public void Wait()
	{
		Debug.Log("LightWait");
        _directionLight.enabled = false;
	}

	public void Run()
	{
        Debug.Log("LightRun");
        _directionLight.enabled = true;
        //CurrentState = IState.Wait;
	}

	public void Pause()
	{
        Debug.Log("LightPause");
	}

    public void Excute()
    {
        if (CurrentState != IState.Invalid)
        {
            if (FunctionDispatch.IsMeetCondition(FunctionID, CurrentState))  //条件公式判断，用功能ID去索引
            {
                if (IsSingle)  //为单次触发
                {
                    if (CurrentState == IState.Run)
                    {
                        Wait();
                        CurrentState = IState.Wait;
                    }
                    else
                    {
                        Run();
                        CurrentState = IState.Run;
                    } 
                }
                else  //为连续触发
                { 
                
                }
            }
        }
    }
}

public class CubeRun : IFunction
{
    //是否为单次触发
	public bool IsSingle
	{
		get { return _isSingle; }
	}
	private bool _isSingle = false;

	//当前状态
	public IState CurrentState
	{
		get { return _currentState; }
		set { _currentState = value; }
	}
	private IState _currentState = IState.Run;

    //当前功能ID
    public string FunctionID
    {
        get { return _id; }
        set { _id = value; }
    }
    private string _id;

	//构造函数
    public CubeRun(string ID, bool is_single, IState ini_state)
	{
		_isSingle = is_single;
        _id = ID;
		//FunctionDispatch.AddFunction(this, ID);
	}

	public void Invalid()
	{
		Debug.Log("CubeInvalid");
	}

	public void Wait()
	{
		Debug.Log("CubeWait");
	}

	public void Run()
	{
		Debug.Log("CubeRun");
	}

	public void Pause()
	{
		Debug.Log("CubePause");
	}

}


public class SphereRun : IFunction
{
    //是否为单次触发
	public bool IsSingle
	{
		get { return _isSingle; }
	}
	private bool _isSingle = false;

	//当前状态
	public IState CurrentState
	{
		get { return _currentState; }
		set { _currentState = value; }
	}
	private IState _currentState = IState.Run;

    //当前功能ID
    public string FunctionID
    {
        get { return _id; }
        set { _id = value; }
    }
    private string _id;

	//构造函数
    public SphereRun(string ID, bool is_single, IState ini_state)
	{
		_isSingle = is_single;
        _id = ID;
		//FunctionDispatch.AddFunction(this, ID);
	}

	public void Invalid()
	{
		Debug.Log("SphereInvalid");
	}

	public void Wait()
	{
		Debug.Log("SphereWait");
	}

	public void Run()
	{
		Debug.Log("SphereRun");
	}

	public void Pause()
	{
		Debug.Log("SpherePause");
	}
}
