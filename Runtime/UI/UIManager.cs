//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using EP.U3D.LIBRARY.UI;
using LuaInterface;
using System.Collections.Generic;

namespace EP.U3D.RUNTIME.LUA.UI
{
    public class UIManager : LIBRARY.UI.UIManager
    {
        public static UIWindow OpenWindow(LuaTable _target, LuaTable _below = null, LuaTable _above = null)
        {
            return OpenWindow(new UIMeta(_target), new UIMeta(_below), new UIMeta(_above));
        }

        public static void FocusWindow(LuaTable _meta, bool always)
        {
            FocusWindow(new UIMeta(_meta), always);
        }

        public static void CloseWindow(LuaTable _meta, bool resume = true)
        {
            CloseWindow(new UIMeta(_meta), resume);
        }

        public static void CloseAllWindowsExcept(params LuaTable[] _filter)
        {
            List<UIMeta> filter = new List<UIMeta>();
            for (int i = 0; i < _filter.Length; i++)
            {
                var f = new UIMeta(_filter[i]);
                filter.Add(f);
            }
            CloseAllWindowsExcept(filter.ToArray());
        }

        public static UIWindow IsWindowOpened(LuaTable _meta)
        {
            return IsWindowOpened(new UIMeta(_meta));
        }
    }
}