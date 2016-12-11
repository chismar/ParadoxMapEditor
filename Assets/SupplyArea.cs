using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SupplyArea
{
    public List<State> States = new List<State>();
    public int ID;
    public int SupplyValue;
    public string Name;

    public void Format(StringBuilder builder)
    {
        builder.Append("state={").AppendLine();
        builder.Append("\t").AppendFormat("id={0}", ID).AppendLine();
        builder.Append("\t").AppendFormat("name={0}", Name).AppendLine();
        builder.Append("\t").AppendFormat("value={0}", SupplyValue).AppendLine();
        
        builder.Append("\t").Append("states={").AppendLine();
        builder.Append('\t', 2);
        foreach (var state in States)
            builder.Append(state.ID).Append(" ");
        builder.AppendLine();
        builder.Append("\t").Append("}").AppendLine();
        builder.Append("}");
    }

    public void TextureColor(ref Color32 color)
    {

        color.b = (byte)(ID & 255);
        color.a = (byte)((ID >> 8) & 255);
    }
}