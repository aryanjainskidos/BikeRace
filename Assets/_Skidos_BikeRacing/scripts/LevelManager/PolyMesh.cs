namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;



[RequireComponent(typeof(MeshFilter))]
public class PolyMesh : MonoBehaviour, ISaveable
{
    public List<Vector3> keyPoints = new List<Vector3>();
    public List<Vector3> curvePoints = new List<Vector3>();
    public List<bool> isCurve = new List<bool>();
    public List<bool> isFoliage = new List<bool>();
    public EdgeCollider2D collider2d;
    [Range(0.01f, 1)]
    public float curveDetail = 0.1f;
    public float colliderDepth = 1;
    public bool buildColliderEdges = true;
    public bool buildColliderFront;
    public Vector2 uvPosition;
    public float uvScale = 1;
    public float uvRotation;
    public float foliageThickness = 0.1f;
    public float foliageThicknessAltPercentageA = 0.5f; // % no foliageThickness - cik tuvu pie zemes pietuvináties mekléjot pieskares (nepára pieskares bús tik tuvu | pára pieskares bús 1.0 foliageThickness attaĺumá no zemes)
    public float foliageThicknessAltPercentageB = 0.2f; // % no foliageThickness - cik tuvu ĺaut pietuvináties pie zemes pieskares viduspunktam
    public enum FoliageTypes
    { // tipi ir prefabi resources/visuals/foliage direktorijá; nosakumam enumá ir jasakrít ar prefaba nosaukumu + katram prefabam ir nepiecieśams táda paśa nosaukuma matereáls ar pareizo 2 krásu śeideri
        grass,
        jungleGrass,
        jungleGrassRain,
        sand,
        snow,
        mountainGrass
    }
    public FoliageTypes foliageType = FoliageTypes.grass;
    public float shadingBeltThickness = 0.5f;
    public bool generateCollider = false;
    public string orderLayerName = "Default";
    public int orderInLayer = 0;



    public List<Vector3> GetEdgePoints()
    {
        //Build the point list and calculate curves
        var points = new List<Vector3>();
        for (int i = 0; i < keyPoints.Count; i++)
        {
            if (isCurve[i])
            {
                //Get the curve control point
                var a = keyPoints[i];
                var c = keyPoints[(i + 1) % keyPoints.Count];
                var b = Bezier1.Control(a, c, curvePoints[i]);

                //Build the curve
                var count = Mathf.Ceil(1 / curveDetail);
                for (int j = 0; j < count; j++)
                {
                    var t = (float)j / count;
                    points.Add(Bezier1.Curve(a, b, c, t));
                }
            }
            else
                points.Add(keyPoints[i]);
        }
        return points;
    }

    public void BuildMesh(bool rebuildChildren)
    {

        var points = GetEdgePoints();
        var vertices = points.ToArray();

        //Build the index array
        var indices = new List<int>();
        while (indices.Count < points.Count)
            indices.Add(indices.Count);

        //Build the triangle array
        var triangles = Triangulate.Points(points);

        //Build the uv array
        var scale = uvScale != 0 ? (1 / uvScale) : 0;
        var matrix = Matrix4x4.TRS(-uvPosition, Quaternion.Euler(0, 0, uvRotation), new Vector3(scale, scale, 1));
        var uv = new Vector2[points.Count];
        for (int i = 0; i < uv.Length; i++)
        {
            var p = matrix.MultiplyPoint(points[i]);
            uv[i] = new Vector2(p.x, p.y);
        }

        //Find the mesh (create it if it doesn't exist)
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        var mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "PolyMesh-Mesh";
            meshFilter.mesh = mesh;
        }

        //Update the mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        ;

        if (rebuildChildren)
        {
            UpdateCollider(points, triangles);
            RebuildFoliage();
        }
        GetComponent<Renderer>().sortingLayerName = orderLayerName;
        GetComponent<Renderer>().sortingOrder = orderInLayer;
    }


    void UpdateCollider(List<Vector3> points, int[] tris)
    {

        foreach (Transform child in transform)
        {
            if (child.name.Length >= 10 && child.name.Substring(0, 10) == "2dCollider")
            {
                DestroyImmediate(child.gameObject);//iznícinu visus(!) iepriekśéjos 2d kolaiderus (daźreiz káds suśḱis aizḱerás)
            }
        }

        if (generateCollider && collider2d == null)
        { //vajag kolaideru, bet táds nepastáv
            GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("2dCollider-ground") as GameObject;
            Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + prefab);
            //GameObject prefab = Resources.Load("Prefabs/PolyMeshComponents/2dCollider-ground") as GameObject;
            GameObject collider2dGO = Instantiate(prefab) as GameObject;
            collider2dGO.name = "2dCollider";
            collider2dGO.tag = "do-not-save";
            collider2dGO.transform.parent = transform;
            collider2dGO.transform.localPosition = Vector3.zero;
            collider2dGO.transform.localScale = Vector3.one; //bérns tiek skeilots automátiski, kad skeilo parentu, lai bérns necínítos pretí skeilośanai - vińam vajag noresetot paśa izméru
            collider2d = collider2dGO.GetComponent<EdgeCollider2D>(); //objektá pieglabáju śo jaunizveidoto kolaideri	(patiesíbá vińa komponenti - Edge kolaideri)
        }



        if (collider2d != null)
        {
            var vertices = new List<Vector2>();
            for (int i = 0; i < points.Count; i++)
            {
                vertices.Add(points[i]);
            }
            vertices.Add(points[0]); //+ no pédéjá uz pirmo (citádi milzígs caurums)
            collider2d.points = vertices.ToArray();
        }



    }

    bool IsRightTurn(List<Vector3> points, int a, int b, int c)
    {
        var ab = points[b] - points[a];
        var bc = points[c] - points[b];
        return (ab.x * bc.y - ab.y * bc.x) < 0;
    }

    bool IntersectsExistingLines(List<Vector3> points, Vector3 a, Vector3 b)
    {
        for (int i = 0; i < points.Count; i++)
            if (LinesIntersect(points, a, b, points[i], points[(i + 1) % points.Count]))
                return true;
        return false;
    }

    bool LinesIntersect(List<Vector3> points, Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        if (point1 == point3 || point1 == point4 || point2 == point3 || point2 == point4)
            return false;

        float ua = (point4.x - point3.x) * (point1.y - point3.y) - (point4.y - point3.y) * (point1.x - point3.x);
        float ub = (point2.x - point1.x) * (point1.y - point3.y) - (point2.y - point1.y) * (point1.x - point3.x);
        float denominator = (point4.y - point3.y) * (point2.x - point1.x) - (point4.x - point3.x) * (point2.y - point1.y);

        if (Mathf.Abs(denominator) <= 0.00001f)
        {
            if (Mathf.Abs(ua) <= 0.00001f && Mathf.Abs(ub) <= 0.00001f)
                return true;
        }
        else
        {
            ua /= denominator;
            ub /= denominator;

            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                return true;
        }

        return false;
    }

    /**
     * foliage -- sniegs vai zále virs objekta
     */
    public void RebuildFoliage()
    {
        bool drawFoliage = true;
        bool drawShadingBelt = true;


        for (int i = 0; i < 2; i++) //daru 2x, cos bug or smnt, ar 1 reizi nepietiek :(
        {
            foreach (Transform child in transform)
            {
                if (child.name.Length >= 7 && child.name.Substring(0, 7) == "foliage")
                {
                    DestroyImmediate(child.gameObject);//iznícinu visus(!) iepriekśéjos foliage objektus
                }
                else if (child.name.Length >= 7 && child.name.Substring(0, 7) == "shading")
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        if (foliageThickness < 0.01)
        {
            drawFoliage = false;
        }

        if (shadingBeltThickness < 0.01)
        {
            drawShadingBelt = false;
        }

        if (!drawFoliage && !drawShadingBelt)
        {//ejam májás, ja nevajag nevienu no śiem
            return;
        }

        float foliageOffsetZ = -0.5f;
        //float z = foliageOffsetZ + transform.position.z; //globálajás koordinátés
        List<Vector3> terrainEdges = GetEdgePoints();





        Dictionary<Vector3, Vector3> edgeNormales = new Dictionary<Vector3, Vector3>();  //katram punktam piederošā šķautnes normāli (škautni veido šis punkts (dict key) un nākamais punkts terrainEdges sarakstā)

        //katram punktam iedod viņa blakus esošās šķautnes iekšupvērsto normāli
        for (int i = 0; i < terrainEdges.Count; i++)
        {

            int a, b; //apskatámie punkti, kas veido zemes śḱautni

            if (i < terrainEdges.Count - 1)
            { //normála virsotnes - ies párí ar nákamo
                a = i;
                b = i + 1;
            }
            else
            { //apskatam pédéjo virsotni, tá ies párí ar pirmo
                a = i;
                b = 0;
            }
            float dx = terrainEdges[b].x - terrainEdges[a].x;
            float dy = terrainEdges[b].y - terrainEdges[a].y;
            Vector3 normalInLocal = new Vector3(dy, -dx, 0);
            normalInLocal.Normalize(); //normáles garums ir atkarígs no sḱautnes garuma, tas neder
            edgeNormales[terrainEdges[a]] = normalInLocal;

        }

        //float correction = Random.Range(0.5f,1f); //cik % biezu foliage taisít
        //float speed = 0.5f; //tik daudz foliage biezums uz katru metru samazinásies
        //int direction = -1;//foliage biezums samazinásies vai pieaugs

        Dictionary<Vector3, Vector3> pointNormalesF = new Dictionary<Vector3, Vector3>();  //katra punkta normāli (vidējā no blakus esošo šķautņu normālēm) - pielágotas foliage biezumam
        Dictionary<Vector3, Vector3> pointNormalesSB = new Dictionary<Vector3, Vector3>();  //ShadingBelt biezumá

        //katru normāli izlīdzina (AVG) ar blakus esošo normāli
        for (int i = 0; i < terrainEdges.Count; i++)
        {
            int a, b;
            if (i < terrainEdges.Count - 1)
            {
                a = i;
                b = i + 1;
            }
            else
            {
                a = i;
                b = 0;
            }

            Vector3 avg = (edgeNormales[terrainEdges[a]] + edgeNormales[terrainEdges[b]]) * 0.5f;
            avg.Normalize();

            pointNormalesF[terrainEdges[b]] = new Vector3(avg.x, avg.y, 0) * foliageThickness;
            pointNormalesSB[terrainEdges[b]] = new Vector3(avg.x, avg.y, 0) * shadingBeltThickness;

        }
        edgeNormales = null;



        //pārbaudīšu visu punktu normāles - vai tās nesaskaras ar blakus esošajām, ja saskaras - abas saísináśu - milimetru ísákas par saskarśanás punktu (@todo -- varétu nevis ísinát, bat abas likvidét un izveidot vienu jaunu)
        if (drawFoliage)
        {
            for (int k = 0; k <= 3; k++)
            {
                int numConflicts = 0;
                bool conflict = false;

                for (int i = 0; i < terrainEdges.Count; i++)
                {
                    for (int j = 0; j < terrainEdges.Count; j++)
                    {

                        if (i == j)
                        {
                            continue;
                        }

                        Vector3 a1 = terrainEdges[i] + transform.position; //zemītes punkts A
                        Vector3 a2 = a1 + pointNormalesF[terrainEdges[i]]; //zemītes punkta A normāle - zemītes iekšienē esošais punkts
                        Vector3 b1 = terrainEdges[j] + transform.position;
                        Vector3 b2 = b1 + pointNormalesF[terrainEdges[j]];

                        // Debug.DrawLine(a1, a2, Color.red, 2f);

                        Vector3 X = Vector3.zero; //normáĺu krustpunkts
                        if (LineIntersection(a1, a2, b1, b2, ref X))
                        { //nákamá normále
                          //Debug.Log(i + " punkta normāle saskarās ar nákamo  " + j + " punkta normāli X=" + X);
                            numConflicts++;

                            //abas normáles tagad beidzas viená punktá 
                            pointNormalesF[terrainEdges[i]] = X - transform.position - terrainEdges[i];

                            pointNormalesF[terrainEdges[j]] = X - transform.position - terrainEdges[j];


                            //abas normáles padaru par dźifiju ísákas, lai nebutu párkláśanás draudi (tehniski: pastumju beigu punktu(normáli) tuvák sákumpunktam (zemítes punktam)  )
                            pointNormalesF[terrainEdges[i]] = Vector3.MoveTowards(pointNormalesF[terrainEdges[i]], Vector3.zero, 0.001f);
                            pointNormalesF[terrainEdges[j]] = Vector3.MoveTowards(pointNormalesF[terrainEdges[j]], Vector3.zero, 0.001f);

                            conflict = true;
                        }
                    }
                }
                if (!conflict)
                {
                    //Debug.Log("no more conflicts, moving on");
                    break;
                }

            }
        }


        //NON-DRY - tieśi tas pats shadingBelt normálém
        if (drawShadingBelt)
        {
            for (int k = 0; k <= 3; k++)
            {
                int numConflicts = 0;
                bool conflict = false;
                for (int i = 0; i < terrainEdges.Count; i++)
                {
                    for (int j = 0; j < terrainEdges.Count; j++)
                    {
                        if (i == j) { continue; }
                        Vector3 a1 = terrainEdges[i] + transform.position;
                        Vector3 a2 = a1 + pointNormalesSB[terrainEdges[i]];
                        Vector3 b1 = terrainEdges[j] + transform.position;
                        Vector3 b2 = b1 + pointNormalesSB[terrainEdges[j]];
                        Vector3 X = Vector3.zero;
                        if (LineIntersection(a1, a2, b1, b2, ref X))
                        {
                            numConflicts++;
                            pointNormalesSB[terrainEdges[i]] = X - transform.position - terrainEdges[i];
                            pointNormalesSB[terrainEdges[j]] = X - transform.position - terrainEdges[j];
                            pointNormalesSB[terrainEdges[i]] = Vector3.MoveTowards(pointNormalesSB[terrainEdges[i]], Vector3.zero, 0.001f);
                            pointNormalesSB[terrainEdges[j]] = Vector3.MoveTowards(pointNormalesSB[terrainEdges[j]], Vector3.zero, 0.001f);
                            conflict = true;
                        }
                    }
                }
                if (!conflict) { break; }
            }
        }


        //vél joka péc párbaudíśu vai normáles nelien árá no ǵeometrijas, arí tad jásaísina
        for (int i = 0; i < terrainEdges.Count; i++)
        {
            //print("vai normále " +terrainEdges[i] + " -> " +  (terrainEdges[i] +  pointNormalesF[terrainEdges[i]] )  + " neśkérso .." );

            for (int j = 1; j <= terrainEdges.Count; j++)
            {


                int j_0 = j - 1;
                int j_1 = j;

                if (j == terrainEdges.Count)
                { //pédéjais un pirmais punkts
                    j_0 = j - 1;
                    j_1 = 0;
                }

                if (i == j_0 || i == j_1)
                {
                    continue; //neapskatu malu no kuras sákas pati normále
                }


                //print(" .. malu " + terrainEdges[j-1] + " -> " + terrainEdges[j] ); 

                Vector3 a1 = terrainEdges[i] + transform.position; //zemītes punkts A
                Vector3 a2 = a1 + pointNormalesF[terrainEdges[i]]; //zemītes punkta A normāle - zemītes iekšienē esošais punkts
                Vector3 b = terrainEdges[j_0] + transform.position; //malas punkts B
                Vector3 c = terrainEdges[j_1] + transform.position; // malas punkts C

                Vector3 X = Vector3.zero; //normáĺu krustpunkts
                                          //Vector3 plus = new Vector3(0,0,-1.1f);
                if (LineIntersection(a1, a2, b, c, ref X))
                {
                    //Debug.DrawLine(a1+plus, a2+plus, Color.cyan, 5f);
                    //Debug.DrawLine(b+plus, c+plus, Color.cyan, 5f);
                    //Debug.DrawLine(X+plus + new Vector3(0.01f,0.01f,0), X+plus + new Vector3(-0.01f,-0.01f,0), Color.yellow, 5f);
                    //Debug.DrawLine(X+plus + new Vector3(-0.01f,0.01f,0), X+plus + new Vector3(0.01f,-0.01f,0), Color.yellow, 5f);					
                    float distanceOverTheEdge = Vector3.Distance(X, a2) + 0.01f;
                    //print("got it: " + X + "  d:" + distanceOverTheEdge);
                    pointNormalesF[terrainEdges[i]] = Vector3.MoveTowards(pointNormalesF[terrainEdges[i]], Vector3.zero, distanceOverTheEdge); // saísinu normáli, lídz tá ir nedaudz iekśá objektá
                }
            }
        }


        //NON-DRY - tas pats shader belt normálém
        for (int i = 0; i < terrainEdges.Count; i++)
        {
            for (int j = 1; j <= terrainEdges.Count; j++)
            {
                int j_0 = j - 1;
                int j_1 = j;
                if (j == terrainEdges.Count) { j_0 = j - 1; j_1 = 0; }
                if (i == j_0 || i == j_1) { continue; }
                Vector3 a1 = terrainEdges[i] + transform.position;
                Vector3 a2 = a1 + pointNormalesSB[terrainEdges[i]];
                Vector3 b = terrainEdges[j_0] + transform.position;
                Vector3 c = terrainEdges[j_1] + transform.position;
                Vector3 X = Vector3.zero;
                if (LineIntersection(a1, a2, b, c, ref X))
                {
                    float distanceOverTheEdge = Vector3.Distance(X, a2) + 0.01f;
                    pointNormalesSB[terrainEdges[i]] = Vector3.MoveTowards(pointNormalesSB[terrainEdges[i]], Vector3.zero, distanceOverTheEdge);
                }
            }
        }





        /*#if UNITY_EDITOR
        //tikai normāļu zīmēšanai

		if(drawFoliage){
	        for (int i = 0; i < terrainEdges.Count; i++) {
	            int a;//, b;
	            if (i < pointNormalesF.Count - 1) { //parasts punkts
	                a = i;
	            } else { //pēdējais punkts
	                a = i;
	            }

	            Vector3 a1 = terrainEdges[a] + transform.position + new Vector3(0, 0, -0.61f);
	            Vector3 a2 = a1 + pointNormalesSB[terrainEdges[a]];

	            Debug.DrawLine(a1, a2, Color.red, 3);
	        }
		}
		#endif
		*/

        //izveidoju lookuptable punkts => vai vajag taisít foliage

        Dictionary<Vector3, bool> foliageNeededAtThisPoint = new Dictionary<Vector3, bool>();
        if (drawFoliage)
        {
            try
            {
                for (int i = 0; i < keyPoints.Count; i++)
                {
                    foliageNeededAtThisPoint[keyPoints[i]] = isFoliage[i];
                    //Debug.Log(i + " set needed " + keyPoints[i] + " " + isFoliage[i]);
                }
            }
            catch /*(System.Exception e)*/
            {
                //śis izńémums ir veciem objektiem, kas radíti pirms isFoliage datu struktúras				
                //Debug.Log("prepopuléju isFoliage");

                isFoliage = new List<bool>();
                for (int i = 0; i < keyPoints.Count; i++)
                {
                    isFoliage.Add(true);
                }
            }
        }



        /**
         * mekléśu pieskares, kas izlidzinás foliage apakśéjo líniju
         * pieskare: 
         * 	 atrast táláko normáli B, lídz kurai punkta A var novilkt nogriezni, kurś krusto visas pa visu esośás normáles
         */
        float minThickness = foliageThicknessAltPercentageB * foliageThickness;
        Color32 fColorUp = new Color32(255, 255, 255, 0);
        Color32 fColorDown = new Color32(0, 0, 0, 0);

        List<List<Vector3>> foliages = new List<List<Vector3>>(); //saraksts ar sniedzińiem (viens sniedzíńś ir saraksts ar punktiem - sniedzińa virsmu)
        List<List<Vector3>> fTangents = new List<List<Vector3>>(); //saraksts ar (pieskarém (pieskare ir saraksts ar punktiem))  <= katram foliage objektam pa vienam
        List<List<Vector3>> fTerrainEdges = new List<List<Vector3>>(); // saraksts ar (zemítes punktu sarakstiem, kas pieder katram no foliage objektiem) <= katram foliage objektam pa vienam
        List<List<Color32>> fColors = new List<List<Color32>>(); //saraksts ar krásu sarakstiem - katram foliage punktam, pa sarakstam, kas saka káda krása vińa virsotni krásot (diferencé augśéjás no apakśéjám)

        bool fNeeded = false;
        bool fStarted = false;
        int foliageNum = 0; //foliage objektu skaits
        int addFakeToThisFoliage = -1; //hakińś foliage objektam, kas robeźosies ar pirmo punktu; te ieliks, kuram "foliageNum" ir tas jádara (ja vispár)

        if (drawFoliage)
        {

            foliages.Add(new List<Vector3>());
            fTangents.Add(new List<Vector3>());//sagatavojos 1. foliage objektam - vińa pieskaru liste ..
            fTerrainEdges.Add(new List<Vector3>());// .. un vińa zemítes punktu liste
            fColors.Add(new List<Color32>());

            //jáatrod cik neatkarígu foliage objektu bús, katram bus savs pieskaru saraksts 


            /**
	         * atradíśus visus foliage objektus 
	         * katram objektam iedośu vińam piederośos zemítes punktus (vélák tiem tiks pielikti klát dziĺie punkti - izveidojot noslégtu líniju)
	         * 
	         */
            for (int i = 0; i < terrainEdges.Count; i++)
            {

                /**
	             * Vai šim punktam ir jāzīmē foliage
	             * keypointiem ir pierakstīts vai tam ir jāzīmē
	             * bezjē punktiem tas nav pierakstīts (viņiem nosaka vai iepriekšējam keypointam bija jāzīmē)
	             */
                if (foliageNeededAtThisPoint.ContainsKey(terrainEdges[i]))
                {
                    fNeeded = foliageNeededAtThisPoint[terrainEdges[i]];
                }

                if (fNeeded)
                {
                    foliages[foliageNum].Add(terrainEdges[i]);
                    fTerrainEdges[foliageNum].Add(terrainEdges[i]);
                    fColors[foliageNum].Add(fColorUp);

                    /**
	                 * ja foliage satur pēdējo punktu, tad vajag manuāli piešaut klāt arī pirmo punktu (bet ne īsti pirmo punktu - nedaudz nostāk (citādi zīmētājs atsakās zīmēt - overlapping n shit) ) 
	                 * citādi pietrūkst viena šķautne                      
	                 */
                    if (i == terrainEdges.Count - 1 && foliageNeededAtThisPoint[terrainEdges[0]])
                    {
                        Vector3 fakeFirstPoint = terrainEdges[0] + new Vector3(0.001f, 0.001f, 0);
                        foliages[foliageNum].Add(fakeFirstPoint); //vél vajag vienu piefeikot zíméjot foliage dziĺos punktus
                        fColors[foliageNum].Add(fColorUp);

                        //bet, šim feikajam punktam nav piekošota normāle, arī to ir jānofeiko
                        pointNormalesF[fakeFirstPoint] = pointNormalesF[terrainEdges[0]] + new Vector3(0.001f, 0.001f, 0);
                        addFakeToThisFoliage = foliageNum;


                    }

                    fStarted = true;
                }
                else
                {
                    if (fStarted)
                    {
                        //konstatēts pārtraukums
                        //taisam nākamo foliage objektu
                        foliageNum++;
                        foliages.Add(new List<Vector3>());
                        fTangents.Add(new List<Vector3>());
                        fTerrainEdges.Add(new List<Vector3>());
                        fColors.Add(new List<Color32>());
                        fStarted = false;
                    }
                }
            }
        }

        if (drawFoliage)
        {
            for (int fol = 0; fol <= foliageNum; fol++)
            { //ikviens foliage objekts @note -- iteréjoties jálieto "<= fNum", jo iespéjams valíds stávoklis, kad fNum == 0 !

                if (fTerrainEdges[fol].Count < 2)
                {
                    continue; //skipo tukśus foliage objektus
                }


                int tNum = 0; //atrasto pieskaru skaits
                Vector3 a1;//punkts A, pieskares sákumpunkts

                if ((tNum % 2) == 0)
                { // pára/nepára skaitlis:  ńem normáles beigu punktu (tálák no zemites) vai normáles punktu pie zemítes
                    a1 = fTerrainEdges[fol][0] + transform.position + pointNormalesF[fTerrainEdges[fol][0]];
                }
                else
                {
                    a1 = fTerrainEdges[fol][0] + transform.position + (pointNormalesF[fTerrainEdges[fol][0]] * foliageThicknessAltPercentageA);
                }

                fTangents[fol].Add(a1 - transform.position);//sarakstá glabáju bez objekta pozícijas, réḱinot vajag
                int tEnd = 0; //pieskares tálákais derígais galapunkts
                int tStart = 0; //apskatámás pieskares sakumpunkts
                bool done = false;
                while (!done)
                {
                    for (int t = tStart + 1; t < fTerrainEdges[fol].Count; t++)
                    {// "t" => tangent;  potenciálie pieskares beigupunkti

                        Vector3 a2;//potenciálais pieskares beigupunkts
                        if ((tNum % 2) == 0)
                        { // pára/nepára skaitlis:  ńem normáles beigu punktu (tálák no zemites) vai normáles punktu pie zemítes
                            a2 = fTerrainEdges[fol][t] + transform.position + pointNormalesF[fTerrainEdges[fol][t]];
                        }
                        else
                        {
                            a2 = fTerrainEdges[fol][t] + transform.position + (pointNormalesF[fTerrainEdges[fol][t]] * foliageThicknessAltPercentageA);
                        }
                        a2 = ExtendLineSegment(a1, a2, 0.001f); //pabídu a2 punktu mazliet tálák (pagarinu meklétájpieskari) jo daźreiz "T" veida krustojumos netiek noḱerta saskárśanás


                        //vai potenciálá pieskare śḱérso visas normales 1..t ? 
                        bool ok = true;
                        for (int o = tStart + 1; o <= t; o++)
                        {

                            Vector3 b1 = fTerrainEdges[fol][o] + transform.position; //b1->b2 normále, ko jáśḱérso
                            Vector3 b2 = b1 + pointNormalesF[fTerrainEdges[fol][o]];
                            b2 = ExtendLineSegment(b1, b2, 0.001f); //pabídu b2 punktu mazliet tálák (pagarinu normáli) jo daźreiz "T" veida krustojumos netiek noḱerta saskárśanás


                            Vector3 crossPoint = Vector3.zero;
                            if (LineIntersection(a1, a2, b1, b2, ref crossPoint))
                            { //śḱérso normáli

                                if (Vector3.Distance(fTerrainEdges[fol][o] + transform.position, crossPoint) > minThickness)
                                { //un normáles śḱérsośanas punkts ir pietiekami tálu no zemítes malas (jáievéro min. foliage biezums)
                                  //print(tNum +  ". ok, "+ t + "-> pieskare pagaidám ir ok " + o + ". punkts sasniedzams. ");							
                                }
                                else
                                {
                                    //print(tNum +  ". ok, "+ t + "-> pieskare pagaidám ir ok " + o + ". punkts sasniedzams. betpar tuvu zemítei"  );
                                    ok = false;
                                    break;
                                }

                                if (o >= fTerrainEdges[fol].Count - 1)
                                {
                                    //print("sasniegts pédéjais punkts, ejam májás");
                                    done = true;
                                    break;
                                }
                            }
                            else
                            {
                                //print(tNum +  ". Viss, "+ t + "-> pieskare nav laba, jo " + o + ". punks ir nesasniedzams" );
                                ok = false;
                                break;
                            }

                        }

                        if (ok)
                        {
                            //print(tNum +  ". pieskare ->"+ tStart + " ("  + t + ") izpétíta un ir laba");
                            tEnd = t;
                        }
                        else
                        {
                            //print(tNum +  ". pieskare ->" + tStart + " ("  + t + ") izrádíjás nederíga");
                            break;
                        }

                    }

                    //sagatavojos nákamajai iterácijai				
                    //atrastais pieskares beigu punkts tagad kĺúst par nákamás sákumpunktu
                    if ((tNum % 2) == 0)
                    {
                        a1 = fTerrainEdges[fol][tEnd] + transform.position + (pointNormalesF[fTerrainEdges[fol][tEnd]]);
                    }
                    else
                    {
                        a1 = fTerrainEdges[fol][tEnd] + transform.position + (pointNormalesF[fTerrainEdges[fol][tEnd]] * 0.5f);
                    }
                    fTangents[fol].Add(a1 - transform.position);//sarakstá glabáju bez objekta pozícijas, réḱinot vajag
                    tStart = tEnd;
                    tEnd++;
                    tNum++;

                    if (tEnd >= fTerrainEdges[fol].Count - 1)
                    {
                        //print("dívainá veidá sasniegts pédéjais punkts, pietiek");
                        done = true;
                    }
                }
            }
            fTerrainEdges = null;
        }


        /*#if UNITY_EDITOR
        //zíméju pieskares 
		if(drawFoliage){
	        for (int fol = 0; fol <= foliageNum; fol++) { //ikviens foliage objekts
	            for (int i = 0; i < fTangents[fol].Count - 1; i++) {
	                Color c = Color.green;
	                Debug.DrawLine(fTangents[fol][i] + new Vector3(0, 0, -1.3f) + transform.position, fTangents[fol][i + 1] + new Vector3(0, 0, -1.3f) + transform.position, c, 3);
	            }
	        }
		}
		#endif*/

        if (drawFoliage)
        {
            for (int fol = 0; fol <= foliageNum; fol++)
            { //katram foliage objektam zīmēs otro líniju -- zemītē iekšā iestumtie punkti (śajá gadíjumá pieskaru atrastie punkti)

                if (fol == addFakeToThisFoliage)
                {//spec gadíjums, pédéjais foliage objekts, kam beigás ir jásavienojas ar zemítes sákumpunktu - te vajag pieśaut papildus punktińu
                    foliages[fol].Add(fTangents[0][0] + fTangents[0][0] * 0.001f); //pirmás pieskares pirmais punkts
                    fColors[fol].Add(fColorDown);
                }

                for (int i = fTangents[fol].Count - 1; i >= 0; i--)
                { //foliages listé ir jau salikti zemítes punkti - pulksteńrádítája kustíbas virziená; tagad pretéji tam, sákot no beigám, salikśu pieskaru punktus				
                    foliages[fol].Add(fTangents[fol][i]);
                    fColors[fol].Add(fColorDown);
                }

            }


            for (int f = 0; f <= foliageNum; f++)
            { //apskatu ikvienu foliage objektu, ko túdaĺ zímésim
                if (foliages[f].Count == 0)
                { //skipo tukśos				
                    continue;
                }

                string prefabName = foliageType.ToString(); // System.Enum.GetName(typeof(FoliageTypes), foliageType);
                                                            //instancéju foliage prefabu (prefabs, nevis new-obj, tádéĺ, ka tá vieglaka mainít materiálus/śeiderus)
                Debug.Log("<color=yellow>Prefab is loading from = </color>" + prefabName);
                GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(prefabName) as GameObject;
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + prefab);
                //GameObject prefab = Resources.Load("Prefabs/PolyMeshComponents/" + prefabName) as GameObject;
                GameObject foliage = Instantiate(
                    prefab,
                    transform.position + new Vector3(0, 0, foliageOffsetZ),//zemítes pozícija  + 0.5f uz kameras pusi
                    transform.rotation //noretéju tápat ká paśu zemíti
                    ) as GameObject;
                foliage.name = "foliage-" + foliageType + "-" + f;
                foliage.transform.parent = transform;
                foliage.transform.localScale = Vector3.one; //bérns tiek skeilots automátiski, kad skeilo parentu, lai bérns necínítos pretí skeilośanai - vińam vajag noresetot paśa izméru
                foliage.tag = "do-not-save";


                //shared mesh nodrośina pret mesh-leakage (there is such a thing)
                var meshFilter = foliage.GetComponent<MeshFilter>();
                var mesh = meshFilter.sharedMesh;
                if (mesh == null)
                {
                    mesh = new Mesh();
                    mesh.name = "foliage-mesh";
                    meshFilter.mesh = mesh;
                }



                //automátiski izveidoju UV mapi (neizmanotju scale vai rotation parametrus, ko lieto galvená kluça tekstúrai)
                var uv = new Vector2[foliages[f].Count];
                for (int i = 0; i < uv.Length; i++)
                {
                    var p = foliages[f][i];
                    uv[i] = new Vector2(p.x, p.y);
                }


                mesh.Clear();
                mesh.vertices = foliages[f].ToArray();
                mesh.colors32 = fColors[f].ToArray();
                mesh.triangles = Triangulate.Points(foliages[f]);
                ;
                mesh.uv = uv;
                mesh.RecalculateNormals();

                foliage.GetComponent<Renderer>().sortingLayerName = orderLayerName;
                foliage.GetComponent<Renderer>().sortingOrder = orderInLayer;

                foliage.GetComponent<MeshRenderer>().sortingLayerName = orderLayerName;
                foliage.GetComponent<MeshRenderer>().sortingOrder = orderInLayer;
                //Debug.Log("foliage MeshRenderer layer " + foliage.GetComponent<MeshRenderer>().sortingOrder);
            }
        }

        //uzzímé shadingBelt - lídzígs foliage objektiem - bet ir viens pa visu perimetru

        GameObject prfb = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("shadingBelt") as GameObject;
        Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + prfb);
        //GameObject prfb = Resources.Load("Prefabs/PolyMeshComponents/shadingBelt") as GameObject;
        GameObject belt = Instantiate(
            prfb,
            transform.position + new Vector3(0, 0, foliageOffsetZ + 0.05f),
            transform.rotation //noretéju tápat ká paśu zemíti
            ) as GameObject;
        belt.name = "shadingBelt";
        belt.transform.parent = transform;
        belt.transform.localScale = Vector3.one; //bérns tiek skeilots automátiski, kad skeilo parentu, lai bérns necínítos pretí skeilośanai - vińam vajag noresetot paśa izméru
        belt.tag = "do-not-save";

        List<Vector3> vs = new List<Vector3>();
        List<Color32> cs = new List<Color32>();


        for (int i = 0; i < terrainEdges.Count; i++)
        { //polimeśa árpuses punkti
            vs.Add(terrainEdges[i]);
            cs.Add(fColorUp);

            //Debug.Log("A: " + terrainEdges[i]);
        }


        int last = terrainEdges.Count - 1;
        int skippedNum = 0;
        for (int i = last; i >= 0; i--)
        { //polimeśa iekśéjie punkti - otrádá secíbá - veido gandŕiz noslégtu pakavu 

            float d = Vector3.Distance(terrainEdges[i] + pointNormalesSB[terrainEdges[i]], terrainEdges[i]); ///normáles garums = (punkts + normále)  => punkts
			if (skippedNum < 3 && d < 0.5f * shadingBeltThickness)
            {

                skippedNum++;
                //continue;//skipo punktus, kas atrodas ísu normáĺu galos
            }

            skippedNum--;
            if (skippedNum < 0)
            {
                skippedNum = 0;
            }

            vs.Add(pointNormalesSB[terrainEdges[i]] + terrainEdges[i]);
            cs.Add(fColorDown);
            //Debug.Log("B: " + pointNormales[terrainEdges[i]] + terrainEdges[i] );
        }

        //lai veidotu gandríz noslégtu loku, pielieku papildus 2 punktus - ĺoti tuvu pédéjam virmas punktam (un vińa iekśéjam punktam)
        vs.Add((pointNormalesSB[terrainEdges[last]] + terrainEdges[last]) + new Vector3(0.001f, 0.001f, 0));
        cs.Add(fColorDown);

        vs.Add(terrainEdges[last] + new Vector3(0.001f, 0.001f, 0));
        cs.Add(fColorUp);


        var mf = belt.GetComponent<MeshFilter>();
        var _mesh = mf.sharedMesh;
        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.name = "belt-mesh";
            mf.mesh = _mesh;
        }

        var _uv = new Vector2[vs.Count];
        for (int i = 0; i < _uv.Length; i++)
        {
            var p = vs[i];
            _uv[i] = new Vector2(p.x, p.y);
        }


        _mesh.Clear();
        _mesh.vertices = vs.ToArray();
        _mesh.colors32 = cs.ToArray();
        _mesh.triangles = Triangulate.Points(vs);
        ;
        _mesh.uv = _uv;
        _mesh.RecalculateNormals();


        belt.GetComponent<MeshRenderer>().sortingLayerName = orderLayerName;
        belt.GetComponent<MeshRenderer>().sortingOrder = orderInLayer;
        Debug.Log("belt MeshRenderer layer " + belt.GetComponent<MeshRenderer>().sortingOrder);

        //*/	
    }




    bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {

        Vector2 a = p2 - p1;
        Vector2 b = p3 - p4;
        Vector2 c = p1 - p3;

        float alphaNumerator = b.y * c.x - b.x * c.y;
        float alphaDenominator = a.y * b.x - a.x * b.y;
        float betaNumerator = a.x * c.y - a.y * c.x;
        float betaDenominator = alphaDenominator;

        bool doIntersect = true;

        if (alphaDenominator == 0 || betaDenominator == 0)
        {
            doIntersect = false;
        }
        else
        {

            if (alphaDenominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                {
                    doIntersect = false;
                }
            }
            else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
            {
                doIntersect = false;
            }

            if (doIntersect && betaDenominator > 0)
            {
                if (betaNumerator < 0 || betaNumerator > betaDenominator)
                {
                    doIntersect = false;
                }
            }
            else if (betaNumerator > 0 || betaNumerator < betaDenominator)
            {
                doIntersect = false;
            }
        }

        return doIntersect;
    }

    /**
     * pagarina nogriezni a->b, punkta b virziená, "length" reizes
     * tikai 2D plakné (nemaina Z komponenti)
     * atgrieź c punktu (a->c bús pagarinátais nogrieznis)
     */
    private Vector3 ExtendLineSegment(Vector3 a, Vector3 b, float length)
    {

        float lenAB = Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
        if (lenAB == 0 || length == 0)
        {
            return b;
        }

        float x = b.x + (b.x - a.x) / lenAB * length;
        float y = b.y + (b.y - a.y) / lenAB * length;

        return new Vector3(x, y, b.z);
    }


    /* 
        Vector3 A = new Vector3(10,10,-2);
        Vector3 B = new Vector3(11,12,-2);
        Debug.DrawLine(A, B, Color.black, 3);
        Vector3 C = AbbreviateLineSegment(A,B,2f);
        Debug.DrawLine(A + new Vector3(0.01f,0,0), C+ new Vector3(0.01f,0f,0), Color.yellow, 3);
    //*/


    private static bool LineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, ref Vector3 intersection)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
        float x1lo, x1hi, y1lo, y1hi;

        Ax = p2.x - p1.x;
        Bx = p3.x - p4.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p2.x; x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x; x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return false;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;


        // Y bound box test//		
        if (Ay < 0)
        {
            y1lo = p2.y; y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y; y1lo = p1.y;
        }

        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return false;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//		
        f = Ay * Bx - Ax * By;  // both denominator//		


        // alpha tests//		
        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }

        e = Ax * Cy - Ay * Cx;  // beta numerator//			

        // beta tests //		
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }


        // check if they are parallel		
        if (f == 0) return false;
        // compute intersection coordinates //		
        num = d * Ax;
        intersection.x = p1.x + num / f;
        num = d * Ay;
        intersection.y = p1.y + num / f;
        return true;

    }

    private static bool same_sign(float a, float b)
    {
        return ((a * b) >= 0f);
    }

    /**
     * serializé śo objektu, atgrieź stringu
     */
    public JSONClass Save()
    {

        collider2d = null; //resetoju referenci uz kolaideri (neserializésim taçu references, come on!) skripts vélák atradís, ja bús śis objekts

        var J = new JSONClass();
        for (int i = 0; i < keyPoints.Count; i++)
        {
            J["keyPoints"][i] = JSONHelper.Vector3ToJSONString(keyPoints[i]); //mans paśtaisítais Vektora serializétájs ar niecígu float precizitáti | vektors ir neNULLējams tips, tāpēc nevarēju uztaisīt JSON libā, kā pārējos tipus
        }
        for (int i = 0; i < curvePoints.Count; i++)
        {
            J["curvePoints"][i] = JSONHelper.Vector3ToJSONString(curvePoints[i]);
        }
        for (int i = 0; i < isCurve.Count; i++)
        {
            J["isCurve"][i].AsBool = isCurve[i];
        }
        for (int i = 0; i < isFoliage.Count; i++)
        {
            J["isFoliage"][i].AsBool = isFoliage[i];
        }

        J["curveDetail"].AsFloat = curveDetail;
        J["colliderDepth"].AsFloat = colliderDepth;
        J["buildColliderEdges"].AsBool = buildColliderEdges;
        J["buildColliderFront"].AsBool = buildColliderFront;
        J["uvPosition"] = JSONHelper.Vector2ToJSONString(uvPosition);
        J["uvScale"].AsFloat = uvScale;
        J["uvRotation"].AsFloat = uvRotation;
        J["foliageThickness"].AsFloat = foliageThickness;
        J["foliageThicknessAltPercentageA"].AsFloat = foliageThicknessAltPercentageA;
        J["foliageThicknessAltPercentageB"].AsFloat = foliageThicknessAltPercentageB;
        J["foliageType"].AsInt = (int)foliageType;//enums
        J["generateCollider"].AsBool = generateCollider;
        J["shadingBeltThickness"].AsFloat = shadingBeltThickness;
        J["orderInLayer"].AsInt = orderInLayer;




        return J;
    }



    /**
     * sańem serializétu info ar visiem śí objekta parametriem un datu struktúrám, tos visus saliek atpakáĺ objektá
     * śí info ir vajadzíga turpmákai polimeśa rediǵéśanai, tápéc skipojam produkcijas kodá (ietaupot ĺoti daudz sekunźu)
     */
    public void Load(JSONNode N)
    {

        keyPoints = new List<Vector3>(N["keyPoints"].Count);// izveidoju listi vajadzigajá garumá, lai katrs Add neprasítu jaunu atmińu
        for (int i = 0; i < N["keyPoints"].Count; i++)
        {
            keyPoints.Add(JSONHelper.Vector3FromJSONString(N["keyPoints"][i]));
        }


        curvePoints = new List<Vector3>(N["curvePoints"].Count);
        for (int i = 0; i < N["curvePoints"].Count; i++)
        {
            curvePoints.Add(JSONHelper.Vector3FromJSONString(N["curvePoints"][i]));
        }

        isCurve = new List<bool>(N["isCurve"].Count);
        for (int i = 0; i < N["isCurve"].Count; i++)
        {
            isCurve.Add(N["isCurve"][i].AsBool);
        }

        isFoliage = new List<bool>(N["isFoliage"].Count);
        for (int i = 0; i < N["isFoliage"].Count; i++)
        {
            isFoliage.Add(N["isFoliage"][i].AsBool);
        }

        curveDetail = N["curveDetail"].AsFloat;
        colliderDepth = N["colliderDepth"].AsFloat;
        buildColliderEdges = N["buildColliderEdges"].AsBool;
        buildColliderFront = N["buildColliderFront"].AsBool;
        uvPosition = JSONHelper.Vector2FromJSONString(N["uvPosition"]);
        uvScale = N["uvScale"].AsFloat;
        uvRotation = N["uvRotation"].AsFloat;
        foliageThickness = N["foliageThickness"].AsFloat;
        foliageThicknessAltPercentageA = N["foliageThicknessAltPercentageA"].AsFloat;
        foliageThicknessAltPercentageB = N["foliageThicknessAltPercentageB"].AsFloat;
        foliageType = (FoliageTypes)N["foliageType"].AsInt; // = (int)foliageType;//enums
        generateCollider = N["generateCollider"].AsBool;
        shadingBeltThickness = N["shadingBeltThickness"].AsFloat;
        orderInLayer = N["orderInLayer"].AsInt;

    }



}

public static class Bezier1
{
    public static float Curve(float from, float control, float to, float t)
    {
        return from * (1 - t) * (1 - t) + control * 2 * (1 - t) * t + to * t * t;
    }
    public static Vector3 Curve(Vector3 from, Vector3 control, Vector3 to, float t)
    {
        from.x = Curve(from.x, control.x, to.x, t);
        from.y = Curve(from.y, control.y, to.y, t);
        from.z = Curve(from.z, control.z, to.z, t);
        return from;
    }

    public static Vector3 Control(Vector3 from, Vector3 to, Vector3 curve)
    {
        //var center = Vector3.Lerp(from, to, 0.5f);
        //return center + (curve - center) * 2;
        var axis = Vector3.Normalize(to - from);
        var dot = Vector3.Dot(axis, curve - from);
        var linePoint = from + axis * dot;
        return linePoint + (curve - linePoint) * 2;
    }
}

public static class Triangulate
{

    public static int[] Points(Vector3[] points)
    {
        return Points(points.ToList());
    }

    public static int[] Points(List<Vector3> points)
    {
        var indices = new List<int>();

        int n = points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area(points) > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(points, u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    static float Area(List<Vector3> points)
    {
        int n = points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector3 pval = points[p];
            Vector3 qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    static bool Snip(List<Vector3> points, int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector3 A = points[V[u]];
        Vector3 B = points[V[v]];
        Vector3 C = points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector3 P = points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }


}

}
