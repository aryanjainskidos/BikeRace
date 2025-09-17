namespace vasundharabikeracing
{
    using UnityEngine;
    using System.Collections;
    using SimpleJSON;

    public interface ISaveable
    {

        void Load(JSONNode node);
        JSONClass Save();

    }

}
