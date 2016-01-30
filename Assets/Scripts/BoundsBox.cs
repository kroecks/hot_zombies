using System;
using UnityEngine;
using System.Collections.Generic;

public class BoundsBox
{
    public BoundsBox(Vector3 start)
    {
        minVec = start;
        maxVec = start;
    }

    public Vector3 minVec = Vector3.zero;
    public Vector3 maxVec = Vector3.zero;

    public Vector3 GetCenter()
    {
        return (minVec + maxVec) / 2.0f;
    }

    public Vector3 GetMin()
    {
        return minVec;
    }

    public Vector3 GetMax()
    {
        return maxVec;
    }

    public void GrowTo(Vector3 growTo)
    {
        for (int axis = 0; axis < 3; axis++)
        {
            if (growTo[axis] > maxVec[axis])
            {
                maxVec[axis] = growTo[axis];
            }
            else if (growTo[axis] < minVec[axis])
            {
                minVec[axis] = growTo[axis];
            }
        }
    }

    public void GrowBy(Vector3 growVec)
    {
        minVec -= growVec;
        maxVec += growVec;
    }

    public void GrowBy( float growBy )
    {
        for( int axis = 0; axis < 3; axis++)
        {
            minVec[axis] -= growBy;
            maxVec[axis] += growBy;
        }
    }

    public Vector3 GetSize()
    {
        return maxVec - minVec;
    }

    public float MaxDistance()
    {
        float maxDistance = 0f;
        Vector3 size = GetSize();
        for( int axis = 0; axis < 3; axis++)
        {
            if( size[axis] > maxDistance)
            {
                maxDistance = size[axis];
            }
        }

        return maxDistance;
    }
}
