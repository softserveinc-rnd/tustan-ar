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
using System.Linq;
using System.Collections.Generic;

namespace TustanAR.Smoothing{

	/// Add Extension Methods to Quaternion lists.
    public static class Extensions{

        /// Get average rotation.
		public static Quaternion Average(this IList<Quaternion> elements){

			// transform all quaternions to matrices 4x4
			IList<Matrix4x4> rotations = elements.Select(e => Matrix4x4.TRS(Vector3.zero, e, Vector3.one)).ToList();

			Matrix4x4 rotations_sum = Matrix4x4.zero;
			foreach(Matrix4x4 rotation in rotations)
				rotations_sum = rotations_sum.Add(rotation);

			Matrix4x4 average = rotations_sum.Divide(rotations.Count);

			return average.rotation;
		}

		/// Adds to matrices 4x4.
		private static Matrix4x4 Add(this Matrix4x4 first, Matrix4x4 second){
			Matrix4x4 result = Matrix4x4.zero;

			result.m00 = first.m00 + second.m00;
			result.m01 = first.m01 + second.m01;
			result.m02 = first.m02 + second.m02;
			result.m03 = first.m03 + second.m03;
			
			result.m10 = first.m10 + second.m10;
			result.m11 = first.m11 + second.m11;
			result.m12 = first.m12 + second.m12;
			result.m13 = first.m13 + second.m13;
			
			result.m20 = first.m20 + second.m20;
			result.m21 = first.m21 + second.m21;
			result.m22 = first.m22 + second.m22;
			result.m23 = first.m23 + second.m23;

			result.m30 = first.m30 + second.m30;
			result.m31 = first.m31 + second.m31;
			result.m32 = first.m32 + second.m32;
			result.m33 = first.m33 + second.m33;			

			return result;
		}

		/// Divides every element of matrix 4x4 by floating point number.
		private static Matrix4x4 Divide(this Matrix4x4 matrix, float num){
			Matrix4x4 result = Matrix4x4.zero;

			result.m00 = matrix.m00 / num;
			result.m01 = matrix.m01 / num;
			result.m02 = matrix.m02 / num;
			result.m03 = matrix.m03 / num;
			
			result.m10 = matrix.m10 / num;
			result.m11 = matrix.m11 / num;
			result.m12 = matrix.m12 / num;
			result.m13 = matrix.m13 / num;
			
			result.m20 = matrix.m20 / num;
			result.m21 = matrix.m21 / num;
			result.m22 = matrix.m22 / num;
			result.m23 = matrix.m23 / num;

			result.m30 = matrix.m30 / num;
			result.m31 = matrix.m31 / num;
			result.m32 = matrix.m32 / num;
			result.m33 = matrix.m33 / num;			

			return result;
		}
    }
}