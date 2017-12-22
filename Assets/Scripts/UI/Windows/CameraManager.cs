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
using Vuforia;

namespace TustanAR.UI.Windows
{
    using Controls;
    using Animation;
    using VuforiaExtensions;

    /// 
    /// Sigleton which works with UI Canvas on AR Camera window.
    /// 
    public sealed class CameraManager: IUIWindow {

        private GameObject cameraWindow;

        private static CameraManager instance;
    
        private CameraManager() {}

        public static CameraManager Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new CameraManager();
                }
                return instance;
            }
        }

        /// Set calues for proper working AR Camera window.
        public void Init(){
            this.cameraWindow = new GameObject(Names.CAMERA_WINDOW_NAME);
            this.cameraWindow.transform.SetParent(Runtime.canvas.transform);

            Runtime.cameraWindow = this.cameraWindow;
        }

        public void Close(){
            // this window is never closed
        }

        public void ShowCameraWindow(){
            CreateLowerPanel();
            CreateUpperPanel();
        }

        private void CreateLowerPanel(){
            /* LOWER PANEL */
            GameObject lowerPanelObject = new GameObject(Names.LOWER_PANEL_NAME, typeof(UnityEngine.UI.Image));
            UnityEngine.UI.Image lowerPanelImage = lowerPanelObject.GetComponent<UnityEngine.UI.Image>();
            lowerPanelImage.color = Color.black;

            RectTransform lowerPanelRT = lowerPanelObject.GetComponent<RectTransform>();
            lowerPanelRT.sizeDelta = new Vector2(Screen.width, Relative.LOWER_PANEL_HEIGHT);
            lowerPanelRT.position = new Vector2(Screen.width, Relative.LOWER_PANEL_HEIGHT) / 2f;

            lowerPanelRT.SetParent(cameraWindow.transform);

            // "make photo" button
            Runtime.makeScreenshotButton = new Image.Builder()
                .SetName(Names.CAPTURE_SCREENSHOT_BUTTON_NAME)
                .AddOnClickListener(Runtime.canvas.GetComponent<ScreenshotManager>().Capture)
                .SetImage(SpriteLoader.LoadSprite("shutter_ico"))
                .SetParent(lowerPanelRT)
                .SetSize(size: Vector2.one * Relative.MAKE_PHOTO_BUTTON_HEIGHT, additionalClickableArea:1.5f)
                .SetPosition(new Vector2(Screen.width / 2f, Relative.LOWER_PANEL_HEIGHT / 2f))
                .SetRotationPoint(RotationPoint.Center)
                .build();

            // "revert camera" button
            new Image.Builder()
                .SetName(Names.REVERT_CAMERA_BUTTON_NAME)
                .AddOnClickListener(OnRevertCamera)
                .SetImage(SpriteLoader.LoadSprite("camera_ico"))
                .SetParent(lowerPanelRT)
                .SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:3f)
                .SetPosition(new Vector2(Screen.width / 4f, Relative.LOWER_PANEL_HEIGHT / 2f))
                .SetRotationPoint(UI.RotationPoint.Center)
                .build();

            Runtime.playButton = new UI.Controls.Image.Builder()
                .SetName("Play Button")
                .SetImage(SpriteLoader.LoadSprite("play_ico"))
                .SetParent(lowerPanelRT)
                .AddOnClickListener(delegate(){
                    Runtime.pauseButton.gameObject.SetActive(true);
                    AnimationHandler.ANIMATION_ENABLED = true;
                    Runtime.playButton.gameObject.SetActive(false);
                })
                .SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:3f)
                .SetPosition(new Vector2(3 * Screen.width / 4f, Relative.LOWER_PANEL_HEIGHT / 2f))
                .SetRotationPoint(RotationPoint.Center)
                .SetActive(false)
                .build()
                .gameObject;

            Runtime.pauseButton = new UI.Controls.Image.Builder()
                .SetName("Pause Button")
                .SetImage(SpriteLoader.LoadSprite("pause_ico"))
                .SetParent(lowerPanelRT)
                .AddOnClickListener(delegate(){
                    Runtime.playButton.gameObject.SetActive(true);
                    AnimationHandler.ANIMATION_ENABLED = false;
                    Runtime.pauseButton.gameObject.SetActive(false);
                })
                .SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:3f)
                .SetPosition(new Vector2(3 * Screen.width / 4f, Relative.LOWER_PANEL_HEIGHT / 2f))
                .SetRotationPoint(RotationPoint.Center)
                .SetActive(false)
                .build()
                .gameObject;
        }

        private void CreateUpperPanel() {

            GameObject upperPanelObject = new GameObject(Names.UPPER_PANEL_NAME, typeof(UnityEngine.UI.Image));
            upperPanelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Relative.UPPER_PANEL_HEIGTH);
            upperPanelObject.transform.position = new Vector2(Screen.width / 2f, Screen.height - Relative.UPPER_PANEL_HEIGTH / 2f);
            upperPanelObject.GetComponent<UnityEngine.UI.Image>().color = Color.black;
            upperPanelObject.transform.SetParent(cameraWindow.transform);

            new UI.Controls.Image.Builder()
                .SetName(Names.MAP_BUTTON_NAME)
                .AddOnClickListener(MapManager.Instance.ShowMap)
                .SetImage(SpriteLoader.LoadSprite("map_ico"))
                .SetParent(upperPanelObject.transform)
                .SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:2f)
                .SetPosition(new Vector2(Screen.width - Relative.BUTTON_HEIGHT - 3 * Relative.BUTTON_MARGIN - Relative.BUTTON_HEIGHT / 2f, Screen.height - Relative.BUTTON_MARGIN - Relative.BUTTON_HEIGHT / 2f))
                .SetRotationPoint(UI.RotationPoint.Center)
                .build();

            new UI.Controls.Image.Builder()
                .SetName(Names.INFO_BUTTON_NAME)
                .AddOnClickListener(InfoManager.Instance.ShowInfo)
                .SetImage(SpriteLoader.LoadSprite("info_ico"))
                .SetParent(upperPanelObject.transform)
                .SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:2f)
                .SetPosition(new Vector2(Screen.width - Relative.BUTTON_MARGIN - Relative.BUTTON_MARGIN / 2f - Relative.BUTTON_HEIGHT / 2f, Screen.height - Relative.BUTTON_MARGIN - Relative.BUTTON_HEIGHT / 2f)) // Relative.BUTTON_MARGIN / 2f in width is additional
                .SetRotationPoint(UI.RotationPoint.Center)
                .build();

            Runtime.resetExtendedTrackingButton = new UI.Controls.Image.Builder()
                .SetName(Names.RESET_EXTENDED_TRACKING_BUTTON)
                .AddOnClickListener(ExtendedTrackingBehaviour.ResetExtendedTracking)
                .SetImage(SpriteLoader.LoadSprite("reset_ico"))
                .SetParent(upperPanelObject.transform)
                .SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:2f)
                .SetPosition(new Vector2(Screen.width - Relative.BUTTON_MARGIN - Relative.BUTTON_MARGIN / 2f - Relative.BUTTON_HEIGHT / 2f, Screen.height - Relative.UPPER_PANEL_HEIGTH - Relative.BUTTON_MARGIN - Relative.BUTTON_HEIGHT / 2f)) // Relative.BUTTON_MARGIN / 2f in width is additional
                .SetRotationPoint(UI.RotationPoint.Center)
                .SetActive(false)
                .build()
                .gameObject;
        }

        private void OnRevertCamera(){
            CameraDevice.CameraDirection newDirection;
            if(CameraDevice.Instance.GetCameraDirection() == CameraDevice.CameraDirection.CAMERA_FRONT){
                newDirection = CameraDevice.CameraDirection.CAMERA_BACK;
            }
            else{
                newDirection = CameraDevice.CameraDirection.CAMERA_FRONT;
            }

            CameraDevice.Instance.Stop();
            CameraDevice.Instance.Deinit();
    
            CameraDevice.Instance.Init(newDirection);
            CameraDevice.Instance.Start();
        }
    }
}