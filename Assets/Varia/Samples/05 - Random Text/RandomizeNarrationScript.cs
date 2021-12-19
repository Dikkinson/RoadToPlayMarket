using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeNarrationScript : MonoBehaviour
{
    public GameObject target;

    public void Randomize()
    {
        var newGo = Instantiate(target, gameObject.transform.parent);
        newGo.name = target.name;
        Destroy(target);
        target = newGo;
    }

}
