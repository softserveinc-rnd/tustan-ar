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
using System.Collections.Generic;

namespace TustanAR.Smoothing{
    public class OpticalSmoothManager: ISmoothManager{
		private GameObject castle;
		private GameObject imageTarget;
		private IList<Quaternion> lastImageTargerRotations = new List<Quaternion>();
		private bool targetFound = false;
		private bool isFrameFirst = false;
		private uint framesDelay;

		public OpticalSmoothManager(uint withFramesDelay){
			this.framesDelay = withFramesDelay;
		}

		public void OnTargetFound(GameObject castle, GameObject imageTarget){
			this.castle = castle;
			this.imageTarget = imageTarget;

			targetFound = true;
		}

		public void OnTargetLost(){
			targetFound = false;
		}

		public void OnTargetUpdate(){
			if(targetFound){
					if(isFrameFirst){ // just founded
						isFrameFirst = false;
						lastImageTargerRotations.Clear();
						lastImageTargerRotations.Add(imageTarget.transform.rotation);
					}

					if(lastImageTargerRotations.Count > framesDelay)
						lastImageTargerRotations.RemoveAt(0);
				
					castle.transform.position = imageTarget.transform.position;
					castle.transform.rotation = lastImageTargerRotations.Average();

					lastImageTargerRotations.Add(imageTarget.transform.rotation);
				}
				else{
					// target is not on view, do nothing
					isFrameFirst = true; // will be used on next targetFoundEvent
					imageTarget = null;
				}	
		}
	}
}