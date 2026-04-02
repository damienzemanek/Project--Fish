using System;
using System.Collections;
using EMILtools.Extensions;
using UnityEngine;

public class DamageFlasher : MonoBehaviour
{
    static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
    static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
    
    [Range(0, 1)] public float flashTime = 0.5f;
    [ColorUsage(true, true)] public Color flashColor = Color.white;
    public AnimationCurve flashCurve;
    public SpriteRenderer[] sprites;
    Material[] mats;

    Coroutine flashCoroutine;

    void Awake()
    {
        if(sprites == null || sprites.Length == 0) { Debug.LogError("No Sprites Assigned to Damage Flasher"); return; }
        mats = new Material[sprites.Length];
        
        for(int i = 0; i < sprites.Length; i++) 
            mats[i] = sprites[i].material;
        
        SetFlashColor();
    }

    public void Flash() => flashCoroutine = StartCoroutine(C_Flash());

    IEnumerator C_Flash()
    {
        Debug.Log("Flashing");
        float currAmount = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            float eval = Mathf.Clamp01(elapsedTime / flashTime);
            currAmount = flashCurve.Evaluate(eval);
            SetFlashAmount(currAmount);
            yield return null;
        }
    }

    void SetFlashColor()
    {
        for (int i = 0; i < sprites.Length; i++)
            sprites[i].material.SetColor(FlashColor, flashColor);
    }

    void SetFlashAmount(float currAmount)
    {
        for(int i = 0; i < sprites.Length; i++)
            mats[i].SetFloat(FlashAmount, currAmount);
    }
}
