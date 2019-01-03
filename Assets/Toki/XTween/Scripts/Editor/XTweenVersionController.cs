﻿/**********************************************************************************
/*		File Name 		: XTweenVersionController.cs
/*		Author 			: Robin
/*		Description 	: 
/*		Created Date 	: 2019-1-3
/*		Modified Date 	: 
/**********************************************************************************/

using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Toki.Tween
{
	public class XTweenVersionController
	{
		/************************************************************************
		*	 	 	 	 	Static Variable Declaration	 	 	 	 	 	    *
		************************************************************************/
		
		/************************************************************************
		*	 	 	 	 	Static Method Declaration	 	 	 	     	 	*
		************************************************************************/
		
		/************************************************************************
		*	 	 	 	 	Private Variable Declaration	 	 	 	 	 	*
		************************************************************************/
		private const string STORE_CHECKED_DATE = "xtween.store_checked_date";
		private const string STORE_LAST_VERSION = "xtween.store_last_version";
		private const string URL = "https://toki-labs.github.io/XTween/Assets/Toki/XTween/Scripts/Editor/xtween_config.json";
		private UnityWebRequest _http;
		
		/************************************************************************
		*	 	 	 	 	Protected Variable Declaration	 	 	 	 	 	*
		************************************************************************/
			
		/************************************************************************
		*	 	 	 	 	Public Variable Declaration	 	 	 	 	 		*
		************************************************************************/
		private Action<string> _listener;

		/************************************************************************
		*	 	 	 	 	Getter & Setter Declaration	 	 	 	 	 		*
		************************************************************************/
		public bool IsDownloading { get; set; }
		public string StoredLastVersion
		{
			get
			{
				return EditorPrefs.GetString(STORE_LAST_VERSION, XTweenEditorManager.Instance.Data.version);
			}
		}

		private string TempPath
		{
			get
			{
				return Path.Combine(XTweenEditorManager.AbsPath, "XTweenTemp");
			}
		}
		
		/************************************************************************
		*	 	 	 	 	Initialize & Destroy Declaration	 	 	 		*
		************************************************************************/
		
		/************************************************************************
		*	 	 	 	 	Life Cycle Method Declaration	 	 	 	 	 	*
		************************************************************************/
		public XTweenVersionController(Action<string> listener)
		{
			this._listener = listener;
		}
		
		/************************************************************************
		*	 	 	 	 	Coroutine Declaration	 	  			 	 		*
		************************************************************************/
		private IEnumerator CoroutineVersionLoad()
		{
			this._http = UnityWebRequest.Get(URL);
			this._http.SetRequestHeader("Content-Type", "application/json");
			this._http.SetRequestHeader("Accepted", "application/json");

			yield return this._http.SendWebRequest();
			do
			{
				yield return null;
			}
			while (!this._http.isDone);

			bool isSuccess = false;
			if( string.IsNullOrEmpty(this._http.error) || 
				(this._http.responseCode < 300 && this._http.responseCode >= 200) )
			{
				//Check version
				try
				{
					XTweenData data = JsonUtility.FromJson<XTweenData>(this._http.downloadHandler.text);
					EditorPrefs.SetString(STORE_CHECKED_DATE, this.GetToday());
					EditorPrefs.SetString(STORE_LAST_VERSION, data.version);
					this._listener(data.version);
					isSuccess = true;
				}
				catch ( System.Exception e ) {}
			}

			if( !isSuccess )
			{
				//Error
				this._listener("error");
			}
		}

		private IEnumerator CoroutinePackageLoad()
		{
			string fileName = "XTween_" + this.StoredLastVersion + ".unitypackage";
			string tempPath = this.TempPath;
			Directory.CreateDirectory(tempPath);
			string filePath = Path.Combine(tempPath, fileName);
			string url = "https://github.com/Toki-Labs/XTween/raw/master/Bin/" + fileName;
			this._http = UnityWebRequest.Get(url);
			this._http.downloadHandler = new DownloadHandlerFile(filePath);
			yield return this._http.SendWebRequest();
			do
			{
				yield return null;
			}
			while (!this._http.isDone);
			if( string.IsNullOrEmpty(this._http.error) )
			{
				Debug.Log("Downloaded");
				string rootPath = Path.Combine(XTweenEditorManager.AbsPath, "/Assets/Toki/XTween/Scripts");
				string[] dirs = 
					new string[]{Path.Combine(rootPath, "Editor"), Path.Combine(rootPath, "Runtime")};
				foreach ( var path in dirs )
				{
					Directory.Delete(path,true);
				}
				
				AssetDatabase.ImportPackage(filePath, false);
			}
			else
			{
				//Error
				EditorUtility.DisplayDialog("Error!", "Something wrong with download file. try next time.", "OK");
			}
		}

		private void EmptyTemp()
		{
			string tempPath = this.TempPath;
			if( Directory.Exists(tempPath) ) Directory.Delete(tempPath);
		}
		
		/************************************************************************
		*	 	 	 	 	Private Method Declaration	 	 	 	 	 		*
		************************************************************************/
		private string GetToday()
		{
			DateTime date = DateTime.Now;
			return date.Year + "-" + date.Month + "-" + date.Day;
		}
		
		/************************************************************************
		*	 	 	 	 	Protected Method Declaration	 	 	 	 	 	*
		************************************************************************/
		
		/************************************************************************
		*	 	 	 	 	Public Method Declaration	 	 	 	 	 		*
		************************************************************************/
		public void Check()
		{
			string date = EditorPrefs.GetString(STORE_CHECKED_DATE, null);
			bool needToCheck = false;
			if( string.IsNullOrEmpty(date) )
			{
				needToCheck = true;
			}
			else
			{
				string today = this.GetToday();
				if( !date.Equals(today) )
				{
					needToCheck = true;
				}
			}

			if( needToCheck )
			{
				EditorCoroutine.Start(CoroutineVersionLoad());
			}
			else
			{
				this._listener(null);
			}
		}

		public void Update()
		{
			EditorCoroutine.Start(CoroutinePackageLoad());
		}
	}
	
	public class EditorCoroutine
	{
		public static EditorCoroutine Start( IEnumerator _routine )
		{
			EditorCoroutine coroutine = new EditorCoroutine(_routine);
			coroutine.Start();
			return coroutine;
		}

		private readonly IEnumerator _enumerator;
		EditorCoroutine( IEnumerator _routine )
		{
			this._enumerator = _routine;
		}

		void Start()
		{
			EditorApplication.update += Update;
		}
		public void Stop()
		{
			EditorApplication.update -= Update;
		}

		void Update()
		{
			if (!_enumerator.MoveNext())
			{
				Stop();
			}
		}
	}
}