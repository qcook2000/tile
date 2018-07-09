using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_SobelBase : POFXLayer
        {
            public new const string path = "Filter";
            public const bool spriteCompatible = true;

            [System.Serializable]
            public class Parameters
            {                
                public float invert = 0f;
                public float mixColor = 0f;
                
            }

            public Parameters m_params = new Parameters();


            public override void Init()
            {                
                m_material = GetMaterial("POFX/Sobel");
                base.Init();
            }

            
            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();                                
                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetColor("_Color", m_cParams.color);
                m_pb.SetFloat("_Outline", m_cParams.outline);
                m_pb.SetFloat("_Invert", m_params.invert);
                m_pb.SetFloat("_MixColor", m_params.mixColor);
            }
        }
    }	
}
