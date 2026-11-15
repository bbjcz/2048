using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetText(string value)
    {
        text.text = value;
    }   

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
