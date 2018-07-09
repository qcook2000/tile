using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_RefractionBase : POFXLayer
        {
            public new const string path = "Deformation";

            [System.Serializable]
            public class Parameters
            {
                public Texture2D distorsionTex;
                public Vector2 distorsionTexScale = Vector2.one;
                public Vector2 distorsionTexOffset = Vector2.zero;
                public float distorsion = 1f;
                public float refraction = 10f;
                public float fresnel = .8f;
                
            }

            public Parameters m_params = new Parameters();


            public override void Init()
            {
                
                m_material = GetMaterial("POFX/Refraction");
                base.Init();
            }

            
            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();

                if (m_params.distorsionTex != null)
                {
                    m_pb.SetTexture("_DistorsionTex", m_params.distorsionTex);
                    m_pb.SetVector("_DistorsionTexScale", m_params.distorsionTexScale);
                    m_pb.SetVector("_DistorsionTexOffset", m_params.distorsionTexOffset);
                }
                m_pb.SetFloat("_Distorsion", m_params.distorsion);
                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetColor("_Color", m_cParams.color);
                m_pb.SetFloat("_Outline", m_cParams.outline);
                m_pb.SetFloat("_Refraction", m_params.refraction);
                m_pb.SetFloat("_Fresnel", m_params.fresnel);

            }
        }
    }	
}
