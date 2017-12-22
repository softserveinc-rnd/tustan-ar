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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TustanAR.Animation{
    public sealed class BridgeAnimator : MonoBehaviour {
    private float speed = 0.2f;
    private float direction = 1f;
    private float minAngle = 0f;
    private float maxAngle = -0.7f;
                
        void Update () 
        {
            if(AnimationHandler.ANIMATION_ENABLED){
                //Debug.Log(hingeHandle.transform.localRotation.z);
                if (transform.localRotation.z <= maxAngle) {
                    direction = 1f;
                }
                if (transform.localRotation.z >= minAngle) {
                    direction = -1f;
                }
                transform.Rotate(new Vector3(0, 0, speed * direction), Space.Self);
            }
        }
    }
}
