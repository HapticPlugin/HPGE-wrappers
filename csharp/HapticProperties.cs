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

// Class defining all the Haptic Properties an object can have
// It's useful in order to detect for changes in the unity UI
[System.Serializable]
public class HapticProperties
{
    // Haptic Properties
    public double Stiffness;
    public bool Surface; // 0 = No surface
    public double StaticFriction;
    public double DynamicFriction;
    public double Level;

    public double MagneticDistance;
    public double MagneticForce;

    public double Viscosity;

    public double SticksplipStiffness;
    public double SticksplipForce;

    public double VibrationFreq;
    public double VibrationAmplitude;

    public HapticProperties(double stiffness, bool surface,
	    double staticFriction, double dynamicFriction,
	    double level,
	    double magneticDistance, double magneticForce,
	    double viscosity,
	    double stickslipstiffness,
	    double stickslipforce,
	    double vibrationfreq, double vibrationamplitude)
    {
	Stiffness = stiffness;
	Surface = surface;
	StaticFriction = staticFriction;
	DynamicFriction = dynamicFriction;
	Level = level;
	MagneticDistance = magneticDistance;
	MagneticForce = magneticForce;
	Viscosity = viscosity;
	SticksplipStiffness = stickslipstiffness;
	SticksplipForce = stickslipforce;
	VibrationFreq = vibrationfreq;
	VibrationAmplitude = vibrationamplitude;
    }

    public HapticProperties()
    {
	Stiffness = 0.4;
	Surface = true;
	StaticFriction = 0.1;
	DynamicFriction = 0.1;
	Level = 0.3;

	MagneticDistance = 0.0;
	MagneticForce = 0.0;

	Viscosity = 0.0;
	SticksplipStiffness = 0.0;
	SticksplipForce = 0.0;

	VibrationFreq = 0.0;
	VibrationAmplitude = 0.0;
    }
    public static bool operator ==(HapticProperties h1, HapticProperties h2)
    {
	return h1.Equals(h2);
    }
    public static bool operator !=(HapticProperties h1, HapticProperties h2)
    {
	return !h1.Equals(h2);
    }

    // https://stackoverflow.com/questions/9317582/correct-way-to-override-equals-and-gethashcode
    public override bool Equals(object obj)
    {
	var item = obj as HapticProperties;

	if (item == null)
	{
	    return false;
	}

	return this == item;
    }

    public override int GetHashCode()
    {
	// FIXME: I think this is seriously wrong
	return Stiffness.GetHashCode() +
	    Surface.GetHashCode() +
	    StaticFriction.GetHashCode() +
	    DynamicFriction.GetHashCode() +
	    Level.GetHashCode() +
	    MagneticDistance.GetHashCode() +
	    MagneticForce.GetHashCode() +
	    Viscosity.GetHashCode() +
	    SticksplipStiffness.GetHashCode() +
	    SticksplipForce.GetHashCode() +
	    VibrationFreq.GetHashCode() +
	    VibrationAmplitude.GetHashCode();
    }

    public bool Equals(HapticProperties h2)
    {
	return (Stiffness == h2.Stiffness) &&
	    (Surface == h2.Surface) &&
	    // Friction
	    (StaticFriction == h2.StaticFriction) &&
	    (DynamicFriction == h2.DynamicFriction) &&
	    (Level == h2.Level) &&
	    // Magnetic
	    (MagneticDistance == h2.MagneticDistance) &&
	    (MagneticForce == h2.MagneticForce) &&
	    // Viscosity
	    (Viscosity == h2.Viscosity) &&

	    (SticksplipStiffness == h2.SticksplipStiffness) &&
	    (SticksplipForce == h2.SticksplipForce) &&

	    (VibrationFreq == h2.VibrationFreq) &&
	    (VibrationAmplitude == h2.VibrationAmplitude);
    }
    public HapticProperties Copy()
    {
	return new HapticProperties(Stiffness, Surface,
		StaticFriction, DynamicFriction,
		Level,
		MagneticDistance, MagneticForce,
		Viscosity, SticksplipStiffness,
		SticksplipForce,
		VibrationFreq, VibrationAmplitude);
    }
}
