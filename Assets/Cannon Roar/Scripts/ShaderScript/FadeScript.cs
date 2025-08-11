using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    Renderer material;

    public bool faded;
    public bool inProcess;

    public float lerpSpeed;
    public float bufferAmount;

    private void Awake()
    {
        material = GetComponent<Renderer>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inProcess = true;
        }

        if (inProcess)
        {
            float currentDisolve = material.material.GetFloat("DisolveFloat");

            if (faded)
            {
                if (currentDisolve > 0 + bufferAmount)
                {
                    float newDisolve = Mathf.Lerp(currentDisolve, 0, lerpSpeed * Time.deltaTime);
                    material.material.SetFloat("DisolveFloat", newDisolve);
                }
                else
                {
                    material.material.SetFloat("DisolveFloat", 0);
                    faded = false;
                    inProcess = false;
                }
            }
            else
            {
                if (currentDisolve < 1 - bufferAmount)
                {
                    float newDisolve = Mathf.Lerp(currentDisolve, 1, lerpSpeed * Time.deltaTime);
                    material.material.SetFloat("DisolveFloat", newDisolve);
                }
                else
                {
                    material.material.SetFloat("DisolveFloat", 1);
                    faded = true;
                    inProcess = false;
                }
            }
        }
    }
}
