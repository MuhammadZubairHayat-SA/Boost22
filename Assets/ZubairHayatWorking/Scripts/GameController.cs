using System;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        StartButton();
    }

    void StartButton()
    {

#if UNITY_IOS
        Version currentVersion = new Version(UnityEngine.iOS.Device.systemVersion);
        Version ios14 = new Version("14.0");

        if (currentVersion >= ios14 && ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
            ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif


        SkillzCrossPlatform.LaunchSkillz(new SkillzGameController());
    }


    void Start()
    {

    }
}
