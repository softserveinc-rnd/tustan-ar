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
using System.Collections;
using System.Collections.Generic;

namespace TustanAR.UI{

	public enum RotationPoint{
        /// Rotate around center of GameObject.
        Center,

        /// Rotate around lower side of GameObject.
        Bottom
    }

	public sealed class RotationHandler : MonoBehaviour {
		public static List<UI.RotationCase> RotationCases;

		private DeviceOrientation oldOrientation;

		private static float rotateNull = 0f;
		private static float rotateClockwise = 90f;
		private static float rotateCounterClockwise = -90f;
		private static float rotateUpsideDown = 180f;

		void Awake(){
			RotationCases = new List<UI.RotationCase>();
		}

		void Start() {
			// initial orientation
			oldOrientation = Input.deviceOrientation;
		}

		void Update() {
			if(Input.deviceOrientation != oldOrientation) { // orintation changed
				
				float rotationAngle = GetRotationAngle(oldOrientation, Input.deviceOrientation);

				oldOrientation = Input.deviceOrientation;

				StartCoroutine(AnimateRotation(rotationAngle));

				this.GetComponent<Controls.ControlsAnimator>().destroyAllControls();
			}
		}

		private static float GetRotationAngle(DeviceOrientation oldOrientation, DeviceOrientation newOrientation){
			float rotationAngle = 0f;

			if (oldOrientation == DeviceOrientation.FaceDown
			|| oldOrientation == DeviceOrientation.FaceUp
			|| oldOrientation == DeviceOrientation.Portrait
			|| oldOrientation == DeviceOrientation.Unknown) {
				switch (newOrientation) {
					case DeviceOrientation.FaceDown:
					case DeviceOrientation.FaceUp:
					case DeviceOrientation.Portrait:
					case DeviceOrientation.Unknown: rotationAngle = rotateNull; break;
					case DeviceOrientation.LandscapeLeft: rotationAngle = rotateClockwise; break;
					case DeviceOrientation.LandscapeRight: rotationAngle = rotateCounterClockwise; break;
					case DeviceOrientation.PortraitUpsideDown: rotationAngle = rotateUpsideDown; break;
				}
			} 
			else if (oldOrientation == DeviceOrientation.LandscapeLeft) {
				switch (newOrientation) {
					case DeviceOrientation.FaceDown:
					case DeviceOrientation.FaceUp:
					case DeviceOrientation.Portrait:
					case DeviceOrientation.Unknown: rotationAngle = rotateCounterClockwise; break;
					case DeviceOrientation.LandscapeLeft: rotationAngle = rotateNull; break;
					case DeviceOrientation.LandscapeRight: rotationAngle = rotateUpsideDown; break;
					case DeviceOrientation.PortraitUpsideDown: rotationAngle = rotateClockwise; break;
				}
			}
			else if(oldOrientation == DeviceOrientation.LandscapeRight) {
				switch (newOrientation) {
					case DeviceOrientation.FaceDown:
					case DeviceOrientation.FaceUp:
					case DeviceOrientation.Portrait:
					case DeviceOrientation.Unknown: rotationAngle = rotateClockwise; break;
					case DeviceOrientation.LandscapeLeft: rotationAngle = rotateUpsideDown; break;
					case DeviceOrientation.LandscapeRight: rotationAngle = rotateNull; break;
					case DeviceOrientation.PortraitUpsideDown: rotationAngle = rotateCounterClockwise; break;
				}
			}
			else if(oldOrientation == DeviceOrientation.PortraitUpsideDown) {
				switch (newOrientation) {
					case DeviceOrientation.FaceDown:
					case DeviceOrientation.FaceUp:
					case DeviceOrientation.Portrait:
					case DeviceOrientation.Unknown: rotationAngle = rotateUpsideDown; break;
					case DeviceOrientation.LandscapeLeft: rotationAngle = rotateCounterClockwise; break;
					case DeviceOrientation.LandscapeRight: rotationAngle = rotateClockwise; break;
					case DeviceOrientation.PortraitUpsideDown: rotationAngle = rotateNull; break;
				}
			}

			return rotationAngle;
		}

		private IEnumerator AnimateRotation(float rotationAngle) {
			int framesCount = 10;
			for (int i = 0; i < framesCount; i++) {
				foreach(var rotationCase in RotationCases)
					rotationCase.gameObject.transform.RotateAround(rotationCase.rotationPoint, Vector3.back, rotationAngle / framesCount);

				yield return new WaitForEndOfFrame();
			}
		}

		public static void SetInitialRotation(RotationCase rotationCase){
			float initialRotation = GetRotationAngle(DeviceOrientation.Portrait, Input.deviceOrientation);
			rotationCase.gameObject.transform.RotateAround(rotationCase.rotationPoint, Vector3.back, initialRotation);
		}
	}
}