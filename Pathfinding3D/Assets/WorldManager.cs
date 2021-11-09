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
                    GameObject point = Instantiate(_GridPointPrefab, pos, Quaternion.identity);
                    point.transform.parent = gridParent.transform;
                    Grid[i][j][k] = point.GetComponent<GridPoint>().Point;
                    Grid[i][j][k].Coords = new Vector3Int(i, j,k);
                    Grid[i][j][k].Transform = point.transform;
                    Grid[i][j][k].Transform.localScale = Vector3.one * PointSize;
                    if(Physics.CheckBox(Grid[i][j][k].Transform.position, Vector3.one * PointDistance/2f, Quaternion.identity))
                    {
                        Grid[i][j][k].Invalid = true;
                        Grid[i][j][k].Transform.localScale = Vector3.one * PointDistance* InvalidPointSize;
                    }
                    for (int p = -1; p <= 1; p++)
                    {
                        for (int q = -1; q <= 1; q++)
                        {
                            for (int g = -1; g <= 1; g++)
                            {
                                if (i == p && q == j && g == p)
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

    public int ID = 0;
    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            for (int i = 0; i <GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    for (int k = 0; k < GridLength; k++)
                    {
                        Grid[i][j][k].Transform.localScale = Vector3.one * 0.5f;
                        Grid[i][j][k].Invalid = false;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ID = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ID = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ID = 3;
        }
    }

    public Point GetClosestPointWorldSpace(Vector3 position)
    {
        float sizeX = PointDistance * GridWidth;
        float sizeY = PointDistance * GridHeight;
        float sizeZ = PointDistance * GridLength;
        Vector3 pos = position - startPoint;
        float percentageX = pos.x / sizeX;
        float percentageY = pos.y / sizeY;
        float percentageZ = pos.z / sizeZ;
        int x = Mathf.RoundToInt(percentageX * GridWidth);
        int y = Mathf.RoundToInt(percentageY * GridHeight);
        int z = Mathf.RoundToInt(percentageZ * GridLength);
        return Grid[x][y][z];
    }
}
