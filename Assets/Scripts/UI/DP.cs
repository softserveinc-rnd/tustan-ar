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

namespace TustanAR.UI{
    
    ///
    /// Class for work with density-indepent pixels.
    ///
    public class DP{

        /// DPI which is standart for dp-values (Read Only).
        protected static readonly float sdpi = 160f;

        /// Count of density-independent pixels.
        protected float dp_count;

        public DP(float dp_count){
            this.dp_count = dp_count;
        }

        /// Get size of object in screen pixels.
        public virtual float ToPixels(){
            return dp_count * Screen.dpi / sdpi;
        }

        public SP ToSP(){
            return new SP(this.dp_count);
        }

        public static DP operator +(DP dp1, DP dp2){
            return new DP(dp1.dp_count + dp2.dp_count);
        }

        public static DP operator -(DP dp1, DP dp2){
            return new DP(dp1.dp_count - dp2.dp_count);
        }

        public static DP operator *(float multiplier, DP dp){
            return new DP(multiplier * dp.dp_count);
        }

        public static DP operator *(DP dp, float multiplier){
            return new DP(multiplier * dp.dp_count);
        }

        public static DP operator /(DP dp, float divisor) {
            return new DP(dp.dp_count / divisor);
        }

        public static implicit operator float(DP dp) {
            return dp.ToPixels();
        }

        public static implicit operator int(DP dp){
            return (int)dp.ToPixels();
        }
    }
}