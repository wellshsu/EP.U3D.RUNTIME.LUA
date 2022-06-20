﻿//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using LuaInterface;

namespace EP.U3D.RUNTIME.LUA.EVT
{
    public class Evt : LIBRARY.EVT.Evt
    {
        [LuaByteBuffer]
        public byte[] LParam;
    }
}