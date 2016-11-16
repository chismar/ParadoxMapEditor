using UnityEngine;
using System.Collections;

public class LoaderWatchDog : MonoBehaviour {

    public LoaderProgressUI Loader;
    public GameObject WaitPanel;
    void Update()
    {
        if (Loader.MaxProgress == 0)
            WaitPanel.SetActive(false);
        else
            WaitPanel.SetActive(true);
    }
}
