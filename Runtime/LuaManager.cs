//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using System.IO;
using UnityEngine;
using LuaInterface;
using System;
using EP.U3D.LIBRARY.BASE;
using EP.U3D.LIBRARY.ASSET;

namespace EP.U3D.RUNTIME.LUA
{
    public class LuaManager : MonoBehaviour
    {
        public static LuaManager Instance;
        public static GameObject Root;
        public static LuaState LuaState;
        public static LuaLooper LuaLooper = null;
        public static LuaLoader LuaLoader = null;
        private static LuaFunction mLuaNewFunc = null;
        public static LuaFunction LuaNEWFunc
        {
            get
            {
                if (mLuaNewFunc == null)
                {
                    mLuaNewFunc = LuaState.GetFunction("NEW");
                }
                return mLuaNewFunc;
            }
        }

#if UNITY_EDITOR 
        [NoToLua]
        public static LuaFunction StackTrace = null;
#endif 

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public static void PreInit(Transform root)
        {
            Helper.Log("luajit arch: {0}", IntPtr.Size == 4 ? "x86" : "x64");
            Root = new GameObject("Lua");
            if (root) Root.transform.SetParent(root);
            Root.AddComponent<LuaManager>();
            LuaState = new LuaState();
            InitializeLibs();
            InitializeCJson();
            if (Application.isEditor)
            {
                InitializeSocket();
            }
            LuaState.LuaSetTop(0);
            LuaCoroutine.Register(LuaState, Instance);
            if (Constants.SCRIPT_BUNDLE_MODE)
            {
                LuaLoader = new LuaLoader();
                // Avoid io exception on pc platform.
                if (Application.isMobilePlatform)
                {
                    LoadAll();
                }
            }
        }

        public static void PostInit(FileManifest.DifferInfo differ)
        {
            if (Constants.SCRIPT_BUNDLE_MODE && LuaLoader.zipMap.Count == 0)
            {
                LoadAll();
            }
            if (differ != null && Constants.SCRIPT_BUNDLE_MODE)
            {
                LoadDiff(differ);
            }
            LuaState.Start();
            LuaLooper = Instance.gameObject.AddComponent<LuaLooper>();
            LuaLooper.luaState = LuaState;
#if UNITY_EDITOR
            StackTrace = LuaState.GetFunction("debug.traceback");
#endif
            AssetManager.BeforeLoadAsset += LuaComponent.BeforeHook;
            AssetManager.AfterLoadAsset += LuaComponent.AfterHook;
        }

        private static void InitializeLibs()
        {
            LuaState.OpenLibs(LuaDLL.luaopen_pb);
            LuaState.OpenLibs(LuaDLL.luaopen_lpeg);
            LuaState.OpenLibs(LuaDLL.luaopen_bit);
        }

        private static void InitializeCJson()
        {
            LuaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            LuaState.OpenLibs(LuaDLL.luaopen_cjson);
            LuaState.LuaSetField(-2, "cjson");
            LuaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
            LuaState.LuaSetField(-2, "cjson.safe");
        }

        private static void InitializeSocket()
        {
            LuaState.OpenLibs(LuaDLL.luaopen_socket_core);
            LuaState.BeginPreLoad();
            LuaState.RegFunction("socket.core", LuaDLL.luaopen_socket_core);
            LuaState.RegFunction("mime.core", LuaDLL.luaopen_mime_core);
            LuaState.EndPreLoad();
        }

        private static void LoadAll()
        {
            string manifest = Helper.StringFormat("{0}{1}", Constants.LOCAL_LUA_BUNDLE_PATH, Constants.MANIFEST_FILE);
            if (File.Exists(manifest) == false)
            {
                return;
            }
            string[] lines = File.ReadAllLines(manifest);
            if (lines == null || lines.Length == 0)
            {
                return;
            }
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string bundleName = line.Split('|')[0];
                if (bundleName.EndsWith(Constants.LUA_BUNDLE_FILE_EXTENSION))
                {
                    LuaLoader.AddBundle(bundleName);
                }
            }
        }

        private static void LoadDiff(FileManifest.DifferInfo differ)
        {
            if (differ != null)
            {
                if (differ.Added.Count > 0)
                {
                    for (int i = 0; i < differ.Added.Count; i++)
                    {
                        FileManifest.FileInfo info = differ.Added[i];
                        string bundleName = info.Name;
                        if (bundleName.EndsWith(Constants.LUA_BUNDLE_FILE_EXTENSION))
                        {
                            LuaLoader.AddBundle(bundleName);
                        }
                    }
                }
                if (differ.Modified.Count > 0)
                {
                    for (int i = 0; i < differ.Modified.Count; i++)
                    {
                        FileManifest.FileInfo info = differ.Modified[i];
                        string bundleName = info.Name;
                        if (bundleName.EndsWith(Constants.LUA_BUNDLE_FILE_EXTENSION))
                        {
                            LuaLoader.RemoveBundle(bundleName);
                            LuaLoader.AddBundle(bundleName);
                        }
                    }
                }
                if (differ.Deleted.Count > 0)
                {
                    for (int i = 0; i < differ.Deleted.Count; i++)
                    {
                        FileManifest.FileInfo info = differ.Deleted[i];
                        string bundleName = info.Name;
                        if (bundleName.EndsWith(Constants.LUA_BUNDLE_FILE_EXTENSION))
                        {
                            LuaLoader.RemoveBundle(bundleName);
                        }
                    }
                }
            }
        }

        public static void DoFile(string filename)
        {
            if (LuaState != null && string.IsNullOrEmpty(filename) == false)
            {
                LuaState.DoFile(filename);
            }
        }

        public static void DoString(string str)
        {
            if (LuaState != null && string.IsNullOrEmpty(str) == false)
            {
                LuaState.DoString(str);
            }
        }

        public static void CallFunction(string fullFuncName, params object[] args)
        {
            LuaFunction func = LuaState.GetFunction(fullFuncName, false);
            if (func != null)
            {
                func.Call(args);
            }
        }

        public static LuaFunction GetFunction(LuaTable table, string module, string name)
        {
            LuaFunction func = LuaState.GetFunction(module + "." + name, false);
            if (func == null) return null;
            func = table.GetLuaFunction(name);
            return func;
        }

        public static void GC()
        {
            if (LuaState != null)
            {
                LuaState.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
            }
        }
    }

    public class LuaLoader : LuaFileUtils
    {
        public LuaLoader()
        {
            instance = this;
            beZip = true;
        }

        public void AddBundle(string bundleName)
        {
            string key = bundleName.Substring(0, bundleName.IndexOf(Constants.LUA_BUNDLE_FILE_EXTENSION));
            if (zipMap.ContainsKey(key) == true)
            {
                return;
            }
            string fileName = bundleName.ToLower();
            string url = Constants.LOCAL_LUA_BUNDLE_PATH + fileName;
            if (File.Exists(url))
            {
                AssetBundle bundle = Helper.LoadAssetBundle(url);
                if (bundle != null)
                {
                    AddSearchBundle(key, bundle);
                }
            }
            else
            {
                Helper.LogError(Constants.RELEASE_MODE ? null : "Add lua bundle fail caused by null file: {0}.", url);
            }
        }

        public void RemoveBundle(string bundleName)
        {
            string key = bundleName.Substring(0, bundleName.IndexOf(Constants.LUA_BUNDLE_FILE_EXTENSION));
            AssetBundle bundle;
            if (zipMap.TryGetValue(key, out bundle))
            {
                if (bundle)
                {
                    bundle.Unload(true);
                }
                zipMap.Remove(key);
            }
        }

        /// <summary>
        /// 当LuaVM加载Lua文件的时候，这里就会被调用，
        /// 用户可以自定义加载行为，只要返回byte[]即可
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override byte[] ReadFile(string fileName)
        {
            return base.ReadFile(fileName);
        }
    }
}