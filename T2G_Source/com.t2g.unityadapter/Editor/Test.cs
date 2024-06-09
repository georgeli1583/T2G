using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*

namespace bu.t2gadapter.Editor
{
	[Serializable]
	public class Test
	{
    		private static DateTime _preTime;
            
            private static Action _callFunction;
            
            private static int _count;
            
	   	[InitializeOnLoadMethod]
    		public static void Start()
    		{
        		_preTime = DateTime.Now;
        		EditorApplication.update += EditorUpdate;
                if (_callFunction == null)
                {
	                _callFunction = callFuncitonHandler;
                }
				
				//AssetDatabase.DisallowAutoRefresh();
				UnityEditor.AssetDatabase.AllowAutoRefresh();
				AssetDatabase.Refresh();
            }
    
    		static void EditorUpdate()
    		{
        		if ((DateTime.Now - _preTime).TotalSeconds > 1.0)
        		{
            		callFuncitonHandler();	
	                Debug.LogWarning($"updating  {_count}"  );
            		_preTime = DateTime.Now;
        		}
    		}
            
            
            static void callFuncitonHandler()
            {
	            _count++;
            }
	}
}
*/