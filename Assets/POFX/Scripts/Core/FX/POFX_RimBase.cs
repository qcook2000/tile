using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_RimBase : POFXLayer
        {
            public new const string path = "Highlight";

            [System.Serializable]
            public class Parameters
            {
                public float rimpower = .5f;
            }

            public Parameters m_params = new Parameters();


            public override void Init()
            {
                
                m_material = GetMaterial("POFX/Rim");
                base.Init();
            }
            

            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();
                
                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetColor("_Color", m_cParams.color);
                m_pb.SetFloat("_Outline", m_cParams.outline);
                m_pb.SetFloat("_RimPower", m_params.rimpower);
            }
        }
    }	
}
