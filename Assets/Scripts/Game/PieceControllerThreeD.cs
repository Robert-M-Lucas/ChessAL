using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceControllerThreeD : MonoBehaviour
{
    private Vector3 from;
    private Vector3 to;
    private bool jump;
    private float time;
    private float jumpHeight;
    private float progress = float.NaN;


    public void Move(Vector3 from, Vector3 to, bool jump, float time, float jumpHeight)
    {
        this.from= from;
        this.to = to;
        this.jump = jump;
        this.time = time;
        this.jumpHeight = jumpHeight;
        progress = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (progress is not float.NaN)
        {
            progress += Time.deltaTime / time;
            if (progress >= 1f) progress= 1f;

            float current_progress = MathP.CosSmooth(progress);

            Vector3 jump_v = Vector3.zero;

            if (jump)
            {
                jump_v += Vector3.up * MathP.SmoothJump(current_progress) * jumpHeight;
            }

            transform.localPosition = Vector3.Lerp(from, to, current_progress) + jump_v;

            if (progress == 1f) progress = float.NaN;
        }
    }
}
