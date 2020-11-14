using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class ButtonDephtOfField : MonoBehaviour
{
    [SerializeField]PostProcessVolume _volume = null;
    DepthOfField _dof;

    Button butao;
    void Start()
    {
        butao = GetComponent<Button>();
        _dof = _volume.profile.GetSetting<DepthOfField>();
        butao.onClick.AddListener(click);
        _dof.active = true;
        _dof.focusDistance.value = 2.65f;
    }

    void click()
    {
        _dof.active = false;
    }

}
