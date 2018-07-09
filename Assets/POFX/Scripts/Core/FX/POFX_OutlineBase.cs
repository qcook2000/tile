using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_OutlineBase : POFXLayer
        {
            public new const string path = "Highlight";

            [System.Serializable]
            public class Parameters
            {
                public float stencilRef = 128;
            }

            public Parameters m_params = new Parameters();

            public override void Init()
            {
                
                m_material = GetMaterial("POFX/Outline");
                base.Init();
            }


            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();

                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetColor("_Color", m_cParams.color);
                m_pb.SetFloat("_Outline", m_cParams.outline);    
                if(m_material.GetFloat("_StencilRef") != m_params.stencilRef )                            
                    m_material.SetFloat("_StencilRef", m_params.stencilRef);
            }
        }
    }	
}
