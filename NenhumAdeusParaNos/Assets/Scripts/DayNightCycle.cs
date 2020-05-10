using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class DayNightCycle : MonoBehaviour
{
    public PostProcessVolume _ppv;
    private ColorGrading _grading;
    
    private Vector4 dayLift = new Vector4(1, 1, 1, 0);
    private Vector4 dayGamma = new Vector4(1, 1, 1, 0);
    private Vector4 dayGain = new Vector4(1, 1, 1, 0);
    
    public Vector4 nightLift = new Vector4(1, 1, 1, -0.524659f);
    public Vector4 nightGamma = new Vector4(0, 0.1631746f, 1, -0.4931795f);
    public Vector4 nightGain = new Vector4(1, 1, 1, -0.6505772f);
    
    [Tooltip("x: 0~1, 24 horas\ny: 1 = dia, 0 = noite")]
    public AnimationCurve dayNightCurve = new AnimationCurve();

    public static DayNightCycle Instance;

    void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _ppv = GetComponent<PostProcessVolume>();
        _grading = _ppv.profile.GetSetting<ColorGrading>();

        dayLift = _grading.lift;
        dayGamma = _grading.gamma;
        dayGain = _grading.gain;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePostProcess(Time.time * 2f);
    }
    
    public void UpdatePostProcess(float time)
    {
        var lift = Vector4.Lerp(nightLift, dayLift, dayNightCurve.Evaluate(time / 24f));
        var gamma = Vector4.Lerp(nightGamma, dayGamma, dayNightCurve.Evaluate(time / 24f));
        var gain = Vector4.Lerp(nightGain, dayGain, dayNightCurve.Evaluate(time / 24f));
        
        _grading.lift.Override(lift);
        _grading.gamma.Override(gamma);
        _grading.gain.Override(gain);
    }
}
