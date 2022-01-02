using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Gate : MonoBehaviour
{
    #region public Fields

    public GameObject gateTrigger;
    public GameObject goodKey;
    public Animator gateAnimator;
    public int numGate;
    public bool gateIsOpen = false;
    public Triggers trigger;
    
    #endregion

    #region private Fields

    private static readonly int CloseGate = Animator.StringToHash("closeGate");
    private static readonly int OpenGate = Animator.StringToHash("openGate");
    
    private bool _animationFinish = true;

    #endregion
    
    #region MonoBehaviour

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Opening the gate
        if (!(other.CompareTag("Key") & _animationFinish & other.gameObject == goodKey)) return;
        _animationFinish = false;
        GateAnimation();
    }

    public void SetGateOpen()
    {
        gateIsOpen = true;
        _animationFinish = true;
    }

    public void SetGateClose()
    {
        gateIsOpen = false;
        _animationFinish = true;
    }

    /**
     * Triggers gate animation.
     */
    private void GateAnimation()
    {
        gateAnimator.SetTrigger(gateIsOpen ? CloseGate : OpenGate);
    }

    /**
     * Transferring the player into the castle after
     * opening the gate and colliding with it.
     */
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!(other.gameObject.CompareTag("Player") & gateIsOpen)) return;
        SetGateOpen();
        trigger.playerIsOverlapping = true;
    }

    /**
     * Opening the gate, if the player is inside and the gate got closed.
     */
    public void OpenGateFromOutSide()
    {
        var position = gateTrigger.transform.position;
        var temp = position.y;
        gateAnimator.SetTrigger(OpenGate);
        gateIsOpen = true;
    }

    #endregion
}