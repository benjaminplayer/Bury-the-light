using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int secretsFound;
    public Dictionary<string, bool> fragmentsCollected;
    // when new game -> high score = 0;
    public GameData()
    {
        this.secretsFound = 0;
        fragmentsCollected = new Dictionary<string, bool>();
    }

}
