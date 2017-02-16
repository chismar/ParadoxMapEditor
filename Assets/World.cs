using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class World : ScriptableObject
{
	public static int Year;	
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
	public void LoadFrom(string directory)
	{
		dir = directory;
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
            var country = Create(tag);
            var gfxPath = Common() + tagToGFXPath[tag];
            //Debug.Log(gfxPath);
            country.Load(this, histFile, gfxPath, null);
        }

	}
	string dir;
	public void SaveTo(string directory)
	{
		
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

	public Country Create(string tag)
	{
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
	public string OOB { get { return Tag + "_" + World.Year; } }

	List<Commander> Commanders = new List<Commander>();
	List<Leader> Leaders = new List<Leader>();
	public void Load(World world, string countryHist, string countryGFX, ScriptTable countryColor)
	{
		//var historyTable = ScriptsLoader.LoadScript (countryHist);
		var gfxTable = ScriptsLoader.LoadScriptNoRoot (countryGFX);
        /*Capital = world.Map.States.Find (s => s.ID == historyTable.Get<ScriptValue> ("capital").IntValue ());
		var techs = historyTable.Get<ScriptTable> ("set_technology");
		foreach (var op in techs.AllData) {
			Technologies.Add (new CountryTech (){ Name = op.Key.StringValue (), Value = (op.Value as ScriptValue).IntValue () });
		}

		var pol = historyTable.Get<ScriptTable> ("set_politics");
		var parties = pol.Get<ScriptTable> ("parties");
		var rulingParty = pol.String ("ruling_party");
		ElectionsAllowed = pol.Bool ("elections_allowed");

		foreach (var partyT in parties.AllData) {
			Party party = new Party ();
			party.Ideology = world.Ideologies [partyT.Key.StringValue()];
			party.Popularity = (partyT.Value as ScriptTable).Value ("popularity");
			Parties.Add (party);
		}

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

		CultureGFX = gfxTable.String ("graphical_culture");
		CultureGFX2D = gfxTable.String ("graphical_culture_2d");

        if(countryColor != null)
        {

            this.Color = countryColor.Color("color");
            this.ColorUI = countryColor.Color("color_ui");
        }*/
        if (gfxTable != null)
            CultureColor = gfxTable.Color("color");
        else
            CultureColor = new Color32(255, 255, 255, 255);
    }

	public void SaveHistory(string dir)
	{
	}

	public void SaveGFX(string dir)
	{
	}

	public void SaveColors(string dir)
	{
	}

	public Country Clone(Country c)
	{
		return c;
	}
}

public class Party
{
	public Ideology Ideology;
	public int Popularity;
}
public class Ideology
{
	public string ID;
	public List<SubIdeology> Subs;
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
}

public class Commander
{
	public string Name;
	public string PortraitPath;
	public List<string> Traits;
	public int Skill;
	public void LoadFrom(ScriptTable table)
	{

		Name = table.String ("name");
		PortraitPath = table.String ("picture");
		Skill = table.Value ("skill");
		Traits = new List<string> ();
		foreach (var traitId in table.List("traits").AllData)
			Traits.Add (traitId.Key.StringValue());
	}
}

public class CountryTech
{
	public string Name;
	public int Value;
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
		return c;

	}
}