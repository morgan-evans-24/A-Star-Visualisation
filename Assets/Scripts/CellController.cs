using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CellController : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public enum CellState
    {
        blank,
        start,
        end,
        wall,
        solution

    }

    public GameObject fill;

    private CellState myState = CellState.blank;

    public static float cellCount = 0;

    private void setState(CellState newState)
    {
        myState = newState;
    }

    public CellState getState()
    {
        return myState;
    }

    public void setColor(Color newColor)
    {
        fill.GetComponent<SpriteRenderer>().color = newColor;
    }

    public void setWall()
    {
        AStarAlgo.boardChanged = true;
        setState(CellState.wall);
        setColor(Color.whiteSmoke);

    }

    public void setBlank()
    {
        setState(CellState.blank);
        ColorUtility.TryParseHtmlString("#4D4D4D", out Color baseColor);
        setColor(baseColor);
    }
    public void setStart()
    {
        setState(CellState.start);
        setColor(Color.orange);
    }
    public void setEnd()
    {
        setState(CellState.end);
        setColor(Color.purple);
    }

    public void setSolution(float fValue, float bestFValue)
    {
        float t = (float)(fValue - bestFValue) / ((cellCount / 2) - bestFValue);
        t = t % 1;
        setState(CellState.solution);
        setColor(Color.HSVToRGB(t, 1f, 1f));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (myState != CellState.start && myState != CellState.end)
            {
                setWall();
            }
        }
        if (Mouse.current.rightButton.isPressed)
        {
            if (myState == CellState.wall)
            {
                setBlank();
                AStarAlgo.boardChanged = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (myState != CellState.start && myState != CellState.end)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                setWall();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                setBlank();
                AStarAlgo.boardChanged = true;
            }
        }
    }
}
