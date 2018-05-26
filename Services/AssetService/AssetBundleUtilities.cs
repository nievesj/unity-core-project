using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Core.Services.Assets
{
    public static class AssetBundleUtilities
    {
        /// <summary>
        /// Returns string "android", "webgl", or "ios", depending on the running client. Useful for
        /// downloading the correct asset bundle target. If running in the Editor, returns the same
        /// string based on the build target. Throws an exception if detected running on another platform.
        /// </summary>
        public static AssetDeviceType ClientPlatform
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WebGLPlayer:
                        return AssetDeviceType.WebGL;

                    case RuntimePlatform.IPhonePlayer:
                        return AssetDeviceType.iOS;

                    case RuntimePlatform.Android:
                        return AssetDeviceType.Android;

                    case RuntimePlatform.WindowsPlayer:
                        return AssetDeviceType.StandaloneWindows64;

                    case RuntimePlatform.OSXPlayer:
                        return AssetDeviceType.StandaloneOSXUniversal;

                    default:
#if UNITY_EDITOR
                        switch (EditorUserBuildSettings.activeBuildTarget)
                        {
                            case BuildTarget.WebGL:
                                return AssetDeviceType.WebGL;

                            case BuildTarget.iOS:
                                return AssetDeviceType.iOS;

                            case BuildTarget.Android:
                                return AssetDeviceType.Android;

                            case BuildTarget.StandaloneWindows64:
                                return AssetDeviceType.StandaloneWindows64;

                            case BuildTarget.StandaloneOSX:
                                return AssetDeviceType.StandaloneOSX;

                            default:
                                throw new System.Exception("Build target unsupported as client platform: " + EditorUserBuildSettings.activeBuildTarget);
                        }
#else
						throw new System.Exception("Running on an unsupported platform: " + Application.platform);
#endif
                }
            }
        }
    }
}