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

namespace TustanAR.Positioning{
	
	public class Location{
		public double latitude { get; private set; }
		public double longitude { get; private set; }
		public double altitude { get; private set; }

		public Location(double latitude = 0, double longitude = 0, double altitude = 0){
			this.latitude = latitude;
			this.longitude = longitude;
			this.altitude = altitude;
		}

		public Location(LocationInfo locationInfo){
			latitude = locationInfo.latitude;
			longitude = locationInfo.longitude;
			altitude = locationInfo.altitude;
		}

		public Location(Vector3 worldCorners){
			latitude = worldCorners.y;
			longitude = worldCorners.x;
			altitude = 0;
		}

		/// Get current location in Location object.
		public static Location GetCurrentLocation(){
			return new Location(Input.location.lastData);
		}

		/// 
		/// Calculates distance to other <see cref="Location"/> (in meters) using Haversine formula.
		/// 
		public double distanceTo(Location anotherLocation){
			
            var d1 = this.latitude * (Math.PI / 180.0);
            var num1 = this.longitude * (Math.PI / 180.0);
            var d2 = anotherLocation.latitude * (Math.PI / 180.0);
            var num2 = anotherLocation.longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
		}

		///
		/// Calculates distances between points along horizontal axes (in meters).
		/// Reference frame is set to invoking object.
		/// First component of Vector accords to distance along meridian axe, second to parallel axe
		///
		public Vector2 distanceAlongHorizontalAxes(Location anotherLocation){
			Location rectangularTriangleVertex = new Location(this.latitude, anotherLocation.longitude);

			double distanceAlongMeridianAxe = anotherLocation.distanceTo(rectangularTriangleVertex);
			double distanceAlongParalelAxe = this.distanceTo(rectangularTriangleVertex);

			if(this.latitude > anotherLocation.latitude) // works only for points in Northern Hemisphere!!!
				distanceAlongMeridianAxe *= -1;

			if(this.longitude > anotherLocation.longitude) // works only for points in Eastern Hemispere!!!
				distanceAlongParalelAxe *= -1;

			return new Vector2(
				(float)distanceAlongMeridianAxe,
				(float)distanceAlongParalelAxe
			);
		}

		public static bool operator ==(Location first, Location second){
			return (first.latitude == second.latitude && first.longitude == second.longitude && first.altitude == second.altitude);
		}

		public static bool operator !=(Location first, Location second){
			return !(first==second);
		}
		
		public override bool Equals(object o){
			return this == (Location)o;
		}

		public override int GetHashCode(){
			// https://msdn.microsoft.com/en-us/library/ms182358.aspx
			return ((int)(1000000*latitude))^((int)(1000000*longitude));
		}
	}
}