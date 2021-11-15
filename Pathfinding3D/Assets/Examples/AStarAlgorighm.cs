using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples
{

    //Data Specific for each path
    public class PointData
    {
        public Vector2Int Coords;
        public float GScore;
        public float FScore;
        public Vector2Int CameFrom;
    }

    public class AStarAlgorighm : MonoBehaviour
    {
        [SerializeField] WorldPoint _PointPrefab;
        [SerializeField] Texture2D _GridTexture;
        WorldPoint[][] _WorldGrid;
        List<WorldPoint> _Path;
        WorldPoint start;
        WorldPoint end;

        private void Start()
        {
            WorldSetup();
        }

        private void WorldSetup()
        {
            _WorldGrid = new WorldPoint[_GridTexture.width][];
            for(int i = 0; i < _WorldGrid.Length; i++)
            {
                _WorldGrid[i] = new WorldPoint[_GridTexture.height];
                for(int j = 0; j < _WorldGrid[i].Length; j++)
                {
                    _WorldGrid[i][j] = Instantiate(_PointPrefab, transform);
                    _WorldGrid[i][j].transform.position = new Vector3(i, 0, j);
                    _WorldGrid[i][j].Coords = new Vector2Int(i, j);
                    _WorldGrid[i][j].BaseColor = Color.white;
                    if (_GridTexture.GetPixel(i, j) == Color.black)
                    {
                        _WorldGrid[i][j].Invalid = true;
                        _WorldGrid[i][j].BaseColor=Color.black;
                    }
                    _WorldGrid[i][j].ResetColor();
                    for (int p = -1; p <= 1; p++)
                    {
                        for (int q = -1; q <= 1; q++)
                        {
                            int x = i + p;
                            int y = j + q;
                            if (x > -1 && x < _GridTexture.width &&
                                y > -1 && y < _GridTexture.height)
                            {
                                if (x != i || y != j)
                                {
                                    _WorldGrid[i][j].Neighbours.Add(new Vector2Int(x, y));
                                }
                            }
                        }
                    }
                }
            }
        }

        private float Distance(Vector3 a,Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }

        private List<WorldPoint> ReconstructPath(PointData current,PointData[][] dataSet)
        {
            List<WorldPoint> totalPath = new List<WorldPoint>();
            totalPath.Add(_WorldGrid[current.Coords.x][current.Coords.y]);
            while (current.CameFrom.x != -1)
            {
                current = dataSet[current.CameFrom.x][current.CameFrom.y];
                totalPath.Add(_WorldGrid[current.Coords.x][current.Coords.y]);
            }

            //Reverse order;
            List<WorldPoint> path = new List<WorldPoint>();
            for(int i = totalPath.Count - 1; i >= 0; i--)
            {
                path.Add(totalPath[i]);
            }

            return path;
        }

        private List<WorldPoint> AStar(Vector2Int startPoint,Vector2Int endPoint)
        {
            PointData[][] dataSet = new PointData[_GridTexture.width][];
            for(int i=0;i<dataSet.Length; i++)
            {
                dataSet[i] = new PointData[_GridTexture.height];
                for(int j = 0; j < dataSet[i].Length; j++)
                {
                    dataSet[i][j] = new PointData();
                    dataSet[i][j].Coords = new Vector2Int(i, j);
                    dataSet[i][j].FScore = Mathf.Infinity;
                    dataSet[i][j].GScore = Mathf.Infinity;
                    dataSet[i][j].CameFrom = new Vector2Int(-1, -1);
                }
            }

            dataSet[startPoint.x][startPoint.y].GScore = 0;

            List<PointData> openSet = new List<PointData>();
            openSet.Add(dataSet[startPoint.x][startPoint.y]);

            while (openSet.Count > 0)
            {
                //Find point with Minimum FScore
                PointData current = openSet[0];
                float minFScore = openSet[0].FScore;
                for(int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].FScore < minFScore)
                    {
                        minFScore = openSet[i].FScore;
                        current = openSet[i];
                    }
                }

                //Check if we have reached the target
                if (current.Coords == endPoint)
                {
                    //Reconstruct path;
                    return ReconstructPath(current,dataSet);
                }

                openSet.Remove(current);

                WorldPoint currentPoint = _WorldGrid[current.Coords.x][current.Coords.y];
                for (int i = 0; i < currentPoint.Neighbours.Count; i++)
                {
                    Vector2Int neighbourCoords = currentPoint.Neighbours[i];
                    PointData neighbour = dataSet[neighbourCoords.x][neighbourCoords.y];
                    WorldPoint neighbourPoint = _WorldGrid[neighbourCoords.x][neighbourCoords.y];

                    if (!neighbourPoint.Invalid)
                    {
                        float tmpGScore = current.GScore + Distance(currentPoint.transform.position, 
                            neighbourPoint.transform.position);
                        if (tmpGScore < neighbour.GScore)
                        {
                            neighbour.CameFrom = current.Coords;
                            neighbour.GScore = tmpGScore;
                            //The distance function is a Heuristic function
                            float heuristic = Distance(neighbourPoint.transform.position,
                                _WorldGrid[endPoint.x][endPoint.y].transform.position);
                            neighbour.FScore = tmpGScore + heuristic;
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private IEnumerator Coroutine_AStar(Vector2Int startPoint, Vector2Int endPoint)
        {
            PointData[][] dataSet = new PointData[_GridTexture.width][];
            for (int i = 0; i < dataSet.Length; i++)
            {
                dataSet[i] = new PointData[_GridTexture.height];
                for (int j = 0; j < dataSet[i].Length; j++)
                {
                    dataSet[i][j] = new PointData();
                    dataSet[i][j].Coords = new Vector2Int(i, j);
                    dataSet[i][j].FScore = Mathf.Infinity;
                    dataSet[i][j].GScore = Mathf.Infinity;
                    dataSet[i][j].CameFrom = new Vector2Int(-1, -1);
                }
            }

            dataSet[startPoint.x][startPoint.y].GScore = 0;

            List<PointData> openSet = new List<PointData>();
            openSet.Add(dataSet[startPoint.x][startPoint.y]);

            while (openSet.Count > 0)
            {
                //Find point with Minimum FScore
                PointData current = openSet[0];
                float minFScore = openSet[0].FScore;
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].FScore < minFScore)
                    {
                        minFScore = openSet[i].FScore;
                        current = openSet[i];
                    }
                }

                //Check if we have reached the target
                if (current.Coords == endPoint)
                {
                    //Reconstruct path;
                    _WorldGrid[current.Coords.x][current.Coords.y].SetColor(Color.yellow);
                    _Path = ReconstructPath(current, dataSet);
                    for(int k = 0; k < _Path.Count; k++)
                    {
                        _Path[k].SetColor(Color.red);
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                }
                else
                {
                    openSet.Remove(current);

                    WorldPoint currentPoint = _WorldGrid[current.Coords.x][current.Coords.y];
                    currentPoint.SetColor(Color.yellow);
                    yield return new WaitForSeconds(0.2f);
                    for (int i = 0; i < currentPoint.Neighbours.Count; i++)
                    {
                        Vector2Int neighbourCoords = currentPoint.Neighbours[i];
                        PointData neighbour = dataSet[neighbourCoords.x][neighbourCoords.y];
                        WorldPoint neighbourPoint = _WorldGrid[neighbourCoords.x][neighbourCoords.y];
                        if (!neighbourPoint.Invalid)
                        {
                            float tmpGScore = current.GScore + Distance(currentPoint.transform.position,
                                neighbourPoint.transform.position);

                            Debug.DrawLine(currentPoint.transform.position + Vector3.up * 2, neighbourPoint.transform.position + Vector3.up * 2, Color.red, 0.5f);
                            yield return new WaitForSeconds(0.2f);

                            if (tmpGScore < neighbour.GScore)
                            {
                                neighbour.CameFrom = current.Coords;
                                neighbour.GScore = tmpGScore;
                                //The distance function is a Heuristic function
                                float heuristic = Distance(neighbourPoint.transform.position,
                                    _WorldGrid[endPoint.x][endPoint.y].transform.position);
                                neighbour.FScore = tmpGScore + heuristic;
                                Debug.Log(neighbour.GScore + "|" + neighbour.FScore);
                                //if (neighbourCoords == endPoint)
                                //{
                                //    openSet.Clear();
                                //    openSet.Add(neighbour);
                                //    break;
                                //}
                                if (!openSet.Contains(neighbour))
                                {
                                    neighbourPoint.SetColor(Color.blue);
                                    openSet.Add(neighbour);
                                }
                            }
                        }
                        else
                        {
                            neighbourPoint.ResetColor();
                        }
                    }
                }
            }

            _Path = null;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        if (start != null && end != null)
                        {
                            ResetPointColors();
                            start = null;
                            end = null;
                        }
                        WorldPoint worldPoint = hit.transform.GetComponent<WorldPoint>();

                        if (start == null)
                        {
                            start = worldPoint;
                            return;
                        }
                        if (end == null)
                        {
                            end = worldPoint;
                            StopAllCoroutines();
                            StartCoroutine(Coroutine_AStar(start.Coords, end.Coords));
                            //_Path = AStar(start.Coords, end.Coords);
                            //StopAllCoroutines();
                            //StartCoroutine(Coroutine_FollowPath(_Path));
                            return;
                        }
                    }
                }
            }
        }

        private void ResetPointColors()
        {
            for(int i = 0; i < _WorldGrid.Length; i++)
            {
                for(int j = 0; j < _WorldGrid[i].Length; j++)
                {
                    _WorldGrid[i][j].ResetColor();
                }
            }
        }

        IEnumerator Coroutine_FollowPath(List<WorldPoint> path)
        {
            for(int i = 0; i < path.Count; i++)
            {
                path[i].SetColor(Color.blue);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
