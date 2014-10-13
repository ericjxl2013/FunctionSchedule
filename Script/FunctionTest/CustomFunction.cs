using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//灯控制类
public class LightFunc : BaseFunction
{
	private Light _light;

	public LightFunc(string ID, bool is_single, IState ini_state, Light light)
		: base(ID, is_single, ini_state)
	{
		ID = ID.ToUpper();
		_light = light;
		FunctionDispatch.AddFunction(this, ID);
	}

	public override void Execute()
	{
		if (CurrentState == IState.Wait)
		{
			ChangeState(IState.Run);
		}
		else if (CurrentState == IState.Run)
		{
			ChangeState(IState.Wait);
		}
		else if (CurrentState == IState.Pause)
		{

		}
		else
		{

		}
	}

	public override void ChangeState(IState change_state)
	{
		if (FunctionDispatch.IsMeetCondition(FunctionID, change_state))
		{
			if (change_state == IState.Wait)
			{
				Wait();
				SetState(change_state);
			}
			else if (change_state == IState.Run)
			{
				Run();
				SetState(change_state);
			}
		}
	}

	public override void Wait()
	{
		_light.enabled = false;
		Debug.Log("灯灭，转为等待状态");
	}

	public override void Run()
	{
		_light.enabled = true;
		Debug.Log("灯亮，转为运行状态");
	}
}

//平移控制类
public class MoveFunc : BaseFunction
{
	private Transform _moveTrans;
	private float _speedValue = 4f;  //平移速率

	public MoveFunc(string ID, bool is_single, IState ini_state, Transform trans)
		: base(ID, is_single, ini_state)
	{
		ID = ID.ToUpper();
		_moveTrans = trans;
		FunctionDispatch.AddFunction(this, ID);
	}

	public override void Execute()
	{
		if (CurrentState == IState.Wait)
		{

		}
		else if (CurrentState == IState.Run)
		{
			Run();
		}
		else if (CurrentState == IState.Pause)
		{

		}
		else
		{

		}
	}

	public override void ChangeState(IState change_state)
	{
		if (FunctionDispatch.IsMeetCondition(FunctionID, change_state))
		{
			if (change_state == IState.Invalid)
			{
				SetState(change_state);
			}
			else if (change_state == IState.Run)
			{
				SetState(change_state);
			}
		}
	}

	public override void Run()
	{
		if (Input.GetKey(KeyCode.A) && _moveTrans.position.x > -12f)
		{
			_moveTrans.Translate(Vector3.left * _speedValue * Time.deltaTime, Space.Self);
		}
		else if (Input.GetKey(KeyCode.D) && _moveTrans.position.x < 12f)
		{
			_moveTrans.Translate(-Vector3.left * _speedValue * Time.deltaTime, Space.Self);
		}
		else if (Input.GetKey(KeyCode.W) && _moveTrans.position.z < 12f)
		{
			_moveTrans.Translate(Vector3.forward * _speedValue * Time.deltaTime, Space.Self);
		}
		else if (Input.GetKey(KeyCode.S) && _moveTrans.position.z > -12f)
		{
			_moveTrans.Translate(-Vector3.forward * _speedValue * Time.deltaTime, Space.Self);
		}
	}
}

//Station1控制类
public class Station1Func : BaseFunction
{
	private Transform _moveTrans;
	private GameObject _backStation;
	private Material _defaultMaterial;
	private Material _activeMaterial;
	private bool _lock = false;

	public Station1Func(string ID, bool is_single, IState ini_state, Transform trans, GameObject station_back, Material defaultMa, Material activeMa)
		: base(ID, is_single, ini_state)
	{
		ID = ID.ToUpper();
		_moveTrans = trans;
		_backStation = station_back;
		_defaultMaterial = defaultMa;
		_activeMaterial = activeMa;
		FunctionDispatch.AddFunction(this, ID);
	}

	public override void Execute()
	{
		if (_moveTrans.position.x >= -1f && _moveTrans.position.x <= 1f && _moveTrans.position.z >= 3f && _moveTrans.position.z <= 5f)
		{
			if (!_lock && CurrentState != IState.Invalid)
			{
				{
					ChangeState(IState.Run);
				}
			}
		}
		else
		{
			if (_lock && CurrentState != IState.Invalid)
			{
				if (CurrentState == IState.Run)
				{
					ChangeState(IState.Wait);
				}
			}
		}
	}

	public override void ChangeState(IState change_state)
	{
		if (FunctionDispatch.IsMeetCondition(FunctionID, change_state))
		{
			if (change_state == IState.Invalid)
			{
				Invalid();
				SetState(change_state);
			}
			else if (change_state == IState.Run)
			{
				Run();
				SetState(change_state);
			}
			else if (change_state == IState.Wait)
			{
				Wait();
				SetState(change_state);
			}
		}
	}

	public override void Invalid()
	{
		_backStation.renderer.material = _defaultMaterial;
		_lock = false;
	}

	public override void Wait()
	{
		_backStation.renderer.material = _defaultMaterial;
		_lock = false;
	}

	public override void Run()
	{
		_backStation.renderer.material = _activeMaterial;
		_lock = true;
	}
}

//Station2控制类
public class Station2Func : BaseFunction
{
	private Transform _moveTrans;
	private GameObject _backStation;
	private Material _defaultMaterial;
	private Material _activeMaterial;
	private bool _lock = false;

	public Station2Func(string ID, bool is_single, IState ini_state, Transform trans, GameObject station_back, Material defaultMa, Material activeMa)
		: base(ID, is_single, ini_state)
	{
		ID = ID.ToUpper();
		_moveTrans = trans;
		_backStation = station_back;
		_defaultMaterial = defaultMa;
		_activeMaterial = activeMa;
		FunctionDispatch.AddFunction(this, ID);
	}

	public override void Execute()
	{
		if (_moveTrans.position.x >= -1f && _moveTrans.position.x <= 1f && _moveTrans.position.z >= -5f && _moveTrans.position.z <= -3f)
		{
			if (!_lock && CurrentState != IState.Invalid)
			{
				{
					ChangeState(IState.Run);
				}
			}
		}
		else
		{
			if (_lock && CurrentState != IState.Invalid)
			{
				if (CurrentState == IState.Run)
				{
					ChangeState(IState.Wait);
				}
			}
		}
	}

	public override void ChangeState(IState change_state)
	{
		if (FunctionDispatch.IsMeetCondition(FunctionID, change_state))
		{
			if (change_state == IState.Invalid)
			{
				Invalid();
				SetState(change_state);
			}
			else if (change_state == IState.Run)
			{
				Run();
				SetState(change_state);
			}
			else if (change_state == IState.Wait)
			{
				Wait();
				SetState(change_state);
			}
		}
	}

	public override void Invalid()
	{
		_backStation.renderer.material = _defaultMaterial;
		_lock = false;
	}

	public override void Wait()
	{
		_backStation.renderer.material = _defaultMaterial;
		_lock = false;
	}

	public override void Run()
	{
		_backStation.renderer.material = _activeMaterial;
		_lock = true;
	}
}

//Station3控制类
public class Station3Func : BaseFunction
{
	private Transform _moveTrans;
	private GameObject _backStation;
	private Material _defaultMaterial;
	private Material _activeMaterial;
	private bool _lock = false;

	public Station3Func(string ID, bool is_single, IState ini_state, Transform trans, GameObject station_back, Material defaultMa, Material activeMa)
		: base(ID, is_single, ini_state)
	{
		ID = ID.ToUpper();
		_moveTrans = trans;
		_backStation = station_back;
		_defaultMaterial = defaultMa;
		_activeMaterial = activeMa;
		FunctionDispatch.AddFunction(this, ID);
	}

	public override void Execute()
	{
		if (_moveTrans.position.x >= -5f && _moveTrans.position.x <= -3f && _moveTrans.position.z >= -1f && _moveTrans.position.z <= 1f)
		{
			if (!_lock && CurrentState != IState.Invalid)
			{
				{
					ChangeState(IState.Run);
				}
			}
		}
		else
		{
			if (_lock && CurrentState != IState.Invalid)
			{
				if (CurrentState == IState.Run)
				{
					ChangeState(IState.Wait);
				}
			}
		}
	}

	public override void ChangeState(IState change_state)
	{
		if (FunctionDispatch.IsMeetCondition(FunctionID, change_state))
		{
			if (change_state == IState.Invalid)
			{
				Invalid();
				SetState(change_state);
			}
			else if (change_state == IState.Run)
			{
				Run();
				SetState(change_state);
			}
			else if (change_state == IState.Wait)
			{
				Wait();
				SetState(change_state);
			}
		}
	}

	public override void Invalid()
	{
		_backStation.renderer.material = _defaultMaterial;
		_lock = false;
	}

	public override void Wait()
	{
		_backStation.renderer.material = _defaultMaterial;
		_lock = false;
	}

	public override void Run()
	{
		_backStation.renderer.material = _activeMaterial;
		_lock = true;
	}
}

//Station4控制类
public class Station4Func : BaseFunction
{
	private Transform _moveTrans;
	private GameObject _backStation;
	private Material _defaultMaterial;
	private Material _activeMaterial;
	private bool _lock = false;

	public Station4Func(string ID, bool is_single, IState ini_state, Transform trans, GameObject station_back, Material defaultMa, Material activeMa)
		: base(ID, is_single, ini_state)
	{
		ID = ID.ToUpper();
		_moveTrans = trans;
		_backStation = station_back;
		_defaultMaterial = defaultMa;
		_activeMaterial = activeMa;
		FunctionDispatch.AddFunction(this, ID);
	}

	public override void Execute()
	{
		if (CurrentState != IState.Invalid)
		{
			if (_moveTrans.position.x >= 3f && _moveTrans.position.x <= 5f && _moveTrans.position.z >= -1f && _moveTrans.position.z <= 1f)
			{
				if (!_lock)
				{
					{
						ChangeState(IState.Run);
					}
				}
			}
			else
			{
				if (_lock)
				{
					if (CurrentState == IState.Run)
					{
						ChangeState(IState.Wait);
					}
				}
			}
		}
	}

	public override void ChangeState(IState change_state)
	{
		if (FunctionDispatch.IsMeetCondition(FunctionID, change_state))
		{
			if (change_state == IState.Invalid)
			{
				Invalid();
				SetState(change_state);
			}
			else if (change_state == IState.Run)
			{
				Run();
				SetState(change_state);
			}
			else if (change_state == IState.Wait)
			{
				Wait();
				SetState(change_state);
			}
		}
	}

	public override void Invalid()
	{
		_backStation.renderer.material = _defaultMaterial;
		_lock = false;
	}

	public override void Wait()
	{
		_backStation.renderer.material = _defaultMaterial;
		_lock = false;
	}

	public override void Run()
	{
		_backStation.renderer.material = _activeMaterial;
		_lock = true;
	}
}

