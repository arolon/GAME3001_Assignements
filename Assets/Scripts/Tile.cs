using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public int x, y;  // Grid coordinates
    public int cost = 1;  // Default movement cost
    public bool isObstacle = false;
    public TMP_Text costText;
    private Color originalColor;

    void Start()
    {
        originalColor = isObstacle ? Color.black : Color.white;
        GetComponent<SpriteRenderer>().color = originalColor;
    }
    public void SetCost(int cost)
    {
        this.cost = cost;
        costText.text = cost.ToString();
    }

    public void SetTileData(int _x, int _y, bool _isObstacle)
    {
        x = _x;
        y = _y;
        isObstacle = _isObstacle;
        originalColor = isObstacle ? Color.black : Color.white;
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = originalColor;
    }

    public void ToggleDebug(bool isActive)
    {
        costText.enabled = isActive;
    }
}
