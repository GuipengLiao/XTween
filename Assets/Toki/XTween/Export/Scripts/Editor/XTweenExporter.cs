﻿/**********************************************************************************
/*		File Name 		: XTweenExporter.cs
/*		Author 			: Robin
/*		Description 	: 
/*		Created Date 	: 2016-7-27
/*		Modified Date 	: 
/**********************************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Toki.Tween
{
    [InitializeOnLoad]
    public class XTweenExporter
    {
        /************************************************************************
        *	 	 	 	 	Static Variable Declaration	 	 	 	 	 	    *
        ************************************************************************/
        
        
        /************************************************************************
        *	 	 	 	 	Static Method Declaration	 	 	 	     	 	*
        ************************************************************************/
        private static XTweenExporter _instance;
        public static XTweenExporter Instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = new XTweenExporter();
                }
                return _instance;
            }
        }

        public static void UpdateRelease()
        {
            Instance.Export(false,true);
            Instance.UpdateReleasePath();
        }

        public static void MovePackageFile()
        {
            string exportRootPath = XTweenEditorManager.AbsPath + "/Bin";
            string exportFileName = "XTween_" + XTweenEditorManager.Instance.Data.version + ".unitypackage";
            string exportPath = exportRootPath + "/" + exportFileName;
            string packageFile = exportRootPath + "/XTween.unitypackage";
            File.Copy(packageFile, exportPath);
            File.Delete(packageFile);
        }

        [MenuItem("Window/XTween Build #%B",priority=16)]
        public static void ExampleBuild()
        {
            string buildPath = XTweenEditorManager.AbsPath + "/Export/WebGL";
            if( Directory.Exists(buildPath) )
                Directory.Delete(buildPath, true);

            BuildPipeline.BuildPlayer
            ( 
                FindEnabledEditorScenes(), 
                "Export/WebGL", 
                BuildTarget.WebGL, 
                BuildOptions.None
            );
        }

        protected static string[] FindEnabledEditorScenes()
        {
            List<string> EditorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled)
                {
                    continue;
                }

                EditorScenes.Add(scene.path);
            }
            return EditorScenes.ToArray();
        }

        /************************************************************************
        *	 	 	 	 	Private Variable Declaration	 	 	 	 	 	*
        ************************************************************************/
        private List<string> _folderList = new List<string>
        { 
            "Assets/Toki/XTween/Scripts"
        };
        
        /************************************************************************
        *	 	 	 	 	Protected Variable Declaration	 	 	 	 	 	*
        ************************************************************************/
        
        
        /************************************************************************
        *	 	 	 	 	Public Variable Declaration	 	 	 	 	 		*
        ************************************************************************/
        
        
        /************************************************************************
        *	 	 	 	 	Getter & Setter Declaration	 	 	 	 	 		*
        ************************************************************************/
        public static string ExportDefaultPath
        {
            get
            {
                string path = "";
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                else
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Documents";
                }
                return path;
            }
        }

        
        /************************************************************************
        *	 	 	 	 	Initialize & Destroy Declaration	 	 	 		*
        ************************************************************************/
        
        
        /************************************************************************
        *	 	 	 	 	Life Cycle Method Declaration	 	 	 	 	 	*
        ************************************************************************/
        public XTweenExporter()
        {
            
        }

        /************************************************************************
        *	 	 	 	 	Coroutine Declaration	 	  			 	 		*
        ************************************************************************/
        
        
        /************************************************************************
        *	 	 	 	 	Private Method Declaration	 	 	 	 	 		*
        ************************************************************************/
        private string ReplaceTargetStringInContent( string patternStart, string patternEnd, string replaceString, string content )
        {
            int indexStart = content.IndexOf(patternStart);
            int indexEnd = content.IndexOf(patternEnd, indexStart) + patternEnd.Length;
            string target = content.Substring(indexStart, indexEnd - indexStart);
            return content.Replace(target, replaceString);
        }
        
        private void UpdateReleasePath()
        {
            XTweenData data = XTweenEditorManager.Instance.Data;
            string first = "Version(Alpha) ";
            string end = ".unitypackage)";
            string replace = "Version(Alpha) {VER} - [XTween_{VER}.unitypackage](https://github.com/Toki-Labs/XTween/raw/master/Bin/XTween_{VER}.unitypackage)";
            replace = replace.Replace("{VER}", data.version);
            string filePath = XTweenEditorManager.AbsPath + "/README.md";
            string content = XTweenEditorManager.ReadText(filePath);
            content = ReplaceTargetStringInContent(first, end, replace, content);
            XTweenEditorManager.WriteText(filePath, content);

            first = "<!--Version Start";
            end = "Version End-->";
            replace = 
            "<!--Version Start-->\n" +
            "<p>Version(Alpha) "+ data.version +" - <a href=\"https://github.com/Toki-Labs/XTween/raw/master/Bin/XTween_"+ data.version +".unitypackage\">XTween_"+ data.version +".unitypackage</a></p>\n" +
            "<!--Version End-->";
            filePath = XTweenEditorManager.AbsPath + "/Export/index.html";
            content = XTweenEditorManager.ReadText(filePath);
            content = ReplaceTargetStringInContent(first, end, replace, content);
            XTweenEditorManager.WriteText(filePath, content);
        }
        
        /************************************************************************
        *	 	 	 	 	Protected Method Declaration	 	 	 	 	 	*
        ************************************************************************/
        
        
        /************************************************************************
        *	 	 	 	 	Public Method Declaration	 	 	 	 	 		*
        ************************************************************************/
        public string ExportVersion
        {
            get
            {
                string version = XTweenEditorManager.Instance.Data.version;
                if( string.IsNullOrEmpty( version ) )
                {
                    return "0.0.1";
                }
                else
                {
                    string[] versions = version.Split('.');
                    int head = int.Parse(versions[0]);
                    int mid = int.Parse(versions[1]);
                    int tail = int.Parse(versions[2]);
                    tail++;
                    if (tail > 999)
                    {
                        mid++;
                        tail = 0;
                    }
                    if (mid > 9)
                    {
                        head++;
                    }
                    return head.ToString() + "." + mid.ToString() + "." + tail.ToString();
                }
            }
        }
        
        public void Export(bool packingAll, bool release = false )
        {
            XTweenData data = XTweenEditorManager.Instance.Data;
            string addStr = "";
            string exportRootPath = XTweenEditorManager.AbsPath + "/Bin";
            List<string> exportPathList = new List<string>(this._folderList.ToArray());
            if( packingAll )
            {
                addStr = "All_";
                exportRootPath = ExportDefaultPath;
                exportPathList.Add("Assets/Toki/XTween/Export");
                exportPathList.Add("Assets/Toki/XTween/Example");
                exportPathList.Add("Assets/Toki/XTween/Images");
                exportPathList.Add("Assets/Toki/XTween/Resources");
            }
            else
            {
                string version = this.ExportVersion;
                data.version = version;
                XTweenEditorManager.Instance.Save();
            }
            string exportFileName = "XTween_" + addStr + data.version + ".unitypackage";
            string exportPath = exportRootPath + "/" + exportFileName;
            if( !release )
            {
                AssetDatabase.ExportPackage(exportPathList.ToArray(), exportPath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
            }
        }
    }
}