using System;
using UnityEngine;
using UnityEditor;

namespace T2G.UnityAdapter
{
    public class CommunicatorServerEditor : EditorWindow
    {
        static CommunicatorServer _server;

        [MenuItem("T2G/Communicator", false)]
        public static void Dashboard()
        {
            Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            EditorWindow.GetWindow<CommunicatorServerEditor>("Communicator Server", new Type[] { inspectorType });
        }

        [InitializeOnLoadMethod]
        public static void InitializeOnLoad()
        {
            if (_server == null)
            {
                _server = CommunicatorServer.Instance;
                AssemblyReloadEvents.beforeAssemblyReload += () =>
                {
                    _server.StopServer();
                };
            }

            if (PlayerPrefs.GetInt(Defs.k_InitStartListener, 1) != 0)
            {
                _server.StartServer();
            }
        }

        public void OnGUI()
        {
            if(_server == null)
            {
                _server = CommunicatorServer.Instance;
                AssemblyReloadEvents.beforeAssemblyReload += () => {
                    _server.StopServer();
                };
            }

            bool isActive = EditorGUILayout.Toggle("Server is active: ", _server.IsActive);
            if(isActive != _server.IsActive)
            {
                if (isActive)
                {
                    _server.StartServer();
                }
                else
                {
                    _server.StopServer();
                }
                PlayerPrefs.SetInt(Defs.k_InitStartListener, isActive ? 1 : 0);
            }

            if (_server.IsActive)
            {
                EditorGUILayout.Toggle("Client is connected: ", _server.IsConnected);
            }
        }
    }
}