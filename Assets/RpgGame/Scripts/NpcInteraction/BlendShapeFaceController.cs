using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeFaceController : MonoBehaviour
{
    public bool neutral = false;
    public bool happy = false;
    public bool sad = false;
    public bool love = false;
    public bool pleased = false;
    public bool angry = false;
    public bool confused = false;
    public bool crying = false;
    public SkinnedMeshRenderer face_SMR_BlendShapes;
    public SkinnedMeshRenderer eyes_SMR_BlendShapes;
    public SkinnedMeshRenderer tears_SMR_BlendShapes;

    public float blinkSpeed = 1f; // Speed at which the blend shape moves
    public float blinkDuration = 0.2f; // Duration of the blink
    private bool isBlinking = false; // Flag indicating if a blink is in progress

    public bool isTalking = false;
    public float checkInterval = 1f;
    private float nextCheckTime; // Time for the next check
    void Start()
    {
        StartCoroutine(PerformBlink());
    }
    public void UpdateBlendShape(SkinnedMeshRenderer smr, string blendShapeName, float weight)
    {
        int index = smr.sharedMesh.GetBlendShapeIndex(blendShapeName);
        if (index != -1)
        {
            smr.SetBlendShapeWeight(index, weight);
        }
        else
        {
            Debug.LogWarning($"Blendshape {blendShapeName} not found.");
        }
    }
    private void ChangeExpression(float eb_angry_op = 0f, float e_angry_op = 0f, float e_sad_op = 0f,
        float m_angry_op = 0f, float m_lonely_op = 0f, float t_big = 0f)
    {
        UpdateBlendShape(face_SMR_BlendShapes, "KK Eyebrows_angry_op", eb_angry_op);
        UpdateBlendShape(face_SMR_BlendShapes, "KK Eyes_angry_op", e_angry_op);
        UpdateBlendShape(face_SMR_BlendShapes, "KK Eyes_sad_op", e_sad_op);

        if (!isTalking)
        {
            UpdateBlendShape(face_SMR_BlendShapes, "KK Mouth_angry_op", m_angry_op);
            UpdateBlendShape(face_SMR_BlendShapes, "KK Mouth_lonely_op", m_lonely_op);
        }
        else {
            UpdateBlendShape(face_SMR_BlendShapes, "KK Mouth_angry_op", 0);
            UpdateBlendShape(face_SMR_BlendShapes, "KK Mouth_lonely_op", 0);
        }
        UpdateBlendShape(tears_SMR_BlendShapes, "Tears big", t_big);
    }
    public void NeutralExpression()
    {
        angry = false;
        sad = false;
        neutral = true;
        ChangeExpression();
    }
    public void Update()
    {
        if (neutral)
            NeutralExpression();
        else if (angry)
            AngryExpression();
        else if (sad)
            SadExpression();

        //Blink
        //UpdateBlendShape(face_SMR_BlendShapes, "KK Eyes_default_cl", 100f);

        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;

            // Check if someone is talking
            if (isTalking)
            {
                StartCoroutine(PerformTalk());
            }
        }

        //UpdateBlendShape(face_SMR_BlendShapes,"KK Eyebrows_angry_cl", 100.0f);

        //UpdateBlendShape(eyes_SMR_BlendShapes, "Gag eye 02", 100.0f);
        //UpdateBlendShape(eyes_SMR_BlendShapes, "Gag eye 01", 100.0f);

        //UpdateBlendShape(tears_SMR_BlendShapes, "Tears big", 100.0f);
    }
    public void AngryExpression()
    {
        angry = true;
        sad = false;
        neutral = false;
        ChangeExpression(eb_angry_op: 100f, e_angry_op: 100f, m_angry_op: 100f);
    }
    public void SadExpression()
    {
        angry = false;
        sad = true;
        neutral = false;
        ChangeExpression(e_sad_op: 100f, m_lonely_op: 100f, t_big: 100f);
    }
    public void ConfusedExpression()
    {
        ChangeExpression(e_sad_op: 100f, m_lonely_op: 100f, t_big: 100f);
    }
    public void ConfusedLove()
    {
        ChangeExpression(e_sad_op: 100f, m_lonely_op: 100f, t_big: 100f);
    }
    public void ChangeTalking()
    {
        isTalking = !isTalking;
    }
    IEnumerator PerformBlink()
    {
        while (true)
        {
            // Start the blink
            isBlinking = true;

            // Move the blend shape from 0 to 100
            float elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                float blendValue = Mathf.Lerp(0f, 100f, elapsedTime / blinkDuration);
                UpdateBlendShape(face_SMR_BlendShapes, "KK Eyes_default_cl", blendValue);
                elapsedTime += Time.deltaTime * blinkSpeed;
                yield return null;
            }

            // Move the blend shape from 100 back to 0
            elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                float blendValue = Mathf.Lerp(100f, 0f, elapsedTime / blinkDuration);
                UpdateBlendShape(face_SMR_BlendShapes, "KK Eyes_default_cl", blendValue);
                elapsedTime += Time.deltaTime * blinkSpeed;
                yield return null;
            }

            // End the blink
            isBlinking = false;

            // Wait for a random interval before the next blink
            float blinkInterval = Random.Range(1, 3);
            yield return new WaitForSeconds(blinkInterval);
        }
    }
    IEnumerator PerformTalk()
    {
        while (isTalking)
        {
            // Move the blend shape from 0 to 100
            float elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                float blendValue = Mathf.Lerp(0f, 100f, elapsedTime / blinkDuration);
                UpdateBlendShape(face_SMR_BlendShapes, "KK Mouth_a_big_op", blendValue);
                elapsedTime += Time.deltaTime * blinkSpeed;
                yield return null;
            }

            // Move the blend shape from 100 back to 0
            elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                float blendValue = Mathf.Lerp(100f, 0f, elapsedTime / blinkDuration);
                UpdateBlendShape(face_SMR_BlendShapes, "KK Mouth_a_big_op", blendValue);
                elapsedTime += Time.deltaTime * blinkSpeed;
                yield return null;
            }

            // Wait for a random interval before the next blink
            //float blinkInterval = Random.Range(1, 3);
            yield return new WaitForSeconds(1);
        }
    }
}
