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

        public void OnGUI()
        {
            if(_server == null)
            {
                _server = CommunicatorServer.Instance;
            }

            bool isActive = EditorGUILayout.Toggle("Server is active: ", _server.IsActive);
            if(isActive != _server.IsActive)
            {
                if (isActive)
                    _server.StartServer();
                else
                    _server.StopServer();
            }
            EditorGUILayout.Toggle("Client is connected: ", _server.IsConnected);
            
        }
    }
}