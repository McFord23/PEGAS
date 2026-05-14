using UnityEngine;
using UnityEngine.UI;

public class Сredits : MonoBehaviour
{
    public float speed = 0.05f;
    public GameObject skip;

    void Start()
    {
        skip.SetActive(false);
    }

    void Update()
    {
        if (transform.position.y < 30) transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
        else Exit();

        if (skip.activeSelf)
        {
            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
            {
                Exit();
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
        {
            skip.SetActive(true);
            skip.GetComponent<Button>().Select();
        }
    }

    public void Exit()
    {
        SceneManagerAdapter.Instance.LoadScene("Main Menu");
    }
}
