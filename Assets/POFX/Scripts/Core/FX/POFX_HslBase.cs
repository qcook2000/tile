using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    namespace POFX
    {
        public class POFX_HslBase : POFXLayer
        {
            public new const string path = "Color";
            public const bool spriteCompatible = true;

            [System.Serializable]
            public class Parameters
            {
                public float hue = 0;
                public float brightness = 0;
                public float contrast = 0;
                public float saturation = 0;
                public float invert = 0f;
            }

            public Parameters m_params = new Parameters();


            public override void Init()
            {
                m_material = GetMaterial("POFX/Hsl");
                base.Init();

            }
            

            public override void UpdatePropertyBlock()
            {
                base.UpdatePropertyBlock();
                m_pb.SetFloat("_Intensity", m_cParams.intensity);
                m_pb.SetColor("_Color", m_cParams.color);
                m_pb.SetFloat("_Outline", m_cParams.outline);
                m_pb.SetFloat("_Hue", m_params.hue);
                m_pb.SetFloat("_Brightness", m_params.brightness);
                m_pb.SetFloat("_Contrast", m_params.contrast);
                m_pb.SetFloat("_Saturation", m_params.saturation);
                m_pb.SetFloat("_Invert", m_params.invert);
            }
        }
    }	
}
