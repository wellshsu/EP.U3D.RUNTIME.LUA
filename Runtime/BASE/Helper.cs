//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using LuaInterface;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace EP.U3D.RUNTIME.LUA.BASE
{
    public class Helper : LIBRARY.BASE.Helper
    {
        public static new void Log(object format, params object[] args)
        {
            HandleLog(format, LogType.Log, args);
        }

        public static new void LogError(object format, params object[] args)
        {
            HandleLog(format, LogType.Error, args);
        }

        public static new void LogWarning(object format, params object[] args)
        {
            HandleLog(format, LogType.Warning, args);
        }

        private static void HandleLog(object format, LogType type, params object[] args)
        {
            string log = null;
            if (format != null && !(format is bool))
            {
                StackTrace trace = new StackTrace(true);
                StackFrame[] frames = trace.GetFrames();
                StackFrame frame = null;
                MethodBase method = null;
                if (frames != null && frames.Length > 2)
                {
                    frame = frames[2];
                    if (frame != null)
                    {
                        method = frame.GetMethod();
                    }
                }
                if (frame != null && method != null && method.DeclaringType != null)
                {
                    if (method.DeclaringType.FullName == "EP_U3D_RUNTIME_LUA_BASE_HelperWrap")
                    {
                        int lineNumber = LuaDLL.tolua_where(LuaManager.LuaState.L, 1);
                        string fileName = LuaDLL.lua_tostring(LuaManager.LuaState.L, -1);
                        string head = StringFormat("[{0}.lua:{1}]", fileName, lineNumber);
                        if (format is string && args != null && args.Length > 0)
                        {
                            string temp = StringFormat((string)format, args);
                            log = StringFormat("{0}{1}", head, temp);
                        }
                        else
                        {
                            log = StringFormat("{0}{1}", head, format);
                        }
#if UNITY_EDITOR
                        LuaManager.StackTrace.BeginPCall();
                        LuaManager.StackTrace.PCall();
                        string stackinfo = LuaManager.StackTrace.CheckString();
                        LuaManager.StackTrace.EndPCall();
                        if (!string.IsNullOrEmpty(log) && !string.IsNullOrEmpty(stackinfo))
                        {
                            log = StringFormat("{0}\n[Lua]{1}", log, stackinfo);
                        }
#endif
                    }
                    else
                    {
                        string head = StringFormat("[{0}]", method.DeclaringType.FullName);
                        if (format is string && args != null && args.Length > 0)
                        {
                            string temp = StringFormat((string)format, args);
                            log = StringFormat("{0}{1}", head, temp);
                        }
                        else
                        {
                            log = StringFormat("{0}{1}", head, format);
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(log) == false)
            {
                if (type == LogType.Log)
                {
                    UnityEngine.Debug.Log(log);
                }
                else if (type == LogType.Warning)
                {
                    UnityEngine.Debug.LogWarning(log);
                }
                else if (type == LogType.Error)
                {
                    UnityEngine.Debug.LogError(log);
                }
            }
        }

        [LuaByteBuffer]
        public static new byte[] OpenFile(string path)
        {
            byte[] bytes = new byte[0];
            try
            {
                if (File.Exists(path) == false)
                {
                    return bytes;
                }
                using (var file = File.OpenRead(path))
                {
                    if (file != null)
                    {
                        bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        file.Close();
                        file.Dispose();
                        return bytes;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
            return bytes;
        }
    }
}
