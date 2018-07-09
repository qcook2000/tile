using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_MatcapBase : POFXLayer
        {
            public new const string path = "Highlight";

            [System.Serializable]
            public class Parameters
            {
                public Texture2D matcap;
                public float mix = .5f;
                public float addMul = .5f;
            }

            public Parameters m_params = new Parameters();

            public override void Init()
            {
                
                m_material = GetMaterial("POFX/Matcap");
                base.Init();
            }


            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();

                if (m_params.matcap != null)
                    m_pb.SetTexture("_MainTex", m_params.matcap);
                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetColor("_Color", m_cParams.color);
                m_pb.SetFloat("_Outline", m_cParams.outline);
                m_pb.SetFloat("_Mix", m_params.mix);
                m_pb.SetFloat("_AddMul", m_params.addMul);
            }
        }
    }
}
