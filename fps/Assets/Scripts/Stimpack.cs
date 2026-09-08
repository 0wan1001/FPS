using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stimpack : MonoBehaviour, IInteractable
{
    public GameObject GameObject { get => gameObject; }
    private Outline _outline;
    
    private void Awake() => CacheComponents();

    private void Start() => Init();
    
    public void Targeting()
    {
        _outline.enabled = true;
    }

    public void Untargeting()
    {
        _outline.enabled = false;
    }
    public void Interact(IInteractor owner)
    {
        if (!(owner is PlayerController)) return;
        
        PlayerController player =  (PlayerController)owner;

        player.UseStimpack();
        
        Destroy(gameObject);
    }

    private void Init()
    {
        _outline.enabled = false;
    }
    
    private void CacheComponents()
    {
        _outline = gameObject.GetComponent<Outline>();
    }
}