using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ProjectPanel : MonoBehaviour {
    [SerializeField]
    Text NameText;
    [SerializeField]
    Text DirText;
    [SerializeField]
    Text DateText;
    public event System.Action<Project> OnDelete;
    Project proj;
    public void Set(Project proj)
    {
        this.proj = proj;
        NameText.text = proj.Name;
        DirText.text = proj.Directory;
        DateText.text = proj.Date;
    }

    public void DeleteObject()
    {
        if (OnDelete != null)
            OnDelete(proj);
        Destroy(gameObject);
    }
}
