using System.Collections.Generic;
using UnityEngine;

public class DebugItemRegistry // registry class, za lazji referencing in dostop v custom editorju
{

    public static List<IDebugItem> allitems = new List<IDebugItem>(); // private list z debugItem interface, saj generic classa (T) ne mores referencat v List<>()

    public static void Register(IDebugItem item)
    {
        allitems.Add(item);
    }

}
