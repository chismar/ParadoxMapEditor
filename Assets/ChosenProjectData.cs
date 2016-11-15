using UnityEngine;
using System.Collections;

public class ChosenProjectData : MonoBehaviour {

    public Project ChosenProject;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
