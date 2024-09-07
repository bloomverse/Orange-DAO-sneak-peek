using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class LaserGun : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPositions(new Vector3[] { transform.position, transform.position });
        }

        private void Update()
        {
            while (true)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 200f))
                {
                    lineRenderer.SetPosition(1, hit.point);
                }
                else
                {
                    lineRenderer.SetPosition(1, transform.position + transform.forward * 200f);

                }
            }
        }
    }
}
