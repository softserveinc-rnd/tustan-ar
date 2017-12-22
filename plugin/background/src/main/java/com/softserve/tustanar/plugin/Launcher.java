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

package com.softserve.tustanar.plugin;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.IBinder;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

public class Launcher {

    BackgroundService mService;
    boolean mBound = false;

    private static com.softserve.tustanar.plugin.Launcher instance;

    private Launcher() {
        com.softserve.tustanar.plugin.Launcher.instance = this;
    }

    public static com.softserve.tustanar.plugin.Launcher instance() {
        if(instance == null) {
            instance = new com.softserve.tustanar.plugin.Launcher();
        }
        return instance;
    }

    /** Defines callbacks for service binding, passed to bindService() */
    private ServiceConnection mConnection = new ServiceConnection() {

        @Override
        public void onServiceConnected(ComponentName className,
                                       IBinder service) {
            // We've bound to LocalService, cast the IBinder and get LocalService instance
            BackgroundService.LocalBinder binder = (BackgroundService.LocalBinder) service;
            mService = binder.getService();
            mBound = true;
        }

        @Override
        public void onServiceDisconnected(ComponentName arg0) {
            mBound = false;
        }
    };

    public void startTustanService(String _markers, String _markerTitlesString, String localizationString, String separator, String coordsSeparator, int cooldownInterval, float cooldownDistance, float notificationDistance, float acceptableAccuracy){
        String[] markers = _markers.split(separator);
        String[] markerTitlesString = _markerTitlesString.split(separator);
        String[] localizationStrings = localizationString.split(separator);

        Intent startServiceIntent = new Intent(UnityPlayer.currentActivity, BackgroundService.class);

        startServiceIntent.putExtra("markers", markers);
        startServiceIntent.putExtra("markerTitles", markerTitlesString);
        startServiceIntent.putExtra("notificationTitle", localizationStrings[0]);
        startServiceIntent.putExtra("notificationSubtitle", localizationStrings[1]);
        startServiceIntent.putExtra("coordsSeparator", coordsSeparator);

        Intent notificationIntent = new Intent(UnityPlayer.currentActivity, UnityPlayer.currentActivity.getClass());
        startServiceIntent.putExtra("notificationIntent", notificationIntent);

        Constants.COOLDOWN_INTERVAL = cooldownInterval;
        Constants.COOLDOWN_DISTANCE = cooldownDistance;
        Constants.NOTIFICATION_DISTANCE = notificationDistance;
        Constants.ACCEPTABLE_ACCURACY = acceptableAccuracy;

        UnityPlayer.currentActivity.bindService(startServiceIntent, mConnection, Context.BIND_AUTO_CREATE);
    }
}
