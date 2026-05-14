using System.Collections;
using UnityEngine;

public class Log : MonoBehaviour
{
    private uint qsize = 15;
    private Queue myLogQueue = new Queue();
    private GUIStyle style = new GUIStyle();
    private bool fadeOut = false;

    private IEnumerator autoHide;

    private void Awake()
    {
        style.normal.textColor = Color.white;
        autoHide = AutoHide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            float alpha = style.normal.textColor.a == 1 ? 0 : 1;
            
            if (alpha == 1)
            {
                StopCoroutine(autoHide);
                fadeOut = false;
            }
            
            ChangeTextAplha(alpha);
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);

        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);

        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();

        Color color = style.normal.textColor;
        color.a = 1f;
        style.normal.textColor = color;

        if (!fadeOut) StartCoroutine(autoHide);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()), style);
        GUILayout.EndArea();
    }

    private void ChangeTextAplha(float alpha)
    {
        Color color = style.normal.textColor;
        color.a = alpha;
        style.normal.textColor = color;
    }

    IEnumerator AutoHide()
    {
        fadeOut = true;

        yield return new WaitForSeconds(3);

        while (style.normal.textColor.a > 0)
        {
            float alpha = style.normal.textColor.a - 0.001f;
            ChangeTextAplha(alpha);
            yield return null;
        }

        fadeOut = false;
    }
}
