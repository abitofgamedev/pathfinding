using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : MonoBehaviour
{
    public Point Point;
    public Renderer Renderer;
    
    public void SetColor(Color color)
    {
        Renderer.material.SetColor("_BaseColor", color);
    }
}
