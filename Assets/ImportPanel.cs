using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ImportPanel : MonoBehaviour {

	public event System.Action ImportClick;
    public event System.Action CancelClick;
    
    public void Cancel()
    {
        if (CancelClick != null)
            CancelClick();
    }

    public string Name;
    public string Dir;
    public void Import()
    {
        Name = NameText.text;
        Dir = DirText.text;
        if (ImportClick != null)
            ImportClick();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.KeypadEnter))
            Import();
    }

    [SerializeField]
    InputField NameText;
    [SerializeField]
    InputField DirText;

    void OnDisable()
    {
        NameText.text = "";
        DirText.text = "";
    }
}
