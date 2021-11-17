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

    public void AddMovingData(AStarAgent obj, float time,bool stationary=false)
    {
        if (MovingData == null)
        {
            MovingData = new List<MovingData>();
        }
        MovingData existing = MovingData.Find(x => x.MovingObj == obj);
        if (existing == null)
        {
            MovingData.Add(new MovingData() { MovingObj = obj, TimeToReach = time, TimeStarted = Time.time,Stationary=stationary });
        }
        else
        {
            existing.TimeStarted = Time.time;
            existing.TimeToReach = time;
            existing.Stationary = stationary;
        }
    }

    public void RemoveMovingData(CharacterMoveControl obj)
    {
        MovingData.Remove(MovingData.Find(x => x.MovingObj == obj));
    }

    public void CheckForIntersections()
    {
        if (MovingData != null)
        {
            List<MovingData> toRemove = new List<MovingData>();
            for (int i = 0; i < MovingData.Count; i++)
            {
                MovingData data = MovingData[i];
                for (int j = 0; j < MovingData.Count; j++)
                {
                    if (i != j)
                    {
                        MovingData data2 = MovingData[j];
                        if (data2.Stationary)
                        {
                            toRemove.Add(data);
                            break;
                        }
                        if (data.MovingObj.Priority < data2.MovingObj.Priority )
                        {
                            float ttReach = data.TrueTimeToReach();
                            float ttReach2 = data2.TrueTimeToReach();
                            if (ttReach <= 0 || ttReach2 <= 0)
                            {
                                continue;
                            }
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
            for (int i = 0; i < toRemove.Count; i++)
            {
                MovingData.Remove(toRemove[i]);
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                toRemove[i].MovingObj.RePath();
            }
        }
    }

    public bool CheckPointAvailability(float timeToReach, int priority)
    {
        bool available = true;
        if (MovingData != null)
        {
            List<MovingData> toRemove = new List<MovingData>();
            for (int i = 0; i < MovingData.Count; i++)
            {
                if (MovingData[i].Stationary)
                {
                    return false;
                }
                if (MovingData[i].MovingObj.Priority > priority)
                {
                    float ttReach = MovingData[i].TrueTimeToReach();
                    if (ttReach <= 0)
                    {
                        toRemove.Add(MovingData[i]);
                        continue;
                    }
                    float difference = Mathf.Abs(ttReach - timeToReach);
                    if (difference < distanceFactor)
                    {
                        available = false;
                        break;
                    }
                }
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                MovingData.Remove(toRemove[i]);
            }
        }
        return available;
    }
}

public class MovingData
{
    public AStarAgent MovingObj;
    public float TimeToReach;
    public float TimeStarted;
    public bool Stationary;

    public float TrueTimeToReach()
    {
        if (TimeToReach == 0)
        {
            return 0;
        }
        return Mathf.Max(TimeToReach - (Time.time - TimeStarted), 0);
    }
}