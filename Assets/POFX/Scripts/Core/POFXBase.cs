using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace Kalagaan
{
    namespace POFX
    {        

        [ExecuteInEditMode]
        public class POFXBase : MonoBehaviour
        {
            public static string version = "1.1.0";
            public string m_version = "1.1.0";

            Mesh m_mesh;
            public List<POFXLayer> m_layers = new List<POFXLayer>();            
            public bool m_hideRenderer = false;
            public bool m_hideEditorSelection = false;
            //public List<Material> m_materials = new List<Material>();

            
            Renderer m_renderer = null;
            bool m_skinnedMesh = false;
            Vector3[] m_smoothNormals = null;
            
            public SpriteRenderer m_spriteRenderer;

            public int layerCount
            {
                get { return m_layers.Count; }
            }


            void Awake()
            {
                Init();
            }

            void Init()
            {
                //m_pb = new MaterialPropertyBlock();
                m_renderer = gameObject.GetComponent<MeshRenderer>();

                if (m_renderer == null)
                {
                    m_renderer = gameObject.GetComponent<SkinnedMeshRenderer>();
                    m_skinnedMesh = true;
                    m_mesh = new Mesh();
                }
                else
                {
                    MeshFilter mf = gameObject.GetComponent<MeshFilter>();
                    m_skinnedMesh = false;
                    m_mesh = new Mesh();
                    m_mesh.vertices = mf.sharedMesh.vertices;
                    m_mesh.triangles = mf.sharedMesh.triangles;
                    m_mesh.normals = mf.sharedMesh.normals;
                    ComputeSmoothNormals();
                    m_mesh.normals = m_smoothNormals;
                }


                if (gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    m_skinnedMesh = false;
                    m_renderer = m_spriteRenderer;
                }



                if (m_renderer == null)
                {
                    Debug.LogError("POFX require a MeshRenderer or a skinnedMeshRender");
                    m_skinnedMesh = false;
                }

            }


            public void AddLayer(POFXLayer layer)
            {                
                m_layers.Add(layer);
                layer.Init();
                layer.hideFlags = HideFlags.HideInInspector;
            }

            

            public POFXLayer GetLayer( int id )
            {
                if (id < 0 || id > m_layers.Count - 1)
                    return null;
                return m_layers[id];
            }


            public POFXLayer GetLayer<T>()
            {
                for (int i = 0; i < m_layers.Count - 1; ++i)
                    if ((m_layers[i].GetType() == typeof(T) ))
                        return m_layers[i];
                return null;
            }


            public POFXLayer[] GetLayers<T>()
            {
                List<POFXLayer> lst = new List<POFXLayer>();
                for (int i = 0; i < m_layers.Count - 1; ++i)
                    if ((m_layers[i].GetType() == typeof(T)))
                        lst.Add(m_layers[i]);
                return lst.ToArray();
            }


            public void ComputeSmoothNormals()
            {
                Dictionary<Vector3, List<int>> verticesPos = new Dictionary<Vector3, List<int>>();
                Vector3[] Vertices = m_mesh.vertices;
                m_smoothNormals = m_mesh.normals;
                
                if (Vertices == null)
                    return;

                for (int i = 0; i < Vertices.Length; ++i)
                {
                    if (!verticesPos.ContainsKey(Vertices[i]))
                    {
                        verticesPos.Add(Vertices[i], new List<int>());
                        verticesPos[Vertices[i]].Add(i);
                    }
                    else
                    {
                        verticesPos[Vertices[i]].Add(i);
                    }
                }

                foreach (KeyValuePair<Vector3, List<int>> kvp in verticesPos)
                {
                    Vector3 norm = Vector3.zero;
                    for (int n = 0; n < kvp.Value.Count; ++n)
                    {
                        norm += m_smoothNormals[kvp.Value[n]];
                    }
                    norm.Normalize();

                    for (int n = 0; n < kvp.Value.Count; ++n)
                    {
                        m_smoothNormals[kvp.Value[n]] = norm;
                    }
                }

            }


            public void RemoveLayer( POFXLayer l )
            {
                m_layers.Remove(l);
                l.Clean();

                if (Application.isPlaying)
                   Destroy(l);
                else
                   DestroyImmediate(l);
            }


            public void Refresh()
            {
                LateUpdate();
            }

            public void LateUpdate()
            {
                
                    
                //m_renderer.material.renderQueue = 2001;

                if (m_mesh == null)
                    Init();

                if (m_renderer != null)
                    m_renderer.enabled = !m_hideRenderer;

                /*
                if(m_rendererClone == null)
                {
                    GameObject go = new GameObject("POFX");
                    m_rendererClone = go.AddComponent<SkinnedMeshRenderer>();
                    m_rendererClone.transform.parent = transform;
                    m_rendererClone.transform.localScale = Vector3.one;
                    m_rendererClone.transform.localPosition = Vector3.zero;
                    m_rendererClone.transform.localRotation = Quaternion.identity;
                    m_rendererClone.materials = new Material[m_layers.Count];
                }*/


                if (m_skinnedMesh)
                {
                    (m_renderer as SkinnedMeshRenderer).BakeMesh(m_mesh);
                    if (m_smoothNormals == null)
                        ComputeSmoothNormals();
                    m_mesh.normals = m_smoothNormals;
                }

                m_layers.Remove(null);

                for (int i=0; i < m_layers.Count; ++i)
                {
                    if (m_layers[i].m_material == null)
                        m_layers[i].Init();

                    m_layers[i].Refresh();

                    if (m_layers[i].m_cParams.intensity == 0 || !m_layers[i].m_enable)
                        continue;

                    m_layers[i].m_material.renderQueue = m_layers[i].m_renderQueue;

                    m_layers[i].UpdatePropertyBlock();

                    if (m_spriteRenderer == null)
                    {
                        Matrix4x4 m = Matrix4x4.TRS(transform.position, transform.rotation, m_skinnedMesh ? Vector3.one : transform.lossyScale);

                        if (m_mesh.subMeshCount <= 1)
                            Graphics.DrawMesh(m_mesh, m, m_layers[i].m_material, gameObject.layer, null, 0, m_layers[i].m_pb);
                        else
                        {
                            for (int j = 0; j < m_mesh.subMeshCount; ++j)
                            {
                                if (m_layers[i].m_enableMaterials.Count < m_mesh.subMeshCount)
                                    m_layers[i].m_enableMaterials.Add(true);

                                if(m_layers[i].m_enableMaterials[j])
                                    Graphics.DrawMesh(m_mesh, m, m_layers[i].m_material, gameObject.layer, null, j, m_layers[i].m_pb);
                            }
                        }
                    }
                    else
                    {
                        if (m_layers[i].m_spriteRenderer == null)
                            m_layers[i].Init();                       
                            
                        m_layers[i].m_spriteRenderer.SetPropertyBlock(m_layers[i].m_pb);
                    }                    
                }
            }



                        
        }
    }
}
