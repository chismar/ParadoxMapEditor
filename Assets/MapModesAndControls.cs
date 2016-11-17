using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapModesAndControls : MonoBehaviour {

    public GameObject ButtonPrefab;
    Dictionary<System.Action, GameObject> buttonsPerCallback = new Dictionary<System.Action, GameObject>();
    public void RegisterCallback(System.Action callback, string name)
    {
        var button = GameObject.Instantiate(ButtonPrefab).GetComponent<Button>();
        button.GetComponentInChildren<Text>().text = name;
        button.transform.SetParent(transform);
        button.onClick.AddListener(() => callback());
        buttonsPerCallback.Add(callback, button.gameObject);
    }

    public void UnregisterCallback(System.Action callback)
    {
        var go = buttonsPerCallback[callback];
        go.GetComponent<Button>().onClick.RemoveAllListeners();
        Destroy(go);
        buttonsPerCallback.Remove(callback);
    }
}
