using UnityEngine;
using UnityEngine.UI;

public class Сredits : MonoBehaviour
{
    public float speed = 0.05f;
    public Button skip;

    private void Start()
    {
        skip.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (transform.position.y < 30) transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
        else Exit();

        if (Controls.Apply || Controls.Pause)
        {
            if (skip.gameObject.activeSelf)
            {
                Exit();
            }
            else
            {
                skip.gameObject.SetActive(true);
                skip.Select();
            }
        }
    }

    public void Exit()
    {
        SceneManagerAdapter.Instance.LoadScene("Main Menu");
    }
}
