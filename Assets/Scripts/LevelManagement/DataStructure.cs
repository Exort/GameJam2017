using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public static System.Random r = new System.Random();
    public int LevelNumber;
    public float SpeedMultiplier;
     public float PositiveWeight;
     public float NegativeWeight;
    private float Delay = 0;
    private float ObjectFilledUpTo = 0;
    public int targetScore = 0;
    public float SecondsBetweenPositiveWave = 0.8f;
    
    public List<ObjectInstance> theObjects = new List<ObjectInstance>();

    public List<WeightedType> PositiveWaveType = new List<WeightedType>();

    public List<WeightedType> NegativeObjectType = new List<WeightedType>();
    public List<WeightedType> PositiveObjectType = new List<WeightedType>();
    public void NextLevel()
    {
       // if (LevelNumber != 10)
        {
            LevelNumber++;
            InitLevel();
        }
    }
    private void InitLevel()
    {
        targetScore += (20 * LevelNumber);
        SpeedMultiplier = (float)LevelNumber / (float)10.0;
        ShuffleObjectType();
        PositiveWeight = ((float)1 / (float)LevelNumber) * 100;
        NegativeWeight = 100 - PositiveWeight;
 
    }
    public Level(int LvlNumber)
    {
        LevelNumber = LvlNumber;
        InitLevel();
    }
    private void ShuffleObjectType()
    {
        PositiveWaveType.Clear();
        PositiveWaveType.Add(new WeightedType(typeof(PositiveWave), 1));

        NegativeObjectType.Clear();
        NegativeObjectType.Add(new WeightedType(typeof(NegativeWave), r.Next(1,10)));
        //NegativeObjectType.Add(new WeightedType(typeof(FireWall), r.Next(1,2)*LevelNumber));
        NegativeObjectType.Add(new WeightedType(typeof(WindowsLogo), r.Next(1, 2) * LevelNumber));
        NegativeObjectType.Add(new WeightedType(typeof(Packet), r.Next(1, 2) * LevelNumber));

        PositiveObjectType.Clear();
        PositiveObjectType.Add(new WeightedType(typeof(PositiveWave), r.Next(1,10)));
    }
    private static System.Type GetTypeToSpawn(List<WeightedType> typeList)
    {
        int totalCount = 0;
        foreach(WeightedType wt in typeList)
        {
            totalCount += wt.Weight;
        }

        if(totalCount < 1)
        {
            totalCount = 1;
        }
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
        if(retType==null)
        {
            Debug.Log("Nope");
        }

        return retType;

    }
    private float getSpeedMultiplier(int currentLevel)
    {
        float max = (float)currentLevel * 1.5f*100;
        float min = (float)1/currentLevel * 100;
        float ret = r.Next((int)(min), (int)max);
        return ret / 100;
    }
    private float getDelayBetweenObject()
    {
      
        int minMSInterval = 0;
        if(LevelNumber < 3)
        {
            minMSInterval = 3000 - (LevelNumber*1000);
        }
        if(minMSInterval < 0 )
        {
            minMSInterval = 0;
        }
        int maxMSInterval = 2000;
        if(LevelNumber>3)
        {
            maxMSInterval = maxMSInterval - ((LevelNumber - 3) * 500);
           
        }
        if (maxMSInterval < 100)
        {
            maxMSInterval =100;
        }
        int found = r.Next(minMSInterval, maxMSInterval);
        return (float)found/(float)1000;
    }
    
    private void FillObjects(float secondsToFill)
    {

        string log = "";
       
        List<ObjectInstance> toAdd = new List<ObjectInstance>();
        float endTime = ObjectFilledUpTo + secondsToFill;
        log += "Generating objects between " + ObjectFilledUpTo + " and " + endTime + "\r\n";
        log += "\r\n";
        log += "Generating Positive waves  \r\n";
        for (float t = ObjectFilledUpTo; t < endTime; t += SecondsBetweenPositiveWave)
        {
            System.Type tt = GetTypeToSpawn(PositiveWaveType);
            
            ObjectInstance oi = (ObjectInstance)System.Activator.CreateInstance(tt);
            oi.Timestamp = t;
            oi.Speed = getSpeedMultiplier(LevelNumber);
            log += "Type : " + tt.ToString() + "\r\n";
            log += "Lane : " + oi.LaneIndex.ToString() + "\r\n";
            log += "Time : " + t.ToString() + "\r\n";
            log += "\r\n";
            toAdd.Add(oi);
        }
        log += "Generating negative wave\r\n";
        float currentTime = ObjectFilledUpTo + getDelayBetweenObject();
      
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
                /*Should be positive...*/
                toInstanciate = GetTypeToSpawn(NegativeObjectType);
            }
            if(toInstanciate == null)
            {
                Debug.Log("test");
            }
            ObjectInstance oi = (ObjectInstance)System.Activator.CreateInstance(toInstanciate);
            oi.Timestamp = currentTime;
            oi.Speed = getSpeedMultiplier(LevelNumber);
            toAdd.Add(oi);

            log += "Type : " + toInstanciate.ToString() + "\r\n";
            log += "Lane : " + oi.LaneIndex.ToString() + "\r\n";
            log += "Time : " + currentTime.ToString() + "\r\n";
            log += "\r\n";
            currentTime += getDelayBetweenObject();
        }
    

        
        toAdd.Sort(delegate (ObjectInstance o1, ObjectInstance o2)
       {
           return o1.Timestamp.CompareTo(o2.Timestamp);
       });
        theObjects.AddRange(toAdd);
        Debug.Log(log);
        ObjectFilledUpTo = endTime;

    }
    public List<ObjectInstance> GetObjectsToSpawn(float delaySinceLastCall)
    {
       
        if (Delay + delaySinceLastCall > ObjectFilledUpTo - 2)
        {
            FillObjects(4);
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
        Delay += delaySinceLastCall;
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
    public float Speed = 0;
    public bool Kill = false;
   
    public float Timestamp = 0;
    public ObjectInstance()
    {
        SelectRandomLane();
    }
    private void SelectRandomLane()
    {
      
        LaneIndex = Level.r.Next(0, NbLanes );
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

public class WindowsLogo : ObjectInstance
{
}

public class Packet : ObjectInstance
{ }