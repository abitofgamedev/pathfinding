using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    [SerializeField] GameObject _GridPointPrefab;
    [SerializeField] int GridWidth;
    [SerializeField] int GridHeight;
    [SerializeField] int GridLength;
    public Point[][][] Grid;
    public float PointDistance;
    public float PointSize;
    public float InvalidPointSize;

    Vector3 startPoint;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeGrid();
    }

    private void AddNeighbour(Point p, Vector3Int neighbour) 
    { 
        if(neighbour.x>-1 && neighbour.x< GridWidth &&
           neighbour.y > -1 && neighbour.y < GridHeight &&
           neighbour.z > -1 && neighbour.z < GridLength)
        {
            p.Neighbours.Add(neighbour);
        }
    }

    private void InitializeGrid()
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
                    Grid[i][j][k].Coords = new Vector3Int(i, j,k);
                    Grid[i][j][k].WorldPosition = pos;
                    if(Physics.CheckBox(Grid[i][j][k].WorldPosition, Vector3.one * PointDistance/2f, Quaternion.identity))
                    {
                        Grid[i][j][k].Invalid = true;
                    }
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
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWidth, GridHeight, GridLength)*PointDistance);
    }

    public Point GetClosestPointWorldSpace(Vector3 position)
    {
        float sizeX = PointDistance * GridWidth;
        float sizeY = PointDistance * GridHeight;
        float sizeZ = PointDistance * GridLength;

        Vector3 pos = position - startPoint;
        float percentageX = Mathf.Clamp01(pos.x / sizeX);
        float percentageY = Mathf.Clamp01(pos.y / sizeY);
        float percentageZ = Mathf.Clamp01(pos.z / sizeZ);
        int x = Mathf.Clamp(Mathf.RoundToInt(percentageX * GridWidth), 0, GridWidth - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(percentageY * GridHeight), 0, GridHeight - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(percentageZ * GridLength), 0, GridLength - 1);
        Point result= Grid[x][y][z];
        while (result.Invalid)
        {
            int step = 1;
            List<Point> freePoints = new List<Point>();
            for (int p = -step; p <= step; p++)
            {
                for (int q = -step; q <= step; q++)
                {
                    for (int g = -step; g <= step; g++)
                    {
                        if (x == p && y == q && z == g)
                        {
                            continue;
                        }
                        int i = x + p;
                        int j = y + q;
                        int k = z + g;
                        if (i > -1 && i < GridWidth &&
                            j > -1 && j < GridHeight &&
                            k > -1 && k < GridLength)
                        {
                            if (!Grid[x + p][y + q][z + g].Invalid)
                            {
                                freePoints.Add(Grid[x + p][y + q][z + g]);
                            }
                        }
                    }
                }
            }
            float distance = Mathf.Infinity;
            for (int i = 0; i < freePoints.Count; i++)
            {
                float dist = (freePoints[i].WorldPosition - position).sqrMagnitude;
                if (dist < distance)
                {
                    result = freePoints[i];
                    dist = distance;
                }
            }
        }
        return result;
    }

    public List<Point> GetFreePoints()
    {
        List<Point> freePoints = new List<Point>();
        for (int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[i].Length; j++)
            {
                for (int k = 0; k < Grid[i][j].Length; k++)
                {
                    if (!Grid[i][j][k].Invalid)
                    {
                        freePoints.Add(Grid[i][j][k]);
                    }
                }
            }
        }
        return freePoints;
    }
}
