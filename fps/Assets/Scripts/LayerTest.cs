using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTest : MonoBehaviour
{
    
    public LayerMask TargetLayer;
    public float Range;

    private void Start()
    {
        TargetLayer = TargetLayer.Add(9);
        TargetLayer = TargetLayer.Remove(9);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (TargetLayer.Contains(other))
        {
            Debug.Log("찾음");
        }
        
    }
}
public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    public static bool Contains(this LayerMask mask, GameObject go)
    {
        return(mask.value & (1 << go.layer)) != 0;
    }

    public static bool Contains(this LayerMask mask, Component component)
    {
        return (mask.value & (1 << component.gameObject.layer)) != 0;
    }

    public static LayerMask Add(this LayerMask mask, int layer)
    {
        return (mask.value | (1 << layer));
    }

    public static LayerMask Add(this LayerMask mask, GameObject go)
    {
        return (mask.value | (1 << go.layer));
    }

    public static LayerMask Add(this LayerMask mask, Component component)
    {
        return (mask.value | (1 << component.gameObject.layer));
    }

    public static LayerMask Remove(this LayerMask mask, int layer)
    {
        return (mask.value & ~(1 << layer));
    }

    public static LayerMask Remove(this LayerMask mask, GameObject go)
    {
        return  (mask.value & ~(1 << go.layer));
    }

    public static LayerMask Remove(this LayerMask mask, Component component)
    {
        return  (mask.value & ~(1 << component.gameObject.layer));
    }

    public static LayerMask Everything(this LayerMask mask)
    {
        return ~0;
    }
    public static LayerMask Nothing(this LayerMask mask)
    {
        return 0;
    }
}
