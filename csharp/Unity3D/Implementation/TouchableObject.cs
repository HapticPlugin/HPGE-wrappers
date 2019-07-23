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

public abstract class TouchableObject : Touchable {
    [Header("Object Haptic Properties")]
    /// <summary>
    /// An assets that specifies the haptic properties
    /// </summary>
    public HapticMaterial material;
    // public bool Spherical = false; // Can't find any difference. Might be because of the mesh-based import
    // if true, find the texture of the mesh we are attached (first material) and use it
    // as an haptic texture
    public bool UseObjectTexture = true; // TODO: sane default?

    private Texture2D GetMeshTexture()
    {
	Texture2D t = null;
	Material[] materials = GetComponent<Renderer>().sharedMaterials;
	foreach (Material m in materials)
	{
	    if (m.mainTexture != null)
	    {
		t = (Texture2D)m.mainTexture;
		break;
	    }
	}
	return t;
    }

    private void SetTexture()
    {
	// Here texture can be null
    Texture2D texture = UseObjectTexture ? GetMeshTexture() : material.textureImage;
	if (texture == null) { return; } // no more
    if (Verbosity > 1) { Debug.Log("Setting Texture"); }

	if (UnityHaptics.SetTexture(ObjectId, texture, material.ReverseTexture)
	    != HapticNativePlugin.SUCCESS)
	{
	    Debug.LogWarning("Could not set Texture");
	}
    }

    public bool UpdateMaterial(HapticMaterial hm) {
	material = hm;
	return UpdateMaterial();
    }

    public bool UpdateMaterial() {
	if (material == null)
	{
	    Debug.LogWarning("Empty material on object: " + name);
	    return false;
	}
	if (Verbosity > 2) { Debug.Log("Updating " + name + " material"); }

	bool success = UnityHaptics.SetMaterial(ObjectId, material) ==
	    HapticNativePlugin.SUCCESS;

	SetTexture();

	return success;
    }

    private void SetObjectMaterial()
    {
	if (UpdateMaterial())
	{
	    // Tell the material to update this object properties
	    // OnValidate
	    material.AddInstance(this);
	} else
	{
	    Debug.LogError("Could not set haptic property for object " + name);
	}
    }
    override protected void SetObjectProperties() {
	    SetObjectMaterial();
	SetTexture();
    }
}
