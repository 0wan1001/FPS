using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    public ObjectPool Pool { get; set; }
    public Transform tr { get; }
    
    public void ReturnToPool();
}
