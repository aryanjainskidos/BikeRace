namespace vasundharabikeracing {
[System.Serializable]
public class LevelDataEntry
{

    public string name;
    public int starsToUnlock;
    public int mpWinsToUnlock;

    public static int Compare(LevelDataEntry x, LevelDataEntry y)
    {
        return x.name.CompareTo(y.name);
    }
}

}
