using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLevelData
{
    public int level;
    public bool isStarCleared;
    public int numLeastRopeUsedToClear;
    public float shortestSecondsTakenToClear;

    public UserLevelData(int level, bool isStarCleared, int numLeastRopeUsedToClear, float shortestSecondsTakenToClear)
    {
        this.level = level;
        this.isStarCleared = isStarCleared;
        this.numLeastRopeUsedToClear = numLeastRopeUsedToClear;
        this.shortestSecondsTakenToClear = shortestSecondsTakenToClear;
    }
}
