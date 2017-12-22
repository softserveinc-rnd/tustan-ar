/* 
 * This file is part of the Tustan AR distribution 
 * (https://github.com/softserveinc-rnd/tustan-ar).
 * Copyright (c) 2017 Softserve Inc.
 *
 * Tustan 3D model Copyright 2017 Vasyl Rozhko
 * 3D model of the 5th period of the cliffside city-fortress Tustan's log 
 * constructions was created on the basis of the architectural analysis 
 * of graphic reconstructions by Mykhailo Rozhko (1939-2004), an architect,  
 * archeologist and researcher of Tustan. 
 * 3D model's creators: Vasyl Rozhko, Maksym Yasinskyi, Vasyl Dmytruk, 
 * Oleh Rybchynskyi and Andriy Dedyshyn.
 * 
 * This file is part of Tustan AR.
 *
 * Tustan AR is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Tustan AR is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Tustan AR. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TustanAR{
    using UI.Windows;

    /// Class that simulates stack of windows (activities, uicontrollers).
    public static class History {
        /// Stack of actions to close all open windows considering overlaying.
        private static List<IUIWindow> callStack = new List<IUIWindow>();

        public static void GoBack() {
            int windows_count = callStack.Count;
                if(windows_count > 0){
                    IUIWindow currentWindow = callStack[windows_count - 1];
                    currentWindow.Close();
                    callStack.RemoveAt(windows_count - 1);
                }
                else {
                    Minimize();
                }
        }

        public static void AddWindow(IUIWindow window){
            callStack.Add(window);
        }

        private static void Minimize (){
            #if UNITY_ANDROID
            using(AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")){ 
                if(unityPlayer==null){ return; }           
                using(AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity")){
                    using(AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent")){
                        using(AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent", intentClass.GetStatic<string> ("ACTION_MAIN"))){
                            intentObject.Call<AndroidJavaObject>("addCategory", intentClass.GetStatic<string>("CATEGORY_HOME"));
                            currentActivity.Call("startActivity", intentObject);
                        }
                    }
                }
            }
            #endif
        }
    }
}