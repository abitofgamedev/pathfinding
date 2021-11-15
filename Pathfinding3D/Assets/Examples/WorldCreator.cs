using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [SerializeField] GameObject _GridPointPrefab;
    [SerializeField] int GridWidth;
    [SerializeField] int GridHeight;
    [SerializeField] int GridLength;
    public Point[][][] Grid;
    public float PointDistance;
    public float PointSize;
    public float InvalidPointSize;
    Vector3 startPoint;
    [SerializeField] GameObject VisualPoint;
    [SerializeField] Color _ValidColor;
    [SerializeField] Color _InvalidColor;

    private void Start()
    {
        StartCoroutine(InitializeGrid());
    }

    private void AddNeighbour(Point p, Vector3Int neighbour)
    {
        if (neighbour.x > -1 && neighbour.x < GridWidth &&
           neighbour.y > -1 && neighbour.y < GridHeight &&
           neighbour.z > -1 && neighbour.z < GridLength)
        {
            p.Neighbours.Add(neighbour);
        }
    }

    private IEnumerator InitializeGrid()
    {
        startPoint = new Vector3(-GridWidth, -GridHeight, -GridLength) / 2f * PointDistance + transform.position;
        GameObject gridParent = new GameObject("Grid");
        Grid = new Point[GridWidth][][];
        for (int i = 0; i < GridWidth; i++)
        {
            Grid[i] = new Point[GridHeight][];
            for (int j = 0; j < GridHeight; j++)
            {
                Grid[i][j] = new Point[GridLength];
                for (int k = 0; k < GridLength; k++)
                {
                    Vector3 pos = startPoint + new Vector3(i, j, k) * PointDistance;
                    Grid[i][j][k] = new Point();
                    Grid[i][j][k].Coords = new Vector3Int(i, j, k);
                    Grid[i][j][k].WorldPosition = pos;
                    VisualPoint.transform.localScale = Vector3.one * PointDistance;
                    VisualPoint.transform.position = pos;
                    VisualPoint.GetComponent<Renderer>().material.SetColor("_BaseColor",_ValidColor);
                    if (Physics.CheckBox(Grid[i][j][k].WorldPosition, Vector3.one * PointDistance / 2f, Quaternion.identity))
                    {
                        VisualPoint.GetComponent<Renderer>().material.SetColor("_BaseColor", _InvalidColor);
                        GameObject point= Instantiate(_GridPointPrefab, transform);
                        point.transform.localScale = Vector3.one * PointDistance;
                        point.transform.position = pos;
                        Grid[i][j][k].Invalid = true;
                    }
                    yield return new WaitForSeconds(0.05f);
                    for (int p = -1; p <= 1; p++)
                    {
                        for (int q = -1; q <= 1; q++)
                        {
                            for (int g = -1; g <= 1; g++)
                            {
                                if (i == p && g == q && k == g)
                                {
                                    continue;
                                }
                                AddNeighbour(Grid[i][j][k], new Vector3Int(i + p, j + q, k + g));
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        startPoint = new Vector3(-GridWidth, -GridHeight, -GridLength) / 2f * PointDistance + transform.position;
        Gizmos.color = new Color(1, 1, 1, 0.1f);
        for (int i = 0; i < GridWidth; i++)
        {
            for (int j = 0; j < GridHeight; j++)
            {
                for (int k = 0; k < GridLength; k++)
                {
                    Vector3 pos = startPoint + new Vector3(i, j, k) * PointDistance;
                    Vector3 scale = Vector3.one * PointDistance;

                    Gizmos.DrawWireCube(pos, scale);
                }
            }
        }
    }
}
