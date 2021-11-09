using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Point
{
    public Vector3Int Coords;
    public Transform Transform;
    public List<Vector3Int> Neighbours;
    public bool Invalid;
    public AStarPathfinding MovingObj;

    public Point(Transform point)
    {
        Transform = point;
        Neighbours = new List<Vector3Int>();
    }
}

public class PointData
{
    public float GScore;
    public float FScore;
    public Vector3Int CameFrom;
    public Vector3Int Coords;

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
    Point start;
    Point end;
    public bool MovingObstacle;
    public int ID;

    private void Start()
    {
        if (MoveRandom)
        {
            List<Point> freePoints = new List<Point>();
            for (int i = 0; i < WorldManager.Instance.Grid.Length; i++)
            {
                for (int j = 0; j < WorldManager.Instance.Grid[i].Length; j++)
                {
                    for (int k = 0; k < WorldManager.Instance.Grid[i].Length; k++)
                    {
                        if (!WorldManager.Instance.Grid[i][j][k].Invalid)
                        {
                            freePoints.Add(WorldManager.Instance.Grid[i][j][k]);
                        }
                    }
                }
            }

            start = freePoints[Random.Range(0, freePoints.Count)];
            transform.position = start.Transform.position;

            StartCoroutine(Coroutine_MoveRandom());
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

    private List<Point> ReconstructPath(PointData start,PointData current,PointData[][][] dataSet)
    {
        List<Point> totalPath = new List<Point>();
        totalPath.Add(WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z]);
        int count = 0;
        while (current.CameFrom.x != -1 && count<10000)
        {
            totalPath.Add(WorldManager.Instance.Grid[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z]);
            current = dataSet[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];
            if(start.Coords == current.Coords)
            {
                break;
            }
            count++;

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
                bool priorityPass = neighbour.MovingObj == null || neighbour.MovingObj.ID > ID;
                if (!neighbour.Invalid && priorityPass)
                {
                    float tenativeScore = current.GScore + WorldManager.Instance.PointDistance;
                    if (tenativeScore < neighbourData.GScore)
                    {
                        neighbourData.CameFrom = current.Coords;
                        neighbourData.GScore = tenativeScore;
                        neighbourData.FScore = neighbourData.GScore + HeuristicFunction(neighbour.Transform.position, goal.Transform.position);
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

    Point takenPoint = null;
    private void Update()
    {
        if (takenPoint != null)
        {
            takenPoint.MovingObj = null;
            if (MovingObstacle)
            {
                takenPoint.Transform.localScale = Vector3.one *0.5f;
            }
        }
        takenPoint = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
        takenPoint.MovingObj = this;

        if (MovingObstacle)
        {
            takenPoint.Transform.localScale = Vector3.one * WorldManager.Instance.PointDistance;
        }

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
                start = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
                end = WorldManager.Instance.GetClosestPointWorldSpace(hit.point);
                //if (!start.Invalid && !end.Invalid)
                //{
                //    totalPath = AStar(start, end);
                //    StopAllCoroutines();
                //    StartCoroutine(Coroutine_CharacterFollowPath());
                //}
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (start != null && end != null && !end.Invalid)
            {
                totalPath = AStar(start, end);
                StopAllCoroutines();
                StartCoroutine(Coroutine_CharacterFollowPath());
            }
        }
    }
    public int pathBreakPoint = -1;
    IEnumerator Coroutine_CharacterFollowPath()
    {
        pathBreakPoint = -1;
        for(int i = totalPath.Count - 1; i >= 0; i--)
        {
            SetPathColor(PathColor);
            if (i!=pathBreakPoint)
            {
                float length = (transform.position - totalPath[i].Transform.position).magnitude;
                while (length > Speed * Time.deltaTime)
                {
                    transform.position = Vector3.MoveTowards(transform.position, totalPath[i].Transform.position, Speed * Time.deltaTime);
                    length = (transform.position - totalPath[i].Transform.position).magnitude;
                    totalPath[i].MovingObj = this;
                    yield return null;
                }
                totalPath[i].MovingObj = null;
            }



            for (int j=i;j >= 0; j--)
            {
                bool priorityPass = totalPath[j].MovingObj == null || totalPath[j].MovingObj.ID > ID;
                if (!priorityPass && totalPath[j]!=takenPoint)
                {
                    Point s = totalPath[i];
                    Point e = totalPath[0];
                    List<Point> newPath = AStar(s, e);
                    if (newPath != null)
                    {
                        pathBreakPoint = -1;
                        totalPath = newPath;
                        i = totalPath.Count - 1;
                    }
                    else
                    {
                        pathBreakPoint = j;
                    }
                    break;
                }
            }

            while (i == pathBreakPoint)
            {
                Point s = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
                Point e = totalPath[0];
                List<Point> newPath = AStar(s, e);
                if (newPath != null)
                {
                    totalPath = newPath;
                    i = totalPath.Count - 1;
                    pathBreakPoint = -1;
                }
                else
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }

    IEnumerator Coroutine_MoveRandom()
    {
        yield return null;
        while (true)
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

            Point p = freePoints[Random.Range(0, freePoints.Count)];
            start = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
            totalPath = AStar(start, p);
            while (totalPath == null) 
            {
                p = freePoints[Random.Range(0, freePoints.Count)];
                start = WorldManager.Instance.GetClosestPointWorldSpace(transform.position);
                totalPath = AStar(start, p);
                yield return null;
            }
            yield return StartCoroutine(Coroutine_CharacterFollowPath());

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
            Debug.DrawLine(totalPath[j + 1].Transform.position, totalPath[j].Transform.position, color,1);
        }
    }
}
