using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldShaderControl : MonoBehaviour
{
    Material mat;
    public float brokenValue = 0.25f;
    bool breaking = false;

    public Color full;
    public Color medium;
    public Color broken;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    public void SetShieldValue(float rate)
    {
        if (!breaking)
        {
            if (rate > 0.25) mat.color = Color.Lerp(medium, full, rate);
            else mat.color = Color.Lerp(broken, medium, rate);
            float v = (1 - rate) * brokenValue;
            mat.SetFloat("_BreakAmount", v);
        }
    }

    IEnumerator BreakAnim()
    {
        breaking = true;
        float rate = 0.25f;

        while (rate < 1)
        {
            yield return new WaitForEndOfFrame();
            rate += Time.deltaTime * 0.75f;
            mat.SetFloat("_BreakAmount", rate);
        }
        breaking = false;
        gameObject.SetActive(false);        
    }
}
