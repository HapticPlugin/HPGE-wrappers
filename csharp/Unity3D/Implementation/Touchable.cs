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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Select
using UnityEditor;

public abstract class Touchable : MonoBehaviour {
    [Header("Haptic Library Information")]
    /// <summary>
    /// Id of this object in HGPE
    /// </summary>
    [ReadOnly]
    public int ObjectId = -1;
    [Header("Set Position Configuration")]
    /// <summary>
    /// Instead of updating the position immediately, interpolate it
    /// in $InterpolationPeriod framse
    /// </summary>
    public bool positionInterpolation = false;
    public int InterpolationPeriod = 20; // TODO: which is a sane default?
					 // TODO: Should those be protected?
    private Vector3 lastPos = Vector3.zero;
    private Vector3 lastScale = Vector3.zero;
    private Quaternion lastRot = new Quaternion();
    [Header("Debug Settings")]
    /// <summary>
    /// Issue log information
    /// </summary>
    [Range(0, 3)]
    public int Verbosity = 0;

    /// <summary>
    ///  Disable object haptic rendering
    /// </summary>
    public void Disable()
    {
	HapticNativePlugin.disable_object(ObjectId);
	if (Verbosity > 2) { Debug.Log("Disabling object: " + name); }
    }
    /// <summary>
    ///  Enable object haptic rendering
    /// </summary>
    public void Enable()
    {
	HapticNativePlugin.enable_object(ObjectId);
	if (Verbosity > 2) { Debug.Log("Enabling object: " + name); }
    }
    public void OnDisable()
    {
	Disable();
    }
    public void OnEnable()
    {
	Enable();
    }
    private void SetTag() {
	HapticNativePlugin.SetObjectTag(ObjectId, name);
    }
    // This should be called by a parent like a MultiMesh or a SingleMesh
    private void AddToWorld()
    {
	HapticNativePlugin.add_object_to_world(ObjectId);
    }

    private bool UpdatePosition()
    {
	bool success = false;
	if (lastPos != transform.position)
	{
	    if (Verbosity > 2) { Debug.Log("Updating position"); }

	    success =
		UnityHaptics.SetObjectPosition(ObjectId, transform.position)
		    == HapticNativePlugin.SUCCESS;
	    lastPos = transform.position;
	}
	return success;
    }
    private bool UpdateRotation()
    {
	bool success = false;

	if (lastRot != transform.rotation)
	{
	    if (Verbosity > 2) { Debug.Log("Updating rotation"); }

	    success =
		UnityHaptics.SetObjectRotationEuler(ObjectId, transform.rotation)
		  == HapticNativePlugin.SUCCESS;
	    lastRot = transform.rotation;
	}
	return success;
    }

    private int UpdateScale()
    {
	int status = 0;
	if (lastScale != transform.lossyScale)
	{
	    status =
		UnityHaptics.SetObjectScale(ObjectId, transform.lossyScale);
	    lastScale = transform.lossyScale;
	}

	return status;
    }

    private void OnValidate()
    {
	if (!Application.isPlaying || !HapticNativePlugin.IsRunning())
	{
	    return;
	}
	if (Verbosity > 2) { Debug.Log("Validating"); }

	SetObjectProperties();

	// FIXME: re-implement
	//if (prevTexture != texture)
	//{
	//    SetObjectTexture();
	//    prevTexture = texture;
	//    if (verboseUpdates) Debug.Log("Texture change updated!");
	//}
    }

    // Use this for initialization
    void Start () {
	CreateObject();
	AddToWorld();
	if (positionInterpolation)
	{
	    HapticNativePlugin.enable_position_interpolation(ObjectId);
	    HapticNativePlugin.set_interpolation_period(ObjectId, InterpolationPeriod, 1.0);
	}
	SetObjectProperties();
    }
    protected abstract void SetObjectProperties();
    /// <summary>
    /// The implementation depends on the "object" type
    /// </summary>
    protected abstract void CreateObject();
    private void FixedUpdate()
    {
	UpdatePosition();
	UpdateRotation();
	// UpdateScale(); // FIXME: Still not working
	// Debug.Log(Time.deltaTime);
    }
}
