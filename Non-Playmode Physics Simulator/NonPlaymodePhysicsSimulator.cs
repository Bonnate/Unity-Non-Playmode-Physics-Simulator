/*
 * Author: [Bonnate] https://bonnate.tistory.com/
 * License: None (Provided without explicit license)
 *
 * Description:
 * NonPlaymodePhysicsSimulator is a tool designed to focus on the Simulation feature. It does not handle unpredictable exceptions resulting from user actions. Before using this feature, please make sure to understand how to use it and be aware of the following precautions:
 *
 * Precautions:
 * 0. This feature is primarily designed for Simulation functionality. It does not handle unpredictable exceptions resulting from user actions. Make sure to familiarize yourself with how to use this feature and the precautions before using it.
 * 1. Edit your Scene in Edit mode. Be cautious as unintended physics simulation results may occur.
 * 2. This feature does not automatically save the Scene. If you believe the simulation results are incorrect, you can revert to the previous results by reloading the Scene.
 * 3. The Lock feature sets the Constraints of currently active Rigidbody components. Be sure to unlock them after the simulation.
 * 4. Performing actions unrelated to the simulation, such as removing or modifying objects while the Lock is in place, can lead to issues.
 *
 * Usage:
 * 1. Open the Non-Playmode Physics Simulator window from the "Tools" menu in the Unity Editor.
 * 2. In the window, you will find two buttons:
 *    - "Enable Physics": This button activates the physics simulation. Click it to start the simulation.
 *    - "Disable Physics": This button deactivates the physics simulation. Click it to stop the simulation.
 * 3. While the physics simulation is enabled:
 *    - You can interact with the Scene as usual, making changes to GameObjects and their positions, rotations, or scales.
 *    - Observe how physics simulation affects your changes in real-time.
 * 4. To pause or stop the simulation, click the "Disable Physics" button.
 * 5. You can simulate Rigidbody components on selected GameObjects by clicking the "Lock Except Selections" button. This freezes unselected object's physics properties temporarily.
 * 6. Use the "Unlock All" button to release the locks placed on Rigidbody components. Make sure to unlock them after the simulation to restore normal physics behavior.
 * 7. The simulation mode can be continuously updated by keeping the "Enable Physics" button pressed.
 */

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Bonnate
{
    public class NonPlaymodePhysicsSimulator : EditorWindow
    {
        private struct RigidbodyLockData
        {
            public Rigidbody mRigidbodyComp;
            public RigidbodyConstraints mConstraints;
        }

        private static Dictionary<Rigidbody, RigidbodyLockData> mRigidbodyLockDatas = new Dictionary<Rigidbody, RigidbodyLockData>();
        private bool mIsRunningPhysics = false; // Flag indicating whether physics simulation is running

        private GUIStyle mGreenButtonStyle; // Green button style
        private GUIStyle mOrangeButtonStyle; // Orange button style

        private string mScriptFolderPath; // Folder path of the script
        private Texture2D mMainLogoTexture; // Main logo texture

        [MenuItem("Tools/Bonnate/Non-Playmode Physics Simulator")]
        public static void ShowWindow()
        {
            var window = GetWindow<NonPlaymodePhysicsSimulator>("Physics Simulator");
            window.minSize = new Vector2(300, 210);
            window.maxSize = new Vector2(300, 210);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            // Set green button style
            mGreenButtonStyle = new GUIStyle(GUI.skin.button);
            mGreenButtonStyle.normal.textColor = Color.green;

            // Set orange button style
            mOrangeButtonStyle = new GUIStyle(GUI.skin.button);
            mOrangeButtonStyle.normal.textColor = Color.red;

            // Display "Enable Physics" button as enabled and clickable when mIsRunningPhysics is false
            GUI.enabled = !mIsRunningPhysics;
            if (GUILayout.Button("Enable Physics", mGreenButtonStyle))
            {
                mIsRunningPhysics = true;
            }

            // Display "Disable Physics" button as enabled and clickable when mIsRunningPhysics is true
            GUI.enabled = mIsRunningPhysics;
            if (GUILayout.Button("Disable Physics", mOrangeButtonStyle))
            {
                mIsRunningPhysics = false;
            }

            // Reset GUI.enabled to its original state for other UI elements
            GUI.enabled = true;

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if (GUILayout.Button("Lock Except Selections"))
            {
                RestoreAll();

                foreach (Rigidbody rigidbody in FindObjectsOfType<Rigidbody>(false))
                {
                    RigidbodyLockData rigidbodyLockData;
                    rigidbodyLockData.mRigidbodyComp = rigidbody;
                    rigidbodyLockData.mConstraints = rigidbody.constraints;

                    rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                    mRigidbodyLockDatas.Add(rigidbody, rigidbodyLockData);
                }

                foreach (System.Object obj in Selection.objects)
                {
                    GameObject go = (GameObject)obj;
                    if (!go.activeInHierarchy)
                        continue;

                    Rigidbody? rigidbody = null;

                    if (go.TryGetComponent<Rigidbody>(out rigidbody))
                    {
                        RestoreRigidbodyConstraints(mRigidbodyLockDatas[rigidbody]);
                        mRigidbodyLockDatas.Remove(rigidbody);
                    }
                }
            }

            if (GUILayout.Button($"Unlock All{(mRigidbodyLockDatas.Count > 0 ? $" ({mRigidbodyLockDatas.Count} locked!)" : "")}"))
            {
                RestoreAll();
            }

            if (mMainLogoTexture != null)
            {
                GUILayout.Space(10);

                float inspectorWidth = EditorGUIUtility.currentViewWidth;
                float width = inspectorWidth;
                float height = mMainLogoTexture.height * (width / mMainLogoTexture.width);

                Rect rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                if (GUI.Button(rect, mMainLogoTexture, GUIStyle.none))
                {
                    Application.OpenURL("https://github.com/bonnate");
                }
            }

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Powered by: Bonnate");

            if (GUILayout.Button("Github", GetHyperlinkLabelStyle()))
            {
                OpenURL("https://github.com/bonnate");
            }

            if (GUILayout.Button("Blog", GetHyperlinkLabelStyle()))
            {
                OpenURL("https://bonnate.tistory.com/");
            }

            GUILayout.EndHorizontal();
        }

        private void Update()
        {
            if (mIsRunningPhysics)
            {
                StepPhysics();
            }

            if (mRigidbodyLockDatas.Count > 0)
            {
                Log($"{mRigidbodyLockDatas.Count} rigidbodies are in locked!", LogType.Warning);
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            mScriptFolderPath = Path.GetDirectoryName(scriptPath);

            string imagePath = $"{mScriptFolderPath}/Images/logo.png";
            mMainLogoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
        }

        private void OnDisable()
        {
            mIsRunningPhysics = false;
            RestoreAll();

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            RestoreAll();
        }

        private void RestoreRigidbodyConstraints(RigidbodyLockData rigidbodyLockData)
        {
            rigidbodyLockData.mRigidbodyComp.constraints = rigidbodyLockData.mConstraints;
        }

        private void RestoreAll()
        {
            if (mRigidbodyLockDatas.Count <= 0)
                return;

            foreach (RigidbodyLockData rigidbodyLockData in mRigidbodyLockDatas.Values)
                RestoreRigidbodyConstraints(rigidbodyLockData);

            Log($"{mRigidbodyLockDatas.Count} rigidbodies were unlocked", LogType.Log);

            mRigidbodyLockDatas.Clear();
        }

        private void StepPhysics()
        {
            Physics.simulationMode = SimulationMode.Script;
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.simulationMode = SimulationMode.FixedUpdate;
        }

        #region _HYPERLINK
        private GUIStyle GetHyperlinkLabelStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = new Color(0f, 0.5f, 1f);
            style.stretchWidth = false;
            style.wordWrap = false;
            return style;
        }

        private void OpenURL(string url)
        {
            EditorUtility.OpenWithDefaultApp(url);
        }
        #endregion

        #region 
        private void Log(string content, LogType logType)
        {
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log($"<color=cyan>[Non-Playmode Physics Simulator]</color> {content}");
                    break;

                case LogType.Warning:
                    Debug.LogWarning($"<color=yellow>[Non-Playmode Physics Simulator]</color> {content}");
                    break;

                case LogType.Error:
                    Debug.LogError($"<color=red>[Non-Playmode Physics Simulator]</color> {content}");
                    break;
            }
        }
        #endregion        
    }
}
#endif