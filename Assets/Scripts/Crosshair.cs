using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof (LineRenderer))]
    public class Crosshair : MonoBehaviour
    {
        private FirstPersonController _controller;
        private Terrain _terrain;
        //private LineRenderer _lineRenderer;

        // Use this for initialization
        void Start()
        {
            _controller = FindObjectOfType<FirstPersonController>();
            _terrain = FindObjectOfType<Terrain>();
            //_lineRenderer = GetComponent<LineRenderer>();
            //_lineRenderer.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            CastRayToTerrain();
        }

        void FixedUpdate()
        {
        }

        private void CastRayToTerrain()
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_terrain.GetComponent<Collider>().Raycast(ray, out hit, 100))
            {
                print("WORLD POINT: " + hit.point);
                if (Input.GetMouseButtonDown(0))
                {
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (hit.distance != 0)
                        _controller.transform.position = hit.point;
                }
            }
        }

        private void Slide()
        {
            // Find the position that the mouse is hitting
            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var ray = Camera.main.ScreenPointToRay(mouseWorldPosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);

            if (Input.GetMouseButtonDown(0))
            {
                //_lineRenderer.enabled = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                //_lineRenderer.enabled = false;
                if (hit.distance != 0)
                    _controller.transform.position = hit.point;
            }
        }

        void OnGUI()
        {
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "");
        }
    }
}