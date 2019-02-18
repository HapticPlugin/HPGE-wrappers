// This file is part of HPGE, an Haptic Plugin for Game Engines
// -----------------------------------------

// Software License Agreement (BSD License)
// Copyright (c) 2017-2019,
// Istituto Italiano di Tecnologia (IIT), All rights reserved.

// (iit.it, contact: gabriel.baud-bovy <at> iit.it)

// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:

// 1. Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.

// 2. Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.

// 3. Neither the name of HPGE, Istituto Italiano di Tecnologia nor the
// names of its contributors may be used to endorse or promote products
// derived from this software without specific prior written permission.

// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Haptic Material")]
public class HapticMaterial : ScriptableObject {
    private List<TouchableObject> Instances = new List<TouchableObject>();

    [Header("Surface Haptic Properties")]
    // Haptic Properties
    public bool HasSurface;
    [Range(0, 1)]
    public double Stiffness;
    [Range(0, 1)]
    public double StaticFriction;
    [Range(0, 1)]
    public double DynamicFriction;
    [Range(0, 1)]
    public double MagneticForce;
    [Range(0, 100)]
    public double MagneticDistance;
    [Header("Textures")]
    [Range(0, 100)]
    public double TextureLevel;
    public Texture2D textureImage;
    /// <summary>
    /// Invert black and white colors in the texture
    /// </summary>
    public bool ReverseTexture = false;
    [Header("Volume Haptic Properties")]
    [Range(0, 1)]
    public double Viscosity;
    [Range(0, 1)]
    public double SticksplipStiffness;
    [Range(0, 1)]
    public double SticksplipForce;
    [Range(0, 10)]
    public double VibrationFreq;
    [Range(0, 5)]
    public double VibrationAmplitude;
    public void AddInstance(TouchableObject o) {
	if (o.Verbosity > 1)
	{
	    Debug.Log("Added " + o.name +
		" to hapticMaterial " + name
		+ " instances (update will be synced)");
	}
	Instances.Add(o);
    }
    public void OnEnable()
    {
	Instances.Clear();
    }
    public void OnValidate() {
	if (!Application.isPlaying || !HapticNativePlugin.IsRunning()) { return; }

	for (int i = 0; i < Instances.Count; ++i) {
	    Instances[i].UpdateMaterial();
	}
    }

    public HapticMaterial()
    {
	Stiffness = 0.4;
	HasSurface = true;
	StaticFriction = 0.1;
	DynamicFriction = 0.1;
	TextureLevel = 0.3;

	MagneticDistance = 0.0;
	MagneticForce = 0.0;

	Viscosity = 0.0;
	SticksplipStiffness = 0.0;
	SticksplipForce = 0.0;

	VibrationFreq = 0.0;
	VibrationAmplitude = 0.0;
    }
}
