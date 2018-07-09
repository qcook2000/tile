using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_EraserBase : POFXLayer
        {
            public new const string path = "Other";
            public const bool spriteCompatible = true;

            [System.Serializable]
            public class Parameters
            {
                public float blur = 200;
            }

            public Parameters m_params = new Parameters();


            public override void Init()
            {
                
                m_material = GetMaterial("POFX/Eraser");
                base.Init();
            }


            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();
                m_pb.SetFloat("_Outline", m_cParams.outline);
            }
        }
    }
}
