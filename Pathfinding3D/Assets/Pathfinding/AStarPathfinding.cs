using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Point
{
    public Vector3Int Coords;
    public Vector3 WorldPosition;
    public List<Vector3Int> Neighbours;
    public bool Invalid;
    public List<MovingData> MovingData;
    public float distanceFactor = 0.5f;
    public Point()
    {
        Neighbours = new List<Vector3Int>();
    }

    public void AddMovingData(AStarPathfinding obj,float time)
    {
        if (MovingData == null)
        {
            MovingData = new List<MovingData>();
        }
        MovingData.Add(new MovingData() { MovingObj = obj, TimeToReach = time,TimeStarted=Time.time });
    }

    public void RemoveMovingData(AStarPathfinding obj)
    {
        MovingData.Remove(MovingData.Find(x => x.MovingObj == obj));
    }

    public void CheckForIntersections()
    {
        if (MovingData != null)
        {
            List<MovingData> toRemove = new List<MovingData>();
            for(int i = 0; i < MovingData.Count; i++)
            {
                MovingData data = MovingData[i];
                for(int j = 0; j < MovingData.Count; j++)
                {
                    if (i != j)
                    {
                        MovingData data2 = MovingData[j];
                        if (data.MovingObj.ID < data2.MovingObj.ID)
                        {
                            float difference = Mathf.Abs(data.TrueTimeToReach() - data2.TrueTimeToReach());
                            if (difference < distanceFactor)
                            {
                                toRemove.Add(data);
                                break;
                            }
                        }
                    }
                }
            }
            for(int i = 0; i < toRemove.Count; i++)
            {
                MovingData.Remove(toRemove[i]);
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                toRemove[i].MovingObj.RePath();
            }
        }
    }

    public bool CheckPointAvailability(float timeToReach,int priority)
    {
        bool available = true;
        if (MovingData != null)
        {
            for (int i = 0; i < MovingData.Count; i++)
            {
                if (MovingData[i].MovingObj.ID > priority)
                {
                    float difference = Mathf.Abs(MovingData[i].TrueTimeToReach() - timeToReach);
                    if (difference < distanceFactor)
                    {
                        available = false;
                        break;
                    }
                }
            }
        }
        return available;
    }
}

public class MovingData
{
    public AStarPathfinding MovingObj;
    public float TimeToReach;
    public float TimeStarted;

    public float TrueTimeToReach()
    {
        return Mathf.Max(TimeToReach - (Time.time - TimeStarted),0);
    }
}

public class PointData
{
    public float GScore;
    public float FScore;
    public Vector3Int CameFrom;
    public Vector3Int Coords;
    public float TimeToReach;

    public PointData(Point point)
    {
        GScore = Mathf.Infinity;
        FScore = Mathf.Infinity;
        CameFrom = new Vector3Int(-1, -1, -1);
        Coords = point.Coords;
    }
}


public class AStarPathfinding : MonoBehaviour
{   
    public float Speed;
    public bool MoveRandom;
    public Color PathColor;
    List<Point> totalPath;
    Point _start;
    Point _end;
    public bool MovingObstacle;
    public int ID;
    PathCreator _pathCreator;
    [SerializeField] PathCreator _PathCreatorPrefab;

    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    private void Start()
    {
        if (MoveRandom)
        {
            StartCoroutine(Coroutine_MoveAB());
        }
    }

    private float HeuristicFunction(Vector3 p1, Vector3 p2)
    {
        return (p2 - p1).sqrMagnitude;
    }

    private float Distance(Vector3 p1, Vector3 p2)
    {
        return (p2 - p1).sqrMagnitude;
    }

    List<Point> _cornerPoints;

    private List<Point> ReconstructPath(PointData start,PointData current,PointData[][][] dataSet)
    {
        _cornerPoints = new List<Point>();
        List<Point> totalPath = new List<Point>();

        PointData currentPointData = dataSet[current.Coords.x][current.Coords.y][current.Coords.z];
        Point currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];

        currentPoint.AddMovingData(this, currentPointData.TimeToReach);
        totalPath.Add(currentPoint);

        Point cameFromPoint = WorldManager.Instance.Grid[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];

        Vector3 direction =(currentPoint.Coords-cameFromPoint.Coords);
        direction = direction.normalized;

        _cornerPoints.Add(currentPoint);

        int count = 0;
        while (current.CameFrom.x != -1 && count<10000)
        {
            PointData cameFromPointData = dataSet[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];
            cameFromPoint = WorldManager.Instance.Grid[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];

            Vector3 dir = (currentPoint.Coords - cameFromPoint.Coords);
            if (dir != direction)
            {
                _cornerPoints.Add(currentPoint);
                direction = dir;
            }

            cameFromPoint.AddMovingData(this, cameFromPointData.TimeToReach);
            totalPath.Add(cameFromPoint);
            current = dataSet[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];
        }

        _cornerPoints.Add(currentPoint);
        for(int i = 0; i < totalPath.Count; i++)
        {
            totalPath[i].CheckForIntersections();
        }
        return totalPath;
    }

    public int GetIndexFromCoords(Vector2Int coords)
    {
        return coords.x * WorldManager.Instance.Grid[0].Length + coords.y;
    }

    private void Heapify(List<PointData> list,int i)
    {
        int parent = (i - 1) / 2;
        if (parent>-1)
        {
            if (list[i].FScore < list[parent].FScore)
            {
                PointData pom = list[i];
                list[i] = list[parent];
                list[parent] = pom;
                Heapify(list, parent);
            }
        }
    }

    private void HeapifyDeletion(List<PointData> list, int i)
    {
        int smallest = i;
        int l = 2 * i + 1;
        int r = 2 * i + 2;

        if (l < list.Count && list[l].FScore < list[smallest].FScore)
        {
            smallest = l;
        }
        if (r < list.Count && list[r].FScore < list[smallest].FScore)
        {
            smallest = r;
        }
        if (smallest != i)
        {
            PointData pom = list[i];
            list[i] = list[smallest];
            list[smallest] = pom;

            // Recursively heapify the affected sub-tree
            HeapifyDeletion(list, smallest);
        }
    }

    private List<Point> AStar(Point start, Point goal)
    {
        PointData[][][] dataSet = new PointData[WorldManager.Instance.Grid.Length][][];
        for (int i = 0; i < dataSet.Length; i++)
        {
            dataSet[i] = new PointData[WorldManager.Instance.Grid[i].Length][];
            for (int j = 0; j < dataSet[i].Length; j++)
            {
                dataSet[i][j] = new PointData[WorldManager.Instance.Grid[i][j].Length];
            }
        }

        List<PointData> openSet = new List<PointData>();

        PointData startPoint = new PointData(start);
        dataSet[start.Coords.x][start.Coords.y][start.Coords.z] = startPoint;
        startPoint.GScore = 0;
        openSet.Add(startPoint);

        while (openSet.Count > 0)
        {
            PointData current = openSet[0];

            if (current.Coords == goal.Coords)
            {
                return ReconstructPath(startPoint,current,dataSet);
            }
            openSet.RemoveAt(0);
            HeapifyDeletion(openSet, 0);

            Point currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];
            for (int i = 0; i < currentPoint.Neighbours.Count; i++)
            {
                Vector3Int indexes = currentPoint.Neighbours[i];
                Point neighbour = WorldManager.Instance.Grid[indexes.x][indexes.y][indexes.z];
                PointData neighbourData = dataSet[indexes.x][indexes.y][indexes.z];
                bool neighbourPassed = true;
                if (neighbourData == null)
                {
                    neighbourData = new PointData(neighbour);
                    dataSet[indexes.x][indexes.y][indexes.z] = neighbourData;
                    neighbourPassed = false;
                }
                if (!neighbour.Invalid)
                {
                    float tenativeScore = current.GScore + WorldManager.Instance.PointDistance;
                    if (tenativeScore < neighbourData.GScore)
                    {
                        neighbourData.CameFrom = current.Coords;
                        neighbourData.GScore = tenativeScore;
                        neighbourData.FScore = neighbourData.GScore + HeuristicFunction(neighbour.WorldPosition, goal.WorldPosition);
                        if (!neighbourPassed)
                        {
                            openSet.Add(neighbourData);
                            Heapify(openSet,openSet.Count-1);
                        }
                    }
                }
            }
        }

        return null;

    }

    private List<Point> AStarAvoision(Point start, Point goal)
    {
        _start = start;
        _end = goal;
        if (_start == _end)
        {
            return null;
        }

        if (totalPath != null)
        {
            for(int i = 0; i < totalPath.Count; i++)
            {
                totalPath[i].MovingData.Remove(totalPath[i].MovingData.Find(x => x.MovingObj == this));
            }
        }

        PointData[][][] dataSet = new PointData[WorldManager.Instance.Grid.Length][][];
        for (int i = 0; i < dataSet.Length; i++)
        {
            dataSet[i] = new PointData[WorldManager.Instance.Grid[i].Length][];
            for (int j = 0; j < dataSet[i].Length; j++)
            {
                dataSet[i][j] = new PointData[WorldManager.Instance.Grid[i][j].Length];
            }
        }

        List<PointData> openSet = new List<PointData>();

        PointData startPoint = new PointData(start);
        dataSet[start.Coords.x][start.Coords.y][start.Coords.z] = startPoint;
        startPoint.GScore = 0;

        startPoint.TimeToReach = 0;

        openSet.Add(startPoint);



        while (openSet.Count > 0)
        {
            PointData current = openSet[0];

            
            if (current.Coords == goal.Coords)
            {
                return ReconstructPath(startPoint, current, dataSet);
            }

            openSet.RemoveAt(0);
            HeapifyDeletion(openSet, 0);

            Point currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];

            for (int i = 0; i < currentPoint.Neighbours.Count; i++)
            {
                Vector3Int indexes = currentPoint.Neighbours[i];
                Point neighbour = WorldManager.Instance.Grid[indexes.x][indexes.y][indexes.z];
                PointData neighbourData = dataSet[indexes.x][indexes.y][indexes.z];

                bool neighbourPassed = true;
                if (neighbourData == null)
                {
                    neighbourData = new PointData(neighbour);
                    dataSet[indexes.x][indexes.y][indexes.z] = neighbourData;
                    neighbourPassed = false;
                }


                float distance = (currentPoint.WorldPosition - neighbour.WorldPosition).magnitude;
                float timeToReach = current.TimeToReach + distance / Speed;
                bool neighbourAvailable = neighbour.CheckPointAvailability(timeToReach,ID);

                if (!neighbour.Invalid && neighbourAvailable)
                {
                    float tenativeScore = current.GScore + WorldManager.Instance.PointDistance;
                    if (tenativeScore < neighbourData.GScore)
                    {
                        neighbourData.CameFrom = current.Coords;
                        neighbourData.GScore = tenativeScore;
                        neighbourData.FScore = neighbourData.GScore + HeuristicFunction(neighbour.WorldPosition, goal.WorldPosition);
                        neighbourData.TimeToReach = timeToReach;
                        if (!neighbourPassed)
                        {
                            openSet.Add(neighbourData);
                            Heapify(openSet, openSet.Count - 1);
                        }
                    }
                }
            }
        }

        return null;

    }

    bool resetPathMovement;
    public void RePath()
    {
        resetPathMovement = true;
        _start = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
        totalPath = AStarAvoision(_start, _end);
    }

    private void Update()
    {

        if (MoveRandom)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) && ID==WorldManager.Instance.ID)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _start = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
                _end = WorldManager.Instance.GetClosestPointWorldSpace(hit.point);
            }
        }
    }

    public List<Point> GetTotalPath()
    {
        return totalPath;
    }

    IEnumerator Coroutine_CharacterFollowPathAvoision()
    {
        for (int i = totalPath.Count - 1; i >= 0; i--)
        {
            SetPathColor(PathColor);
            float length = (transform.position - totalPath[i].WorldPosition).magnitude;

            while (length > Speed * Time.deltaTime)
            {
                if (resetPathMovement)
                {
                    i = totalPath.Count - 1;
                    resetPathMovement = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, totalPath[i].WorldPosition, Speed * Time.deltaTime);
                length = (transform.position - totalPath[i].WorldPosition).magnitude;
                yield return null;
            }
            totalPath[i].RemoveMovingData(this);
        }
    }

    IEnumerator Coroutine_CharacterFollowPathCurve()
    {
        if (_pathCreator == null)
        {
            _pathCreator = Instantiate(_PathCreatorPrefab, Vector3.zero, Quaternion.identity);
        }
        List<Vector3> points = new List<Vector3>();
        for(int i = _cornerPoints.Count-1; i >= 0; i--)
        {
            points.Add(_cornerPoints[i].WorldPosition);
        }
        BezierPath bezierPath = new BezierPath(points, false, PathSpace.xyz);
        _pathCreator.bezierPath = bezierPath;
        float length = _pathCreator.path.length;
        float l = 0;
        Vector3 pos = transform.position;
        while (l < length)
        {
            SetPathColor(PathColor);
            transform.position = _pathCreator.path.GetPointAtDistance(l, EndOfPathInstruction.Stop);
            transform.forward = transform.position - pos;
            pos = transform.position;
            l += Time.deltaTime * Speed;
            yield return null;
        }
    }

    public List<Point> GetFreePoints()
    {
        List<Point> freePoints = new List<Point>();
        for (int i = 0; i < WorldManager.Instance.Grid.Length; i++)
        {
            for (int j = 0; j < WorldManager.Instance.Grid[i].Length; j++)
            {
                for (int k = 0; k < WorldManager.Instance.Grid[i][j].Length; k++)
                {
                    if (!WorldManager.Instance.Grid[i][j][k].Invalid)
                    {
                        freePoints.Add(WorldManager.Instance.Grid[i][j][k]);
                    }
                }
            }
        }
        return freePoints;
    }

    IEnumerator Coroutine_MoveRandom()
    {
        List<Point> freePoints = GetFreePoints();
        _start = freePoints[Random.Range(0, freePoints.Count)];
        transform.position = _start.WorldPosition;

        while (true)
        {

            Point p = freePoints[Random.Range(0, freePoints.Count)];
            _start = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
            totalPath = AStar(_start, p);
            while (totalPath == null) 
            {
                p = freePoints[Random.Range(0, freePoints.Count)];
                _start = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
                totalPath = AStar(_start, p);
                yield return null;
            }
            yield return StartCoroutine(Coroutine_CharacterFollowPathCurve());

        }
    }


    IEnumerator Coroutine_MoveAB()
    {
        transform.position = pointA.position;
        yield return null;
        while (true)
        {
            Transform target = pointA;
            float distanceB = (transform.position - pointB.position).magnitude;
            float distanceA = (transform.position - pointA.position).magnitude;
            if (distanceB > distanceA)
            {
                target = pointB;
            }
            Point p = WorldManager.Instance.GetClosestPointWorldSpace(target.position);
            _start = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
            totalPath = AStarAvoision(_start, p);
            if (totalPath != null)
            {
                yield return StartCoroutine(Coroutine_CharacterFollowPathAvoision());
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public bool debug;
    public void SetPathColor(Color color)
    {
        if (debug)
        {
            int a = 0;
        }
        for (int j = totalPath.Count - 2; j >= 0; j--)
        {
            Debug.DrawLine(totalPath[j + 1].WorldPosition, totalPath[j].WorldPosition, color,1);
        }
    }

}
