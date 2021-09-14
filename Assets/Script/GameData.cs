using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float highScoreSaved;

    public GameData (SpawnBlocks controller)
    {
        if (highScoreSaved < controller.highScore)
            highScoreSaved = controller.highScore;
        else if(highScoreSaved > controller.highScore)
            highScoreSaved = highScoreSaved;
    }
}
