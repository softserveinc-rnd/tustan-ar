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

using UnityEngine;
using System;
using System.Collections;

namespace TustanAR.UI
{
    using Controls;
    using Locale;
    using Native;

    ///
    /// Class that gives functionality of screensots capturing.
    ///
    public sealed class ScreenshotManager: MonoBehaviour{
        public static bool SCREENSHOT_PROCESSING = false;
        public static bool SCREENSHOT_PROCESSING_ERROR = false;
        public static string SCREENSHOT_PATH = "";
        public static string SCREENSHOT_ERROR_MESSAGE = "";
        public static bool SCREENSHOT_SHARED = false;

        public void Capture(){
            StartCoroutine(CaptureScreenshot());
        }

        private IEnumerator CaptureScreenshot(){

            Runtime.makeScreenshotButton.SetColor(Color.grey);
            string screenshot_name = "screenshot_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            int bottom_margin = (int)(Relative.LOWER_PANEL_HEIGHT);
            int top_margin = Screen.height - (int)(new DP(48f)); // upper panel height
            ScreenshotManager.SCREENSHOT_PROCESSING = true;
            NativeAdapter.Instance.MakeScreenshot(screenshot_name, bottom_margin, top_margin, Input.deviceOrientation);
            
            while(ScreenshotManager.SCREENSHOT_PROCESSING)
                yield return null;

            Runtime.makeScreenshotButton.SetColor(Color.white);

            #if UNITY_ANDROID
            IDialog id = new Dialog.Builder()
                .SetIcon(SpriteLoader.LoadSprite("share_ico"))
                .SetButton(LocalizationManager.Instance.GetLocalizedValue("ok"), ()=>{
                    NativeAdapter.Instance.ShareFile(LocalizationManager.Instance.GetLocalizedValue("share_image_using"), LocalizationManager.Instance.GetLocalizedValue("share_play_store") + " " + Config.urlToPlayStore, ScreenshotManager.SCREENSHOT_PATH, "image/jpeg");
                })
                .SetText(LocalizationManager.Instance.GetLocalizedValue("screenshot_captured"))
                .SetView(Relative.CAMERA_WINDOW_RECT)
                .build();

            id.show();
            yield return new WaitForSeconds(3f);
            if(id != null && id.isActive){
                id.dismiss();
            }
            #elif UNITY_IOS
            if(ScreenshotManager.SCREENSHOT_PROCESSING_ERROR){
                IDialog id = new Dialog.Builder()
                    .SetIcon(SpriteLoader.LoadSprite("share_ico"))
                    .SetButton(LocalizationManager.Instance.GetLocalizedValue("ok"), ()=>{})
                    .SetText(ScreenshotManager.SCREENSHOT_ERROR_MESSAGE)
                    .SetView(Relative.CAMERA_WINDOW_RECT)
                    .build();

                id.show();
                yield return new WaitForSeconds(3f);
                if(id != null && id.isActive){
                    id.dismiss();
                }
            }
            #endif
        }
    }
}