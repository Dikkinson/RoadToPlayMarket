using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObjects : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask targetLayers;
    [Range(0f,1f)]
    [SerializeField] private float objectAlpha = 0.3f;
    
    private Transform[] obstructions;
    private int oldHitsNumber;
    
    
    void FixedUpdate() {
        XRay ();
    }

    // Hacer a los objetos que interfieran con la vision transparentes
    private void XRay() {

        float characterDistance = Vector3.Distance(transform.position, target.position);
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, fwd, characterDistance, targetLayers, QueryTriggerInteraction.Ignore);

        if (hits.Length > 0)
        {   
            int newHits = hits.Length - oldHitsNumber;

            if (obstructions != null && obstructions.Length > 0 && newHits < 0)
            {
                for (int i = 0; i < obstructions.Length; i++)
                {
                    MeshRenderer obstructionRenderer = obstructions[i].GetComponent<MeshRenderer>();
                    Color colorA = obstructionRenderer.material.color;
                    colorA.a = 1f;
                    obstructionRenderer.material.color = colorA;
                }
            }
            obstructions = new Transform[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                Transform obstruction = hits[i].transform;
                MeshRenderer obstructionRenderer = obstruction.GetComponent<MeshRenderer>();
                Color colorA = obstructionRenderer.material.color;
                colorA.a = objectAlpha;
                obstructionRenderer.material.color = colorA;
                obstructions[i] = obstruction;
            }
            oldHitsNumber = hits.Length;
        }
        else
        {   // Mean that no more stuff is blocking the view and sometimes all the stuff is not blocking as the same time
            if (obstructions != null && obstructions.Length > 0)
            {
                for (int i = 0; i < obstructions.Length; i++)
                {
                    MeshRenderer obstructionRenderer = obstructions[i].GetComponent<MeshRenderer>();
                    Color colorA = obstructionRenderer.material.color;
                    colorA.a = 1f;
                    obstructionRenderer.material.color = colorA;
                }
                oldHitsNumber = 0;
                obstructions = null;
            }
        }
    }
}
