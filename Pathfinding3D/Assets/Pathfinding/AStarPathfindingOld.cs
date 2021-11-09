//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//[System.Serializable]
//public class Point
//{
//    public Vector2Int Coords;
//    public Transform Transform;
//    public List<Vector2Int> Neighbours;
//    public Vector2Int CameFrom;
//    public float GScore;
//    public float FScore;
//    public bool Invalid;
//    public Region Region;
//    public List<int> NeighbouringRegions;
//    public void AddNeighbouringReagion(int id)
//    {
//        if (!NeighbouringRegions.Contains(id))
//        {
//            NeighbouringRegions.Add(id);
//        }
//    }


//    public Point(Transform point)
//    {
//        Transform = point;
//        Neighbours = new List<Vector2Int>();
//        GScore = 0;
//        FScore = 0;
//    }
//}

//[System.Serializable]
//public class Region
//{
//    public int ID;
//    public Vector3 Center;
//    public Vector2Int BottomLeft;
//    public Vector2Int TopRight;
//    public List<int> NeighbourIDs;
//    public List<Vector2Int> ConnectionPoints;
//    public LineRenderer Line;
//    public int CameFromID;
//    public float GScore;
//    public float FScore;

//    public void AddNeighbour(int id)
//    {
//        if (!NeighbourIDs.Contains(id))
//        {
//            NeighbourIDs.Add(id);
//        }
//    }
//    public void AddConnectionPoint(Vector2Int coords)
//    {
//        if (!ConnectionPoints.Contains(coords))
//        {
//            ConnectionPoints.Add(coords);
//        }
//    }
//}

//public class AStarPathfinding : MonoBehaviour
//{
//    [SerializeField] Transform _GridStartPoint;
//    [SerializeField] GameObject _GridPointPrefab;
//    [SerializeField] float _PointDistance;
//    [SerializeField] Texture2D _GridTexture;
//    Point[][] _Grid;
//    List<Region> _Regions;

//    private void Start()
//    {
//        _Regions = new List<Region>();
//        StartCoroutine(InitializeGrid());
//    }

//    private IEnumerator InitializeGrid()
//    {
//        GameObject gridParent = new GameObject("Grid");
//        _Grid = new Point[_GridTexture.width][];
//        for (int i = 0; i < _GridTexture.width; i++)
//        {
//            _Grid[i] = new Point[_GridTexture.height];
//            for (int j = 0; j < _GridTexture.height; j++)
//            {
//                Vector3 pos = _GridStartPoint.position + new Vector3(i, 0, j) * _PointDistance;
//                GameObject point = Instantiate(_GridPointPrefab, pos, Quaternion.identity);
//                point.transform.parent = gridParent.transform;

//                _Grid[i][j] = point.GetComponent<GridPoint>().Point;
//                _Grid[i][j].Coords = new Vector2Int(i, j);
//                _Grid[i][j].Transform = point.transform;
//                _Grid[i][j].FScore = Mathf.Infinity;
//                _Grid[i][j].GScore = Mathf.Infinity;
//                _Grid[i][j].Region = null;
//                _Grid[i][j].NeighbouringRegions = new List<int>();
//                if (_GridTexture.GetPixel(i, j).r == 0)
//                {
//                    _Grid[i][j].Invalid = true;
//                    point.transform.localScale = Vector3.one;
//                    continue;
//                }
//            }
//        }
//        for (int i = 0; i < _Grid.Length; i++)
//        {
//            for (int j = 0; j < _Grid[0].Length; j++)
//            {
//                if (i > 0)
//                {
//                    _Grid[i][j].Neighbours.Add(new Vector2Int(i - 1, j));
//                }
//                if (i < _Grid.Length - 1)
//                {
//                    _Grid[i][j].Neighbours.Add(new Vector2Int(i + 1, j));
//                }
//                if (j > 0)
//                {
//                    _Grid[i][j].Neighbours.Add(new Vector2Int(i, j - 1));
//                }
//                if (j < _Grid[0].Length - 1)
//                {
//                    _Grid[i][j].Neighbours.Add(new Vector2Int(i, j + 1));
//                }
//                if (i > 0 && j > 0)
//                {
//                    _Grid[i][j].Neighbours.Add(new Vector2Int(i - 1, j - 1));
//                }
//                if (i < _Grid.Length - 1 && j < _Grid[0].Length - 1)
//                {
//                    _Grid[i][j].Neighbours.Add(new Vector2Int(i + 1, j + 1));
//                }
//                if (i > 0 && j < _Grid[0].Length - 1)
//                {
//                    _Grid[i][j].Neighbours.Add(new Vector2Int(i - 1, j + 1));
//                }
//                if (i < _Grid.Length - 1 && j > 0)
//                {
//                    _Grid[i][j].Neighbours.Add(new Vector2Int(i + 1, j - 1));
//                }

//            }
//        }
//        yield return null;

//        List<Vector2Int> region = new List<Vector2Int>();



//        yield return null;
//    }



//    [SerializeField] LineRenderer _RegionLine;
//    private IEnumerator DebugOneRegion2()
//    {
//        int regionCount = 0;
//        for (int i = 0; i < _GridTexture.width; i++)
//        {
//            for (int j = 0; j < _GridTexture.height; j++)
//            {
//                if (!_Grid[i][j].Invalid && _Grid[i][j].Region == null)
//                {
//                    bool upDirection = false;
//                    bool downDirection = false;
//                    bool leftDirection = false;
//                    bool rightDirection = false;

//                    int minX = -1;
//                    int maxX = _GridTexture.width;
//                    int minY = -1;
//                    int maxY = _GridTexture.height;
//                    int step = 1;

//                    while (!leftDirection)
//                    {
//                        int x = i - step;
//                        if (x < 0)
//                        {
//                            minX = 0;
//                            leftDirection = true;
//                            break;
//                        }
//                        if (!_Grid[x][j].Invalid && _Grid[x][j].Region == null)
//                        {
//                            _Grid[x][j].Transform.localScale = Vector3.one * 0.5f;
//                        }
//                        else
//                        {
//                            leftDirection = true;
//                            minX = x;
//                        }
//                        step++;
//                    }
//                    step = 1;
//                    while (!rightDirection)
//                    {
//                        int x = i + step;
//                        if (x >=_GridTexture.width)
//                        {
//                            maxX = _GridTexture.width-1;
//                            rightDirection = true;
//                            break;
//                        }
//                        if (!_Grid[x][j].Invalid && _Grid[x][j].Region == null)
//                        {
//                            _Grid[x][j].Transform.localScale = Vector3.one * 0.5f;
//                        }
//                        else
//                        {
//                            rightDirection = true;
//                            maxX = x;
//                        }
//                        step++;
//                    }
//                    step = 1;
//                    while (!downDirection)
//                    {
//                        int y = j - step;
//                        if (y <0)
//                        {
//                            minY =0 ;
//                            downDirection = true;
//                            break;
//                        }
//                        for (int x = minX + 1; x < maxX; x++)
//                        {
//                            if (!_Grid[x][y].Invalid && _Grid[x][y].Region == null)
//                            {
//                                _Grid[x][y].Transform.localScale = Vector3.one * 0.5f;
//                            }
//                            else
//                            {
//                                downDirection = true;
//                                minY = y;
//                            }
//                        }
//                        step++;
//                    }
//                    step = 1;
//                    while (!upDirection)
//                    {
//                        int y = j + step;
//                        if (y >= _GridTexture.height)
//                        {
//                            maxY = _GridTexture.height-1;
//                            upDirection = true;
//                            break;
//                        }
//                        for (int x = minX + 1; x < maxX; x++)
//                        {
//                            if (!_Grid[x][y].Invalid && _Grid[x][y].Region == null)
//                            {
//                                _Grid[x][y].Transform.localScale = Vector3.one * 0.5f;
//                            }
//                            else
//                            {
//                                upDirection = true;
//                                maxY = y;
//                            }
//                        }
//                        step++;
//                    }
//                    minX = minX + 1;
//                    minY = minY + 1;
//                    maxX = maxX - 1;
//                    maxY = maxY - 1;
//                    Region region = new Region()
//                    {
//                        ID = regionCount,
//                        BottomLeft = new Vector2Int(minX, minY),
//                        TopRight = new Vector2Int(maxX, maxY),
//                        NeighbourIDs = new List<int>(),
//                        CameFromID = -1,
//                        GScore = Mathf.Infinity,
//                        FScore = Mathf.Infinity,
//                        ConnectionPoints = new List<Vector2Int>()
//                    };



//                    region.Center = _Grid[minX][minY].Transform.position+(_Grid[maxX][maxY].Transform.position - _Grid[minX][minY].Transform.position)/2f;

//                    for (int x = minX ; x <= maxX; x++)
//                    {
//                        for (int y = minY; y <= maxY; y++)
//                        {
//                            _Grid[x][y].Region = region;
//                        }
//                    }
//                    LineRenderer lineRenderer = Instantiate(_RegionLine, transform);
//                    lineRenderer.positionCount = 5;
//                    lineRenderer.SetPosition(0, _Grid[minX][minY].Transform.position + Vector3.up * 2);
//                    lineRenderer.SetPosition(1, _Grid[maxX][minY].Transform.position + Vector3.up * 2);
//                    lineRenderer.SetPosition(2, _Grid[maxX][maxY].Transform.position + Vector3.up * 2);
//                    lineRenderer.SetPosition(3, _Grid[minX][maxY].Transform.position + Vector3.up * 2);
//                    lineRenderer.SetPosition(4, _Grid[minX][minY].Transform.position + Vector3.up * 2);
//                    regionCount++;
//                    region.Line = lineRenderer;
//                    _Regions.Add(region);
//                    yield return null;
//                }
//            }
//        }

//        for (int r = 0; r < _Regions.Count; r++)
//        {
//            Region region = _Regions[r];
//            int minX = region.BottomLeft.x;
//            int minY = region.BottomLeft.y;
//            int maxX = region.TopRight.x;
//            int maxY = region.TopRight.y;
//            for (int x = minX; x <= maxX; x++)
//            {
//                for (int y = minY; y <= maxY; y++)
//                {
//                    _Grid[x][y].Region = region;
//                    if (x == minX || x == maxX || y == minY || y == maxY)
//                    {
//                        if (!_Grid[x][y].Invalid)
//                        {
//                            region.AddConnectionPoint(new Vector2Int(x, y));
//                            int left = x - 1;
//                            int right = x + 1;
//                            int down = y - 1;
//                            int up = y + 1;
//                            if (left > -1)
//                            {
//                                Point p = _Grid[left][y];
//                                if (!p.Invalid && p.Region != null && p.Region.ID != region.ID)
//                                {

//                                    _Grid[x][y].AddNeighbouringReagion(p.Region.ID);
//                                    p.AddNeighbouringReagion(region.ID);
//                                    p.Region.AddNeighbour(region.ID);
//                                    region.AddNeighbour(p.Region.ID);
//                                }
//                            }
//                            if (right < _GridTexture.width)
//                            {
//                                Point p = _Grid[right][y];
//                                if (!p.Invalid && p.Region != null && p.Region.ID != region.ID)
//                                {
//                                    _Grid[x][y].AddNeighbouringReagion(p.Region.ID);
//                                    p.AddNeighbouringReagion(region.ID);
//                                    p.Region.AddNeighbour(region.ID);
//                                    region.AddNeighbour(p.Region.ID);
//                                }
//                            }
//                            if (down > -1)
//                            {
//                                Point p = _Grid[x][down];
//                                if (!p.Invalid && p.Region != null && p.Region.ID != region.ID)
//                                {
//                                    _Grid[x][y].AddNeighbouringReagion(p.Region.ID);
//                                    p.AddNeighbouringReagion(region.ID);
//                                    p.Region.AddNeighbour(region.ID);
//                                    region.AddNeighbour(p.Region.ID);
//                                }
//                            }
//                            if (up < _GridTexture.height)
//                            {
//                                Point p = _Grid[x][up];
//                                if (!p.Invalid && p.Region != null && p.Region.ID != region.ID)
//                                {
//                                    _Grid[x][y].AddNeighbouringReagion(p.Region.ID);
//                                    p.AddNeighbouringReagion(region.ID);
//                                    p.Region.AddNeighbour(region.ID);
//                                    region.AddNeighbour(p.Region.ID);
//                                }
//                            }

//                            if (left > -1 && down > -1)
//                            {
//                                Point p = _Grid[left][down];
//                                if (!p.Invalid && p.Region != null && p.Region.ID != region.ID)
//                                {
//                                    _Grid[x][y].AddNeighbouringReagion(p.Region.ID);
//                                    p.AddNeighbouringReagion(region.ID);
//                                    p.Region.AddNeighbour(region.ID);
//                                    region.AddNeighbour(p.Region.ID);
//                                }
//                            }
//                            if (right < _GridTexture.width && down > -1)
//                            {
//                                Point p = _Grid[right][down];
//                                if (!p.Invalid && p.Region != null && p.Region.ID != region.ID)
//                                {
//                                    _Grid[x][y].AddNeighbouringReagion(p.Region.ID);
//                                    p.AddNeighbouringReagion(region.ID);
//                                    p.Region.AddNeighbour(region.ID);
//                                    region.AddNeighbour(p.Region.ID);
//                                }
//                            }
//                            if (left > -1 && up < _GridTexture.height)
//                            {
//                                Point p = _Grid[left][up];
//                                if (!p.Invalid && p.Region != null && p.Region.ID != region.ID)
//                                {
//                                    _Grid[x][y].AddNeighbouringReagion(p.Region.ID);
//                                    p.AddNeighbouringReagion(region.ID);
//                                    p.Region.AddNeighbour(region.ID);
//                                    region.AddNeighbour(p.Region.ID);
//                                }
//                            }
//                            if (right < _GridTexture.width && up < _GridTexture.height)
//                            {
//                                Point p = _Grid[right][up];
//                                if (!p.Invalid && p.Region != null && p.Region.ID != region.ID)
//                                {
//                                    _Grid[x][y].AddNeighbouringReagion(p.Region.ID);
//                                    p.AddNeighbouringReagion(region.ID);
//                                    p.Region.AddNeighbour(region.ID);
//                                    region.AddNeighbour(p.Region.ID);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }

//    private float HeuristicFunction(Vector3 p1, Vector3 p2)
//    {
//        return (p2- p1).sqrMagnitude;
//    }

//    private float Distance(Vector3 p1, Vector3 p2)
//    {
//        return (p2 - p1).sqrMagnitude;
//    }

//    private List<Point> ReconstructPath(Point current)
//    {
//        List<Point> totalPath = new List<Point>();
//        totalPath.Add(current);
//        while (current.CameFrom.x != -1)
//        {
//            totalPath.Add(_Grid[current.CameFrom.x][current.CameFrom.y]);
//            current = _Grid[current.CameFrom.x][current.CameFrom.y];
//        }
//        return totalPath;
//    }

//    private List<Region> ReconstructPath(Region current)
//    {
//        List<Region> totalPath = new List<Region>();
//        totalPath.Add(current);
//        current.Line.material.SetColor("_BaseColor", Color.red);
//        while (current.CameFromID != -1)
//        {
//            Region next = _Regions.Find(x => x.ID == current.CameFromID);
//            totalPath.Add(next);
//            next.Line.material.SetColor("_BaseColor", Color.red);
//            current = next;
//        }
//        return totalPath;
//    }

//    private List<Point> AStar(Point start, Point goal,Region nextRegion)
//    {
//        if (nextRegion.ID != goal.Region.ID)
//        {
//            float minDistance = Mathf.Infinity;
//            Point nextGoal = null;
//            for(int i = 0; i < nextRegion.ConnectionPoints.Count; i++)
//            {
//                Point p = _Grid[nextRegion.ConnectionPoints[i].x][nextRegion.ConnectionPoints[i].y];
//                if(!p.Invalid && p.NeighbouringRegions.Contains(start.Region.ID))
//                {
//                    float distance = (p.Transform.position - start.Transform.position).sqrMagnitude;
//                    if (distance < minDistance)
//                    {
//                        minDistance = distance;
//                        nextGoal = p;
//                    }
//                }
//            }
//            goal = nextGoal;
//        }

//        List<Point> openSet = new List<Point>();
//        openSet.Add(start);
//        start.GScore = 0;
//        start.FScore = HeuristicFunction(start.Transform.position, goal.Transform.position);

//        while (openSet.Count > 0)
//        {
//            Point current = null;
//            float score = Mathf.Infinity;
//            for (int i = 0; i < openSet.Count; i++)
//            {
//                if (score > openSet[i].FScore)
//                {
//                    score = openSet[i].FScore;
//                    current = openSet[i];
//                }
//            }


//            if (current == goal)
//            {
//                return ReconstructPath(current);
//            }


//            openSet.Remove(current);
//            for (int i = 0; i < current.Neighbours.Count; i++)
//            {
//                Vector2Int indexes = current.Neighbours[i];
//                Point neighbour = _Grid[indexes.x][indexes.y];
//                if (!neighbour.Invalid && (neighbour.Region.ID==current.Region.ID||neighbour.Region.ID==nextRegion.ID))
//                {
//                    float tenativeScore = current.GScore + _PointDistance;
//                    if (tenativeScore < neighbour.GScore)
//                    {
//                        neighbour.CameFrom = current.Coords;
//                        neighbour.GScore = tenativeScore;
//                        neighbour.FScore = neighbour.GScore + HeuristicFunction(neighbour.Transform.position, goal.Transform.position);
//                        if (!openSet.Contains(neighbour))
//                        {
//                            openSet.Add(neighbour);
//                        }
//                    }
//                }
//            }
//        }

//        return null;

//    }

//    private List<Region> AStarRegion(Region start,Region goal)
//    {
//        if (start.ID == goal.ID)
//        {
//            return new List<Region>() { start, goal };
//        }
//        List<Region> openSet = new List<Region>();
//        openSet.Add(start);
//        start.GScore = 0;
//        start.FScore = HeuristicFunction(start.Center, goal.Center);

//        while (openSet.Count > 0)
//        {
//            Region current = null;
//            float score = Mathf.Infinity;
//            for (int i = 0; i < openSet.Count; i++)
//            {
//                if (score > openSet[i].FScore)
//                {
//                    score = openSet[i].FScore;
//                    current = openSet[i];
//                }
//            }


//            if (current == goal)
//            {
//                return ReconstructPath(current);
//            }


//            openSet.Remove(current);
//            for (int i = 0; i < current.NeighbourIDs.Count; i++)
//            {
//                Region neighbour = _Regions.Find(x => x.ID == current.NeighbourIDs[i]);
//                float tenativeScore = current.GScore + HeuristicFunction(neighbour.Center, current.Center);
//                if (tenativeScore < neighbour.GScore)
//                {
//                    neighbour.CameFromID = current.ID;
//                    neighbour.GScore = tenativeScore;
//                    neighbour.FScore = neighbour.GScore + HeuristicFunction(neighbour.Center, goal.Center);
//                    if (!openSet.Contains(neighbour))
//                    {
//                        openSet.Add(neighbour);
//                    }
//                }
//            }
//        }
//        return null;
//    }

//    IEnumerator Coroutine_DebugPathfinding(Point start,Point end)
//    {
//        List<Region> regionPath = AStarRegion(start.Region, end.Region);
//        int regionCount = regionPath.Count - 2;
//        if (regionPath != null)
//        {
//            List<Point> totalPath = AStar(start, end, regionPath[regionCount]);
//            while (totalPath != null)
//            {
//                for (int i = totalPath.Count-1; i >= 0; i--)
//                {
//                    totalPath[i].Transform.localScale = Vector3.one * 0.5f;
//                    yield return new WaitForSeconds(0.1f);
//                }

//                for(int i = 0; i < _GridTexture.width; i++)
//                {
//                    for(int j = 0; j < _GridTexture.height; j++)
//                    {
//                        _Grid[i][j].CameFrom = new Vector2Int(-1, -1);
//                        _Grid[i][j].GScore = Mathf.Infinity;
//                        _Grid[i][j].FScore = Mathf.Infinity;
//                    }
//                }

//                if(totalPath[0] == start)
//                {
//                    break;
//                }
//                regionCount--;
//                totalPath = AStar(totalPath[0], end,regionPath[regionCount]);
//            }
//        }
//    }

//    Point start;
//    Point end;

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            StopAllCoroutines();
//            StartCoroutine(DebugOneRegion2());
//        }
//        if (Input.GetMouseButtonDown(2))
//        {
//            StopAllCoroutines();
//            for (int i = 0; i < _GridTexture.width; i++)
//                for (int j = 0; j < _GridTexture.height; j++)
//                {
//                    if (!_Grid[i][j].Invalid)
//                        _Grid[i][j].Transform.localScale = Vector3.one * 0.2f;
//                    _Grid[i][j].CameFrom = new Vector2Int(-1,-1);
//                    _Grid[i][j].FScore = Mathf.Infinity;
//                    _Grid[i][j].GScore = Mathf.Infinity;
//                }

//            foreach(var region in _Regions)
//            {
//                region.CameFromID = -1;
//                region.GScore = Mathf.Infinity;
//                region.FScore = Mathf.Infinity;
//                region.Line.material.SetColor("_BaseColor", Color.white);
//            }

//        }
//        if (Input.GetMouseButtonDown(0))
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit))
//            {
//                start = null;
//                GridPoint point = hit.transform.GetComponent<GridPoint>();
//                if (point!=null && !point.Point.Invalid)
//                {
//                    start = point.Point;
//                }
//            }
//        }

//        if (Input.GetMouseButtonDown(1))
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit) && start != null)
//            {
//                end = null;
//                GridPoint point = hit.transform.GetComponent<GridPoint>();
//                if (point != null && !point.Point.Invalid)
//                {
//                    end = point.Point;
//                }
//                if (start != null && end != null)
//                {
//                    StopAllCoroutines();
//                    StartCoroutine(Coroutine_DebugPathfinding(start, end));
//                }
//            }
//        }
//    }
//}
