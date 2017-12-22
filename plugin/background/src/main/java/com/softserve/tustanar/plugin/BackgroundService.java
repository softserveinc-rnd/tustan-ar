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

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.location.Location;
import android.location.LocationManager;
import android.os.Binder;
import android.os.Build;
import android.os.Bundle;
import android.os.IBinder;
import android.util.Log;

import java.util.ArrayList;
import java.util.List;

public final class BackgroundService extends Service {

    private final IBinder mBinder = new LocalBinder();

    private LocationManager mLocationManager = null;
    private NotificationManager mNotificationManager = null;

    private Location[] mMarkerLocations = null;
    private List<Boolean> mCanSendNotification = new ArrayList<>();
    private List<Float> mMaxDistance = new ArrayList<>();
    private List<Float> mLastNotifTime = new ArrayList<>();
    private List<String> mMarkerTitles = new ArrayList<>();
    private Intent mNotificationIntent = null;

    private String mNotificationTitle = null;
    private String mNotificationSubtitle = null;

    public class LocalBinder extends Binder {
        BackgroundService getService() {
            return BackgroundService.this;
        }
    }

    private class LocationListener implements android.location.LocationListener {
        Location mLastLocation;

        public LocationListener(String provider) {
            mLastLocation = new Location(provider);
        }

        @Override
        public void onLocationChanged(Location location) {
            mLastLocation.set(location);
            checkDistanceToMarkers(location);
        }

        private void checkDistanceToMarkers(Location location){
            if(location.getAccuracy()<=Constants.ACCEPTABLE_ACCURACY) {

                float current_time = System.currentTimeMillis();

                for (int i=0; i< mMarkerLocations.length; i++){
                    float distance_to_marker = location.distanceTo(mMarkerLocations[i]);
                    if(distance_to_marker >= mMaxDistance.get(i))
                        mMaxDistance.set(i, distance_to_marker);

                    if((current_time - mLastNotifTime.get(i)) >= Constants.COOLDOWN_INTERVAL && mMaxDistance.get(i) >= Constants.COOLDOWN_DISTANCE)
                        mCanSendNotification.set(i, true);

                    if(distance_to_marker <= Constants.NOTIFICATION_DISTANCE && mCanSendNotification.get(i) == true){
                        showNotification(mNotificationTitle + " " + mMarkerTitles.get(i), mNotificationSubtitle);
                        mLastNotifTime.set(i, current_time);
                        mMaxDistance.set(i, 0f);
                        mCanSendNotification.set(i, false);
                    }
                }

            }
            else for (int i=0; i< mMarkerLocations.length; i++)
                mMaxDistance.set(i, Constants.COOLDOWN_DISTANCE);
            
        }

        @Override public void onProviderDisabled(String provider) {}
        @Override public void onProviderEnabled(String provider) {}
        @Override public void onStatusChanged(String provider, int status, Bundle extras) {}
    }

    LocationListener mLocationListener = new LocationListener(LocationManager.GPS_PROVIDER);

    @Override
    public IBinder onBind(Intent intent) {

        String[] markers = intent.getStringArrayExtra("markers");
        String[] markerTitles = intent.getStringArrayExtra("markerTitles");

        mNotificationTitle = intent.getStringExtra("notificationTitle");
        mNotificationSubtitle = intent.getStringExtra("notificationSubtitle");

        mNotificationIntent = intent.getParcelableExtra("notificationIntent");
        mNotificationIntent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_SINGLE_TOP);

        mMarkerLocations = new Location[markers.length];
        for (int i=0; i<markers.length; i++){
            String[] coords = markers[i].split(intent.getStringExtra("coordsSeparator"));

            mMarkerLocations[i] = new Location("");
            mMarkerLocations[i].setLatitude(Double.parseDouble(coords[0]));
            mMarkerLocations[i].setLongitude(Double.parseDouble(coords[1]));

            mMarkerTitles.add(markerTitles[i]);
            mCanSendNotification.add(true);
            mLastNotifTime.add(0f);
            mMaxDistance.add(Constants.COOLDOWN_DISTANCE);
        }

        return mBinder;
    }

    @Override
    public void onCreate() {

        if (mLocationManager == null) {
            mLocationManager = (LocationManager) getApplicationContext().getSystemService(Context.LOCATION_SERVICE);
        }

        mNotificationManager = (NotificationManager) getSystemService(Service.NOTIFICATION_SERVICE);

        try {
            mLocationManager.requestLocationUpdates(
                    LocationManager.GPS_PROVIDER, Constants.LOCATION_INTERVAL, Constants.LOCATION_DISTANCE,
                    mLocationListener);
        } catch (SecurityException ex) {
            Log.e("BackgroundService", "fail to request location update, ignore", ex);
        } catch (IllegalArgumentException ex) {
            Log.e("BackgroundService", "gps provider does not exist " + ex.getMessage());
        }
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (mLocationManager != null) {
            if (Build.VERSION.SDK_INT >= 23) {

                if (checkSelfPermission(android.Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED
                        || checkSelfPermission(android.Manifest.permission.ACCESS_COARSE_LOCATION) == PackageManager.PERMISSION_GRANTED) {

                    mLocationManager.removeUpdates(mLocationListener);
                }
            }
            else
            {
                mLocationManager.removeUpdates(mLocationListener);

            }
        }
    }

    private void showNotification(String title, String subtitle) {
        mNotificationManager.cancelAll();

        PendingIntent pIntent = PendingIntent.getActivity(this, 0, mNotificationIntent, 0);

        Notification n  = new Notification.Builder(this)
                .setContentTitle(title)
                .setContentText(subtitle)
                .setSmallIcon(Constants.ICON_APP)
                .setDefaults(Notification.DEFAULT_ALL)
                .setPriority(Notification.PRIORITY_HIGH)
                .setContentIntent(pIntent)
                .setAutoCancel(true).build();

        mNotificationManager.notify(0, n);
    }

    public void updateMarkerLocation(int markerID, float latitude, float longitude){
        mMarkerLocations[markerID].setLatitude(latitude);
        mMarkerLocations[markerID].setLongitude(longitude);
        mCanSendNotification.set(markerID, true);
        mMaxDistance.set(markerID, Constants.COOLDOWN_DISTANCE);
        mLastNotifTime.set(markerID, 0f);
    }
}