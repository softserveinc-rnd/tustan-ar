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

namespace TustanAR{
    using Positioning;
    using Locale;
    using UI;
    
    ///
    /// Class with all changeable configurations for app lifecycle.
    ///
    public sealed class Config: MonoBehaviour{
        
        /* DEBUG */

        /// Show latitude, longitude, altitude, horizontal and vertical accuracies.
        public static readonly bool DEBUG_GPS = false;
        
        /// Show current screen frame rate (frames per second).
        public static readonly bool DEBUG_FPS = false;

        /// Show current app version.
        public static readonly bool DEBUG_VERSION = false;

        /// Show current app version.
        public static readonly bool DEBUG_TARGET = false;
        public static string CURRENT_TARGET = "";

        /// Show current compass info.
        public static readonly bool DEBUG_COMPASS = false;

        public static readonly bool DEBUG_EXPOSURE = false;


        /* GENERAL/UI */

        /// Direct link to app in Apple App Store.
        public static string urlToAppStore = "https://itunes.apple.com/ua/app/tustanar/id1283043687?mt=8";

        /// Direct link to app in Android Google Play.
        public static string urlToPlayStore = "https://play.google.com/store/apps/details?id=com.softserve.tustanar";

        /// Desired height of UI buttons on screen (in density-independent pixels).
        public static readonly float BUTTON_HEIGHT = 24f;

        /// Desired height of markers on map (in density-independent pixels).
        public static readonly float MAP_MARKER_HEIGHT = 32f;

        /// Desired length of out-of-map arrow (in density-independent pixels).
        public static readonly float OUT_OF_MAP_ARROW_LENGH = 64f;

        /// Height of the lower black panel in Camera Window (in density-independent pixels).
        public static readonly float LOWER_PANEL_HEIGHT = 80f;

        /// Height of the upper black panel (in density-independent pixels).
        public static readonly float UPPER_PANEL_HEIGTH = 48f;

        /// Height of central button in bottom of Camera Window (in density-independent pixels).
        public static readonly float MAKE_PHOTO_BUTTON_HEIGHT = 64f;

        /// Margin of button (in density-independent pixels).
        public static readonly float BUTTON_MARGIN = 12f;

        /// Width of map icon on Map Window (in density-independent pixels).
        public static readonly float MARKER_ICON_WIDTH = 30f;

        /// Height of map icon on Map Window (in density-independent pixels).
        public static readonly float MARKER_ICON_HEIGHT = 38f;

        /// Width of out-of-map icon on Map Window (in density-independent pixels).
        public static readonly float OUT_MARKER_ICON_WIDTH = 32f;

        /// Height of out-of-map icon on Map Window (in density-independent pixels).
        public static readonly float OUT_MARKER_ICON_HEIGHT = 49f;

        /// Horizontal margin around text (in density-independent pixels).
        public static readonly float TEXT_HORIZONTAL_MARGIN = 8f;

        /// Toast margin to appropriate screen side (in density-independent pixels).
        public static readonly float TOAST_MARGIN = 64f;

        /// Horizontal padding between toast text and border (in density-independent pixels).
        public static readonly float TOAST_HORIZONTAL_PADDING = 16f;

        /// Vertical padding between toast text and border (in density-independent pixels).
        public static readonly float TOAST_VERTICAL_PADDING = 8f;

        /// Max toast width (relative to screen width).
        public static readonly float MAX_TOAST_WIDTH = 0.75f;

        /// Color of view when dialog is shown (also color of toast).
        public static readonly Color TOAST_COLOR = new Color(r: 0.2f, g: 0.2f, b: 0.2f, a: 0.8f);

        /// Controls color in app.
        public static readonly Color ORANGE = new Color(r: 248/256f, g: 175/256f, b: 0/256f, a: 1f);

        /// Unified line spacing for all app, measured in relative units.
        public static readonly float FONT_LINE_SPACING = 1.25f;

        /// Scale-independent pixels (sp).
        public static readonly float SMALL_FONT_SIZE = 14f; 
        
        /// Scale-independent pixels (sp).
        public static readonly float MEDIUM_FONT_SIZE = 18f;
        
        /// Scale-independent pixels (sp).
        public static readonly float LARGE_FONT_SIZE = 24f;

        public static readonly string LICENSE_INFO = "Tustan AR Copyright (C) 2017  SoftServe Inc\nThis program comes with ABSOLUTELY NO WARRANTY;\nThis is free software, and you are welcome to redistribute it under certain conditions;  https://github.com/softserveinc-rnd/tustan-ar/blob/master/COPYING";

        /* GEO CONFIGS */

        /// Info about all markers
        public Marker[] Markers;
        void Awake(){
            Markers = new Marker[] {
                new Marker(){
                    id = 1,
                    location = new Location(latitude:49.19194f, longitude:23.41002f),
                    title = "Ворота",
                    animationEnabled = true,
                    sprite = SpriteLoader.LoadSprite("marker_gates")
                },
                new Marker(){
                    id = 2,
                    location = new Location(latitude:49.19191707f, longitude:23.41001708f),
                    title = "Ворота",
                    animationEnabled = true,
                    sprite = SpriteLoader.LoadSprite("marker_transparent")
                },
                new Marker(){
                    id = 3,
                    location = new Location(latitude:49.19166f, longitude:23.40986f),
                    title = "Дитинець",
                    sprite = SpriteLoader.LoadSprite("marker_backyard")
                },
                new Marker(){
                    id = 5,
                    location = new Location(latitude:49.190278f, longitude:23.4091467f),
                    title = "Стави",
                    sprite = SpriteLoader.LoadSprite("marker_lake")
                },
                new Marker(){
                    id = 6,
                    location = new Location(latitude:49.19101f, longitude:23.41021f),
                    title = "Велике Крило",
                    sprite = SpriteLoader.LoadSprite("marker_big_wing")
                }, 
                new Marker(){
                    id = 7, 
                    location = new Location(latitude:49.19204f, longitude:23.40958f), 
                    title = "Північна стіна",
                    sprite = SpriteLoader.LoadSprite("marker_north_wall")
                }
            };
        }
        
        /// Border coordinates of current map.
        public static readonly ImageBorders MapBorders = new ImageBorders(
            _leftUpper: new Location(latitude:49.192448f, longitude:23.407404f), 
            _rightBottom: new Location(latitude:49.189862f, longitude:23.411373f)
        );


        /* NOTIFICATION CONFIGS */

        /// Minimum interval for notification from one marker (in milliseconds).
        public static readonly int COOLDOWN_INTERVAL = 300000;

        /// Distance in metres, to get ability to receive notification from certain marker user must leave marker for this distance.
        public static readonly float COOLDOWN_DISTANCE = 50f;

        /// Distance in metres to marker, from which user can receive on-marker notifications and see marker contour.
        public static readonly float NOTIFICATION_DISTANCE = 20f;

        /// Distance in metres, location accuracy to make location data significant.
        public static readonly float ACCEPTABLE_ACCURACY = 20f;
    }
}