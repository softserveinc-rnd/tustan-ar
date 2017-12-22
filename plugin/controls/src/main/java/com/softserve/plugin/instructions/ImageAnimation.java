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

package com.softserve.plugin.instructions;

import android.os.Handler;
import android.os.Message;

import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

class ImageAnimation<T>{

    private List<T> imageRes;
    private int slideshowTime;
    private OnAnimationListener listener;
    private Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            listener.onAnimate(msg.obj);
        }
    };
    private Timer timer = new Timer();
    private int current = -1;

    ImageAnimation(List<T> images, int slideshowTime, OnAnimationListener listener){
        this.imageRes = images;
        this.slideshowTime = slideshowTime;
        this.listener = listener;
    }

    void startAnimation(){
        timer.scheduleAtFixedRate(new TimerTask() {
            public void run() {
                mHandler.obtainMessage(1, next()).sendToTarget();
            }
        }, 0, slideshowTime);
    }

    private T next(){
        current = (current + 1) % imageRes.size();
        return imageRes.get(current);
    }

    void dismiss(){
        timer.cancel();
        timer = null;
        imageRes = null;
    }

    interface OnAnimationListener {
        void onAnimate(Object next);
    }
}
