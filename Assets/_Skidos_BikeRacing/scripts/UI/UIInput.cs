namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInput : MonoBehaviour
{


    public const string ACCELERATE = "Accelerate";
    public const string VERTICAL = "Vertical";
    public const string BRAKE = "Brake";

    static Dictionary<string, UIInputAxis> axes;

    static bool initialized = false;

    static void Init()
    {
        axes = new Dictionary<string, UIInputAxis>();
        axes.Add(VERTICAL, new UIInputAxis(3, 3, 0.001f, true));
        axes.Add(BRAKE, new UIInputAxis(1000, 1000));
        axes.Add(ACCELERATE, new UIInputAxis(1000, 1000));
        initialized = true;
    }

    public static float GetAxis(string axisName)
    {
        float value = 0;
        if (initialized && axes.ContainsKey(axisName))
        {
            value = axes[axisName].value;
        }
        return value;
    }

    public static void SetAxis(string axisName, float target)
    {
        if (initialized && axes.ContainsKey(axisName))
        {
            axes[axisName].lastTarget = axes[axisName].target;
            axes[axisName].target = target;
        }
    }

    public static void ResetAllAxes()
    {
        //        print("Reset Axes");
        if (initialized)
        {
            foreach (var axisKVP in axes)
            {
                axisKVP.Value.value = 0;
                axisKVP.Value.target = 0;
                axisKVP.Value.lastTarget = 0;
            }
        }
    }

    UIInputAxis axis;//tmp axis for update

    void Awake()
    {
        if (!initialized)
        {
            Init();
        }

        values = new float[axes.Count];
    }

    public float[] values;

    void Update()
    {

        int i = 0;
        foreach (var axisKVP in axes)
        {
            axis = axisKVP.Value;

            if (axis.target != 0)
            {
                //snap
                if (axis.snap)
                {
                    if (axis.target != 0 && axis.lastTarget != 0 &&
                        Mathf.Sign(axis.lastTarget) != Mathf.Sign(axis.target)
                        )
                    {

                        axis.value = 0; //snap to neutral
                        axis.lastTarget = axis.target;
                    }
                }

                //use sensitivity
                axis.value = Mathf.Lerp(axis.value, axis.target, Time.unscaledDeltaTime * axis.sensitivity);
            }
            else
            {
                //use gravity
                axis.value = Mathf.Lerp(axis.value, axis.target, Time.unscaledDeltaTime * axis.gravity);
            }

            if (Mathf.Abs(axis.value) < axis.dead)
            {
                axis.value = 0;
            }

            values[i++] = axis.value;
        }
    }
}

public class UIInputAxis
{
    public float gravity;
    public float dead;
    public float sensitivity;
    public bool snap;
    public float value;
    public float target;
    public float lastTarget;

    public UIInputAxis(float gravity = 3, float sensitivity = 3, float dead = 0.001f, bool snap = false)
    {
        this.snap = snap;
        this.sensitivity = sensitivity;
        this.dead = dead;
        this.gravity = gravity;
        this.value = 0;
        this.target = 0;
        this.lastTarget = 0;
    }
}

}
