//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using UnityEngine;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.Text;
using EP.U3D.LIBRARY.BASE;

namespace EP.U3D.RUNTIME.LUA
{
    public class LuaComponent : MonoBehaviour
    {
        [Serializable]
        public class Field
        {
            public string Key;
            public string Type;
            public UnityEngine.Object OValue;
            public byte[] BValue = new byte[16]; // max struct is vector4 with 16 bytes.

            public void Reset()
            {
                OValue = null;
                BValue = new byte[16];
            }
        }

        public string Module;
        public string Script;
        [NoToLua] [NonSerialized] public bool Inited;
        [NoToLua] [NonSerialized] public bool InitOK;
        [NoToLua] [NonSerialized] public LuaTable Object;
        [NoToLua] public List<Field> Fields = new List<Field>();

        private LuaFunction mAwakeFunc;
        private LuaFunction mStartFunc;
        private LuaFunction mOnEnableFunc;
        private LuaFunction mOnDisableFunc;
        private LuaFunction mUpdateFunc;
        private LuaFunction mLateUpdateFunc;
        private LuaFunction mFixedUpdateFunc;
        private LuaFunction mOnDestroyFunc;
        private LuaFunction mOnTriggerEnterFunc;
        private LuaFunction mOnTriggerExitFunc;
        private LuaFunction mOnCollisionEnterFunc;
        private LuaFunction mOnCollisionExitFunc;

        #region for loadasset hook
        [NoToLua] public static LuaTable DType = null; // for dynamic addcomponent
        [NoToLua] public static bool frame = false;
        [NoToLua] public static readonly List<LuaComponent> comps = new List<LuaComponent>();
        LuaComponent() { if (frame) comps.Add(this); }
        [NoToLua]
        public static void BeforeHook()
        {
            frame = true;
        }
        [NoToLua]
        public static void AfterHook()
        {
            if (comps.Count > 0)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    LuaComponent comp = comps[i];
                    if (comp && !comp.Inited) comp.Init();
                }
            }
            frame = false;
            comps.Clear();
        }
        #endregion

        [NoToLua]
        public void Init()
        {
            if (Inited) return;
            Inited = true;
            if (DType != null)
            {
                try
                {
                    object[] rets = LuaManager.LuaNEWFunc.LazyCall(DType);
                    if (rets.Length != 1)
                    {
                        Helper.LogError(Constants.RELEASE_MODE ? null : "META-{0}: error caused in NEW() function.", DType.ToString());
                        enabled = false;
                        return;
                    }
                    Object = (LuaTable)rets[0];
                    if (Object == null)
                    {
                        Helper.LogError(Constants.RELEASE_MODE ? null : "META-{0}: error caused by nil NEW() ret.", DType.ToString());
                        enabled = false;
                        return;
                    }
                    // TODO: 因Module和Script未赋值，所以编辑器模式下无法检视字段
                }
                catch (Exception e)
                {
                    Helper.LogError(Constants.RELEASE_MODE ? null : "META-{0}: error {1}", DType.ToString(), e.Message);
                    enabled = false;
                    return;
                }
                finally { DType = null; }
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(Script))
                    {
                        enabled = false;
                        return;
                    }
                    LuaTable meta = LuaManager.LuaState.GetTable(Script, false);
                    if (meta == null)
                    {
                        LuaManager.LuaState.DoString(Helper.StringFormat("require '{0}.{1}'", Module, Script));
                        meta = LuaManager.LuaState.GetTable(Script, true);
                    }
                    object[] rets = LuaManager.LuaNEWFunc.LazyCall(meta);
                    if (rets.Length != 1)
                    {
                        Helper.LogError(Constants.RELEASE_MODE ? null : "{0}.{1}: error caused in NEW() function.", Module, Script);
                        enabled = false;
                        return;
                    }
                    Object = (LuaTable)rets[0];
                    if (Object == null)
                    {
                        Helper.LogError(Constants.RELEASE_MODE ? null : "{0}.{1}: error caused by nil NEW() ret.", Module, Script);
                        enabled = false;
                        return;
                    }
                }
                catch (Exception e)
                {
                    Helper.LogError(Constants.RELEASE_MODE ? null : "{0}.{1}: error {2}", Module, Script, e.Message);
                    enabled = false;
                    return;
                }
            }
            // TOFIX[20220612]: 若子类无以下内置函数，则无法调用，需修复支持调用父类内置函数
            mAwakeFunc = Object.GetLuaFunction("Awake");
            mStartFunc = Object.GetLuaFunction("Start");
            mOnEnableFunc = Object.GetLuaFunction("OnEnable");
            mOnDisableFunc = Object.GetLuaFunction("OnDisable");
            mUpdateFunc = Object.GetLuaFunction("Update");
            mLateUpdateFunc = Object.GetLuaFunction("LateUpdate");
            mFixedUpdateFunc = Object.GetLuaFunction("FixedUpdate");
            mOnDestroyFunc = Object.GetLuaFunction("OnDestroy");
            mOnTriggerEnterFunc = Object.GetLuaFunction("OnTriggerEnter");
            mOnTriggerExitFunc = Object.GetLuaFunction("OnTriggerExit");
            mOnCollisionEnterFunc = Object.GetLuaFunction("OnCollisionEnter");
            mOnCollisionExitFunc = Object.GetLuaFunction("OnCollisionExit");
            Object.RawSet("transform", transform);
            Object.RawSet("gameObject", gameObject);
            InitOK = true;
        }

        protected virtual void Awake()
        {
//#if UNITY_EDITOR
//            // [20220331]:单元模式
//            if (LuaManager.Instance == null)
//            {
//                LuaManager.PreInit();
//                LuaManager.PostInit();
//                LuaManager.DoString("require 'Core.Launcher'");
//            }
//#endif
            if (!Inited) Init();
            if (!InitOK) return;
            if (Fields != null && Fields.Count > 0)
            {
                for (int i = 0; i < Fields.Count; i++)
                {
                    var field = Fields[i];
                    if (field.Type == "int")
                    {
                        int v = BitConverter.ToInt32(field.BValue, 0);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "long")
                    {
                        long v = BitConverter.ToInt64(field.BValue, 0);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "float")
                    {
                        float v = BitConverter.ToSingle(field.BValue, 0);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "double")
                    {
                        double v = BitConverter.ToDouble(field.BValue, 0);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "bool" || field.Type == "boolean")
                    {
                        bool v = BitConverter.ToBoolean(field.BValue, 0);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "UnityEngine.Vector2")
                    {
                        Vector2 v = Helper.ByteToStruct<Vector2>(field.BValue);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "UnityEngine.Vector3")
                    {
                        Vector3 v = Helper.ByteToStruct<Vector3>(field.BValue);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "UnityEngine.Vector4")
                    {
                        Vector4 v = Helper.ByteToStruct<Vector4>(field.BValue);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "UnityEngine.Color")
                    {
                        Color v = Helper.ByteToStruct<Color>(field.BValue);
                        Object.RawSet(field.Key, v);
                    }
                    else if (field.Type == "string")
                    {
                        string v = Encoding.UTF8.GetString(field.BValue);
                        Object.RawSet(field.Key, v);
                    }
                    else
                    {
                        if (field.OValue)
                        {
                            if (field.OValue is LuaComponent)
                            {
                                LuaComponent c = field.OValue as LuaComponent;
                                if (c.Script == field.Type)
                                {
                                    if (!c.Inited) c.Init();
                                    Object.RawSet(field.Key, c.Object);
                                }
                            }
                            else
                            {
                                Object.RawSet(field.Key, field.OValue);
                            }
                        }
                    }
                }
            }
            if (Application.isPlaying) // release memory
            {
                Fields.Clear();
                Fields = null;
            }
            if (mAwakeFunc != null) { mAwakeFunc.Call(Object); }
        }

        protected virtual void Start()
        {
            if (mStartFunc != null) { mStartFunc.Call(Object); }
        }

        protected virtual void OnEnable()
        {
            if (Object != null) Object.RawSet("enabled", enabled);
            if (mOnEnableFunc != null) { mOnEnableFunc.Call(Object); }
        }

        protected virtual void OnDisable()
        {
            if (Object != null) Object.RawSet("enabled", enabled);
            if (mOnDisableFunc != null) { mOnDisableFunc.Call(Object); }
        }

        protected virtual void Update()
        {
            if (mUpdateFunc != null) { mUpdateFunc.Call(Object); }
        }

        protected virtual void LateUpdate()
        {
            if (mLateUpdateFunc != null) { mLateUpdateFunc.Call(Object); }
        }

        protected virtual void FixedUpdate()
        {
            if (mFixedUpdateFunc != null) { mFixedUpdateFunc.Call(Object); }
        }

        protected virtual void OnDestroy()
        {
            if (mOnDestroyFunc != null) { mOnDestroyFunc.Call(Object); }
            if (mAwakeFunc != null)
            {
                mAwakeFunc.Dispose();
                mAwakeFunc = null;
            }
            if (mStartFunc != null)
            {
                mStartFunc.Dispose();
                mStartFunc = null;
            }
            if (mOnEnableFunc != null)
            {
                mOnEnableFunc.Dispose();
                mOnEnableFunc = null;
            }
            if (mOnDisableFunc != null)
            {
                mOnDisableFunc.Dispose();
                mOnDisableFunc = null;
            }
            if (mUpdateFunc != null)
            {
                mUpdateFunc.Dispose();
                mUpdateFunc = null;
            }
            if (mLateUpdateFunc != null)
            {
                mLateUpdateFunc.Dispose();
                mLateUpdateFunc = null;
            }
            if (mFixedUpdateFunc != null)
            {
                mFixedUpdateFunc.Dispose();
                mFixedUpdateFunc = null;
            }
            if (mOnDestroyFunc != null)
            {
                mOnDestroyFunc.Dispose();
                mOnDestroyFunc = null;
            }
            if (mOnTriggerEnterFunc != null)
            {
                mOnTriggerEnterFunc.Dispose();
                mOnTriggerEnterFunc = null;
            }
            if (mOnTriggerExitFunc != null)
            {
                mOnTriggerExitFunc.Dispose();
                mOnTriggerExitFunc = null;
            }
            if (mOnCollisionEnterFunc != null)
            {
                mOnCollisionEnterFunc.Dispose();
                mOnCollisionEnterFunc = null;
            }
            if (mOnCollisionExitFunc != null)
            {
                mOnCollisionExitFunc.Dispose();
                mOnCollisionExitFunc = null;
            }
            if (Object != null)
            {
                Object.RawSet("__missing", "this component has been destroyed.");
                Object.RawSet<string, object>("enabled", null);
                Object.RawSet<string, object>("transform", null);
                Object.RawSet<string, object>("gameObject", null);
            }
            Object = null;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (mOnTriggerEnterFunc != null) { mOnTriggerEnterFunc.Call(Object, other); }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (mOnTriggerExitFunc != null) { mOnTriggerExitFunc.Call(Object, other); }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (mOnCollisionEnterFunc != null) { mOnCollisionEnterFunc.Call(Object, collision); }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (mOnCollisionExitFunc != null) { mOnCollisionExitFunc.Call(Object, collision); }

        }

        public static LuaComponent GetInParent(GameObject go, LuaTable type)
        {
            LuaComponent[] rets = HandleGets(go, type, -1);
            if (rets.Length > 0)
            {
                return rets[0];
            }
            else
            {
                return null;
            }
        }

        public static LuaComponent Get(GameObject go, LuaTable type)
        {
            LuaComponent[] rets = HandleGets(go, type, 0);
            if (rets.Length > 0)
            {
                return rets[0];
            }
            else
            {
                return null;
            }
        }

        public static LuaComponent GetInChildren(GameObject go, LuaTable type, bool includeInactive = false)
        {
            LuaComponent[] rets = HandleGets(go, type, 1, includeInactive);
            if (rets.Length > 0)
            {
                return rets[0];
            }
            else
            {
                return null;
            }
        }

        public static LuaComponent[] GetsInParent(GameObject go, LuaTable type, bool includeInactive = false)
        {
            return HandleGets(go, type, -1, includeInactive);
        }

        public static LuaComponent[] Gets(GameObject go, LuaTable type)
        {
            return HandleGets(go, type, 0);
        }

        public static LuaComponent[] GetsInChildren(GameObject go, LuaTable type, bool includeInactive = false)
        {
            return HandleGets(go, type, 1, includeInactive);
        }

        private static LuaComponent[] HandleGets(GameObject go, LuaTable type, int depth, bool includeInactive = false)
        {
            if (go == null)
            {
                Helper.LogError(Constants.RELEASE_MODE ? null : "error caused by nil gameObject.");
                return null;
            }
            if (type == null)
            {
                Helper.LogError(Constants.RELEASE_MODE ? null : "error caused by nil metatable.");
                return null;
            }
            LuaComponent[] coms;
            if (depth == -1)
            {
                coms = go.GetComponentsInParent<LuaComponent>(includeInactive);
            }
            else if (depth == 0)
            {
                coms = go.GetComponents<LuaComponent>();
            }
            else
            {
                coms = go.GetComponentsInChildren<LuaComponent>(includeInactive);
            }
            List<LuaComponent> rets = new List<LuaComponent>();
            for (int i = 0; i < coms.Length; i++)
            {
                var com = coms[i];
                if (com != null && com.Object != null)
                {
                    LuaTable meta = com.Object.GetMetaTable();
                    if (type == meta)
                    {
                        rets.Add(com);
                    }
                    else
                    {
                        LuaTable pmeta = meta;
                        while (true)
                        {
                            LuaTable bmeta = pmeta.RawGet<string, LuaTable>("BASE");
                            if (bmeta == null)
                            {
                                break;
                            }
                            else if (bmeta == type)
                            {
                                rets.Add(com);
                                break;
                            }
                            else
                            {
                                pmeta = bmeta;
                            }
                        }
                    }
                }
            }
            return rets.ToArray();
        }
    }
}