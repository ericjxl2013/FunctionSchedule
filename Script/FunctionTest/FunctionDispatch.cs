using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public abstract class BaseFunction
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
	}
	private IState _currentState = IState.Wait;

	//当前功能ID
	public string FunctionID
	{
		get { return _id; }
	}
	private string _id;

	//构造函数
	public BaseFunction(string ID, bool is_single, IState ini_state)
	{
		ID = ID.ToUpper();
		_isSingle = is_single;
		_id = ID;
		_currentState = ini_state;
	}

	public virtual void Invalid()
	{
		Debug.Log(FunctionID + " : Invalid Function");
	}

	public virtual void Wait()
	{
		Debug.Log(FunctionID + " : Wait Function");
	}

	public virtual void Run()
	{
		Debug.Log(FunctionID + " : Run Function");
	}

	public virtual void Pause()
	{
		Debug.Log(FunctionID + " : Pause Function");
	}

	public abstract void Execute();
	public abstract void ChangeState(IState change_state);

	public void SetState(IState set_state)
	{
		IState oldState = _currentState;
		_currentState = set_state;
		Debug.Log("ID:" + FunctionID + "---转化前状态：" + oldState);
		Debug.Log("ID:" + FunctionID + "---转化后状态：" + CurrentState);
		FunctionDispatch.FunctionRecursion(FunctionID, oldState, CurrentState);
	}
}

public class FunctionDispatch
{
	private static IList<BaseFunction> FunctionList = new List<BaseFunction>();
	private static List<string> FunctionID = new List<string>();
	private static Dictionary<string, BaseFunction> FunctionDic = new Dictionary<string, BaseFunction>();
	public static LogicParser _conditionCenter;
	private static bool _isInit = false;

	public static void Init(string file_path)
	{
		if(!_isInit)
			_conditionCenter = new LogicParser(file_path);
	}

	public static void AddFunction(BaseFunction func_instace, string id_string)
	{
		if (FunctionDic.ContainsKey(id_string))
		{
			Debug.LogError("类:" + func_instace.ToString() + "---ID:" + id_string + "。该类已经被实例化一次，请勿再次实例化！");
			return;
		}
		FunctionList.Add(func_instace);
		FunctionID.Add(id_string);
		FunctionDic.Add(id_string, func_instace);
	}


	public static bool IsMeetCondition(string ID, IState current_state)
	{
		//条件公式判断
		return _conditionCenter.IsMeetCondition(ID, current_state);
	}


	public static void FunctionRecursion(string ID, IState old_state, IState new_state)
	{
		//快速找出相关功能
		//根据新旧状态快速找到相关功能ID
		//首先索引到功能，在索引到状态
		//导出一个链表，遍历，查询状态变化

		//旧的状态处理
		//ConditionExp oldCond = _conditionCenter.GetCondition(ID, old_state);
		//for (int i = 0; i < oldCond.Count(); i++)
		//{ 
		
		//}
		//新状态处理
		ConditionExp newCond = _conditionCenter.GetCondition(ID, new_state);
		for (int i = 0; i < newCond.Count(); i++)
		{
			BasicElement aimFunc = newCond.GetElement(i);
			//Debug.Log(ID + "---Condtion ID : " + aimFunc.FunctionID + "; State : " + aimFunc.State.ToString());
			if (FunctionDic.ContainsKey(aimFunc.FunctionID))
			{
				if (FunctionDic[aimFunc.FunctionID].CurrentState != aimFunc.State)
					FunctionDic[aimFunc.FunctionID].ChangeState(aimFunc.State);
			}
		}
	}

	public static void Run()
	{
		for (int i = 0; i < FunctionList.Count; i++)
		{
			FunctionList[i].Run();
		}
	}

	//条件计算
	public static bool Compute(BasicElement ele)
	{
		if (FunctionDic.ContainsKey(ele.FunctionID))
		{
			if (FunctionDic[ele.FunctionID].CurrentState == ele.State)
				return true;
			else
				return false;
		}
		else
			return true;
	}

}

public interface IFunction
{
	//是否为单次触发
	bool IsSingle { get; }

	//当前状态
	IState CurrentState { get; }

	//当前功能ID
	string FunctionID { get; set; }

	//失效
	void Invalid();

	//等待
	void Wait();

	//运行
	void Run();

	//暂停
	void Pause();
}


