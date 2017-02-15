using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class State
{
	public GameObject IconGO;
    public List<Province> Provinces = new List<Province>();
    public int ID;
    public int Manpower;
    public string Name;
    public string StateCategory;
	public string Category;
	public Country Owner;
    SupplyArea area;
    public SupplyArea Supply {  get { return area; } set { if (area == value) return; if (area != null) area.States.Remove(this); area = value; area.States.Add(this); } }
    public void Format(StringBuilder builder)
    {
        builder.Append("state={");
        builder.Append("\t").AppendFormat("id={0}", ID).AppendLine();
        builder.Append("\t").AppendFormat("name={0}", Name).AppendLine();
        builder.Append("\t").AppendFormat("manpower={0}", Manpower).AppendLine();
        builder.Append("\t").AppendFormat("state_category={0}", StateCategory).AppendLine();

        builder.Append("\t").Append("resources={").AppendLine();
        builder.Append("}").AppendLine();
        builder.Append("\t").Append("history={").AppendLine();
		if (Owner != null)
			builder.Append ("\t").Append ("\t").Append ("owner=").Append (Owner);
		builder.Append("}").AppendLine();
        builder.Append("\t").Append("provinces={").AppendLine();
        builder.Append('\t', 2);
        foreach (var province in Provinces)
            if (province.Tiles.Count > 0)
                builder.Append(province.ID).Append(" ");
        builder.AppendLine();
        builder.Append("\t").Append("}").AppendLine();
        builder.Append("}");
    }


    public void TextureColor(ref Color32 color)
    {

        color.b = (byte)(ID & 255);
        color.a = (byte)(150 + ((ID >> 8) & 255));
    }
}