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
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TustanAR.UI.Controls{
    using Windows;
    
    public sealed class ControlsAnimator : MonoBehaviour, IUIWindow{
        private IEnumerator runningCoroutine;
        private GameObject toast;
        private GameObject dialog;

        public void show(Toast t){
            destroyAllControls();

            runningCoroutine = showToast(t);
            StartCoroutine(runningCoroutine);
        }

        public void hide(Toast t){
            runningCoroutine = hideToast(t);
            StartCoroutine(runningCoroutine);
        }

        public void show(Dialog d){
            destroyAllControls();
            runningCoroutine = showDialog(d);
            StartCoroutine(runningCoroutine);
        }

        public void destroyAllControls(){
            if(runningCoroutine != null){
                StopCoroutine(runningCoroutine);
                GameObject.Destroy(toast);
                GameObject.Destroy(dialog);
            }
        }

        public void Close(){
            destroyAllControls();
        }

        public IEnumerator showToast(Toast t){

            yield return null; // for proper toast rendering

            // CREATION PART
            toast = GameObject.Instantiate(Runtime.toastPrefab);
            toast.name = "Toast";
            toast.transform.position = Vector2.one * (-1000f); // out of screen
            toast.transform.SetParent(Runtime.canvas.transform);
            RectTransform toastRT  = toast.GetComponent<RectTransform>();

            GameObject toastText = toast.transform.Find("Text").gameObject;
            Text toastTextComponent = toastText.GetComponent<Text>();

            toastTextComponent.fontSize = Relative.SMALL_FONT_SIZE;
            toastTextComponent.text = t.message;

            yield return null;

            float preferredWidth = toastTextComponent.preferredWidth;
            float preferredHeight = toastTextComponent.preferredHeight;
            float maxToastWidth = Config.MAX_TOAST_WIDTH * Screen.width;

            if(preferredWidth > maxToastWidth){
                toastText.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

                preferredWidth = maxToastWidth;
                toastText.GetComponent<RectTransform>().sizeDelta = new Vector2(preferredWidth, toastText.GetComponent<RectTransform>().sizeDelta.y);
                yield return null;
                preferredHeight = toastText.GetComponent<RectTransform>().sizeDelta.y;
            }

            toastRT.sizeDelta = new Vector2(preferredWidth + 2 * Relative.TOAST_HORIZONTAL_PADDING, preferredHeight + 2 * Relative.TOAST_VERTICAL_PADDING);
        
            switch(Input.deviceOrientation){
                case DeviceOrientation.Portrait:
                case DeviceOrientation.FaceUp:
                case DeviceOrientation.FaceDown:
                case DeviceOrientation.Unknown:
                {
                    toastRT.position = new Vector2(Screen.width / 2f, toastRT.sizeDelta.y / 2f + Relative.TOAST_MARGIN);
                } 
                break;

                case DeviceOrientation.LandscapeLeft: 
                {
                    toastRT.eulerAngles = Vector3.back * 90f;
                    toastRT.position = new Vector2(toastRT.sizeDelta.y / 2f + Relative.TOAST_MARGIN, Screen.height / 2f);
                } 
                break;

                case DeviceOrientation.LandscapeRight: 
                {
                    toastRT.eulerAngles = Vector3.back * (-90f);
                    toastRT.position = new Vector2(Screen.width - (toastRT.sizeDelta.y / 2f + Relative.TOAST_MARGIN), Screen.height / 2f);
                } 
                break;

                case DeviceOrientation.PortraitUpsideDown: 
                {
                    toastRT.eulerAngles = Vector3.back * 180f;
                    toastRT.position = new Vector2(Screen.width / 2f, Screen.height - (toastRT.sizeDelta.y / 2f + Relative.TOAST_MARGIN));
                } 
                break;
            }

            Color fontColor = toastTextComponent.color;

            for(int i = 0; i <= 10; i++){
                Color c = Config.TOAST_COLOR;
                c.a *= i / 10f;
                toast.GetComponent<UnityEngine.UI.Image>().color = c;

                fontColor.a = i / 10f;
                toastTextComponent.color = fontColor;
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(t.lengthMillis / 1000f);

            t.dismiss();
        }

        public IEnumerator hideToast(Toast t){

            GameObject toastText = toast.transform.Find("Text").gameObject;
            Text toastTextComponent = toastText.GetComponent<Text>();

            Color fontColor = toastTextComponent.color;

            for(int i = 10; i >= 0; i--){
                Color c = Config.TOAST_COLOR;
                c.a *= i / 10f;
                toast.GetComponent<UnityEngine.UI.Image>().color = c;

                fontColor.a = i / 10f;
                toastTextComponent.color = fontColor;
                yield return new WaitForSeconds(0.05f);
            }
            
            GameObject.Destroy(toast);
        }

        public IEnumerator showDialog(Dialog d){
            yield return null; // for proper dialog rendering
            
            if(!(d.areControlsSet()))
                throw new Exception("Not all elements of dialog has been set.");

            dialog = new GameObject("Icon Dialog");
            dialog.transform.SetParent(Runtime.canvas.transform);
            dialog.transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);

            // creating dim (background) view
            GameObject dialogDim = new GameObject("Dim", typeof(Button), typeof(UnityEngine.UI.Image));
            dialogDim.transform.SetParent(dialog.transform);
            dialogDim.transform.localPosition = Vector2.zero;
            dialogDim.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
            dialogDim.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            dialogDim.GetComponent<Button>().targetGraphic = dialogDim.GetComponent<UnityEngine.UI.Image>();
            dialogDim.GetComponent<Button>().onClick.AddListener(() => {
                History.GoBack();
            });

            History.AddWindow(this);

            // creating panel
            GameObject dialogPanel = new GameObject("Dialog Panel", typeof(UnityEngine.UI.Image));
            dialogPanel.transform.SetParent(dialog.transform);
            dialogPanel.GetComponent<UnityEngine.UI.Image>().color = Config.ORANGE;
            dialogPanel.transform.localPosition = Vector2.zero;

            float panelWidth = Mathf.Min(Screen.width, Screen.height) - new DP(16f); // in pixels
            float iconWidth = new DP(72f);
            float buttonWidth = new DP(90f);
            float textWidthWithMargins = panelWidth - iconWidth - buttonWidth;
            float textWidth = textWidthWithMargins - new DP(8f);

            float panelHeigth = new DP(80f);

            GameObject dialogIcon = new GameObject("Icon", typeof(UnityEngine.UI.Image));
            dialogIcon.transform.SetParent(dialogPanel.transform);
            dialogIcon.GetComponent<RectTransform>().sizeDelta = Vector2.one * new DP(24f);
            dialogIcon.GetComponent<UnityEngine.UI.Image>().sprite = d.IconSprite;

            GameObject dialogText = new GameObject("Text", typeof(Text));
            dialogText.transform.SetParent(dialogPanel.transform);
            dialogText.GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, dialogText.GetComponent<RectTransform>().sizeDelta.y);
            dialogText.GetComponent<Text>().font = Resources.Load<Font>("fonts/Roboto-Regular");
            dialogText.GetComponent<Text>().fontSize = new SP(14f);
            dialogText.GetComponent<Text>().color = Color.black;
            dialogText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            dialogText.GetComponent<Text>().text = d.Text;

            dialogText.GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, dialogText.GetComponent<Text>().preferredHeight);

            GameObject positiveButton = new GameObject("Button", typeof(Button), typeof(UnityEngine.UI.Image));
            positiveButton.transform.SetParent(dialogPanel.transform);
            GameObject positiveButtonText = new GameObject("Text", typeof(Text));
            Text positiveButtonTextComponent = positiveButtonText.GetComponent<Text>();
            positiveButtonText.transform.SetParent(positiveButton.transform);
            positiveButtonTextComponent.font = Resources.Load<Font>("fonts/Roboto-Regular");
            positiveButtonTextComponent.fontSize = new SP(14f);
            positiveButtonTextComponent.alignment = TextAnchor.MiddleCenter;
            positiveButtonTextComponent.color = Color.white;
            positiveButtonTextComponent.text = d.ButtonText.ToUpper();
            positiveButton.GetComponent<RectTransform>().sizeDelta = positiveButtonText.GetComponent<RectTransform>().sizeDelta;
            positiveButton.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
            positiveButton.GetComponent<Button>().targetGraphic = positiveButton.GetComponent<UnityEngine.UI.Image>();
            positiveButton.GetComponent<Button>().onClick.AddListener(() => {
                History.GoBack();
                d.OnButtonClick();
            });

            dialogPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(panelWidth, panelHeigth);
            float panelLeftX = dialogPanel.transform.position.x - panelWidth / 2f;
            float panelRightX = dialogPanel.transform.position.x + panelWidth / 2f;

            float iconPositionX = panelLeftX + iconWidth / 2f;
            dialogIcon.transform.position = new Vector2(iconPositionX, dialogPanel.transform.position.y);
            
            float textPositionX = panelLeftX + iconWidth + textWidthWithMargins / 2f;
            dialogText.transform.position = new Vector2(textPositionX, dialogPanel.transform.position.y);

            float buttonPositionX = panelRightX - buttonWidth / 2f;
            positiveButton.transform.position = new Vector2(buttonPositionX, dialogPanel.transform.position.y);

            float panelXposition = default(float), panelYposition = default(float), panelRotation = default(float);
            float panelMargin = new UI.DP(5f);
            // set dialog position
            switch(Input.deviceOrientation){
                case DeviceOrientation.FaceDown:
                case DeviceOrientation.FaceUp:
                case DeviceOrientation.Portrait:
                case DeviceOrientation.Unknown:{
                    panelRotation = 0f;
                    panelXposition = Screen.width / 2f;
                    panelYposition = d.view.yMin + panelMargin + panelHeigth / 2f;
                } break;
                case DeviceOrientation.LandscapeLeft:{
                    panelRotation = 270f;
                    panelXposition = d.view.xMin + panelMargin + panelHeigth / 2f;;
                    panelYposition = Screen.height / 2f;
                } break;
                case DeviceOrientation.LandscapeRight:{
                    panelRotation = 90f;
                    panelXposition = d.view.xMax - panelMargin - panelHeigth / 2f;;
                    panelYposition = Screen.height / 2f;
                } break;
                case DeviceOrientation.PortraitUpsideDown:{
                    panelRotation = 180f;
                    panelXposition = Screen.width / 2f;
                    panelYposition = d.view.yMax - panelMargin - panelHeigth / 2f;
                } break;
            }

            dialogPanel.transform.eulerAngles = Vector3.forward * panelRotation;
            dialogPanel.transform.position = new Vector2(panelXposition, panelYposition);
        }

        public IEnumerator destroy(Dialog d){
            destroyAllControls();
            yield return null;
        }
    }
}