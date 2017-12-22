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
    using Controls;
    
    ///
    /// Struct that sets sizes and margins for all elements on page.
    ///
    public struct Relative {

        /* FROM CONFIGS */

        /// Horizontal text margin (in absolute value, screen width - 1).
        public static readonly DP TEXT_HORIZONTAL_MARGIN = new DP(Config.TEXT_HORIZONTAL_MARGIN);

        /// Desired height of UI buttons on screen (in density-independent pixels) (Read Only).
        public static readonly DP BUTTON_HEIGHT = new DP(Config.BUTTON_HEIGHT);

        /// Desired height of markers on map (in density-independent pixels) (Read Only).
        public static readonly DP MAP_MARKER_HEIGHT = new DP(Config.MAP_MARKER_HEIGHT);

        /// Desired length of out-of-map arrow (in density-independent pixels) (Read Only).
        public static readonly DP OUT_OF_MAP_ARROW_LENGH = new DP(Config.OUT_OF_MAP_ARROW_LENGH);

        /// Height of the lower black panel in Camera Window (in density-independent pixels) (Read Only).
        public static readonly DP LOWER_PANEL_HEIGHT = new DP(Config.LOWER_PANEL_HEIGHT);

        /// Height of the upper black panel (in density-independent pixels) (Read Only).
        public static readonly DP UPPER_PANEL_HEIGTH = new DP(Config.UPPER_PANEL_HEIGTH);

        /// Height of central button in bottom of Camera Window (in density-independent pixels) (Read Only).
        public static readonly DP MAKE_PHOTO_BUTTON_HEIGHT = new DP(Config.MAKE_PHOTO_BUTTON_HEIGHT);

        /// Margin of button (in density-independent pixels) (Read Only).
        public static readonly DP BUTTON_MARGIN = new DP(Config.BUTTON_MARGIN);

        /// Width of map icon on Map Window (in density-independent pixels) (Read Only).
        public static readonly DP MARKER_ICON_WIDTH = new DP(Config.MARKER_ICON_WIDTH);

        /// Height of map icon on Map Window (in density-independent pixels) (Read Only).
        public static readonly DP MARKER_ICON_HEIGHT = new DP(Config.MARKER_ICON_HEIGHT);

        /// Width of out-of-map icon on Map Window (in density-independent pixels) (Read Only).
        public static readonly DP OUT_MARKER_ICON_WIDTH = new DP(Config.OUT_MARKER_ICON_WIDTH);

        /// Height of out-of-map icon on Map Window (in density-independent pixels) (Read Only).
        public static readonly DP OUT_MARKER_ICON_HEIGHT = new DP(Config.OUT_MARKER_ICON_HEIGHT); 

        /// Toast margin to appropriate screen side (in density-independent pixels) (Read Only).
        public static readonly DP TOAST_MARGIN = new DP(Config.TOAST_MARGIN);
        
        /// Horizontal padding between toast text and border (in density-independent pixels) (Read Only).
        public static readonly DP TOAST_HORIZONTAL_PADDING = new DP(Config.TOAST_HORIZONTAL_PADDING);

        /// Vertical padding between toast text and border (in density-independent pixels). (Read Only)
        public static readonly DP TOAST_VERTICAL_PADDING = new DP(Config.TOAST_VERTICAL_PADDING);

        /// scale-independent pixels (sp) (Read Only).
        public static readonly SP SMALL_FONT_SIZE = new SP(Config.SMALL_FONT_SIZE); 
        
        /// scale-independent pixels (sp) (Read Only).
        public static readonly SP MEDIUM_FONT_SIZE = new SP(Config.MEDIUM_FONT_SIZE);
        
        /// scale-independent pixels (sp) (Read Only).
        public static readonly SP LARGE_FONT_SIZE = new SP(Config.LARGE_FONT_SIZE);

        /* NOT FORM CONFIGS */
        /// Camera Window rect without upper and lower panels (in pixels) (Read Only). 
        public static readonly Rect CAMERA_WINDOW_RECT = new Rect(x: 0, y: LOWER_PANEL_HEIGHT, width: Screen.width, height: Screen.height - LOWER_PANEL_HEIGHT - UPPER_PANEL_HEIGTH); 
        
        /// Map Window rect without upper panel (in pixels) (Read Only). 
        public static readonly Rect MAP_WINDOW_RECT = new Rect(x: 0, y: 0, width: Screen.width, height: Screen.height - UPPER_PANEL_HEIGTH);
    }
}