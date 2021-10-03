using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sync
{
    // Start is called before the first frame update
    [RuntimeInitializeOnLoadMethod]
    static void OnRunTimeInitializeOnLoad()
    {
        Debug.Log("script is load");
    }
}
