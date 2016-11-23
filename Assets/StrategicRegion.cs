using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StrategicRegion
{
    public List<Province> Provinces = new List<Province>();
    public int ID;
    public string Name;
    public void Format(StringBuilder builder)
    {
        builder.Append("strategic_region=={");
        builder.Append("\t").AppendFormat("id={0}", ID).AppendLine();
        builder.Append("\t").AppendFormat("name={0}", Name).AppendLine();
        builder.Append("\t").Append("provinces={").AppendLine();
        builder.Append('\t', 2);
        foreach (var province in Provinces)
            builder.Append(province.ID).Append(" ");
        builder.Append("\t").Append("}").AppendLine();
        builder.Append("}");
    }


    public void TextureColor(ref Color32 color)
    {
        color.r = (byte)(ID & 255);
        color.g = (byte)((ID >> 8) & 255);
    }
}
