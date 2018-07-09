using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_BlurBase : POFXLayer
        {
            public new const string path = "Filter";
            public const bool spriteCompatible = true;

            [System.Serializable]
            public class Parameters
            {
                public float blurNear = 3;
                public float blurFar = 10;
                public float distanceCamFar = 10;
            }

            public Parameters m_params = new Parameters();


            public override void Init()
            {                
                m_material = GetMaterial("POFX/Blur");
                base.Init();
            }
            

            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();
                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetFloat("_BlurNear", m_params.blurNear);
                m_pb.SetFloat("_BlurFar", m_params.blurFar);
                m_pb.SetFloat("_Outline", m_cParams.outline);
            }
        }
    }	
}
