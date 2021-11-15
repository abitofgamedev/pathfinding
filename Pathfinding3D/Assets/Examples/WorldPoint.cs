using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Examples
{
    public class WorldPoint : MonoBehaviour
    {
        public Vector2Int Coords;
        public bool Invalid;
        public List<Vector2Int> Neighbours;

        public Color BaseColor;

        [SerializeField] Renderer renderer;

        public void SetColor(Color color)
        {
            renderer.material.SetColor("_BaseColor", color);
        }

        public void ResetColor()
        {
            renderer.material.SetColor("_BaseColor", BaseColor);
        }
    }
}
