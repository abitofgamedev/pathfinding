using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    [SerializeField] Transform _GridStartPoint;
    [SerializeField] GameObject _GridPointPrefab;
    [SerializeField] Texture2D _GridTexture;
    public Point[][] Grid;
    public float PointDistance;

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

    private void InitializeGrid()
    {
        GameObject gridParent = new GameObject("Grid");
        Grid = new Point[_GridTexture.width][];
        for (int i = 0; i < _GridTexture.width; i++)
        {
            Grid[i] = new Point[_GridTexture.height];
            for (int j = 0; j < _GridTexture.height; j++)
            {
                Vector3 pos = _GridStartPoint.position + new Vector3(i, 0, j) * PointDistance;
                GameObject point = Instantiate(_GridPointPrefab, pos, Quaternion.identity);
                point.transform.parent = gridParent.transform;
                Grid[i][j] = point.GetComponent<GridPoint>().Point;
                Grid[i][j].Coords = new Vector2Int(i, j);
                Grid[i][j].Transform = point.transform;
                if (_GridTexture.GetPixel(i, j).r == 0)
                {
                    Grid[i][j].Invalid = true;
                    point.transform.localScale = Vector3.one*PointDistance;
                    continue;
                }
                if (i > 0)
                {
                    Grid[i][j].Neighbours.Add(new Vector2Int(i - 1, j));
                }
                if (i < Grid.Length - 1)
                {
                    Grid[i][j].Neighbours.Add(new Vector2Int(i + 1, j));
                }
                if (j > 0)
                {
                    Grid[i][j].Neighbours.Add(new Vector2Int(i, j - 1));
                }
                if (j < Grid[0].Length - 1)
                {
                    Grid[i][j].Neighbours.Add(new Vector2Int(i, j + 1));
                }
                if (i > 0 && j > 0)
                {
                    Grid[i][j].Neighbours.Add(new Vector2Int(i - 1, j - 1));
                }
                if (i < Grid.Length - 1 && j < Grid[0].Length - 1)
                {
                    Grid[i][j].Neighbours.Add(new Vector2Int(i + 1, j + 1));
                }
                if (i > 0 && j < Grid[0].Length - 1)
                {
                    Grid[i][j].Neighbours.Add(new Vector2Int(i - 1, j + 1));
                }
                if (i < Grid.Length - 1 && j > 0)
                {
                    Grid[i][j].Neighbours.Add(new Vector2Int(i + 1, j - 1));
                }
            }
        }
    }

    public int ID = 0;
    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            for (int i = 0; i < _GridTexture.width; i++)
            {
                for (int j = 0; j < _GridTexture.height; j++)
                {
                    if (_GridTexture.GetPixel(i, j).r == 1)
                    {
                        Grid[i][j].Transform.localScale = Vector3.one * 0.5f;
                        Grid[i][j].Invalid = false;
                    }
                    else
                    {
                        Grid[i][j].Transform.localScale = Vector3.one*PointDistance;
                        Grid[i][j].Invalid = true;
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
        float sizeX = PointDistance * _GridTexture.width;
        float sizeY = PointDistance * _GridTexture.height;
        Vector3 pos = position - _GridStartPoint.position;
        float percentageX = pos.x / sizeX;
        float percentageY = pos.z / sizeY;
        int x = Mathf.RoundToInt(percentageX * _GridTexture.width);
        int y = Mathf.RoundToInt(percentageY * _GridTexture.height);
        return Grid[x][y];
    }
}
