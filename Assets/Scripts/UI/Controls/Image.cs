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

namespace TustanAR.UI.Controls {

    public interface IImage: IDisposable{
        GameObject gameObject { get; }
        RotationPoint rotationPoint { get; }
        void UpdatePosition(Vector3 position);
        void SetColor(Color color);
    }

    public interface IImageBuilder{
        IImageBuilder SetName(string name);
        IImageBuilder AddOnClickListener(UnityEngine.Events.UnityAction call);
        IImageBuilder SetImage(Sprite imageSprite);
        IImageBuilder SetParent(Transform parent);
        IImageBuilder SetSize(Vector2 size, float additionalClickableArea = 1f);
        IImageBuilder SetPosition(Vector2 position);
        IImageBuilder SetRotationPoint(RotationPoint rotationPoint);
        IImageBuilder SetActive(bool setActive);
        IImage build();
    }

    ///
    /// Class that adds additional features to UnityEngine.UI.Image.
    ///
    public class Image: IImage {

        public class Builder : IImageBuilder {
            private Image image = new Image();
            public IImageBuilder SetName(string name){
                this.image.clickable.name = name;
                this.image.image.name = name + " Image";
                return this;
            }
            public IImageBuilder AddOnClickListener(UnityEngine.Events.UnityAction call){
                this.image.clickable.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(call);
                return this;
            }
            public IImageBuilder SetImage(Sprite imageSprite){
                this.image.image.GetComponent<UnityEngine.UI.Image>().sprite = imageSprite;
                return this;
            }
            public IImageBuilder SetParent(Transform parent){
                this.image.clickable.transform.SetParent(parent);
                return this;
            }
            public IImageBuilder SetSize(Vector2 size, float additionalClickableArea = 1f){
                this.image.clickable.GetComponent<RectTransform>().sizeDelta = size * additionalClickableArea;
                this.image.image.GetComponent<RectTransform>().sizeDelta = size;
                return this;
            }
            public IImageBuilder SetPosition(Vector2 position){
                this.image.clickable.GetComponent<RectTransform>().position = position;
                return this;
            }
            public IImageBuilder SetRotationPoint(RotationPoint rotationPoint){
                image.rotationPoint = rotationPoint;

                Vector3 rotationPointVector = image.clickable.transform.position; // rotate around center by default
                if(rotationPoint == RotationPoint.Center){
                    rotationPointVector = image.clickable.transform.position;
                }
                else if(rotationPoint == RotationPoint.Bottom){
                    rotationPointVector = image.clickable.transform.position - new Vector3(0, image.clickable.GetComponent<RectTransform>().sizeDelta.y / 2f);
                }

                image.rotationCase = new RotationCase(){
                    gameObject = image.clickable,
                    rotationPoint = rotationPointVector
                };
                
                RotationHandler.RotationCases.Add(image.rotationCase);
                return this;
            }
            public IImageBuilder SetActive(bool active){
                image.clickable.SetActive(active);
                return this;
            }
            public IImage build(){
                if(image.rotationCase != null)
                    RotationHandler.SetInitialRotation(image.rotationCase);
                return this.image;
            }
        }

        public GameObject gameObject { get { return clickable; } }
        
        protected GameObject clickable;
        protected GameObject image;
        protected RotationCase rotationCase;
        public RotationPoint rotationPoint { get; internal set; }

        protected internal Image(){
            this.clickable = new GameObject("Untitled Image", typeof(UnityEngine.UI.Button), typeof(UnityEngine.UI.Image));
            this.clickable.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
            
            this.image = new GameObject("Untitled Image Image", typeof(UnityEngine.UI.Image));
            this.image.transform.SetParent(this.clickable.transform);
        }

        public void UpdatePosition(Vector3 position){
            // update position
            clickable.transform.position = position;

            // update rotationPoint
            if(rotationCase != null){
                Vector3 rotationPointVector = position; // rotate around center by default
                if(rotationPoint == RotationPoint.Center){
                    rotationPointVector = position;
                }
                else if(rotationPoint == RotationPoint.Bottom){
                    rotationPointVector = position - new Vector3(0, image.GetComponent<RectTransform>().sizeDelta.y / 2f);

                    RectTransform rt = image.GetComponent<RectTransform>();
                    if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
                        rt.position += new Vector3(rt.sizeDelta.x * 0.5f,rt.sizeDelta.y * (-0.5f));
                    else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                        rt.position += new Vector3(rt.sizeDelta.x * (-0.5f),rt.sizeDelta.y * (-0.5f));
                    else if(Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
                        rt.position += new Vector3(0f,rt.sizeDelta.y * (-1f));
                }

                rotationCase.rotationPoint = rotationPointVector;
            }
        }

        public void SetColor(Color color){
            image.GetComponent<UnityEngine.UI.Image>().color = color;
        }

        public void Dispose(){
            RotationHandler.RotationCases.Remove(rotationCase);
        }
    }
}