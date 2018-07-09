using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_BlurBase), true)]
        public class POFX_BlurEditor : POFXLayerEditor
        {

            public override void DrawUI()
            {
                POFX_BlurBase blur = target as POFX_BlurBase;

                if (blur == null)
                    return;

                blur.m_cParams.intensity = EditorGUILayout.Slider("Intensity", blur.m_cParams.intensity, 0, 1);
                blur.m_params.blurNear = EditorGUILayout.FloatField("Blur Near", blur.m_params.blurNear);
                blur.m_params.blurFar = EditorGUILayout.FloatField("Blur Far", blur.m_params.blurFar);
                blur.m_params.distanceCamFar = EditorGUILayout.FloatField("Distance cam Far", blur.m_params.distanceCamFar);
                blur.m_cParams.outline = EditorGUILayout.FloatField("Outline", blur.m_cParams.outline);
            }
        }
    }
}
