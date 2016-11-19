using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class ProjectsManager : MonoBehaviour {
    
    public List<Project> Projects = new List<Project>();
	public void ImportProject(string name, string directory)
    {
        Project proj = ScriptableObject.CreateInstance<Project>();
        proj.Name = name;
        proj.Directory = directory;
        proj.Date = System.DateTime.Now.ToShortDateString();
        Projects.Add(proj);
        LoadProject(proj);
    }
    string myDocumentsPath = null;
    string appDir = null;
    string filePath = null;
    public void LoadProjects()
    {
        myDocumentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        appDir = myDocumentsPath + "/ParadoxMapEditor";
        if (!Directory.Exists(appDir))
            Directory.CreateDirectory(appDir);
        filePath = appDir + "/OpenedProjects.csv";
        if (!File.Exists(filePath))
            File.Create(filePath).Close();
        var lines = File.ReadAllLines(filePath);
        for (int i = 0; i< lines.Length; i++)
        {
            var projectData = lines[i].Split(',');
            Project proj = ScriptableObject.CreateInstance<Project>();
            proj.Name = projectData[0];
            proj.Directory = projectData[1];
            proj.Date = projectData[2];
            Projects.Add(proj);
        }
    }

    void OnDestroy()
    {
        SaveProjects();
    }
    void SaveProjects()
    {
        StringBuilder builder = new StringBuilder();
        foreach (var proj in Projects)
            builder.Append(proj.Name).Append(',').Append(proj.Directory).Append(',').Append(proj.Date).AppendLine();
        File.WriteAllText(filePath, builder.ToString());
    }
    public void LoadProject(Project proj)
    {
        PlayerPrefs.SetString("directory", proj.Directory);
        SceneManager.LoadScene(1);
    }
}
