using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColoredObject : MonoBehaviour {

    [SerializeField]
    Renderer[] renderers = null;

    Material[] materials = null;

    [SerializeField]
    Gradient gradient = new Gradient()
    {
        colorKeys = new GradientColorKey[3] {
            new GradientColorKey(new Color(1, 0, 0), 0),
            new GradientColorKey(new Color(1, 1, 0), 0.5f),
            new GradientColorKey(new Color(0, 1, 1), 1)
        },
        alphaKeys = new GradientAlphaKey[2] {
            new GradientAlphaKey(0.6f, 0),
            new GradientAlphaKey(0.7f, 1)
        }
    };

    [SerializeField]
    float fGradientCycle = 1;

    float fTime = 0;

    void updateRenderers()
    {
        if (renderers == null || !renderers.Any(r => r != null))
        {
            renderers = GetComponentsInChildren<Renderer>();
            materials = renderers.Select(r => r.sharedMaterial).ToArray();
        }

        if ((fTime += Time.deltaTime) > fGradientCycle)
            fTime -= fGradientCycle;

        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].material.color = gradient.Evaluate(fTime / fGradientCycle);
        }
    }

    public void ChangeGradient(Gradient newGradient)
    {
        gradient = newGradient;
    }

    public void RestoreRenderers()
    {
        if (renderers != null)
        {
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].material = materials[i];
            }
            renderers = null;
        }
        materials = null;
    }

    private void Update()
    {
        updateRenderers();

    }

    public static void Start(GameObject target, Gradient newGradient = null)
    {
        var com = target.GetComponent<ColoredObject>();
        if (com == null)
            com = target.AddComponent<ColoredObject>();
        if (newGradient != null)
            com.ChangeGradient(newGradient);
    }

    public static void End(GameObject target)
    {
        var com = target.GetComponent<ColoredObject>();
        if (com != null)
        {
            com.RestoreRenderers();
            Destroy(com);
        }
    }
}
