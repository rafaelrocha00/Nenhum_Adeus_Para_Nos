using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EffectBattleStart : MonoBehaviour
{
    [SerializeField] private Shader shader;
    [SerializeField] private bool renderWithReplacementShader;
    [SerializeField] private float range = 10;

    [SerializeField] private GameObject postProcessVolume;

    private ColorGrading _colorGrading;
    private Camera _camera;
    private static readonly int Range = Shader.PropertyToID("_Range");

    private void Start()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        if (shader == null)
        {
            shader = Shader.Find("Hidden/EffectBattleStart");
        }

        if (postProcessVolume == null)
        {
            Debug.LogError("No post process volume assigned");
        }
        
        var pp = postProcessVolume.GetComponent<PostProcessVolume>();
        pp.profile.TryGetSettings(out _colorGrading);

        StartCoroutine("StartBattleAnimation");
        Invoke("Stop", 5);
    }

    private void Update()
    {
        if (renderWithReplacementShader)
        {
            renderWithReplacementShader = false;
            StartCoroutine(StartBattleAnimation());
        }
    }

    private void RenderWithShader(bool b)
    {
        if (b)
        {
            _camera.SetReplacementShader(shader, "RenderType");
        }
        else
        {
            _camera.ResetReplacementShader();
        }
    }

    public IEnumerator StartBattleAnimation()
    {
        for (int i = 0; i < 20; i++)
        {
            _colorGrading.colorFilter.value *= 0.5f;
            yield return new WaitForSeconds (0.05f);
        }
        
        RenderWithShader(true);
        Shader.SetGlobalFloat(Range, 0);
        
        _colorGrading.colorFilter.value = Color.white;
        
        for (float i = 0; i < range + 1; i += 0.25f)
        {
            Shader.SetGlobalFloat(Range, i);
            yield return new WaitForSeconds (0.05f);
        }
    }
    
    public IEnumerator StopBattleAnimation()
    {
        RenderWithShader(false);
        
        Shader.SetGlobalFloat(Range, range);
        
        _colorGrading.colorFilter.value = Color.white;
        
        for (float i = 0; i < range + 1; i += 0.25f)
        {
            Shader.SetGlobalFloat(Range, range - i);
            yield return new WaitForSeconds (0.05f);
        }
    }

    void Stop()
    {
        StartCoroutine("StopBattleAnimation");
    }
}