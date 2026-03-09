
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EMILtools_Private.Testing.MonoFacade_System
{
    public class FacadeGenerator : EditorWindow
    {
        string facadeName = "";
        [SerializeField] string path = "Assets/EMILtools/Facades/";
    
        [MenuItem("Tools/EMILTools/MonoFacade")]
        public static void ShowWindow() => GetWindow<FacadeGenerator>("EMILtools MonoFacade Generator");
    
        private void OnGUI()
        {
            facadeName = EditorGUILayout.TextField("MonoFacade Name", facadeName);
            path = EditorGUILayout.TextField("Path", path);

            if (GUILayout.Button("Generate MonoFacade Scripts")) 
                GenerateScripts();
        }

        void GenerateScripts()
        {
            path += facadeName;
            string createdPath = path + facadeName;
            if (!Directory.Exists(createdPath)) Directory.CreateDirectory(createdPath);

            // Config
            CreateScript(createdPath, $"{facadeName}Config", 
$@"using UnityEngine;
using EMILtools.Systems;

[CreateAssetMenu(fileName = ""{facadeName}Config"", menuName = ""EMILtools/ScriptableObjects/Configs/{facadeName}"")]
public class {facadeName}Config : Config
{{

}}");


            // Blackboard
            CreateScript(createdPath, $"{facadeName}Blackboard",
$@"using System;
using EMILtools.Systems;

[Serializable]
public class {facadeName}Blackboard : Blackboard
{{

}}");


            // Context
            CreateScript(createdPath, $"{facadeName}Context",
$@"using EMILtools.Systems;

public interface I{facadeName}ContextView : IContextViewImmutable
{{
    // Readonly properties
}}

public class {facadeName}ContextData : ContextData<I{facadeName}ContextView>, I{facadeName}ContextView, IModuleUsabableContext
{{
    // Mutable state
}}");


            // Structure
            CreateScript(createdPath, $"{facadeName}Structure",
$@"using System;
using EMILtools.Systems;

[Serializable]
public class {facadeName}Structure : MonoStructure<
    {facadeName}Blackboard,
    {facadeName}ContextData,
    I{facadeName}ContextView>
{{

}}");


            // Functionality
            CreateScript(createdPath, $"{facadeName}Functionality",
$@"using EMILtools.Systems;

public class {facadeName}Functionality : Functionalities<
    {facadeName}Controller,
    {facadeName}Structure>
{{
    protected override void AddModulesHere()
    {{
        // AddModule(new ExampleModule());
    }}
}}");


            // Controller
            CreateScript(createdPath, $"{facadeName}Controller",
$@"using UnityEngine;
using EMILtools.Systems;

public class {facadeName}Controller : MonoFacade<
    {facadeName}Controller,
    {facadeName}Functionality,
    {facadeName}Config,
    {facadeName}Structure,
    {facadeName}Controller.ActionMap>
{{
    public class ActionMap : IActionMap
    {{
        // Add Available Actions here as PersistentActions
        // Actions are separate from InputActions
    }}

    // If using InputAuthority, put InitializeFacade in InitSubordinate
    protected void Awake()
    {{
        InitializeFacade();
    }}
}}");

            AssetDatabase.Refresh();
            Debug.Log($"Generated MonoFacade: {facadeName} at {createdPath}");
        }

        void CreateScript(string path, string fileName, string content)
            => File.WriteAllText(Path.Combine(path, fileName + ".cs"), content);
    }
}
#endif