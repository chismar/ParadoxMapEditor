using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

public class World : ScriptableObject
{
    public static int Year = 2281;
	public Map Map;
	List<string> cachedOwnersList = new List<string>();
	public List<string> CountriesTags { get { var list = cachedOwnersList; list.Clear (); foreach (var tag in CountriesByTag)
				list.Add (tag.Key); return list; } }
	public Dictionary <string, Country> CountriesByTag = new Dictionary<string, Country>();
	public Dictionary<string, Ideology> Ideologies = new Dictionary<string, Ideology>();
	public SubIdeology LeaderIdeology(string name)
	{
		foreach (var ideology in Ideologies)
			foreach (var sub in ideology.Value.Subs)
				if (sub.ID == name)
					return sub;
		return null;
	}

    void LoadIdeologies()
    {
        var iScript = ScriptsLoader.LoadScript(IdeologiesFile(), true);
        foreach(var ideologyOp in iScript.AllData)
        {
            var ideology = new Ideology();
            ideology.ID = ideologyOp.Key.StringValue();
            Debug.Log("Ideology: " + ideology.ID);
            var subs = ideologyOp.Value as ScriptTable;
            foreach(var sub in subs.Table("types").AllData)
            {
                var subIdeology = new SubIdeology();
                subIdeology.ID = sub.Key.StringValue();
                subIdeology.Parent = ideology;
                ideology.Subs.Add(subIdeology);
            }
            Ideologies.Add(ideology.ID, ideology);
        }
        
    }
	public void LoadFrom(string directory)
	{
		dir = directory;
        LoadIdeologies();
		var histFiles = Directory.GetFiles (CountriesHistoryDir ());
		var tagsLines = File.ReadAllLines (CountryTags ());
        Dictionary<string, string> tagToGFXPath= new Dictionary<string, string>();
        foreach (var tagLine in tagsLines)
        {
            var splitted = tagLine.Split('=');
            var trimmedTag = splitted[0].Trim();
            if (!Regex.IsMatch(trimmedTag, "[A-Z][A-Z][A-Z]"))
                continue;
            var trimmedFile = splitted[1].Trim(' ', '\"');
            tagToGFXPath.Add(trimmedTag, trimmedFile);
        }
        foreach(var histFile in histFiles)
        {
            var name = Path.GetFileName(histFile);
            var tag = name.Substring(0, 3);
            var other = name.Split('-');
            var locName = other[1].Trim();
            locName = locName.Substring(0, locName.Length - 4);
            var country = Create(tag);
            country.Name = locName;

            var gfxPath =  tagToGFXPath.ContainsKey(tag)? Common() + tagToGFXPath[tag]:null;
            //Debug.Log(gfxPath);
            country.Load(this, histFile, gfxPath, null);
        }

	}
	string dir;
	public void SaveTo(string directory)
	{
        //TODO: implement saving actually
        var file = new StreamWriter(CountryTags());
        var hDir = CountriesHistoryDir();
        var gfxDir = CountriesColorsDir();
        foreach(var country in CountriesByTag.Values)
        {
            country.SaveHistory(hDir);
            //country.SaveColors(dir);
            country.SaveGFX(gfxDir);
            file.WriteLine(String.Format("{0} = \"countries/{0} - {1}.txt\"", country.Tag, country.Name));
        }
        file.Close();
	}

	public string CountriesColorsDir()
	{
		return dir + "/common/countries/";
    }
    public string Common()
    {
        return dir + "/common/";
    }
    public string CountryTags()
	{
		return dir + "/common/country_tags/00_countries.txt";
	}

	public string CountriesHistoryDir()
	{
		return dir + "/history/countries";
	}

    public string IdeologiesFile()
    {
        return dir + "/common/ideologies/00_ideologies.txt";
    }
	public Country Create(string tag)
	{
        Debug.Log(tag);
        if (CountriesByTag.ContainsKey(tag))
            return CountriesByTag[tag];
		Country c = new Country ();
		c.Tag = tag;
		CountriesByTag.Add (tag, c);
		return c;
	}

	public Country Create(string tag, Country originalCountry)
	{
		var c = originalCountry.Clone (Create (tag));
		return c;
	}


}

public class Country
{
	public string Tag;
	public List<Party> Parties = new List<Party>();
	public List<CountryTech> Technologies = new List<CountryTech>();
	public List<string> Ideas = new List<string>();
	public State Capital;
	public Color32 Color;
	public Color32 ColorUI;
	public Color32 CultureColor;
	public string CultureGFX;
	public string CultureGFX2D;
	public Party RulingParty;
	public bool ElectionsAllowed;
	public string OOB { get { return "\"" + Tag + "_" + World.Year + "\""; } }
    public string Name = "NoName";
    public string CreateFaction;
    public string AddToFaction;
    List<DiploRelation> relations = new List<DiploRelation>();
    List<Commander> Commanders = new List<Commander>();
	List<Leader> Leaders = new List<Leader>();
	public void Load(World world, string countryHist, string countryGFX, ScriptTable countryColor)
	{
		var historyTable = ScriptsLoader.LoadScriptNoRoot(countryHist);
        //Debug.Log(historyTable);
        
		var gfxTable =countryGFX != null? ScriptsLoader.LoadScriptNoRoot (countryGFX):null;
        var cap = historyTable.Get<ScriptValue>("capital");
        if(cap != null)
        {
            var capitalID = cap.IntValue();
            Capital = world.Map.States.Find(s => s.ID == capitalID);
        }
		var techs = historyTable.Get<ScriptTable> ("set_technology");
		foreach (var op in techs.AllData) {
			Technologies.Add (new CountryTech (){ Name = op.Key.StringValue (), Value = (op.Value as ScriptValue).IntValue () });
		}

		var pol = historyTable.Get<ScriptTable> ("set_politics");
		var parties = pol.Get<ScriptTable> ("parties");
		var rulingParty = pol.String ("ruling_party");
		ElectionsAllowed = pol.Bool ("elections_allowed");
        foreach (var partyT in parties.AllData) {
            Party party = new Party();
            var partyId = partyT.Key.StringValue();
            if (world.Ideologies.ContainsKey(partyId))
                party.Ideology = world.Ideologies[partyId];
            else
            {

                party.Ideology = new Ideology() { ID = partyId };
                Debug.LogErrorFormat("No such ideology {0} in {1} at {2}", partyId, Tag, countryHist);
            }

			party.Popularity = (partyT.Value as ScriptTable).Value ("popularity");
			Parties.Add (party);
		}

        CreateFaction = historyTable.String("create_faction");
        AddToFaction = historyTable.String("add_to_faction");
        RulingParty = Parties.Find(p => p.Ideology.ID == rulingParty);
		var ideasT = historyTable.Get<ScriptList> ("add_ideas");
		foreach (var i in ideasT.AllData)
			Ideas.Add (i.Key.StringValue ());
		foreach(var leaderT in historyTable.AllThat("create_country_leader"))
		{
			Leader l = new Leader ();
			Leaders.Add (l);
			l.LoadFrom (leaderT, world);
		}

		foreach(var commanderT in historyTable.AllThat("create_corps_commander"))
		{
			var c = new Commander ();
			Commanders.Add (c);
			c.LoadFrom (commanderT);
		}
        foreach (var commanderT in historyTable.AllThat("create_field_marshal "))
        {
            var c = new Commander();
            c.IsMarshal = true;
            Commanders.Add(c);
            c.LoadFrom(commanderT);
        }
        if (gfxTable != null)
        {
            CultureGFX = gfxTable.String("graphical_culture");
            CultureGFX2D = gfxTable.String("graphical_culture_2d");
            CultureColor = gfxTable.Color("color");
        }
        else
        {

            CultureColor = new Color32(255, 255, 255, 255);
            Debug.LogWarningFormat("No culture table for: {0}", Tag);
        }


        if (countryColor != null)
        {
            this.Color = countryColor.Color("color");
            this.ColorUI = countryColor.Color("color_ui");
        }

        var diploTables = historyTable.AllThat("diplomatic_relation ");
        foreach(var dTable in diploTables)
        {
            DiploRelation r = new DiploRelation();
            r.LoadFrom(dTable, world);
            relations.Add(r);

        }
    }

	public void SaveHistory(string dir)
	{
        var file = new StreamWriter(String.Format("{0}/{1} - {2}.txt", dir, Tag, Name));

        if (Capital != null)
            file.WriteLine("capital = " + Capital.ID);
        else
            Debug.LogWarning(Tag + " has no capital");
        file.WriteLine("oob = " + OOB);
        file.WriteLine("set_technology = {");
        foreach (var tech in Technologies)
            file.WriteLine(tech.ToString());
        file.WriteLine("}");

        file.WriteLine("set_politics = {");
        file.WriteLine("parties = {");
        foreach (var party in Parties)
        {
            file.WriteLine(party.ToString());
        }
        file.WriteLine("}");
        file.Write("\t\t");
        file.WriteLine("ruling_party = " + RulingParty.Ideology.ID);
        file.Write("\t\t");
        file.WriteLine("elections_allowed = " + (ElectionsAllowed ? "yes" : "no"));
        file.WriteLine("}");

        file.WriteLine("add_ideas = {");
        foreach (var idea in Ideas)
        {
            file.Write("\t\t");
            file.WriteLine(idea);
        }
        file.WriteLine("}");

        foreach(var leader in Leaders)
        {
            file.WriteLine(leader.ToString());
        }

        foreach(var commander in Commanders)
        {
            file.WriteLine(commander.ToString());
        }
        if(CreateFaction != null)
            file.WriteLine("create_faction = " + CreateFaction);
        if (AddToFaction != null)
            file.WriteLine("add_to_faction = " + AddToFaction);
        foreach(var relation in relations)
        {
            file.WriteLine(relation.ToString());
        }
        file.Close();
	}

	public void SaveGFX(string dir)
    {
        var file = new StreamWriter(String.Format("{0}/{1} - {2}.txt", dir, Tag, Name));
        if(CultureGFX != null)
        file.WriteLine("graphical_culture = " + CultureGFX);
        if(CultureGFX2D != null)
        file.WriteLine("graphical_culture_2d = " + CultureGFX2D);
        file.WriteLine(String.Format("color = {{ {0} {1} {2} }}", CultureColor.r, CultureColor.g, CultureColor.b));
        file.Close();
    }

	public void SaveColors(string dir)
	{
	}

	public Country Clone(Country c)
	{
		return c;
	}
}
public class DiploRelation
{
    public string Relation;
    public Country Country;
    public bool Active;
    public void LoadFrom(ScriptTable table, World world)
    {
        Relation = table.String("relation");
        Country = world.CountriesByTag[table.String("country")];
        Active = table.Bool("active");
    }
    static StringBuilder builder = new StringBuilder();
    public override string ToString()
    {
        builder.Length = 0;
        builder.AppendLine("diplomatic_relation = {");
        builder.Append("country =").AppendLine(Country.Tag);
        builder.AppendLine("relation = ").AppendLine(Relation);
        builder.AppendLine("active = ").AppendLine(Active ? "yes" : "no");
        builder.AppendLine("}");
        return builder.ToString();
    }
}

public class Party
{
	public Ideology Ideology;
	public int Popularity;
    public override string ToString()
    {
        return String.Format("{0} = {{\n popularity = {1} \n}}", Ideology.ID, Popularity);
    }
}
public class Ideology
{
	public string ID;
    public List<SubIdeology> Subs = new List<SubIdeology>();
}

public class SubIdeology
{
	public string ID;
	public Ideology Parent;
}

public class Leader
{
	public string Name;
	public string Desc { get { return Name + "_DESC"; } }
	public string Picture;
	public string Expire;
	public SubIdeology Ideology;
	public List<string> Traits;
	public void LoadFrom(ScriptTable table, World world)
	{
		Name = table.String ("name");
		Picture = table.String ("picture");
		Expire = table.String ("expire");
		Ideology = world.LeaderIdeology (table.String ("ideology"));
		Traits = new List<string> ();
		foreach (var traitId in table.List("traits").AllData)
			Traits.Add (traitId.Key.StringValue());
	}
    static StringBuilder builder = new StringBuilder();
    public override string ToString()
    {
        builder.Length = 0;

        builder.AppendLine("create_country_leader  = {");
        builder.Append("name = \"").Append(Name).AppendLine("\"");
        builder.Append("desc = \"").Append(Desc).AppendLine("\"");
        builder.Append("picture = \"").Append(Picture).AppendLine("\"");
        builder.Append("expire = \"").Append(Expire).AppendLine("\"");

        builder.Append("ideology =").AppendLine(Ideology.ID);

        builder.AppendLine("traits = {");
        foreach (var trait in Traits)
            builder.AppendLine(trait);
        builder.AppendLine("}");
        builder.AppendLine("}");
        return builder.ToString();
    }
}

public class Commander
{
	public string Name;
	public string PortraitPath;
	public List<string> Traits;
	public int Skill;
    public bool IsMarshal = false;
	public void LoadFrom(ScriptTable table)
	{

		Name = table.String ("name");
		PortraitPath = table.String ("portrait_path");
		Skill = table.Value ("skill");
		Traits = new List<string> ();
		foreach (var traitId in table.List("traits").AllData)
			Traits.Add (traitId.Key.StringValue());
	}

    static StringBuilder builder = new StringBuilder();
    public override string ToString()
    {
        builder.Length = 0;
        if(IsMarshal)
            builder.AppendLine("create_fields_marshal = {");
        else
            builder.AppendLine("create_corps_commander = {");
        builder.Append("name = \"").Append(Name).AppendLine("\"");
        builder.Append("portrait_path = \"").Append(PortraitPath).AppendLine("\"");
        builder.Append("skill =").AppendLine(Skill.ToString());

        builder.AppendLine("traits = {");
        foreach (var trait in Traits)
            builder.AppendLine(trait);
        builder.AppendLine("}");
        builder.AppendLine("}");
        return builder.ToString();
    }
}

public class CountryTech
{
	public string Name;
	public int Value;
    public override string ToString()
    {
        return Name + " = " +  Value;
    }
}

public static class ScriptListExt
{
	public static Color32 Color(this ScriptTable t, string id)
	{
		var c = new Color32 ();
		var color = t.List(id);
		c.r = (byte)color.AllData [0].Key.IntValue ();
		c.g = (byte)color.AllData [1].Key.IntValue ();
		c.b = (byte)color.AllData [2].Key.IntValue ();
        c.a = 255;
		return c;

	}
}