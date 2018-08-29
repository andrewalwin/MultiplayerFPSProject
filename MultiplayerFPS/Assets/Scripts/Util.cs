using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{

    //set an object and all its children to a specific layer
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if(child == null)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    //enable/disable an object and all its children
    public static void SetEnabledRecursively(GameObject obj, bool enabled)
    {
        if(obj.gameObject == null)
        {
            return;
        }
        obj.gameObject.SetActive(enabled);

        foreach (Transform child in obj.transform)
        {
            if(child == null)
            {
                continue;
            }
            SetEnabledRecursively(child.gameObject, enabled);
        }
    }

    //generate a normally distributed point user a Box-Muller transform given a mean and standard deviation
    public static float GenerateBoxMullerPoint(float mean, float stdDev)
    {
        float u1 = 1.0f - Random.Range(0f, 1f);
        float u2 = 1.0f - Random.Range(0f, 1f);

        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        float randNormal = mean + stdDev * randStdNormal;
        return randNormal;
    }
}
