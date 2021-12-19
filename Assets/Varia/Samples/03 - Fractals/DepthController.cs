using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varia;

public class DepthController : MonoBehaviour
{
    public VariaPreviewer previewer;

    public VariaBehaviour[] behaviours;

    public void UpdateDepth(float fDepth)
    {
        var depth = (int)fDepth;
        var changed = false;
        foreach(var b in behaviours)
        {
            foreach(var c in b.conditionList.conditions)
            {
                if(c.conditionType==VariaConditionType.DepthFilter)
                {
                    if (c.depth != depth)
                    {
                        c.depth = depth;
                        changed = true;
                    }
                }
            }
        }
        if (changed)
        {
            previewer.Refresh();
        }
    }
}
