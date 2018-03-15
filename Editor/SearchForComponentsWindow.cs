namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public class SearchForComponentsWindow : EditorWindow
    {
        private enum SearchMode
        {
            Usage,
            Missing
        }

        private enum CheckMode
        {
            Single,
            All
        }

        private static readonly string[] ModeTitles = { "Search for component usage", "Search for missing components" };
        private static readonly string[] CheckTypeTitles = { "Check single component", "Check all components" };

        private List<string> listResult;
        private List<ComponentResult> prefabComponents;
        private List<ComponentResult> notUsedComponents;
        private List<ComponentResult> addedComponents;
        private List<ComponentResult> existingComponents;
        private List<ComponentResult> sceneComponents;
        private SearchMode editorMode = SearchMode.Usage;
        private CheckMode selectedCheckType = CheckMode.Single;
        private bool recursionVal;
        private MonoScript targetComponent;
        private string componentName = string.Empty;

        private bool showPrefabs;
        private bool showAdded;
        private bool showScene;
        private bool showUnused = true;

        Vector2 scroll, scroll1, scroll2, scroll3, scroll4;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void OnGUI()
        {
            GUILayout.Label(this.position + string.Empty);
            GUILayout.Space(3);
            int oldValue = GUI.skin.window.padding.bottom;
            GUI.skin.window.padding.bottom = -20;
            Rect windowRect = GUILayoutUtility.GetRect(1, 17);
            windowRect.x += 4;
            windowRect.width -= 7;
            this.editorMode = (SearchMode)GUI.SelectionGrid(windowRect, (int)this.editorMode, ModeTitles, 2, "Window");
            GUI.skin.window.padding.bottom = oldValue;

            switch (this.editorMode)
            {
                case SearchMode.Usage:
                {
                    this.DrawUsageGui();
                    break;
                }

                case SearchMode.Missing:
                {
                    this.DrawMissingGui();
                    break;
                }
            }

            if (this.editorMode == SearchMode.Missing || this.selectedCheckType == CheckMode.Single)
            {
                if (this.listResult != null)
                {
                    if (this.listResult.Count == 0)
                    {
                        GUILayout.Label(this.editorMode == 0
                            ? (this.componentName == "" ? "Choose a component" : "No prefabs use component " + this.componentName)
                            : "No prefabs have missing components!\nClick Search to check again");
                    }
                    else
                    {
                        GUILayout.Label(this.editorMode == 0
                            ? string.Concat("The following ", this.listResult.Count, " prefabs use component ", this.componentName, ":")
                            : "The following prefabs have missing components:");
                        this.scroll = GUILayout.BeginScrollView(this.scroll);
                        foreach (string s in this.listResult)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(s, GUILayout.Width(this.position.width / 2));
                            if (GUILayout.Button("Select", GUILayout.Width(this.position.width / 2 - 10)))
                            {
                                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(s);
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                    }
                }
            }
            else
            {
                this.showPrefabs = GUILayout.Toggle(this.showPrefabs, "Show prefab components");
                if (this.showPrefabs)
                {
                    GUILayout.Label("The following components are attatched to prefabs:");
                    this.DisplayResults(ref this.scroll1, ref this.prefabComponents);
                }
                this.showAdded = GUILayout.Toggle(this.showAdded, "Show AddComponent arguments");
                if (this.showAdded)
                {
                    GUILayout.Label("The following components are AddComponent arguments:");
                    this.DisplayResults(ref this.scroll2, ref this.addedComponents);
                }
                this.showScene = GUILayout.Toggle(this.showScene, "Show Scene-used components");
                if (this.showScene)
                {
                    GUILayout.Label("The following components are used by scene objects:");
                    this.DisplayResults(ref this.scroll3, ref this.sceneComponents);
                }
                this.showUnused = GUILayout.Toggle(this.showUnused, "Show Unused ComponentFactories");
                if (this.showUnused)
                {
                    GUILayout.Label("The following components are not used by prefabs, by AddComponent, OR in any scene:");
                    this.DisplayResults(ref this.scroll4, ref this.notUsedComponents);
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawUsageSingleGui()
        {
            this.componentName = this.targetComponent.name;
            string targetPath = AssetDatabase.GetAssetPath(this.targetComponent);
            string[] allPrefabs = GetAllPrefabs();
            this.listResult = new List<string>();
            foreach (string prefab in allPrefabs)
            {
                string[] single = { prefab };
                string[] dependencies = AssetDatabase.GetDependencies(single, this.recursionVal);
                foreach (string dependedAsset in dependencies)
                {
                    if (dependedAsset == targetPath)
                    {
                        this.listResult.Add(prefab);
                    }
                }
            }
        }

        private void DrawUsageAllGui()
        {
            List<string> scenesToLoad = new List<string>();
            this.existingComponents = new List<ComponentResult>();
            this.prefabComponents = new List<ComponentResult>();
            this.notUsedComponents = new List<ComponentResult>();
            this.addedComponents = new List<ComponentResult>();
            this.sceneComponents = new List<ComponentResult>();

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string projectPath = Application.dataPath;
                projectPath = projectPath.Substring(0, projectPath.IndexOf("Assets", StringComparison.Ordinal));

                string[] allAssets = AssetDatabase.GetAllAssetPaths();

                foreach (string asset in allAssets)
                {
                    int indexCS = asset.IndexOf(".cs", StringComparison.Ordinal);
                    int indexJS = asset.IndexOf(".js", StringComparison.Ordinal);
                    if (indexCS != -1 || indexJS != -1)
                    {
                        ComponentResult newComponent = new ComponentResult(this.NameFromPath(asset), String.Empty, asset);
                        try
                        {
                            System.IO.FileStream fileStream = new System.IO.FileStream(projectPath + asset, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                            System.IO.StreamReader streamReader = new System.IO.StreamReader(fileStream);
                            while (!streamReader.EndOfStream)
                            {
                                string line = streamReader.ReadLine();
                                int index1 = line.IndexOf("namespace", StringComparison.Ordinal);
                                int index2 = line.IndexOf("{", StringComparison.Ordinal);
                                if (index1 != -1 && index2 != -1)
                                {
                                    line = line.Substring(index1 + 9);
                                    index2 = line.IndexOf("{", StringComparison.Ordinal);
                                    line = line.Substring(0, index2);
                                    line = line.Replace(" ", string.Empty);
                                    newComponent.NamespaceName = line;
                                }
                            }
                        }
                        catch
                        {
                        }

                        this.existingComponents.Add(newComponent);

                        try
                        {
                            System.IO.FileStream fileStream = new System.IO.FileStream(projectPath + asset, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                            System.IO.StreamReader streamReader = new System.IO.StreamReader(fileStream);

                            int lineNum = 0;
                            while (!streamReader.EndOfStream)
                            {
                                lineNum++;
                                var line = streamReader.ReadLine();
                                int index = line.IndexOf("AddComponent", StringComparison.Ordinal);
                                if (index != -1)
                                {
                                    line = line.Substring(index + 12);
                                    if (line[0] == '(')
                                    {
                                        line = line.Substring(1, line.IndexOf(')') - 1);
                                    }
                                    else if (line[0] == '<')
                                    {
                                        line = line.Substring(1, line.IndexOf('>') - 1);
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    line = line.Replace(" ", string.Empty);
                                    line = line.Replace("\"", string.Empty);
                                    index = line.LastIndexOf('.');
                                    ComponentResult newComp;
                                    if (index == -1)
                                    {
                                        newComp = new ComponentResult(line);
                                    }
                                    else
                                    {
                                        newComp = new ComponentResult(line.Substring(index + 1, line.Length - (index + 1)), line.Substring(0, index), "");
                                    }

                                    string pName = asset + ", Line " + lineNum;
                                    newComp.UsageSource.Add(pName);
                                    index = this.addedComponents.IndexOf(newComp);
                                    if (index == -1)
                                    {
                                        this.addedComponents.Add(newComp);
                                    }
                                    else
                                    {
                                        if (!this.addedComponents[index].UsageSource.Contains(pName)) this.addedComponents[index].UsageSource.Add(pName);
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    int indexPrefab = asset.IndexOf(".prefab", StringComparison.Ordinal);

                    if (indexPrefab != -1)
                    {
                        string[] single = { asset };
                        string[] dependencies = AssetDatabase.GetDependencies(single, this.recursionVal);
                        foreach (string dependedAsset in dependencies)
                        {
                            if (dependedAsset.IndexOf(".cs", StringComparison.Ordinal) != -1 || dependedAsset.IndexOf(".js", StringComparison.Ordinal) != -1)
                            {
                                ComponentResult newComponent = new ComponentResult(this.NameFromPath(dependedAsset), this.GetNamespaceFromPath(dependedAsset), dependedAsset);
                                int index = this.prefabComponents.IndexOf(newComponent);
                                if (index == -1)
                                {
                                    newComponent.UsageSource.Add(asset);
                                    this.prefabComponents.Add(newComponent);
                                }
                                else
                                {
                                    if (!this.prefabComponents[index].UsageSource.Contains(asset)) this.prefabComponents[index].UsageSource.Add(asset);
                                }
                            }
                        }
                    }
                    int indexUnity = asset.IndexOf(".unity", StringComparison.Ordinal);
                    if (indexUnity != -1)
                    {
                        scenesToLoad.Add(asset);
                    }
                }

                for (int i = this.addedComponents.Count - 1; i > -1; i--)
                {
                    this.addedComponents[i].AssetPath = this.GetPathFromNames(this.addedComponents[i].NamespaceName, this.addedComponents[i].ComponentName);
                    if (this.addedComponents[i].AssetPath == string.Empty)
                    {
                        this.addedComponents.RemoveAt(i);
                    }

                }

                foreach (string scene in scenesToLoad)
                {
                    EditorSceneManager.OpenScene(scene);
                    GameObject[] sceneGOs = this.GetAllObjectsInScene();
                    foreach (GameObject g in sceneGOs)
                    {
                        Component[] comps = g.GetComponentsInChildren<Component>(true);
                        foreach (Component c in comps)
                        {

                            if (c != null && c.GetType() != null && c.GetType().BaseType != null && c.GetType().BaseType == typeof(MonoBehaviour))
                            {
                                SerializedObject so = new SerializedObject(c);
                                SerializedProperty p = so.FindProperty("m_Script");
                                string path = AssetDatabase.GetAssetPath(p.objectReferenceValue);
                                ComponentResult newComp = new ComponentResult(this.NameFromPath(path), this.GetNamespaceFromPath(path), path);
                                newComp.UsageSource.Add(scene);
                                int index = this.sceneComponents.IndexOf(newComp);
                                if (index == -1)
                                {
                                    this.sceneComponents.Add(newComp);
                                }
                                else
                                {
                                    if (!this.sceneComponents[index].UsageSource.Contains(scene)) this.sceneComponents[index].UsageSource.Add(scene);
                                }
                            }
                        }
                    }
                }

                foreach (ComponentResult c in this.existingComponents)
                {
                    if (this.addedComponents.Contains(c)) continue;
                    if (this.prefabComponents.Contains(c)) continue;
                    if (this.sceneComponents.Contains(c)) continue;
                    this.notUsedComponents.Add(c);
                }

                this.addedComponents.Sort(this.SortAlphabetically);
                this.prefabComponents.Sort(this.SortAlphabetically);
                this.sceneComponents.Sort(this.SortAlphabetically);
                this.notUsedComponents.Sort(this.SortAlphabetically);
            }
        }

        private void DrawUsageGui()
        {
            this.selectedCheckType = (CheckMode)GUILayout.SelectionGrid((int)this.selectedCheckType, CheckTypeTitles, 2, "Toggle");
            this.recursionVal = GUILayout.Toggle(this.recursionVal, "Search all dependencies");
            GUI.enabled = this.selectedCheckType == CheckMode.Single;
            this.targetComponent = (MonoScript)EditorGUILayout.ObjectField(this.targetComponent, typeof(MonoScript), false);
            GUI.enabled = true;

            if (!GUILayout.Button("Check component usage"))
            {
                return;
            }

            AssetDatabase.SaveAssets();
            switch (this.selectedCheckType)
            {
                case CheckMode.Single:
                {
                    this.DrawUsageSingleGui();
                    break;
                }

                case CheckMode.All:
                {
                    this.DrawUsageAllGui();
                    break;
                }
            }
        }

        private void DrawMissingGui()
        {
            if (!GUILayout.Button("Search!"))
            {
                return;

            }

            string[] allPrefabs = GetAllPrefabs();
            this.listResult = new List<string>();
            foreach (string prefab in allPrefabs)
            {
                UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(prefab);
                try
                {
                    var gameObject = (GameObject) o;
                    Component[] components = gameObject.GetComponentsInChildren<Component>(true);
                    foreach (Component c in components)
                    {
                        if (c == null)
                        {
                            this.listResult.Add(prefab);
                        }
                    }
                }
                catch
                {
                    Debug.Log("For some reason, prefab " + prefab + " won't cast to GameObject");
                }
            }
        }

        private int SortAlphabetically(ComponentResult a, ComponentResult b)
        {
            return String.Compare(a.AssetPath, b.AssetPath, StringComparison.Ordinal);
        }

        private GameObject[] GetAllObjectsInScene()
        {
            List<GameObject> objectsInScene = new List<GameObject>();
            GameObject[] allGOs = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
            foreach (GameObject go in allGOs)
            {
                //if ( go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave )
                //    continue;

                string assetPath = AssetDatabase.GetAssetPath(go.transform.root.gameObject);
                if (!string.IsNullOrEmpty(assetPath))
                    continue;

                objectsInScene.Add(go);
            }

            return objectsInScene.ToArray();
        }

        private void DisplayResults(ref Vector2 scroller, ref List<ComponentResult> list)
        {
            if (list == null)
            {
                return;
            }

            scroller = GUILayout.BeginScrollView(scroller);
            foreach (ComponentResult c in list)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(c.AssetPath, GUILayout.Width(this.position.width / 5 * 4));
                if (GUILayout.Button("Select", GUILayout.Width(this.position.width / 5 - 30)))
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(c.AssetPath);
                }
                GUILayout.EndHorizontal();
                if (c.UsageSource.Count == 1)
                {
                    GUILayout.Label("   In 1 Place: " + c.UsageSource[0]);
                }
                if (c.UsageSource.Count > 1)
                {
                    GUILayout.Label("   In " + c.UsageSource.Count + " Places: " + c.UsageSource[0] + ", " + c.UsageSource[1] + (c.UsageSource.Count > 2 ? ", ..." : ""));
                }
            }

            GUILayout.EndScrollView();
        }

        private string NameFromPath(string s)
        {
            s = s.Substring(s.LastIndexOf('/') + 1);
            return s.Substring(0, s.Length - 3);
        }

        private string GetNamespaceFromPath(string path)
        {
            foreach (ComponentResult result in this.existingComponents)
            {
                if (result.AssetPath == path)
                {
                    return result.NamespaceName;
                }
            }

            return string.Empty;
        }

        private string GetPathFromNames(string space, string sourceName)
        {
            ComponentResult result = new ComponentResult(sourceName, space);
            int index = this.existingComponents.IndexOf(result);
            if (index != -1)
            {
                return this.existingComponents[index].AssetPath;
            }

            return string.Empty;
        }

        private static string[] GetAllPrefabs()
        {
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            List<string> result = new List<string>();
            foreach (string path in assetPaths)
            {
                if (path.Contains(".prefab"))
                {
                    result.Add(path);
                }
            }

            return result.ToArray();
        }

        private class ComponentResult
        {
            public ComponentResult(string comp, string space = "", string path = "")
            {
                this.ComponentName = comp;
                this.NamespaceName = space;
                this.AssetPath = path;
                this.UsageSource = new List<string>();
            }

            public string ComponentName { get; private set; }
            public string NamespaceName { get; set; }
            public string AssetPath { get; set; }
            public List<string> UsageSource { get; private set; }

            public override bool Equals(object obj)
            {
                return ((ComponentResult)obj).ComponentName == this.ComponentName && ((ComponentResult)obj).NamespaceName == this.NamespaceName;
            }

            public override int GetHashCode()
            {
                return this.ComponentName.GetHashCode() + this.NamespaceName.GetHashCode();
            }
        }
    }
}
