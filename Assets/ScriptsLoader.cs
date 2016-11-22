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
        var opNode = rootNode.GetChildAt(0);
        return new ScriptTable(opNode);
    }

    public static List<ScriptTable> LoadScripts(string path)
    {
        TextReader reader = new StreamReader(path);
        DefParser parser = new DefParser(reader);
        var rootNode = parser.Parse();
        reader.Close();
        var childrenCount = rootNode.Count;
        List<ScriptTable> tables = new List<ScriptTable>();
        for (int i = 0; i < childrenCount; i++)
        {
            var opNode = rootNode.GetChildAt(i);
            var table = new ScriptTable(opNode);
            tables.Add(table);
        }
        return tables;
    }
}


public class ScriptTable : ScriptOperator
{
    Dictionary<string, ScriptOperator> content = new Dictionary<string, ScriptOperator>();
    public ScriptTable(Node node)
    {
        Operation = ScriptOperation.Equals;
        var nameNode = node.GetChildAt(0);
        Name = nameNode.ToString();
        var contextNode = node.GetChildAt(2);
        var list = contextNode.GetChildAt(1);
        var childrenCount = list.Count;

        for (int i = 0; i < childrenCount; i++)
        {
            var opNode = list.GetChildAt(i);
            var opr = LoadFromNode(opNode);
            content.Add(opr.Name, opr);
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Name).Append(Operation).Append("{");
        foreach (var op in content)
            builder.Append(op.ToString());
        builder.Append("}");
        return builder.ToString();
    }

    public T Get<T>(string name) where T : ScriptOperator
    {
        return content[name] as T;
    }
}

public enum ScriptOperation {  Equals, More, Less }
public class ScriptOperator
{

    public string Name { get; internal set; }
    public ScriptOperation Operation { get; internal set; }

    public static ScriptOperator LoadFromNode(Node node)
    {
        ScriptOperator op = null;
        var operationChild = node.GetChildAt(1);
        var contextChild = node.GetChildAt(2);
        if(contextChild.GetChildAt(0).Id == (int)DefConstants.OPEN_TABLE)
        {
            //it's a table or a list
            if(contextChild.GetChildAt(0).GetChildAt(0).Id == (int)DefConstants.ATOM)
            {
                //it's a list
                op = new ScriptList(node);
            }
            else
            {
                //it's a table
                op = new ScriptTable(node);
            }
        }
        else
        {
            op = new ScriptValue(node);
        }

        return op;
       
    }
}

public class ScriptList : ScriptOperator
{
    List<int> data = new List<int>();
    
    public List<int> List {  get { return data; } }
    public ScriptList(Node node)
    {
        Operation = ScriptOperation.Equals;
        var nameNode = node.GetChildAt(0);
        Name = nameNode.ToString();
        var contextNode = node.GetChildAt(2);
        var list = contextNode.GetChildAt(1);
        var childrenCount = list.Count;

        for (int i = 0; i < childrenCount; i++)
        {
            var opNode = list.GetChildAt(i);
            var valueNode = opNode.GetChildAt(0).GetChildAt(0).GetChildAt(0).GetValue(0).ToString();
            data.Add(int.Parse(valueNode));
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Name).Append(Operation).Append("{");
        foreach (var op in data)
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

    public ScriptValue(Node node)
    {
        var nameNode = node.GetChildAt(0);
        Name = nameNode.ToString();
        var opId = node.GetChildAt(1).Id;
        Operation = opId == (int)DefConstants.EQUALS ? ScriptOperation.Equals : opId == (int)DefConstants.MORE ? ScriptOperation.More : ScriptOperation.Less;
        var valueNode = node.GetChildAt(2).GetChildAt(0).GetChildAt(0);
        if(valueNode.Id == (int)DefConstants.DECIMAL)
        {
            if(valueNode.Count == 1)
            {
                //int
                numberValue = int.Parse(valueNode.GetChildAt(0).GetValue(0).ToString());
            }
            else
            {
                numberValue = float.Parse(valueNode.GetChildAt(0).GetValue(0).ToString());
                var fraction = valueNode.GetChildAt(2).GetValue(0).ToString();
                numberValue += float.Parse(fraction) / UnityEngine.Mathf.Pow(10, fraction.Length);
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
            stringValue = valueNode.GetValue(0).ToString();
        }
        else if (valueNode.Id == (int)DefConstants.IDENTIFIER)
        {
            stringValue = valueNode.GetValue(0).ToString();
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Name).Append(Operation).Append(stringValue == null ? numberValue.ToString() : stringValue);
        return builder.ToString();
    }
}