
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EMILtools_Private.Testing.MonoFacade_System
{
    public class FacadeGenerator : EditorWindow
    {
        string facadeName = "";
    
        [MenuItem("Tools/EMILTools/MonoFacade")]
        public static void ShowWindow() => GetWindow<FacadeGenerator>("EMILTools MonoFacade Generator");
    
        private void OnGUI()
        {
            facadeName = EditorGUILayout.TextField("MonoFacade Name", facadeName);

            if (GUILayout.Button("Generate MonoFacade Scripts"))
            {
                GenerateScripts();
            }
        }

        void GenerateScripts()
        {
            string path = "Assets/EMILtools/Facades/" + facadeName;
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        
            // Config
            CreateScript(path, $"{facadeName}Config", 
                $"using UnityEngine;\n\n" +
                $"[CreateAssetMenu(fileName = \"{facadeName}Config\", menuName = \"Configs/{facadeName}\")]\n" +
                $"public class {facadeName}Config : Config\n{{\n\n}}");
        
            // Blackboard 
            CreateScript(path, $"{facadeName}Blackboard", 
                $"using System;\nusing UnityEngine;\n\n" +
                $"[Serializable]\n" +
                $"public class {facadeName}Blackboard : Blackboard\n{{\n\n}}");
        
            // Functionality
            CreateScript(path, $"{facadeName}Functionality", 
                $"using EMILtools_Private.Testing;\n\n" +
                $"public class {facadeName}Functionality : Functionalities<{facadeName}Controller>\n" +
                $"{{\n    protected override void AddModulesHere() \n    {{\n        // Add modules here\n    }}\n}}");
        
            // Controller
            CreateScript(path, $"{facadeName}Controller",
                $"using EMILtools_Private.Testing;\n" +
                $"using EMILtools.Core;\n" +
                $"using static {facadeName};\n" +
                $"using UnityEngine;\n\n" +
                $"public class {facadeName}Controller : MonoFacade<\n" +
                $"    {facadeName}Controller, \n" +
                $"    {facadeName}Functionality, \n" +
                $"    {facadeName}Config, \n" +
                $"    {facadeName}Blackboard, \n" +
                $"    {facadeName}Controller.ActionMap>\n" + 
                $"{{\n" +
                $"    public class ActionMap : IActionMap \n" +
                $"    {{\n" +
                $"        // Define system-specific PersistentActions here\n" +
                $"    }}\n\n" +
                $"    protected override void Awake() \n" +
                $"    {{\n" +
                $"        base.Awake();\n" +
                $"        InitializeFacade();\n" +
                $"    }}\n" +
                $"}}");
        
            AssetDatabase.Refresh();
            Debug.Log($"Generated MonoFacade: {facadeName} at {path}");
        }

        void CreateScript(string path, string fileName, string content)
            => File.WriteAllText(Path.Combine(path, fileName + ".cs"), content);
    }
}
#endif