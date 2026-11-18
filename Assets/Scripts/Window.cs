using UnityEngine;

public class Window : MonoBehaviour
{
    public GameObject backgroundPrefab;
    private GameObject background;

    public void Restart()
    {
        Destroy(background);
        background = Instantiate(backgroundPrefab, transform);

        Debug.Log("Restarted");
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        background = Instantiate(backgroundPrefab, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
