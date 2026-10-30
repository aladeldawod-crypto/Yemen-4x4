using UnityEngine;

namespace Yemen4x4.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private bool target60Fps = true;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Application.targetFrameRate = target60Fps ? 60 : 30;
            QualitySettings.vSyncCount = 0;
            Input.multiTouchEnabled = true;
        }
    }
}
