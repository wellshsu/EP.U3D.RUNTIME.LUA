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

namespace EP.U3D.RUNTIME.LUA.SCENE
{
    public class Scene : LIBRARY.SCENE.Scene
    {
        private string mName;
        private LuaTable mScene;
        private LuaFunction mAwakeFunc;
        private LuaFunction mStartFunc;
        private LuaFunction mUpdateFunc;
        private LuaFunction mStopFunc;

        public override string Name() { return mName; }

        public Scene(string name, LuaTable scene)
        {
            mName = name;
            mScene = scene;
            mAwakeFunc = mScene.GetLuaFunction("Awake");
            mStartFunc = mScene.GetLuaFunction("Start");
            mUpdateFunc = mScene.GetLuaFunction("Update");
            mStopFunc = mScene.GetLuaFunction("Stop");
        }

        public override void Awake() { mAwakeFunc?.Call(mScene); }

        public override void Start() { mStartFunc?.Call(mScene); }

        public override void Update() { mUpdateFunc?.Call(mScene); }

        public override void Stop() { mStopFunc?.Call(mScene); }
    }
}