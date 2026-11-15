using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject tilePrefab;
    public Tile tile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject tileObj = Instantiate(tilePrefab, transform);
        tileObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, 50);
        tile = tileObj.GetComponent<Tile>();
        tile.SetText("Hello, World!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
