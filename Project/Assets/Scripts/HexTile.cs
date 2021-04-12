using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class HexTile : MonoBehaviour
    {
        public int q = 0;
        public int r = 0;
        public int s = 0;
        public int cost = 1;

        public bool walkable = true;
        
        public Grid gridRef;
        public void SetIndex(int _q, int _r, int _s)
        {
            q = _q;
            r = _r;
            s = _s;
        }

        //Mouse pressed on the tile collider
        private void OnMouseDown() 
        {
            if (walkable)
                gridRef.TileClicked(new Vector2(q,r));
        }
    }
}
