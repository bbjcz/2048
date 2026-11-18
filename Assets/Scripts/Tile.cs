using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    private enum State
    {
        Idle,
        Moving,
        Zooming
    }

    public float zoomDuration;
    public float moveDuration;

    private State state = State.Idle;
    private float startTime;
    private Vector2 oldPos, newPos;

    public TextMeshProUGUI text;

    private Vector2 GetPos(int r, int c)
    {
        return new Vector2(c * 220 - 330, r * -220 + 330);
    }

    private void SetValue(int value)
    {
        text.text = $"{value}";

        if (value < 10)
        {
            text.fontSize = 110;
        }
        else if (value < 100)
        {
            text.fontSize = 90;
        }else if (value >= 10000)
        {
            int len = $"{value}".Length;
            text.fontSize = 320.0f / len;
        }
    }   

    private void SetPosition(Vector2 position)
    {
        GetComponent<RectTransform>().anchoredPosition = position;
    }

    private Vector2 GetPosition()
    {
        return GetComponent<RectTransform>().anchoredPosition;
    }

    public void BornAt(int r, int c, int value)
    {
        SetValue(value);
        SetPosition(GetPos(r, c));
        state = State.Zooming;
        startTime = Time.time;
    }

    public void MoveTo(int r, int c, bool isUpper)
    {
        oldPos = GetPosition();
        newPos = GetPos(r, c);
        state = State.Moving;
        startTime = Time.time;

        if (isUpper)
        {
            transform.SetAsLastSibling();
        }
        else
        {
            transform.SetAsFirstSibling();
        }
    }

    public bool IsIdle()
    {
        return state == State.Idle;
    }

    void Update()
    {
        if (state == State.Zooming)
        {
            // Perform zooming animation
            float elapsed = Time.time - startTime;
            if (elapsed < zoomDuration)
            {
                // Calculate the interpolation factor
                float t = elapsed / zoomDuration;
                // Apply the zooming effect
                transform.localScale = Vector3.Lerp(new Vector3(0, 0, 1), new Vector3(1, 1, 1), t);
            }
            else
            {
                // Finish the zooming animation
                transform.localScale = Vector3.one;
                state = State.Idle;
            }
        }
        else if (state == State.Moving)
        {
            // Perform moving animation
            float elapsed = Time.time - startTime;
            if (elapsed < moveDuration)
            {
                // Calculate the interpolation factor
                float t = elapsed / moveDuration;
                // Apply the moving effect
                SetPosition(Vector2.Lerp(oldPos, newPos, t));
            }
            else
            {
                // Finish the moving animation
                SetPosition(newPos);
                state = State.Idle;
            }
        }
    }
}
