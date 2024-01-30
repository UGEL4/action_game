using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ActionController : MonoBehaviour
{
    public List<CharacterAction> AllActions {get; set;} = new List<CharacterAction>();
    private List<CharacterAction> _oraderedActions = new List<CharacterAction>();
    public Animator animator;
    private CharacterAction CurAction;

    public void Update()
    {
        foreach (CharacterAction ac in AllActions)
        {
            if (CanActionCancelCurrentAction(ac))
            {
                
            }
        }
    }

    bool CanActionCancelCurrentAction(CharacterAction actionInfo)
    {
        return false;
    }
}