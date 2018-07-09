using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kalagaan
{
    namespace POFX
    {
        public class POFXLayer : MonoBehaviour
        {
            [System.Serializable]
            public class CommonParameters
            {                
                public float intensity = 1;
                public Color color = Color.white;
                public float outline = .0001f;
            }

            public static Dictionary<string, Material> m_materialLibrary;
            public Material m_material = null;
            public MaterialPropertyBlock m_pb;
            public CommonParameters m_cParams = new CommonParameters();
            public const string path = "";
            public int m_renderQueue = 0;
            public int m_sortingOrderOffset = 0;
            public bool m_enable = true;
            public SpriteRenderer m_spriteRendererRef;
            public SpriteRenderer m_spriteRenderer;
            public List<bool> m_enableMaterials = new List<bool>();

            public void Awake()
            {
                Init();

                POFXBase pofx = GetComponent<POFXBase>();
                if(pofx == null)
                {
                    if (Application.isPlaying)
                        Destroy(this);
                    else
                        DestroyImmediate(this);
                }
            }

            public string GetName()
            {
                return m_material.shader.name.Replace("POFX/", "");
            }


            public virtual void Init()
            {
                if(m_renderQueue == 0 && m_material !=null)
                    m_renderQueue = m_material.shader.renderQueue;

                if (m_material.shader.renderQueue != m_renderQueue)
                {
                    m_material = new Material(m_material);
                    m_material.renderQueue = m_renderQueue;
                }

                MeshRenderer mr = GetComponent<MeshRenderer>();
                if(mr != null)
                {
                    while( m_enableMaterials.Count < mr.sharedMaterials.Length )
                        m_enableMaterials.Add(true);
                    
                }

                SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    while (m_enableMaterials.Count < smr.sharedMaterials.Length)                    
                        m_enableMaterials.Add(true);                    
                }


                m_spriteRendererRef = GetComponent<SpriteRenderer>();
                if(m_spriteRendererRef != null && m_spriteRenderer == null )
                {
                    GameObject go = new GameObject(GetName());
                    go.transform.parent = transform;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    go.transform.localRotation = Quaternion.identity;
                    m_spriteRenderer = go.AddComponent<SpriteRenderer>();
                    m_spriteRenderer.material = m_material;
                    //m_sortingOrderOffset = 1;
                    m_material.EnableKeyword("SPRITE_RENDERER");
                    UpdateSprite();
                }

                if(m_spriteRendererRef==null)
                    m_material.DisableKeyword("SPRITE_RENDERER");


            }

            public void Refresh()
            {
                if(m_spriteRenderer!=null)
                    m_spriteRenderer.enabled = m_enable && m_cParams.intensity>0;
            }


            public void UpdateSprite()
            {
                m_spriteRenderer.enabled = m_enable;
                m_spriteRenderer.sprite = m_spriteRendererRef.sprite;
                m_spriteRenderer.flipX = m_spriteRendererRef.flipX;
                m_spriteRenderer.flipY = m_spriteRendererRef.flipY;
                m_spriteRenderer.sortingOrder = m_spriteRendererRef.sortingOrder + m_sortingOrderOffset;
                m_spriteRenderer.sortingLayerID = m_spriteRendererRef.sortingLayerID;
                m_spriteRenderer.gameObject.hideFlags = HideFlags.HideAndDontSave;
                //m_spriteRenderer.gameObject.hideFlags = HideFlags.None;
                m_spriteRenderer.drawMode = m_spriteRendererRef.drawMode;
            }


            public Material GetMaterial(string shaderName)
            {
                if (m_materialLibrary == null)
                    m_materialLibrary = new Dictionary<string, Material>();

                if (!m_materialLibrary.ContainsKey(shaderName))
                    m_materialLibrary.Add(shaderName, new Material(Shader.Find(shaderName)));
                else if(m_materialLibrary[shaderName] == null )
                    m_materialLibrary[shaderName] = new Material(Shader.Find(shaderName));

                return m_materialLibrary[shaderName];
            }


            public virtual void UpdatePropertyBlock()
            {
                if (m_pb == null)
                    m_pb = new MaterialPropertyBlock();

                if (m_spriteRenderer != null)
                {
                    UpdateSprite();
                    m_pb.SetTexture("_MainTex", m_spriteRenderer.sprite.texture);
                }
            }





            public virtual void Clean()
            {
                if (m_spriteRenderer != null)
                {
                    if (Application.isPlaying)
                        Destroy(m_spriteRenderer.gameObject);
                    else
                        DestroyImmediate(m_spriteRenderer.gameObject);
                }
            }



        }
    }
}