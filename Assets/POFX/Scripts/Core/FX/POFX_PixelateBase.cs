using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_PixelateBase : POFXLayer
        {
            public new const string path = "Filter";
            public const bool spriteCompatible = true;

            [System.Serializable]
            public class Parameters
            {
                public float pixelSnapNear = 10;
                public float pixelSnapFar = 3;
                public float distanceCamFar = 10;                
            }

            public Parameters m_params = new Parameters();


            public override void Init()
            {
                
                m_material = GetMaterial("POFX/Pixelate");
                base.Init();
            }

            
            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();

                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetColor("_Color", m_cParams.color);
                m_pb.SetFloat("_PixelSnapNear", m_params.pixelSnapNear);
                m_pb.SetFloat("_PixelSnapFar", m_params.pixelSnapFar);
                m_pb.SetFloat("_DistanceCamFar", m_params.distanceCamFar);                
                m_pb.SetFloat("_Outline", m_cParams.outline);

            }
        }
    }	
}
