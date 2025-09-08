using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateHandOnInput : MonoBehaviour
{
    public string triggerInputName = "Fire1";  // Default to left mouse button or controller trigger
    public string gripInputName = "Fire2";     // Default to right mouse button or controller grip
    public Animator handAnimator;

    // Update is called once per frame
    void Update()
    {
        // For trigger animation, use the specified input name
        float triggerValue = Input.GetAxis(triggerInputName);
        handAnimator.SetFloat("Trigger", triggerValue);

        // For grip animation, use the specified input name
        float gripValue = Input.GetAxis(gripInputName);
        handAnimator.SetFloat("Grip", gripValue);
    }
}
