using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public int q = 0;
    public int r = 0;

    public void SetIndex(int _q, int _r)
    {
        q = _q;
        r = _r;
    }
}
