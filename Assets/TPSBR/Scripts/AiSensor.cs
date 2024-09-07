using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///https://www.youtube.com/watch?v=znZXmmyBF-o

namespace TPSBR
{
    [ExecuteInEditMode]
    public class AiSensor : MonoBehaviour
    {

        public float distance= 10;
        public float angle = 30;
        public float height = 1.0f;
        public Color MeshColor = Color.red;

        public int scanFrecuency = 30;
        public LayerMask layers;
        public LayerMask oclussionLayer;
        public List<GameObject> Objects= new List<GameObject>();

        Collider[] colliders = new Collider[50];
        Mesh mesh;
        int count;
        float scantInterval;
        float scanTimer;

        // Start is called before the first frame update
        void Start()
        {
            scantInterval = 1.0f / scanFrecuency;
        }

        // Update is called once per frame
        void Update()
        {
            scanTimer -= Time.deltaTime;
            if(scanTimer <0){
                scanTimer += scantInterval;
                Scan();
            }
        }

        private void Scan(){
            count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide );
            Objects.Clear();

            for (int i = 0; i < count; i++)
            {
                GameObject obj = colliders[i].gameObject;
                if(isInSight(obj)){
                    Objects.Add(obj);
                }
            }

        }

        public bool isInSight(GameObject obj){
            Vector3 origin = transform.position;
            Vector3 dest = obj.transform.position;
            Vector3 direction = dest - origin;
            if(direction.y < 0 || direction.y > height){
                return false;
            }
            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction,transform.forward);
            if(deltaAngle > angle){
                return false;
            }

            origin.y += height / 2;
            dest.y = origin.y;
            if(Physics.Linecast(origin,dest,oclussionLayer)){
                return false;
            };
            return true;
            
        }

        Mesh CreateWedgeMesh(){

//            Debug.Log("Creatingmesh");
             Mesh mesh = new Mesh();

            int segments = 10;

            int numTriangles = (segments * 4) + 2 + 2 ;
            int numVertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter =  Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0,-angle,0) * Vector3.forward * distance;
            Vector3 bottomRight = Quaternion.Euler(0,angle,0) * Vector3.forward * distance;

            Vector3 TopCenter = bottomCenter + Vector3.up * height;
            Vector3 TopRight = bottomRight + Vector3.up * height;
            Vector3 TopLeft = bottomLeft + Vector3.up * height;

            int vert = 0;

            // Left Side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = TopLeft;

            vertices[vert++] = TopLeft;
            vertices[vert++] = TopCenter;
            vertices[vert++] = bottomCenter;
            
            // Right Side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = TopCenter;
            vertices[vert++] = TopRight;

            vertices[vert++] = TopRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;
            for (int i = 0; i < segments; i++)
            {   
               
                bottomLeft = Quaternion.Euler(0,currentAngle,0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0,currentAngle + deltaAngle,0) * Vector3.forward * distance;

                TopRight = bottomRight + Vector3.up * height;
                TopLeft = bottomLeft + Vector3.up * height;

                // faside
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = TopRight;

                
                vertices[vert++] = TopRight;
                vertices[vert++] = TopLeft;
                vertices[vert++] = bottomLeft;

                // top side
                vertices[vert++] = TopCenter;
                vertices[vert++] = TopLeft;
                vertices[vert++] = TopRight;

                //bottom side
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                
                currentAngle += deltaAngle;
            }


            for (int i = 0; i < numVertices; i++)
            {   
                triangles[i] = i;
            }

            // left side   
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void OnValidate(){
            mesh = CreateWedgeMesh();
            scantInterval = 1.0f / scanFrecuency;
        }
        private void OnDrawGizmos() {
            if(mesh){
                Gizmos.color = MeshColor;
                Gizmos.DrawMesh(mesh,transform.position,transform.rotation);
            }

            Gizmos.DrawWireSphere(transform.position,distance);

            for (int i = 0; i < count; i++)
            {
                Gizmos.DrawSphere(colliders[i].transform.position,0.2f);
            }

            Gizmos.color = Color.green;
            foreach (var obj in Objects)
            {
                 Gizmos.DrawSphere(obj.transform.position,0.2f);
            }
        }
    }
}
