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

namespace EP.U3D.RUNTIME.LUA.UI
{
    public class UIMeta : LIBRARY.UI.UIMeta
    {
        public LuaTable Table;
        public UIMeta(LuaTable table) { Table = table; }

        public bool Cached()
        {
            if (Table != null) { return Table.RawGet<string, bool>("_Cached"); }
            else { return false; }
        }

        public int FixedRQ()
        {
            if (Table != null) { return Table.RawGet<string, int>("_FixedRQ"); }
            else { return 0; }
        }

        public bool Focus()
        {
            if (Table != null) { return Table.RawGet<string, bool>("_Focus"); }
            else { return false; }
        }

        public string Name()
        {
            if (Table != null) { return Table.RawGet<string, string>("_Name"); }
            else { return string.Empty; }
        }

        public bool NoRoot()
        {
            if (Table != null) { return Table.RawGet<string, bool>("_NoRoot"); }
            else { return false; }
        }

        public string Path()
        {
            if (Table != null) { return Table.RawGet<string, string>("_Path"); }
            else { return string.Empty; }
        }
    }
}