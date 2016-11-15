using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProjectsView : MonoBehaviour {

    public ProjectsManager Manager;
    public GameObject ProjectPanel;
    public Transform ContentPanel;

    void Start()
    {
        Manager.LoadProjects();
        ListProjects(Manager.Projects);
    }
	public void ListProjects(IList<Project> projects)
    {
        foreach (var proj in projects)
        {
            ProjectPanel panel = GameObject.Instantiate(ProjectPanel).GetComponent<ProjectPanel>();
            panel.OnDelete += OnProjectDelete;
            panel.Set(proj);
            panel.transform.SetParent(ContentPanel);
            panel.GetComponent<Button>().onClick.AddListener(() => Manager.LoadProject(proj));
        }
    }

    private void OnProjectDelete(Project obj)
    {
        Manager.Projects.Remove(obj);
        Destroy(obj);
    }

    public RectTransform ImportPanel;
    public RectTransform BlockPanel;
    public void OnImportNew()
    {
        BlockPanel.gameObject.SetActive(true);
        ImportPanel.gameObject.SetActive(true);
        var panel = ImportPanel.GetComponent<ImportPanel>();
        panel.CancelClick -= OnCancelImport;
        panel.CancelClick += OnCancelImport;
        panel.ImportClick -= OnSubmitImport;
        panel.ImportClick += OnSubmitImport;
    }

    void OnSubmitImport()
    {
        var p = ImportPanel.GetComponent<ImportPanel>();
        BlockPanel.gameObject.SetActive(false);
        ImportPanel.gameObject.SetActive(false);
        Manager.ImportProject(p.Name, p.Dir);
    }

    void OnCancelImport()
    {
        BlockPanel.gameObject.SetActive(false);
        ImportPanel.gameObject.SetActive(false);

    }

    public void OnExit()
    {
        Application.Quit();
    }
    
}
