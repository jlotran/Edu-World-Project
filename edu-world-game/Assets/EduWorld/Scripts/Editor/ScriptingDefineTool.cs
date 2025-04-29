using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

public class ScriptingDefineTool : EditorWindow
{
    private const string Symbol = "NONFUSION";

    [MenuItem("Tools/Scripting Defines/Toggle NONFUSION")]
    public static void ToggleNONFUSIONSymbol()
    {
        BuildTargetGroup buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);

        var defineList = defines.Split(';').ToList();

        if (defineList.Contains(Symbol))
        {
            defineList.Remove(Symbol);
            Debug.Log($"Removed scripting define symbol: {Symbol}");
        }
        else
        {
            defineList.Add(Symbol);
            Debug.Log($"Added scripting define symbol: {Symbol}");
        }

        string newDefines = string.Join(";", defineList.Distinct());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, newDefines);
    }

    [MenuItem("Tools/Scripting Defines/Show Current Defines")]
    public static void ShowCurrentDefines()
    {
        BuildTargetGroup buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
        Debug.Log($"Current scripting define symbols for {buildTarget}: {defines}");
    }
}