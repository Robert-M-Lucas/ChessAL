using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    [System.Serializable]
    public class CameraMotion
    {
        public Vector3 From;
        public Vector3 FromRotation;
        public Vector3 To;
        public Vector3 ToRotation;
        public float Time;
    }

    public class CameraMoveScript : MonoBehaviour
    {
        public float FadeInPoint;
        public float FadeOutPoint;

        public float PauseBetween;

        public AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.5f, 1f, 1f), new Keyframe(1, 1));

        public CameraMotion[] Motions;

        public Shader FadeShader;

        private int motionIndex = 0;
        private float progress = 0f;

        private bool paused = true;

        private Material m_renderMaterial = null;


        // Start is called before the first frame update
        void Start()
        {
            m_renderMaterial = new Material(FadeShader);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, m_renderMaterial);
        }

        // Update is called once per frame
        void Update()
        {
            if (paused)
            {
                Shader.SetGlobalFloat("_Alpha", 0);
                progress += Time.deltaTime;
                if (progress >= PauseBetween)
                {
                    paused = false;
                    progress = 0f;
                }
                else return;
            }

            progress += Time.deltaTime / Motions[motionIndex].Time;
            if (progress >= 1f)
            {
                progress = 0f;
                motionIndex++;
                if (motionIndex >= Motions.Length) { motionIndex = 0; }
                paused = true;
                return;
            }

            if (progress < FadeInPoint)
            {
                Shader.SetGlobalFloat("_Alpha", FadeCurve.Evaluate(progress / FadeInPoint));
            }
            else if (progress >= FadeOutPoint)
            {
                Shader.SetGlobalFloat("_Alpha", FadeCurve.Evaluate(1f - ((progress - FadeOutPoint) / (1f - FadeOutPoint))));
            }
            else
            {
                Shader.SetGlobalFloat("_Alpha", 1);
            }

            transform.position = Vector3.Lerp(Motions[motionIndex].From, Motions[motionIndex].To, progress);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(Motions[motionIndex].FromRotation), Quaternion.Euler(Motions[motionIndex].ToRotation), progress);
        }
    }

}
