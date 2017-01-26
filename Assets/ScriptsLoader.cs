using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InternalDSL;
using System.IO;
using PerCederberg.Grammatica.Runtime;
class ScriptsLoader
{
    public static ScriptTable LoadScript(string path)
    {
        TextReader reader = new StreamReader(path);
        DefParser parser = new DefParser(reader);
        var rootNode = parser.Parse();
        reader.Close();
        var list = rootNode.GetChildAt(0);
        var idNode = list.GetChildAt(0);
        var opNode = list.GetChildAt(1);
        var ctxNode = list.GetChildAt(2);
        var table = ScriptOperator.LoadFromNode(idNode, opNode, ctxNode) as ScriptTable;
        return table;
    }

    public static List<ScriptTable> LoadScripts(string path)
    {
        TextReader reader = new StreamReader(path);
        DefParser parser = new DefParser(reader);
        var rootNode = parser.Parse();
        reader.Close();
        var list = rootNode.GetChildAt(0);
        var childrenCount = list.Count;

        List<ScriptTable> tables = new List<ScriptTable>();
        for (int i = 0; i < childrenCount / 3; i++)
        {
            var idNode = list.GetChildAt(i * 3);
            var opNode = list.GetChildAt(i * 3 + 1);
            var ctxNode = list.GetChildAt(i * 3 + 2);
            var table = ScriptOperator.LoadFromNode(idNode, opNode, ctxNode) as ScriptTable;
            tables.Add(table);
        }
        return tables;
    }
}


public class ScriptTable : ScriptOperator
{
    Dictionary<string, ScriptOperator> uniqueData = new Dictionary<string, ScriptOperator>();
    public List<KeyValuePair<ScriptValue, ScriptOperator>> AllData = new List<KeyValuePair<ScriptValue, ScriptOperator>>();
    public ScriptTable(Node contextNode)
    {
        Operation = ScriptOperation.Equals;
        
        var list = contextNode.GetChildAt(1);
        if (list.Id != (int)DefConstants.LIST)
            return;

        if (list.Count < 3 || list.GetChildAt(1).Id != (int)DefConstants.OP)
            return;
        var childrenCount = list.Count;
        
        for (int i = 0; i < childrenCount / 3; i++)
        {
            var idNode = list.GetChildAt(i * 3);
            var opNode = list.GetChildAt(i * 3 + 1);
            var ctxNode = list.GetChildAt(i * 3 + 2);
            var opr = LoadFromNode(idNode, opNode, ctxNode);
            if (opr.Name.StringValue() != null)
            {
                if(!uniqueData.ContainsKey(opr.Name.StringValue()))
                    uniqueData.Add(opr.Name.StringValue(), opr);
            }
            AllData.Add(new KeyValuePair<ScriptValue, ScriptOperator>(opr.Name, opr));
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Name).Append(Operation == ScriptOperation.Equals ? " = " : Operation == ScriptOperation.Less ? " < " : " > ").Append("{");
        foreach (var op in uniqueData)
            builder.Append(op.Value).AppendLine();
        builder.Append("}");
        return builder.ToString();
    }

    public T Get<T>(string name) where T : ScriptOperator
    {
        return uniqueData[name] as T;
    }

	public string String(string name)
	{
		return (uniqueData[name] as ScriptValue).StringValue();
	}

	public bool Bool(string name)
	{
		return (uniqueData[name] as ScriptValue).BoolValue();
	}
	public int Value(string name)
	{
		return (uniqueData[name] as ScriptValue).IntValue();
	}

	public List<ScriptTable> AllThat(string id)
	{
		List<ScriptTable> tables = new List<ScriptTable> ();
		foreach (var data in AllData) {
			if (data.Value is ScriptTable) {
				if(data.Key.StringValue() == id)
				tables.Add (data.Value as ScriptTable);

			}
		}
		return tables;
	}

	public ScriptList List(string name)
	{
		return (uniqueData [name] as ScriptList);
	}
}



public enum ScriptOperation {  Equals, More, Less, None }
public class ScriptOperator
{

    public ScriptValue Name { get; internal set; }
    public ScriptOperation Operation { get; internal set; }

    public static ScriptOperator LoadFromNode(Node idNode, Node opNode, Node ctxNode)
    {
        ScriptOperator op = null;
        
        if(ctxNode.GetChildAt(0).Id == (int)DefConstants.OPEN_TABLE)
        {
           
            var listNode = ctxNode.GetChildAt(1);

            op = new ScriptTable(ctxNode);
            
            //it can be empty, and I assumme that it's a table
            if (listNode.Id != (int)DefConstants.LIST)
                op = new ScriptList(ctxNode);
            else //it's a table or a list
            {
                if (listNode.Count < 3 || listNode.GetChildAt(1).Id != (int)DefConstants.OP)
                {

                    //it's a list
                    op = new ScriptList(ctxNode);
                }
                else
                {

                    //it's a table
                    op = new ScriptTable(ctxNode);
                }
            }
            
        }
        else
        {
            op = new ScriptValue(ctxNode);
        }
        var opId = opNode.GetChildAt(0).Id;
        op.Operation = opId == (int)DefConstants.EQUALS ? ScriptOperation.Equals : opId == (int)DefConstants.MORE ? ScriptOperation.More : ScriptOperation.Less;
        op.Name = new ScriptValue(idNode, false);
        op.Name.Operation = ScriptOperation.None;
        return op;
       
    }
}

public class ScriptList : ScriptTable
{
    public ScriptList(Node node) : base(node)
    {
        Operation = ScriptOperation.Equals;
        var list = node.GetChildAt(1);
        if (list.Id != (int)DefConstants.LIST)
            return;
        var childrenCount = list.Count;
        if(AllData.Count == 0)
        for (int i = 0; i < childrenCount; i++)
        {

            var opNode = list.GetChildAt(i);

            var valueNode = new ScriptValue(opNode, false);
            
            AllData.Add(new KeyValuePair<ScriptValue, ScriptOperator>(valueNode, valueNode));
        }
        
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Name).Append(Operation == ScriptOperation.Equals ? " = " : Operation == ScriptOperation.Less ? " < " : " > ").Append("{");
        foreach (var op in AllData)
            builder.Append(op.ToString());
        builder.Append("}");
        return builder.ToString();
    }


}

public class ScriptValue : ScriptOperator
{
    float numberValue;
    string stringValue;
    
    public int IntValue()
    {
        return (int)numberValue;
    }

    public float FloatValue()
    {
        return numberValue;
    }

    public string StringValue()
    {
        return stringValue;
    }

    public bool BoolValue()
    {
        return numberValue > 0;
    }

    public ScriptValue(Node node, bool childValue = true)
    {
        if (childValue)
            node = node.GetChildAt(0);
        if (node.Id == (int)DefConstants.IDENTIFIER)
        {
            stringValue = (node as Token).Image;
        }
        else
        {
            var valueNode = node.GetChildAt(0);
            if (valueNode.Id == (int)DefConstants.DECIMAL)
            {
                if (valueNode.GetChildAt(0).Id != (int)DefConstants.NEGATIVE)
                {

                    if (valueNode.Count == 1)
                        numberValue = int.Parse((valueNode.GetChildAt(0) as Token).Image);
                    else
                    {
                        numberValue = float.Parse((valueNode.GetChildAt(0) as Token).Image);
                        var fraction = (valueNode.GetChildAt(2) as Token).Image;
                        numberValue += float.Parse(fraction) / UnityEngine.Mathf.Pow(10, fraction.Length);
                    }
                }
                else
                {
                    if (valueNode.Count == 2)
                        numberValue = -int.Parse((valueNode.GetChildAt(1) as Token).Image);
                    else
                    {
                        numberValue = -float.Parse((valueNode.GetChildAt(1) as Token).Image);
                        var fraction = (valueNode.GetChildAt(3) as Token).Image;
                        numberValue -= float.Parse(fraction) / UnityEngine.Mathf.Pow(10, fraction.Length);
                    }
                
                }
            }
            else if (valueNode.Id == (int)DefConstants.TRUE)
            {
                numberValue = 1;
            }
            else if (valueNode.Id == (int)DefConstants.FALSE)
            {
                numberValue = 0;
            }
            else if (valueNode.Id == (int)DefConstants.STRING)
            {
                stringValue = (valueNode as Token).Image;
            } else if (valueNode.Id == (int)DefConstants.DATE)
            {
                stringValue = (valueNode.GetChildAt(0) as Token).Image + "." + (valueNode.GetChildAt(2) as Token).Image + "." + (valueNode.GetChildAt(4) as Token).Image;
            }
        }
        
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        if (Operation != ScriptOperation.None)
            builder.Append(Name).Append(Operation == ScriptOperation.Equals ? " = " : Operation == ScriptOperation.Less ? " < " : " > ").Append(stringValue == null ? numberValue.ToString() : stringValue);
        else
            builder.Append(stringValue);
        return builder.ToString();
    }
}