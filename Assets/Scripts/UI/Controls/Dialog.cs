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

namespace TustanAR.UI.Controls{
    public class Dialog: IDialog{

        public interface IDialogBuilder{
            IDialogBuilder SetText(string text);
            IDialogBuilder SetIcon(Sprite icon);
            IDialogBuilder SetButton(string text, UnityEngine.Events.UnityAction onClickAction);
            IDialogBuilder SetView(Rect view);
            IDialog build();
        }

        public sealed class Builder : IDialogBuilder{
            private Dialog dialog = new Dialog();

            public IDialogBuilder SetText(string text){
                dialog.isTextSet = true;
                dialog.Text = text;

                return this;
            }

            public IDialogBuilder SetButton(string text, UnityEngine.Events.UnityAction onClickAction){
                dialog.isButtonSet = true;
                dialog.ButtonText = text;
                dialog.OnButtonClick = onClickAction;

                return this;
            }

            public IDialogBuilder SetView(Rect view){
                dialog.isViewSet = true;
                dialog.view = view;
                return this;
            }

            public IDialogBuilder SetIcon(Sprite icon){
                dialog.isIconSet = true;
                dialog.IconSprite = icon;

                return this;
            }

            public IDialog build(){
                return dialog;
            }
        }

        protected internal Dialog(){
            isActive = false;
        }

        public bool isActive { get; internal set; }
        private bool isTextSet = false;
        private bool isButtonSet = false;
        private bool isIconSet = false;
        private bool isViewSet = false;
        public string Text;
        public string ButtonText;
        public Sprite IconSprite;
        public UnityEngine.Events.UnityAction OnButtonClick;
        public Rect view;

        public void show(){
            Runtime.canvas.GetComponent<ControlsAnimator>().show(this);
            isActive = true;
        }

        public void dismiss(){
            Runtime.canvas.GetComponent<ControlsAnimator>().destroy(this);
            isActive = true;
        }

        public bool areControlsSet(){
            return isTextSet && isButtonSet && isIconSet && isViewSet;
        }
    }
}