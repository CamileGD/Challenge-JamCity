using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class HexTile : MonoBehaviour
    {
        //References
        private Material mat;
        [HideInInspector]
        public Grid gridRef;
        private Vector3 inicialPos = Vector3.zero;

        //Pathfinding vars
        public int x = 0;
        public int y = 0;
        public int cost = 1;
        public bool walkable = true;
        private bool selected = false;

        [Header("Interactions")]
        [Space]
        public Vector3 offsetPos;
        public Color baseColor;
        public Color selectedColor;
        public Color pathColor;

        private void Awake() 
        {
            if (mat != null)
                mat = new Material(mat);

            Renderer rend = GetComponent<Renderer>();

            mat = new Material(rend.material);  //Creating a new unique material for each tile to not change the original one

            if (rend != null){
                rend.material = mat;
            }
        }

        private void Start() 
        {
            inicialPos = transform.position; //Saving initial pos
        }

        public void SetIndex(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        //Mouse pressed on tile collider
        private void OnMouseDown() 
        {
            if (walkable)
                gridRef.TileClicked(new Vector2(x,y));    
        }

        //Reseting to origin
        public void ResetColor()
        {
            mat.SetColor("_Color",  baseColor);
            selected = false;
            transform.position = inicialPos;
        }

        //Changing color and pos if selected
        public void Selected(bool edge = false)
        {
            if (edge)
                mat.SetColor("_Color",  selectedColor);

            else
                mat.SetColor("_Color",  pathColor);

            selected = true;
            transform.position = inicialPos + offsetPos;
        }
    }
}
