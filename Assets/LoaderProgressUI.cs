using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoaderProgressUI : MonoBehaviour
{
    
    public RectTransform ProgressCounter;
    public int MaxProgress;
    public int Progress;
    [SerializeField]
    Text text;
    public string Text {  get { return null; } set { text.text = value; } }
    float percentProgress;
    void Update()
    {
        if(MaxProgress == 0)
        {
            percentProgress = 0f;
        }
        else
        {
            percentProgress = (float)Progress / (float)MaxProgress;
        }
        ProgressCounter.localScale = new Vector3(percentProgress, 1, 1);
    }

}
