using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnattachedAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [Header("whyt he fuck aren't there sliders in the animator")]
    [SerializeField, Range(0, 0.999f)]
    private float aimPitch = 0.5f;
    [SerializeField, Range(0, 0.999f)]
    private float aimYaw = 0.5f;

    private bool placingCoil;
    private float combatTim;
    private float jumpTim;

    void Update()
    {
        if (!animator)
            return;

        Moob();
        Jumb();
        Aiming();
        Shooting();
        
        Tim();
    }
    #region for everyone

    private void Moob() {
        //man it's been so long since I've written a moob function


        Debug.DrawLine(Vector3.zero, Vector3.one * 100);

        float hori = Input.GetAxis("Horizontal");
        float veri = Input.GetAxis("Vertical");

        animator.SetBool("isMoving", Mathf.Abs(hori) + Mathf.Abs(veri) > 0.01f);
        animator.SetFloat("forwardSpeed", veri);
        animator.SetFloat("rightSpeed", hori);
        
        animator.SetBool("isSprinting", Input.GetKey(KeyCode.LeftShift));
    }

    private void Jumb() {

        if (Input.GetKeyDown(KeyCode.Space)) {
            animator.Play("Jump");
            animator.SetBool("isGrounded", false);
            jumpTim = 1.5f;
        }

        jumpTim -= Time.deltaTime;

        animator.SetFloat("upSpeed", Mathf.Lerp(-48, 16, jumpTim / 2f));

        if(jumpTim <= 0) {
            animator.SetBool("isGrounded", true);
        }
    }

    private void Aiming() {

        if (Input.GetKeyDown(KeyCode.Q))
            aimYaw += 0.2f;

        if (Input.GetKeyDown(KeyCode.E))
            aimYaw -= 0.2f;

        aimYaw = Mathf.Clamp(aimYaw, 0, 0.999f);

        animator.SetFloat("aimYawCycle", aimYaw);
        animator.SetFloat("aimPitchCycle", aimPitch);
    }

    #endregion for everyone

    private void Shooting() {

        //if (Input.GetMouseButtonDown(0)) {

        //    animator.Play("HandOut", 2);
        //    animator.Play("Shock", 2);
        //    combatTim = 2;
        //}

        //if (Input.GetMouseButtonDown(1)) {

        //    animator.SetBool("isHandOut", true);
        //}

        //if (Input.GetMouseButtonUp(1)) {

        //    animator.Play("Shock", 2);
        //    animator.SetBool("isHandOut", false);
        //    combatTim = 2;
        //}

        //if (!placingCoil) {

        //    if (Input.GetKeyDown(KeyCode.R)) {

        //        animator.Play("Placing");
        //        placingCoil = true;
        //        combatTim = 2;
        //    }
        //} else {

        //    if (Input.GetKeyDown(KeyCode.R)) {

        //        animator.Play("DoPlace");
        //        placingCoil = false;
        //    }
        //    if (Input.GetKeyDown(KeyCode.LeftShift)) {

        //        animator.Play("CancelPlace");
        //        placingCoil = false;
        //    }
        //}
    }

    #region timescale
    private void setTimeScale(float tim)
    {
        Time.timeScale = tim;

        Debug.Log($"set tim: {Time.timeScale}");
    }

    private void Tim()
    {
        //time keys
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Time.timeScale == 0)
            {
                setTimeScale(Time.timeScale + 0.1f);
            }
            else
            {
                setTimeScale(Time.timeScale + 0.5f);
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {

            setTimeScale(Time.timeScale - 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            setTimeScale(1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            setTimeScale(0);
        }
    }
    #endregion timescale
}
