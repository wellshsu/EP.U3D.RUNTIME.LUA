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
using System.Collections.Generic;
using UnityEngine.UI;
using EP.U3D.LIBRARY.BASE;

namespace EP.U3D.RUNTIME.LUA.UI
{
    public class UIHelper : LIBRARY.UI.UIHelper
    {
        private static readonly LuaTable[] NIL_LOBJECT_ARR = new LuaTable[0];

        public static void SetButtonEvent(Object rootObj, LuaFunction func, LuaTable self = null)
        {
            SetButtonEvent(rootObj, null, func, self);
        }

        public static void SetButtonEvent(Object parentObj, string path, LuaFunction func, LuaTable self = null)
        {
            Button listener = GetComponent(parentObj, path, typeof(Button)) as Button;
            if (listener)
            {
                listener.onClick.AddListener(() => func.Call(self, listener.gameObject));
            }
        }

        public static LuaTable GetComponentInParent(Object rootObj, LuaTable type)
        {
            return GetComponentInParent(rootObj, null, type);
        }

        public static LuaTable GetComponentInParent(Object parentObj, string path, LuaTable type)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent comp = LuaComponent.GetInParent(root.gameObject, type);
                if (comp)
                {
                    return comp.Object;
                }
            }
            return null;
        }

        public static new object GetComponentInParent(Object rootObj, System.Type type)
        {
            return LIBRARY.UI.UIHelper.GetComponentInParent(rootObj, null, type);
        }

        public static new object GetComponentInParent(Object parentObj, string path, System.Type type)
        {
            return LIBRARY.UI.UIHelper.GetComponentInParent(parentObj, path, type);
        }

        public static LuaTable GetComponent(Object rootObj, LuaTable type)
        {
            return GetComponent(rootObj, null, type);
        }

        public static LuaTable GetComponent(Object parentObj, string path, LuaTable type)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent comp = LuaComponent.Get(root.gameObject, type);
                if (comp)
                {
                    return comp.Object;
                }
            }
            return null;
        }

        public static new object GetComponent(Object rootObj, System.Type type)
        {
            return LIBRARY.UI.UIHelper.GetComponent(rootObj, null, type);
        }

        public static new object GetComponent(Object parentObj, string path, System.Type type)
        {
            return LIBRARY.UI.UIHelper.GetComponent(parentObj, path, type);
        }

        public static LuaTable GetComponentInChildren(Object rootObj, LuaTable type, bool includeInactive = false)
        {
            return GetComponentInChildren(rootObj, null, type, includeInactive);
        }

        public static LuaTable GetComponentInChildren(Object parentObj, string path, LuaTable type, bool includeInactive = false)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent comp = LuaComponent.GetInChildren(root.gameObject, type, includeInactive);
                if (comp)
                {
                    return comp.Object;
                }
            }
            return null;
        }

        public static new object GetComponentInChildren(Object rootObj, System.Type type, bool includeInactive = false)
        {
            return LIBRARY.UI.UIHelper.GetComponentInChildren(rootObj, null, type, includeInactive);
        }

        public static new object GetComponentInChildren(Object parentObj, string path, System.Type type, bool includeInactive = false)
        {
            return LIBRARY.UI.UIHelper.GetComponentInChildren(parentObj, path, type, includeInactive);
        }

        public static LuaTable[] GetComponentsInParent(Object rootObj, LuaTable type, bool includeInactive = false)
        {
            return GetComponentsInParent(rootObj, null, type, includeInactive);
        }

        public static LuaTable[] GetComponentsInParent(Object parentObj, string path, LuaTable type, bool includeInactive = false)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return NIL_LOBJECT_ARR;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent[] comps = LuaComponent.GetsInParent(root.gameObject, type, includeInactive);
                List<LuaTable> rets = new List<LuaTable>();
                for (int i = 0; i < comps.Length; i++)
                {
                    rets.Add(comps[i].Object);
                }
                return rets.ToArray();
            }
            return NIL_LOBJECT_ARR;
        }

        public static new object[] GetComponentsInParent(Object rootObj, System.Type type, bool includeInactive = false)
        {
            return LIBRARY.UI.UIHelper.GetComponentsInParent(rootObj, null, type, includeInactive);
        }

        public static new object[] GetComponentsInParent(Object parentObj, string path, System.Type type, bool includeInactive = false)
        {
            return LIBRARY.UI.UIHelper.GetComponentsInParent(parentObj, path, type, includeInactive);
        }

        public static LuaTable[] GetComponents(Object rootObj, LuaTable type)
        {
            return GetComponents(rootObj, null, type);
        }

        public static LuaTable[] GetComponents(Object parentObj, string path, LuaTable type)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return NIL_LOBJECT_ARR;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent[] comps = LuaComponent.Gets(root.gameObject, type);
                List<LuaTable> rets = new List<LuaTable>();
                for (int i = 0; i < comps.Length; i++)
                {
                    rets.Add(comps[i].Object);
                }
                return rets.ToArray();
            }
            return NIL_LOBJECT_ARR;
        }

        public static new object[] GetComponents(Object rootObj, System.Type type)
        {
            return LIBRARY.UI.UIHelper.GetComponents(rootObj, null, type);
        }

        public static new object[] GetComponents(Object parentObj, string path, System.Type type)
        {
            return LIBRARY.UI.UIHelper.GetComponents(parentObj, path, type);
        }

        public static LuaTable[] GetComponentsInChildren(Object rootObj, LuaTable type, bool includeInactive = false)
        {
            return GetComponentsInChildren(rootObj, null, type, includeInactive);
        }

        public static LuaTable[] GetComponentsInChildren(Object parentObj, string path, LuaTable type, bool includeInactive = false)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return NIL_LOBJECT_ARR;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent[] comps = LuaComponent.GetsInChildren(root.gameObject, type, includeInactive);
                List<LuaTable> rets = new List<LuaTable>();
                for (int i = 0; i < comps.Length; i++)
                {
                    rets.Add(comps[i].Object);
                }
                return rets.ToArray();
            }
            return NIL_LOBJECT_ARR;
        }

        public static new object[] GetComponentsInChildren(Object rootObj, System.Type type, bool includeInactive = false)
        {
            return LIBRARY.UI.UIHelper.GetComponentsInChildren(rootObj, null, type, includeInactive);
        }

        public static new object[] GetComponentsInChildren(Object parentObj, string path, System.Type type, bool includeInactive = false)
        {
            return LIBRARY.UI.UIHelper.GetComponentsInChildren(parentObj, path, type, includeInactive);
        }

        public static LuaTable AddComponent(Object rootObj, LuaTable type)
        {
            return AddComponent(rootObj, null, type);
        }

        public static LuaTable AddComponent(Object parentObj, string path, LuaTable type)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent.DType = type;
                LuaComponent comp = root.gameObject.AddComponent<LuaComponent>();
                return comp.Object;
            }
            else
            {
                return null;
            }
        }

        public static new object AddComponent(Object rootObj, System.Type type)
        {
            return LIBRARY.UI.UIHelper.AddComponent(rootObj, null, type);
        }

        public static new object AddComponent(Object parentObj, string path, System.Type type)
        {
            return LIBRARY.UI.UIHelper.AddComponent(parentObj, path, type);
        }

        public static void RemoveComponent(Object rootObj, LuaTable type)
        {
            RemoveComponent(rootObj, null, type, false);
        }

        public static void RemoveComponent(Object rootObj, LuaTable type, bool immediate)
        {
            RemoveComponent(rootObj, null, type, immediate);
        }

        public static void RemoveComponent(Object parentObj, string path, LuaTable type)
        {
            RemoveComponent(parentObj, path, type, false);
        }

        public static void RemoveComponent(Object parentObj, string path, LuaTable type, bool immediate)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent comp = LuaComponent.Get(root.gameObject, type);
                if (comp)
                {
                    if (immediate)
                    {
                        Object.DestroyImmediate(comp);
                    }
                    else
                    {
                        Object.Destroy(comp);
                    }
                }
            }
        }

        public static new void RemoveComponent(Object rootObj, System.Type type)
        {
            LIBRARY.UI.UIHelper.RemoveComponent(rootObj, null, type, false);
        }

        public static new void RemoveComponent(Object rootObj, System.Type type, bool immediate)
        {
            LIBRARY.UI.UIHelper.RemoveComponent(rootObj, null, type, immediate);
        }

        public static new void RemoveComponent(Object parentObj, string path, System.Type type)
        {
            LIBRARY.UI.UIHelper.RemoveComponent(parentObj, path, type, false);
        }

        public static new void RemoveComponent(Object parentObj, string path, System.Type type, bool immediate)
        {
            LIBRARY.UI.UIHelper.RemoveComponent(parentObj, path, type, immediate);
        }

        public static LuaTable SetComponentEnabled(Object rootObj, LuaTable type, bool enabled)
        {
            return SetComponentEnabled(rootObj, null, type, enabled);
        }

        public static LuaTable SetComponentEnabled(Object parentObj, string path, LuaTable type, bool enabled)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                LuaComponent comp = LuaComponent.Get(root.gameObject, type);
                if (comp)
                {
                    comp.enabled = enabled;
                    return comp.Object;
                }
            }
            return null;
        }

        public static new object SetComponentEnabled(Object rootObj, System.Type type, bool enabled)
        {
            return LIBRARY.UI.UIHelper.SetComponentEnabled(rootObj, null, type, enabled);
        }

        public static new object SetComponentEnabled(Object parentObj, string path, System.Type type, bool enabled)
        {
            return LIBRARY.UI.UIHelper.SetComponentEnabled(parentObj, path, type, enabled);
        }
    }
}
