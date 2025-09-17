namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelDataContainer : ScriptableObject
{

    [Header("Ierakstiem ir jābūt alfabētiskā secībā (1,3,2 nestrādās!)")]
    public List<LevelDataEntry> levelDataEntries;

}

}
