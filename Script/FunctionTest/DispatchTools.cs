using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

//单个功能的四种状态，失效、等待、运行和暂停
public enum IState { Invalid, Wait, Run, Pause }

//条件表达式基本单元
public struct BasicElement
{
	//功能ID
	public string FunctionID
	{
		get { return _functionID; }
	}
	private string _functionID;

	//功能状态
	public IState State
	{
		get { return _state; }
	}
	private IState _state;

	//初始化
	public BasicElement(string id, IState state)
	{
		this._functionID = id;
		this._state = state;
	}
}

//条件表达式存储
public struct ConditionExp
{
	//后缀表达式
	private List<BasicElement> Expression;

	//初始化
	public ConditionExp(List<BasicElement> expression)
	{
		Expression = new List<BasicElement>();
		for (int i = 0; i < expression.Count; i++)
		{
			BasicElement tempEle = expression[i];
			this.Expression.Add(tempEle);
		}
	}

	public void Add(BasicElement cond)
	{
		this.Expression.Add(cond);
	}

	public bool Contains(BasicElement cond)
	{
		return Expression.Contains(cond);
	}

	public int Count()
	{
		return Expression.Count;
	}

	public BasicElement GetElement(int index)
	{
		if (index < Expression.Count)
			return Expression[index];
		else
			return new BasicElement("", IState.Invalid);
	}

	public bool Compute()
	{
		if (Expression.Count == 0)  //默认为True
			return true;
		else
		{
			List<bool> oprandList = new List<bool>();
			for (int i = 0; i < Expression.Count; i++)
			{
				if (Expression[i].FunctionID == "!")
				{
					oprandList[oprandList.Count - 1] = !oprandList[oprandList.Count - 1];  //模拟出栈
				}
				else if (Expression[i].FunctionID == "|")
				{
					oprandList[oprandList.Count - 2] = oprandList[oprandList.Count - 1] || oprandList[oprandList.Count - 2];  //模拟出栈
					oprandList.RemoveAt(oprandList.Count - 1);
				}
				else if (Expression[i].FunctionID == "&")
				{
					oprandList[oprandList.Count - 2] = oprandList[oprandList.Count - 1] && oprandList[oprandList.Count - 2];  //模拟出栈
					oprandList.RemoveAt(oprandList.Count - 1);
				}
				else
				{
					oprandList.Add(FunctionDispatch.Compute(Expression[i]));  //模拟进栈
				}
			}
			return oprandList[0];
		}
	}
}

/// <summary>
/// 逻辑表达式解析器
/// </summary>
public class LogicParser
{
	//符号优先级查询
	private Dictionary<string, int> _precedenceDic = new Dictionary<string, int>();
	//条件表达式查询
	private Dictionary<BasicElement, ConditionExp> _conditionExpDic = new Dictionary<BasicElement, ConditionExp>();
	//总功能ID缓存
	private Dictionary<string, bool> _totalFunctionDic = new Dictionary<string, bool>();
	//每个状态是哪些状态的条件索引
	private Dictionary<BasicElement, ConditionExp> _conditionIndexDic = new Dictionary<BasicElement, ConditionExp>();

	public LogicParser(string file_path)
	{
		_precedenceDic.Add("(", 2); _precedenceDic.Add(")", 2); _precedenceDic.Add("（", 2); _precedenceDic.Add("）", 2);
		_precedenceDic.Add("|", 1); _precedenceDic.Add("&", 1); _precedenceDic.Add("!", 1); _precedenceDic.Add("！", 1);
		ExpressionAnalysis(file_path, new string[] { "FUNCTIONLIST", "LOGIC" }, "A", "E");
	}

	/// <summary>
	/// Excel中条件表达式的编译
	/// </summary>
	/// <param name="file_path">表格路径</param>
	/// <param name="sheet_name">表格sheet名</param>
	/// <param name="start_column">起始列</param>
	/// <param name="end_column">终止列</param>
	private void ExpressionAnalysis(string file_path, string[] sheet_name, string start_column, string end_column)
	{
		ExcelOperator excelOp = new ExcelOperator();
		DataSet functionSet = excelOp.ExcelReader(file_path, sheet_name, start_column, end_column);
		if (functionSet.Tables.Count == 2) //加载两个sheet
		{
			//首先加载Function List表格
			for (int i = 0; i < functionSet.Tables["[FUNCTIONLIST$]"].Rows.Count; i++)
			{
				_totalFunctionDic.Add((string)functionSet.Tables["[FUNCTIONLIST$]"].Rows[i][0].ToString().ToUpper(), false);
			}
			//再加载LOGIC表格
			for (int i = 0; i < functionSet.Tables["[LOGIC$]"].Rows.Count; i++)
			{
				string funcID = (string)functionSet.Tables["[LOGIC$]"].Rows[i][0].ToString().ToUpper();
				FunctionWarnning(file_path, "LOGIC", i + 2, funcID, "A");  //检查是否少了这个功能ID
				//失效状态
				StateInterprete(i, funcID, (string)functionSet.Tables["[LOGIC$]"].Rows[i][1].ToString().ToUpper().Trim(), file_path, IState.Invalid);
				//等待状态
				StateInterprete(i, funcID, (string)functionSet.Tables["[LOGIC$]"].Rows[i][2].ToString().ToUpper().Trim(), file_path, IState.Wait);
				//运行状态
				StateInterprete(i, funcID, (string)functionSet.Tables["[LOGIC$]"].Rows[i][3].ToString().ToUpper().Trim(), file_path, IState.Run);
				//暂停状态
				StateInterprete(i, funcID, (string)functionSet.Tables["[LOGIC$]"].Rows[i][4].ToString().ToUpper().Trim(), file_path, IState.Pause);
			}
		}
		else
		{
			Debug.LogError(file_path + "，功能表格FUNCTIONLIST或LOGIC不正确，请检查！");
		}
	}

	//单个状态条件表达式解读
	private void StateInterprete(int i, string funcID, string condString, string file_path, IState state)
	{
		BasicElement funcEle = new BasicElement(funcID, state);
		string columnStr = "B";
		if (state == IState.Invalid)
			columnStr = "B";
		else if (state == IState.Wait)
			columnStr = "C";
		else if (state == IState.Run)
			columnStr = "D";
		else
			columnStr = "E";
		if (!FunctionWarnning(file_path, "LOGIC", i + 2, funcEle, columnStr))  //检查是否多了这个功能ID
		{
			List<BasicElement> elementList = new List<BasicElement>();
			List<bool> operatorList = new List<bool>();
			if (condString != "")  //条件不为空
			{
				//字符串条件表达式编译
				List<string> postfixList = new List<string>();
				string errorString = "";
				bool isCompileRight = true;
				ExpressionParse(condString, postfixList, operatorList, ref isCompileRight, ref errorString);
				if (!isCompileRight)
				{
					Debug.LogError("路径:" + file_path + "---Sheet:LOGIC" + "---位置:(" + (i + 2) + ", " + columnStr + ")---ID:" + funcID + "---状态:" + state.ToString() + "---错误:该表达式编译有错误，表达式计算结果可能已有误，请重新检查！");
					Debug.LogError("编译错误信息:" + errorString);
				}
				bool isExtractRight = true;
				List<string> oprandList = new List<string>();
				//条件提取，并尝试用字符串计算一遍，还是用栈的方式解析后缀表达式，采用list模拟进出栈
				for (int j = 0; j < postfixList.Count; j++)
				{
					if (postfixList[j] == "!")
					{
						if (oprandList.Count > 0)
						{
							elementList.Add(new BasicElement("!", IState.Invalid));
						}
						else
						{
							isExtractRight = false;
						}
					}
					else if (postfixList[j] == "|")
					{
						if (oprandList.Count > 1)
						{
							oprandList.RemoveAt(oprandList.Count - 1);  //操作数两个进行布尔运算变一个
							elementList.Add(new BasicElement("|", IState.Invalid));
						}
						else
						{
							isExtractRight = false;
						}
					}
					else if (postfixList[j] == "&")
					{
						if (oprandList.Count > 1)
						{
							oprandList.RemoveAt(oprandList.Count - 1);  //操作数两个进行布尔运算变一个
							elementList.Add(new BasicElement("&", IState.Invalid));
						}
						else
						{
							isExtractRight = false;
						}
					}
					else
					{
						if (j == postfixList.Count - 1 && postfixList.Count > 1) //最后一个不能为操作数，当然要总数大于1
						{
							isExtractRight = false;
						}
						else
						{
							oprandList.Add(postfixList[j]);  //操作数进栈
							BasicElement tempEle = ConditionAnalysis(postfixList[j], ref isExtractRight);
							if (isExtractRight)
							{
								elementList.Add(tempEle);
								//检查是否少了这个功能ID
								FunctionWarnning(file_path, "LOGIC", i + 2, tempEle.FunctionID, columnStr);
								//将条件所属功能与条件对应起来，在递归遍历时用来索引到其他功能
								if (_conditionIndexDic.ContainsKey(tempEle))
								{
									if (!_conditionIndexDic[tempEle].Contains(funcEle))
										_conditionIndexDic[tempEle].Add(funcEle);
								}
								else
								{
									_conditionIndexDic.Add(tempEle, new ConditionExp(new List<BasicElement>() { funcEle }));
								}
							}
						}
					}
					if (!isExtractRight)  //表达式有错误
					{
						Debug.LogError("路径:" + file_path + "---Sheet:LOGIC" + "---位置:(" + (i + 2) + ", " + columnStr + ")---ID:" + funcID + "---状态:" + state.ToString() + "---错误:表达式格式错误，请重新检查！");
						Debug.LogError("表达式:" + condString + "---该表达式现为空，对应的功能处于自由状态！");
						elementList.Clear();
						break;
					}
				}
			}
			_conditionExpDic.Add(funcEle, new ConditionExp(elementList));
		}
	}

	//条件解析
	private BasicElement ConditionAnalysis(string cond, ref bool is_right)
	{
		string[] condArray = cond.Split('@');
		if (condArray.Length != 2)
		{
			is_right = false;
			return new BasicElement("", IState.Invalid);
		}
		string funcID = condArray[0];
		IState state = IState.Invalid;
		if (condArray[1] == "S")
			state = IState.Invalid;
		else if (condArray[1] == "D")
			state = IState.Wait;
		else if (condArray[1] == "Y")
			state = IState.Run;
		else if (condArray[1] == "Z")
			state = IState.Pause;
		else
		{
			is_right = false;
			return new BasicElement("", IState.Invalid);
		}
		return new BasicElement(funcID, state);
	}


	//少功能ID判断
	private void FunctionWarnning(string file_path, string sheet_name, int row_index, string func_id, string column)
	{
		if (!_totalFunctionDic.ContainsKey(func_id))
		{
			Debug.LogWarning("路径:" + file_path + "---Sheet:" + sheet_name + "---位置:(" + row_index + ", " + column + ")---ID:" + func_id + "---错误:该ID在FUNCTIONLIST表中未列出！");
		}
	}

	//多次写条件表达式判断
	private bool FunctionWarnning(string file_path, string sheet_name, int row_index, BasicElement func_element, string column)
	{
		if (_conditionExpDic.ContainsKey(func_element))
		{
			Debug.LogWarning("路径:" + file_path + "---Sheet:" + sheet_name + "---位置:(" + row_index + ", " + column + ")---ID:" + func_element.FunctionID + "---状态:" + func_element.State.ToString() + "---错误:该条件进行了重复定义！");
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 运算符优先级判断，数值越大，优先级越高
	/// </summary>
	/// <param name="symbol">传入符号</param>
	/// <param name="priority_figure">优先级数值</param>
	/// <returns>是否存在该符号</returns>
	private bool SymbolPriority(string symbol, ref int priority_figure)
	{
		if (!_precedenceDic.ContainsKey(symbol))
		{
			return false;
		}
		else
		{
			priority_figure = _precedenceDic[symbol];
			return true;
		}
	}

	/// <summary>
	/// 栈方法解析简单数学表达式
	/// </summary>
	/// <param name="exp_string">表达式字符串</param>
	/// <param name="postfixList">后缀表达式链表</param>
	/// <param name="operator_list">后缀表达式运算符标示</param>
	/// <param name="is_right">表达式编译是否正确</param>
	/// <param name="error_string">编译错误信息</param>
	private void ExpressionParse(string exp_string, List<string> postfixList, List<bool> operator_list, ref bool is_right, ref string error_string)
	{
		exp_string = exp_string.ToUpper().Trim();
		int priorityFigure = 0;
		StringBuilder symbolBuilder = new StringBuilder();
		Stack operatorStack = new Stack();  //操作符栈
		string lastCh = "";
		for (int i = 0; i < exp_string.Length; i++)
		{
			string singleStr = exp_string.Substring(i, 1);
			if (singleStr == " ")  //空格删除
				continue;
			if (SymbolPriority(singleStr, ref priorityFigure))  //符号处理
			{
				if (priorityFigure == 1)  //逻辑与“&”、逻辑或“|”、非"!"
				{
					if (singleStr == "!" || singleStr == "！")  //非"!"处理
					{
						if (i == exp_string.Length - 1)
						{
							error_string += "表达式结尾不能为'|'、'&'、'('、'!'。\n";
							is_right = false;
							continue;
						}
						else if (lastCh == ")" || lastCh == "）")
						{
							error_string += "表达式中'!'前不能为')'。\n";
							is_right = false;
							continue;
						}
						operatorStack.Push("!");
						lastCh = "!";
						continue;
					}
					//逻辑与“&”、逻辑或“|”处理
					if (i == 0)  //起始位置错误判断
					{
						error_string += "表达式开始时不能为'|'、'&'、')'。\n";
						is_right = false;
						continue;
					}
					else if (i == exp_string.Length - 1)
					{
						error_string += "表达式结尾不能为'|'、'&'、'('、'!'。\n";
						is_right = false;
						continue;
					}
					else if (lastCh == "|" || lastCh == "&" || lastCh == "(" || lastCh == "（" || lastCh == "!" || lastCh == "！")
					{
						error_string += "表达式中符号'|'或'&'前不能为'|'、'&'、'('、'!'。\n";
						is_right = false;
						continue;
					}
					string tempCon = symbolBuilder.ToString();
					postfixList.Add(tempCon);
					operator_list.Add(false);
					symbolBuilder.Remove(0, symbolBuilder.Length);
					if (operatorStack.Count == 0)  //栈为空直接将符号压入栈中
					{
						operatorStack.Push(singleStr);
					}
					else
					{
						do
						{
							string stackTop = (string)operatorStack.Pop();  //栈顶元素
							if (_precedenceDic[singleStr] > _precedenceDic[stackTop] || stackTop == "(" || stackTop == "（")  //栈顶元素优先级小于读取的操作符或栈顶为“(”
							{
								operatorStack.Push(stackTop);
								operatorStack.Push(singleStr);
								break;
							}
							else
							{
								postfixList.Add(stackTop);  //栈顶优先级比较高或同优先级不断弹出直至最后
								operator_list.Add(true);
								if (operatorStack.Count == 0)
								{
									operatorStack.Push(singleStr);
									break;
								}
							}
						}
						while (operatorStack.Count != 0);
					}
				}
				else  //括号
				{
					if (singleStr == "(" || singleStr == "（")
					{  //前括号放入栈中
						if (i == exp_string.Length - 1)
						{
							error_string += "表达式结尾不能为'|'、'&'、'('、'!'。\n";
							is_right = false;
							continue;
						}
						else if (lastCh == ")" || lastCh == "）")
						{
							error_string += "表达式中'('前不能为')'。\n";
							is_right = false;
							continue;
						}
						else if (!(lastCh == "" || lastCh == "!" || lastCh == "！" || lastCh == "(" || lastCh == "（" || lastCh == "|" || lastCh == "&"))
						{
							error_string += "表达式中'('前应为操作符。\n";
							is_right = false;
							continue;
						}
						operatorStack.Push(singleStr);
					}
					else  //后括号处
					{
						if (i == 0)  //起始位置错误判断
						{
							error_string += "表达式开始时不能为'|'、'&'、')'。\n";
							is_right = false;
							continue;
						}
						else if (lastCh == "(" || lastCh == "（" || lastCh == "|" || lastCh == "&" || lastCh == "!" || lastCh == "！")
						{
							error_string += "表达式中')'前不能为'('、'!'、'&'、'|'，应为操作数。\n";
							is_right = false;
							continue;
						}
						string tempCon = symbolBuilder.ToString();
						postfixList.Add(tempCon);
						operator_list.Add(false);
						symbolBuilder.Remove(0, symbolBuilder.Length);
						while (operatorStack.Count != 0)  //不断弹出栈中的内容，直到遇到"("
						{
							singleStr = (string)operatorStack.Pop();
							if (singleStr == "(" || singleStr == "（")
								break;
							else  //加入后缀表达式列表
							{
								postfixList.Add(singleStr);
								operator_list.Add(true);
							}
						}
					}
				}
			}
			else  //字符串处理
			{
				symbolBuilder.Append(singleStr);
				if (i == exp_string.Length - 1)
				{  //最后的字符串处理
					if (symbolBuilder.Length > 0)
					{
						string tempCon = symbolBuilder.ToString();
						postfixList.Add(tempCon);
						operator_list.Add(false);
						symbolBuilder.Remove(0, symbolBuilder.Length);
					}
				}
			}
			lastCh = exp_string.Substring(i, 1);
		}
		while (operatorStack.Count != 0)  //栈中剩余操作符弹出
		{
			string popStr = (string)operatorStack.Pop();
			if (popStr == "(" || popStr == "（")
			{
				error_string += "表达式中有多余的'('符号，表达式计算结果可能已出错。\n";
				is_right = false;
			}
			else
			{
				postfixList.Add(popStr);
				operator_list.Add(true);
			}
		}
		//Debug.Log("输入：" + exp_string);
		symbolBuilder.Remove(0, symbolBuilder.Length);
		for (int i = 0; i < postfixList.Count; i++)
		{
			symbolBuilder.Append(postfixList[i]);
		}
		//Debug.Log("输出：" + symbolBuilder.ToString());
		if (!is_right)
			Debug.LogWarning("错误信息:" + error_string);
	}

	public void ExpressionParse(string exp_string)
	{
		string error_string = "";
		bool is_right = true;
		exp_string = exp_string.ToUpper().Trim();
		int priorityFigure = 0;
		StringBuilder symbolBuilder = new StringBuilder();
		Stack operatorStack = new Stack();  //操作符栈
		List<string> postfixList = new List<string>(); //后缀表达式链表
		List<bool> operator_list = new List<bool>();  //false:不是符号；true:是符号
		string lastCh = "";
		for (int i = 0; i < exp_string.Length; i++)
		{
			string singleStr = exp_string.Substring(i, 1);
			if (singleStr == " ")  //空格删除
				continue;
			if (SymbolPriority(singleStr, ref priorityFigure))  //符号处理
			{
				if (priorityFigure == 1)  //逻辑与“&”、逻辑或“|”、非"!"
				{
					if (singleStr == "!" || singleStr == "！")  //非"!"处理
					{
						if (i == exp_string.Length - 1)
						{
							error_string += "表达式结尾不能为'|'、'&'、'('、'!'。\n";
							is_right = false;
							continue;
						}
						else if (lastCh == ")" || lastCh == "）")
						{
							error_string += "表达式中'!'前不能为')'。\n";
							is_right = false;
							continue;
						}
						operatorStack.Push("!");
						lastCh = "!";
						continue;
					}
					//逻辑与“&”、逻辑或“|”处理
					if (i == 0)  //起始位置错误判断
					{
						error_string += "表达式开始时不能为'|'、'&'、')'。\n";
						is_right = false;
						continue;
					}
					else if (i == exp_string.Length - 1)
					{
						error_string += "表达式结尾不能为'|'、'&'、'('、'!'。\n";
						is_right = false;
						continue;
					}
					else if (lastCh == "|" || lastCh == "&" || lastCh == "(" || lastCh == "（" || lastCh == "!" || lastCh == "！")
					{
						error_string += "表达式中符号'|'或'&'前不能为'|'、'&'、'('、'!'。\n";
						is_right = false;
						continue;
					}
					string tempCon = symbolBuilder.ToString();
					postfixList.Add(tempCon);
					operator_list.Add(false);
					symbolBuilder.Remove(0, symbolBuilder.Length);
					if (operatorStack.Count == 0)  //栈为空直接将符号压入栈中
					{
						operatorStack.Push(singleStr);
					}
					else
					{
						do
						{
							string stackTop = (string)operatorStack.Pop();  //栈顶元素
							if (_precedenceDic[singleStr] > _precedenceDic[stackTop] || stackTop == "(" || stackTop == "（")  //栈顶元素优先级小于读取的操作符或栈顶为“(”
							{
								operatorStack.Push(stackTop);
								operatorStack.Push(singleStr);
								break;
							}
							else
							{
								postfixList.Add(stackTop);  //栈顶优先级比较高或同优先级不断弹出直至最后
								operator_list.Add(true);
								if (operatorStack.Count == 0)
								{
									operatorStack.Push(singleStr);
									break;
								}
							}
						}
						while (operatorStack.Count != 0);
					}
				}
				else  //括号
				{
					if (singleStr == "(" || singleStr == "（")
					{  //前括号放入栈中
						if (i == exp_string.Length - 1)
						{
							error_string += "表达式结尾不能为'|'、'&'、'('、'!'。\n";
							is_right = false;
							continue;
						}
						else if (lastCh == ")" || lastCh == "）")
						{
							error_string += "表达式中'('前不能为')'。\n";
							is_right = false;
							continue;
						}
						else if (!(lastCh == "" || lastCh == "!" || lastCh == "！" || lastCh == "(" || lastCh == "（" || lastCh == "|" || lastCh == "&"))
						{
							error_string += "表达式中'('前应为操作符。\n";
							is_right = false;
							continue;
						}
						operatorStack.Push(singleStr);
					}
					else  //后括号处
					{
						if (i == 0)  //起始位置错误判断
						{
							error_string += "表达式开始时不能为'|'、'&'、')'。\n";
							is_right = false;
							continue;
						}
						else if (lastCh == "(" || lastCh == "（" || lastCh == "|" || lastCh == "&" || lastCh == "!" || lastCh == "！")
						{
							error_string += "表达式中')'前不能为'('、'!'、'&'、'|'，应为操作数。\n";
							is_right = false;
							continue;
						}
						string tempCon = symbolBuilder.ToString();
						postfixList.Add(tempCon);
						operator_list.Add(false);
						symbolBuilder.Remove(0, symbolBuilder.Length);
						while (operatorStack.Count != 0)  //不断弹出栈中的内容，直到遇到"("
						{
							singleStr = (string)operatorStack.Pop();
							if (singleStr == "(" || singleStr == "（")
								break;
							else  //加入后缀表达式列表
							{
								postfixList.Add(singleStr);
								operator_list.Add(true);
							}
						}
					}
				}
			}
			else  //字符串处理
			{
				symbolBuilder.Append(singleStr);
				if (i == exp_string.Length - 1)
				{  //最后的字符串处理
					if (symbolBuilder.Length > 0)
					{
						string tempCon = symbolBuilder.ToString();
						postfixList.Add(tempCon);
						operator_list.Add(false);
						symbolBuilder.Remove(0, symbolBuilder.Length);
					}
				}
			}
			lastCh = exp_string.Substring(i, 1);
		}
		while (operatorStack.Count != 0)  //栈中剩余操作符弹出
		{
			string popStr = (string)operatorStack.Pop();
			if (popStr == "(" || popStr == "（")
			{
				error_string += "表达式中有多余的'('符号，表达式计算结果可能已出错。\n";
				is_right = false;
			}
			else
			{
				postfixList.Add(popStr);
				operator_list.Add(true);
			}
		}
		Debug.Log("输入：" + exp_string);
		symbolBuilder.Remove(0, symbolBuilder.Length);
		for (int i = 0; i < postfixList.Count; i++)
		{
			symbolBuilder.Append(postfixList[i]);
		}
		Debug.Log("输出：" + symbolBuilder.ToString());
		if (!is_right)
			Debug.LogWarning("错误信息:" + error_string);
	}

	public bool IsMeetCondition(string ID, IState current_state)
	{
		BasicElement key = new BasicElement(ID, current_state);
		if (_conditionExpDic.ContainsKey(key))
		{
			return _conditionExpDic[key].Compute();
		}
		else
		{
			return true;
		}
	}

	public ConditionExp GetCondition(string ID, IState current_state)
	{
		BasicElement key = new BasicElement(ID, current_state);
		if (_conditionIndexDic.ContainsKey(key))
		{
			return _conditionIndexDic[key];
		}
		else
		{
			return new ConditionExp(new List<BasicElement>());
		}
	}
}
