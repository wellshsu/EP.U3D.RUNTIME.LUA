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
        public LuaTable LScene;

        private string mName;
        private LuaFunction mAwakeFunc;
        private LuaFunction mStartFunc;
        private LuaFunction mUpdateFunc;
        private LuaFunction mStopFunc;

        public override string Name() { return mName; }

        public Scene(string name, LuaTable scene)
        {
            LScene = scene;
            mName = name;
            mAwakeFunc = LScene.GetLuaFunction("Awake");
            mStartFunc = LScene.GetLuaFunction("Start");
            mUpdateFunc = LScene.GetLuaFunction("Update");
            mStopFunc = LScene.GetLuaFunction("Stop");
        }

        public override void Awake() { mAwakeFunc?.Call(LScene); }

        public override void Start() { mStartFunc?.Call(LScene); }

        public override void Update() { mUpdateFunc?.Call(LScene); }

        public override void Stop() { mStopFunc?.Call(LScene); }
    }
}