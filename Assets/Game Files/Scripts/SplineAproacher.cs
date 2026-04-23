using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))] // ✅ ensure it exists
public class SplineAproacher : MonoBehaviour
{
    public bool inuse;
    
    public SplineAnimate splineAnimate;
    public bool passed = false;
    public float myProgCompletePosition = 1f;

    public SpriteRenderer[] sprites;
    public SpriteRenderer yellowTinter;

    public Vector3 resetScale;
    public Vector3 scaleMin;
    public Transform scaleTarg;

    public float prog;

    // --- ADDED ---
    public int lineResolution = 50;
    private SplineContainer splineContainer;
    private LineRenderer lineRenderer;
    // --- END ADDED ---
    
    void OnEnable()
    {
        passed = false;
        if(sprites == null) sprites = new SpriteRenderer[0];

        // --- ADDED ---
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        if (splineAnimate != null) splineContainer = splineAnimate.Container;

        GenerateLine();
        // --- END ADDED ---
    }
    
    void Update()
    {
        
        if (!inuse) return;

        prog = splineAnimate.NormalizedTime;

        passed = splineAnimate.ElapsedTime >= myProgCompletePosition + (myProgCompletePosition * 0.2f);

        if (passed)
        {
            SetLineAlpha(0f);
            // make the yellow tinter disapaer
            foreach (var sprite in sprites)
            {
                Color c = sprite.color;
                c.a = 1 - prog - (prog * 0.2f); // 🔒 unchanged
                sprite.color = c;
            }

            // fade yellowTinter out as well
            if (yellowTinter != null)
            {
                Color c = yellowTinter.color;
                c.a = 0f;
                yellowTinter.color = c;
            }

            return;
        }
        else
        {
            GenerateLine();
            
            SetLineAlpha(1f);
            // make the yellow tinter appear
            foreach (var sprite in sprites)
            {
                Color c = sprite.color;
                c.a = 1;
                sprite.color = c;
            }
            // fade yellowTinter in from 0 → 0.5 as prog approaches myProgCompletePosition
            if (yellowTinter != null)
            {
                float t = Mathf.Clamp01(prog / myProgCompletePosition) * (splineAnimate.Duration / myProgCompletePosition);

                float alpha;

                Color yl = Color.yellow;
                Color rd = Color.red;
                Color c = yellowTinter.color;

                float thresh1 = 0.5f;
                float thresh2 = 0.8f;

                if (t < thresh1)
                {
                    alpha = 0f;
                }
                else if (t < thresh2)
                {
                    c = rd;
                    float ft = Mathf.InverseLerp(thresh1, thresh2, t);
                    alpha = Mathf.Lerp(0f, 0.6f, ft / 4);
                }
                else
                {
                    c = yl;
                    alpha = 0.4f;
                }

                c.a = alpha;
                yellowTinter.color = c;
            }
        }
        scaleTarg.localScale = Vector3.Lerp(resetScale, scaleMin, prog);
        scaleTarg.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0, 0, 180), prog * 2);
    }

    // --- ADDED ---
    [Button]
    public void GenerateLine()
    {
        if (splineAnimate == null || splineAnimate.Container == null) return;

        splineContainer = splineAnimate.Container;

        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null) return;

        lineRenderer.positionCount = lineResolution;

        for (int i = 0; i < lineResolution; i++)
        {
            float t = i / (float)(lineResolution - 1);
            Vector3 pos = splineContainer.EvaluatePosition(t);
            lineRenderer.SetPosition(i, pos);
        }
    }
    // --- END ADDED ---

    void SetLineAlpha(float alpha)
    {
        if (lineRenderer == null) return;

        Color c1 = lineRenderer.startColor;
        Color c2 = lineRenderer.endColor;

        c1.a = alpha;
        c2.a = alpha;

        lineRenderer.startColor = c1;
        lineRenderer.endColor = c2;
    }
    
    [Button]
    public void Reset()
    {
        scaleTarg.localScale = resetScale;
    }
}