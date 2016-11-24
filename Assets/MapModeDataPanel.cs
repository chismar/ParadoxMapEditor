using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MapModeDataPanel : MonoBehaviour {

    public GameObject TextPrefab;
    public GameObject InputPrefab;
	public Text PostString(string data)
    {
        var text = GameObject.Instantiate(TextPrefab).GetComponent<Text>();

        text.transform.SetParent(transform);
        text.text = data;
        return text;
    }

    public InputField PostInput(string data)
    {
        var field = GameObject.Instantiate(InputPrefab).GetComponent<InputField>();

        field.transform.SetParent(transform);
        return field;
    }
}
