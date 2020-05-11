﻿using UnityEngine.Rendering.PostProcessing;
using System.Reflection;
using TMPro;
using UnityEngine;
using BepInEx;
using UnityEngine.SceneManagement;
using System;

namespace ModdingTales
{
    public class TalesAPI : CSHTTPServer
    {
        public string Folder;

        public TalesAPI() : base()
        {
            this.Folder = "c:\\www";
        }

        public TalesAPI(int thePort, string theFolder) : base(thePort)
        {
            this.Folder = theFolder;
        }

        public override void OnResponse(ref HTTPRequestStruct rq, ref HTTPResponseStruct rp)
        {
            UnityEngine.Debug.Log("ONRESPONSE: " + rq.URL);
            //string path = this.Folder + "\\" + rq.URL.Replace("/", "\\");

            //if (Directory.Exists(path))
            //{
            //    if (File.Exists(path + "default.htm"))
            //        path += "\\default.htm";
            //    else
            //    {
            //        string[] dirs = Directory.GetDirectories(path);
            //        string[] files = Directory.GetFiles(path);

            //        string bodyStr = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\n";
            //        bodyStr += "<HTML><HEAD>\n";
            //        bodyStr += "<META http-equiv=Content-Type content=\"text/html; charset=windows-1252\">\n";
            //        bodyStr += "</HEAD>\n";
            //        bodyStr += "<BODY><p>Folder listing, to do not see this add a 'default.htm' document\n<p>\n";
            //        for (int i = 0; i < dirs.Length; i++)
            //            bodyStr += "<br><a href = \"" + rq.URL + Path.GetFileName(dirs[i]) + "/\">[" + Path.GetFileName(dirs[i]) + "]</a>\n";
            //        for (int i = 0; i < files.Length; i++)
            //            bodyStr += "<br><a href = \"" + rq.URL + Path.GetFileName(files[i]) + "\">" + Path.GetFileName(files[i]) + "</a>\n";
            //        bodyStr += "</BODY></HTML>\n";

            //        rp.BodyData = Encoding.ASCII.GetBytes(bodyStr);
            //        return;
            //    }
            //}

            //if (File.Exists(path))
            //{
            //    RegistryKey rk = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(path), true);

            //    // Get the data from a specified item in the key.
            //    String s = (String)rk.GetValue("Content Type");

            //    // Open the stream and read it back.
            //    rp.fs = File.Open(path, FileMode.Open);
            //    if (s != "")
            //        rp.Headers["Content-type"] = s;
            //}
            //else
            //{

            //    rp.status = (int)RespState.NOT_FOUND;

            //    string bodyStr = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\n";
            //    bodyStr += "<HTML><HEAD>\n";
            //    bodyStr += "<META http-equiv=Content-Type content=\"text/html; charset=windows-1252\">\n";
            //    bodyStr += "</HEAD>\n";
            //    bodyStr += "<BODY>File not found!!</BODY></HTML>\n";

            //    rp.BodyData = Encoding.ASCII.GetBytes(bodyStr);

            //}

        }
    }
    public static class ModdingUtils
    {
        public static Slab GetSelectedSlab()
        {
            try
            {
                var test = (SlabBuilderBoardTool)SingletonBehaviour<SlabBuilderBoardTool>.Instance;
            }
            catch { }
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            if (SingletonBehaviour<BoardToolManager>.HasInstance && (SingletonBehaviour<BoardToolManager>.Instance.IsCurrentTool<SlabBuilderBoardTool>()))
            {
                var sbbt = (SlabBuilderBoardTool)SingletonBehaviour<SlabBuilderBoardTool>.Instance;
                Slab slab = (Slab)sbbt.GetType().GetField("_slab", flags).GetValue(sbbt);
                return slab;
            } else
            {
                return null;
            }

        }

        public static TilePreviewBoardAsset GetSelectedTileAsset()
        {
            try {
                var test = (SingleBuilderBoardTool)SingletonBehaviour<SingleBuilderBoardTool>.Instance;
            } catch {}
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            if (SingletonBehaviour<BoardToolManager>.HasInstance && (SingletonBehaviour<BoardToolManager>.Instance.IsCurrentTool<SingleBuilderBoardTool>()))
            {
                var btm = (SingleBuilderBoardTool)SingletonBehaviour<SingleBuilderBoardTool>.Instance;
                TilePreviewBoardAsset selectedAsset = (TilePreviewBoardAsset)btm.GetType().GetField("_selectedTileBoardAsset", flags).GetValue(btm);
                return selectedAsset;
            }
            else
            {
                return null;
            }

        }

        public static TextMeshProUGUI GetUITextContainsString(string contains)
        {
            TextMeshProUGUI[] texts = UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].text.Contains(contains))
                {
                    return texts[i];
                }
            }
            return null;
        }
        public static TextMeshProUGUI GetUITextByName(string name)
        {
            TextMeshProUGUI[] texts = UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].name == name)
                {
                    return texts[i];
                }
            }
            return null;
        }

        public static PostProcessLayer GetPostProcessLayer()
        {
            return Camera.main.GetComponent<PostProcessLayer>();
        }

        public static BaseUnityPlugin parentPlugin;
        public static void Initialize(BaseUnityPlugin parentPlugin)
        {
            AppStateManager.UsingCodeInjection = true;
            ModdingUtils.parentPlugin = parentPlugin;
            SceneManager.sceneLoaded += ModdingUtils.OnSceneLoaded;
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UnityEngine.Debug.Log("Loading Scene: " + scene.name);
            if (scene.name == "UI") {
                TextMeshProUGUI betaText = GetUITextByName("BETA");
                if (betaText)
                {
                    betaText.text = "INJECTED BUILD - unstable mods";
                }
            } else if (scene.name == "Login")
            {
                TextMeshProUGUI modListText = GetUITextByName("TextMeshPro Text");
                if (modListText)
                {
                    BepInPlugin bepInPlugin = (BepInPlugin)Attribute.GetCustomAttribute(ModdingUtils.parentPlugin.GetType(), typeof(BepInPlugin));
                    if (modListText.text.EndsWith("</size>"))
                    {
                        modListText.text += "\n\nMods Currently Installed:\n";
                    }
                    modListText.text += "\n" + bepInPlugin.Name + " - " + bepInPlugin.Version;
                }
            }
        }
    }
}
