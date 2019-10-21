using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build Asset Bundle")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
    }
}