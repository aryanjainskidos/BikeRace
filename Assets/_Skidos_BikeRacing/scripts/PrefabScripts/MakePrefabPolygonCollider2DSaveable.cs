namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using SimpleJSON;


/**
 * Ká jau zináms, prefabiem tiek seivots tikai "transform" komponents
 * śis skripts, kad iedots prefabam, seivo/ieládé arí PolygonCollider2D komponentu
 */
public class MakePrefabPolygonCollider2DSaveable : MonoBehaviour, ISaveable
{

    public void Load(JSONNode N)
    {
        DestroyImmediate(transform.GetComponent<PolygonCollider2D>()); //iznícinu prefabam piederośo kolaideri, jo deserializétájs izveidos jaunu 
        GameObject go = gameObject;
        N["componentName"] = "UnityEngine.PolygonCollider2D"; //JSONá komponents saucás "MakePrefabPolygonCollider2DSaveable", bet ir jápársauc par "UnityEngine.PolygonCollider2D" - lai deseriaizétájs to atpazítu ká poligona kolaideri
        LevelManager.DeserializeComponent(ref go, N); //ĺauju, lai LevelManager komponentu deserializétájs deserializé manu komponentu - DRY!
    }

    public JSONClass Save()
    {
        PolygonCollider2D collider = transform.GetComponent<PolygonCollider2D>();
        if (collider == null)
        {
            Debug.LogWarning("Hei, šim prefabam nemaz nav PolygonCollider2D !");
            return null;
        }
        return LevelManager.SerializeComponent(collider, gameObject); //padodu komponentu LevelManager komponentu serializétájam - DRY!		
    }

}




}
