using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public Camera cameraToFollow;
    public ParticleSystem rainSystem;
    public ParticleSystem snowSystem;
    public float intensity = 0.0f;   // 0~1
    public float windX = 0.0f;       // -5~+5 권장
    public bool enableRain = true;
    public bool enableSnow = false;

    void Awake()
    {
        if (cameraToFollow == null)
        {
            cameraToFollow = Camera.main;
        }
        FollowCameraImmediate();
    }

    void LateUpdate()
    {
        FollowCameraImmediate();

        ApplySettings(rainSystem, enableRain == true);
        ApplySettings(snowSystem, enableSnow == true);
    }

    void FollowCameraImmediate()
    {
        if (cameraToFollow == null)
        {
            return;
        }
        Vector3 camPos = cameraToFollow.transform.position;
        transform.position = new Vector3(camPos.x, camPos.y, transform.position.z);
    }

    void ApplySettings(ParticleSystem ps, bool enabled)
    {
        if (ps == null)
        {
            return;
        }

        var emission = ps.emission;
        var main = ps.main;
        var vel = ps.velocityOverLifetime;

        if (enabled == true)
        {
            if (ps.isPlaying == false)
            {
                //ps.gameObject.SetActive(true);
                ps.Play();
            }

            // 1) Emission rate: intensity 기반
            float baseRate = 80f; // 프로젝트에 맞게 조정
            float rate = baseRate * Mathf.Clamp01(intensity);

            ParticleSystem.MinMaxCurve rateCurve = emission.rateOverTime;
            rateCurve.constant = rate;

            // 2) 바람: X축 속도 가산(월드 공간)
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.World;
            ParticleSystem.MinMaxCurve vx = vel.x;
            vx.constant = windX;
            vel.x = vx;

            // 3) 속도/크기 보정(강도가 세질수록 더 빠르고 약간 커짐)
            ParticleSystem.MinMaxCurve spd = main.startSpeed;
            spd.constant = Mathf.Lerp(1.5f, 8.0f, Mathf.Clamp01(intensity));
            main.startSpeed = spd;

            ParticleSystem.MinMaxCurve sz = main.startSize;
            sz.constant = Mathf.Lerp(0.05f, 0.15f, Mathf.Clamp01(intensity));
            main.startSize = sz;
        }
        else
        {
            if (ps.isPlaying == true)
            {
                ps.Stop();
                //ps.gameObject.SetActive(false);
            }
        }
    }
}
