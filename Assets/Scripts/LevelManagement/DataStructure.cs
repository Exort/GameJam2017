using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int LevelNumber;
    public decimal SpeedMultiplier;
     public float PositiveWeight;
     public float NegativeWeight;
    private float Delay = 0;
    private float ObjectFilledUpTo = 0;
    public float SecondsBetweenPositiveWave = 1;
    
    public List<ObjectInstance> theObjects = new List<ObjectInstance>();

    public List<WeightedType> PositiveWaveType = new List<WeightedType>();

    public List<WeightedType> NegativeObjectType = new List<WeightedType>();
    public List<WeightedType> PositiveObjectType = new List<WeightedType>();

    public Level(int LvlNumber)
    {
        LevelNumber = LvlNumber;
        SpeedMultiplier = LevelNumber / 10;
        ShuffleObjectType();
        NegativeWeight = (1 / LevelNumber)*100;
        PositiveWeight = 100 - NegativeWeight;
    }
    private void ShuffleObjectType()
    {
        System.Random r = new System.Random();
    
        PositiveWaveType.Clear();
        PositiveWaveType.Add(new WeightedType(typeof(PositiveWave),1));

        NegativeObjectType.Clear();
        NegativeObjectType.Add(new WeightedType(typeof(NegativeWave), r.Next(10)));
        NegativeObjectType.Add(new WeightedType(typeof(FireWall), r.Next(2)*LevelNumber));

        PositiveObjectType.Clear();
        PositiveObjectType.Add(new WeightedType(typeof(PositiveWave), r.Next(10)));

    }
    private static System.Type GetTypeToSpawn(List<WeightedType> typeList)
    {
        int totalCount = 0;
        foreach(WeightedType wt in typeList)
        {
            totalCount += wt.Weight;
        }

        System.Random r = new System.Random();
        int bigbigWinner = r.Next(1, totalCount);

        System.Type retType = null;
        int currentIndex = 0;
        foreach (WeightedType wt in typeList)
        {
            currentIndex = currentIndex + wt.Weight;
            if(bigbigWinner <= currentIndex)
            {
                retType = wt.theType;
                break;
            }
        }


        return retType;

    }
    private float getDelayBetweenObject()
    {
        System.Random r = new System.Random();

        int minMSInterval = 0;
        if(LevelNumber < 3)
        {
            minMSInterval = 3000 - (LevelNumber*1000);
        }
        int maxMSInterval = 5000;
        if(LevelNumber>3)
        {
            maxMSInterval = maxMSInterval - ((LevelNumber - 3) * 500);
        }
        int found = r.Next(minMSInterval, maxMSInterval);
        return found/1000;
    }
    private void FillObjects(float secondsToFill)
    {
        List<ObjectInstance> toAdd = new List<ObjectInstance>();
        float endTime = Delay + secondsToFill;
        for (float t = Delay; t < endTime; t += SecondsBetweenPositiveWave)
        {
            System.Type tt = GetTypeToSpawn(PositiveWaveType);
            ObjectInstance oi = (ObjectInstance)System.Activator.CreateInstance(tt);
            oi.Timestamp = t;
            toAdd.Add(oi);
        }
        float currentTime = Delay + getDelayBetweenObject();
        System.Random r = new System.Random();
        while (currentTime < endTime)
        {
           int percent= r.Next(1, 100);
            System.Type toInstanciate = null;
            if(percent > PositiveWeight)
            {
                toInstanciate = GetTypeToSpawn(NegativeObjectType);
            }
            else
            {
                toInstanciate = GetTypeToSpawn(PositiveObjectType);
            }
            ObjectInstance oi = (ObjectInstance)System.Activator.CreateInstance(toInstanciate);
            oi.Timestamp = currentTime;
            toAdd.Add(oi);
            currentTime += getDelayBetweenObject();
        }



        toAdd.Sort(delegate (ObjectInstance o1, ObjectInstance o2)
       {
           return o1.Timestamp.CompareTo(o2.Timestamp);
       });
        theObjects.AddRange(toAdd);
        ObjectFilledUpTo = endTime;

    }
    public List<ObjectInstance> GetObjectsToSpawn(float delaySinceLastCall)
    {
       if(Delay + delaySinceLastCall > ObjectFilledUpTo - 30)
        {
            FillObjects(60);
        }
     //   Delay += delaySinceLastCall;
        List<ObjectInstance> ret = new List<ObjectInstance>();
        foreach(ObjectInstance o in theObjects)
        {
            if(o.Timestamp > Delay && o.Timestamp <= Delay + delaySinceLastCall)
            {
                ret.Add(o);
            }
        }
        foreach(ObjectInstance o in ret)
        {
            theObjects.Remove(o);
        }
        return ret;
    }
    
}
public class WeightedType
{
    public WeightedType(System.Type t, int w)
    {
        theType = t;
        Weight = w;
    }
    public System.Type theType;
    public int Weight;
}
public abstract class ObjectInstance
{
    public const int NbLanes = 5;
    public int PointValue = 0;
    public int LaneIndex = 0;
    public int Speed = 0;
    public bool Kill = false;
    public int SpeedEffect = 0;
    public float Timestamp = 0;
    public ObjectInstance()
    {
        SelectRandomLane();
    }
    private void SelectRandomLane()
    {
        System.Random r = new System.Random();
        LaneIndex = r.Next(0, NbLanes - 1);
    }
}
public class PositiveWave:ObjectInstance
{
    public PositiveWave()
    {
        PointValue = 1;
    }
}
public class NegativeWave:ObjectInstance
{
    public NegativeWave():base()
    {
        SpeedEffect = -1;

    }
}
public class FireWall:ObjectInstance
{
    public FireWall():base()
    {
        Kill = true;
        PointValue = 1;
    }
}