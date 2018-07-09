using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_ColorSwapBase : POFXLayer
        {
            //public new const string path = "Color";
            //public const bool spriteCompatible = true;

            [System.Serializable]
            public class Parameters
            {
                public Color color2;
                public float threshold = 0;
                public float smooth = 0;
                public Texture2D mainTex;
            }

            public Parameters m_params = new Parameters();


            public override void Init()
            {
                m_material = GetMaterial("POFX/ColorSwap");
                base.Init();

            }
            

            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();
                if (m_params.mainTex != null)
                    m_pb.SetTexture("_MainTex", m_params.mainTex);
                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetColor("_Color", m_cParams.color);
                m_pb.SetColor("_Color2", m_params.color2);
                m_pb.SetFloat("_Threshold", m_params.threshold);
                m_pb.SetFloat("_Smooth", m_params.smooth);
                m_pb.SetFloat("_Outline", m_cParams.outline);                

            }
        }
    }	
}
