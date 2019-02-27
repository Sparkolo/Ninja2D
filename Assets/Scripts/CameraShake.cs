using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Cinemachine.CinemachineVirtualCamera vcam;
    Cinemachine.CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        vcam = GameObject.Find("CM vcam1").GetComponent<Cinemachine.CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    public void Noise()
    {
        noise.m_AmplitudeGain = Random.value  * 5 - 1f;
        noise.m_FrequencyGain = Random.value  * 5 - 1f;
    }

    public void Shake (float length)
    {
        InvokeRepeating("Noise", 0.2f, 0.2f);
        Invoke("StopShake", length);
    }

    void StopShake()
    {
        CancelInvoke("Noise");
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }
}
