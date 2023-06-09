using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Package;

public static class EnabledComponent 
{
   public static NpcBase NpcBase(NpcBase[] npcBases)
   {
        foreach (NpcBase npc in npcBases)
        {
            if (npc.enabled == true)
                return npc;
        }

        return null;
   }
}
