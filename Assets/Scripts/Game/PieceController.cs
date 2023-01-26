using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PieceController2D : MonoBehaviour
    {
        [SerializeField] private VisualManager visualManager = null;

        private Vector2 target;
        private Vector2 origin;
        float progress = float.NaN;
        float totalTime = 0f;

        public void Start()
        {
            visualManager ??= FindObjectOfType<VisualManager>();
        }

        public void MoveTo(V2 target, V2 origin, float time)
        {
            this.origin = origin.Vector2();
            this.target = target.Vector2();
            progress = 0f;
            totalTime = time;
        }

        public void Update()
        {
            if (progress is not float.NaN)
            {
                progress += Time.deltaTime / totalTime;

                if (progress > 1f) progress = 1f;

                visualManager.SizeGameObject(gameObject, origin + ((target - origin) * MathP.CosSmooth(progress)));

                if (progress == 1f) progress = float.NaN;
            }
        }
    }
}