using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TypeWritter : MonoBehaviour
{
    public float WriteDelay = 0.125f;

    private Text txt;
    private string story;
    private int typeIndex = 0;
    private float writerTimer;

    void Awake()
    {
        txt = GetComponent<Text>();
    }

    void Start()
    {
        story = txt.text;
        txt.text = "";

        typeIndex = 0;
        writerTimer = 0f;
    }

    void Update()
    {
        if (typeIndex < story.Length)
        {
            writerTimer += Time.deltaTime;

            if (writerTimer >= WriteDelay)
            {
                txt.text += story[typeIndex];
                typeIndex++;

                writerTimer = 0f;
            }
        }

        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
