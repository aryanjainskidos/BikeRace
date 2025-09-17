namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class ColorReceiver3D : MonoBehaviour
{

    public string group = ""; //-1 - no group
    MeshRenderer meshRenderer;
    SkinnedMeshRenderer skinnedMeshRenderer;
    public Texture mainTexture;
    public Color32 color;
    public Material material;

    // Use this for initialization
    void Awake()
    {
        //Debug.Log("ColorReceiver3D::Awake " + group);

        meshRenderer = GetComponent<MeshRenderer>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        mainTexture = meshRenderer != null ? meshRenderer.material.mainTexture : skinnedMeshRenderer.material.mainTexture;
    }

    public void ChangeMaterial(Material material)
    {
        //print("ChangeMaterial " + group);
        this.material = material;
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.material = material;
            skinnedMeshRenderer.material.mainTexture = mainTexture;
        }
        if (meshRenderer != null)
        {
            meshRenderer.material = material;
            meshRenderer.material.mainTexture = mainTexture;
        }
    }

    public void ChangeColor(Color32 color)
    {
        //print("ChangeColor " + group);
        this.color = color;
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.material.color = color;
        }
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }
    }
}

}
