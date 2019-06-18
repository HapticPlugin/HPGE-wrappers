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

using System.IO;
using UnityEngine;

public class HapticTool : MonoBehaviour {
    public enum Tick { No, Update, FixedUpdate };
    public enum WaitForces { No, Small, Rise };

    private bool Started = false;
    private bool Running = false;

    [Header("Hardware Info")]
    /// <summary>
    /// Show the frequency the haptic loop is running at (Hz)
    /// </summary>
    [ReadOnly]
    public double LoopFrequencyHz = 0.0;
    // This counts the passed loops and is used to update the
    // LoopFrequencyHz variable
    private int LoopNumber = 0;
    /// <summary>
    /// Haptic device to use. By default,
    /// the first it can find (0-based index)
    /// </summary>
    public int DeviceId = 0;
    [InspectorButton("ScanDevices")]
    public bool Scan;

    [Header("Tool Properties")]
    /// <summary>
    /// If the object has an attached collider and it's a
    /// sphere collider, use it's radius for the tool
    /// </summary>
    public bool UseSphereRadius = true;
    /// <summary>
    /// If UseSphereRadius is false, use this value instead
    /// </summary>
    public float Radius = 0.5f;
    public bool EnableDynamicObjects = true;
    // TODO: a possible implementation of this is to use something like PhysiX maximum velocity
    /// <summary>
    /// Don't apply any force until the force to apply is small
    /// or increase the force gradually at start
    /// </summary>
    public WaitForces WaitForForces = WaitForces.Small;
    /// <summary>
    /// If this is true, instead of using the real device position,
    /// uses the proxy to update the tool position and rotation
    /// </summary>
    public bool UseProxyPosition = false;

    [Header("Workspace Properties")]
    //[ReadOnly] // FIXME: This should be removed!
    public double WorkspaceSize = 10; // meters
    [ReadOnly]
    public Transform HapticWorkspace;

    // TODO: This would be better if grouped
    // but it's too much work for now. See
    // https://forum.unity.com/threads/multiple-bools-in-one-line-in-inspector.181778/
    [Header("Logging Options")]
    public bool EnableLog = false;
    /// <summary>
    /// Whether to use unity coordinates or chai3d ones
    /// </summary>
    public bool DeviceCoordinates = false;
    /// <summary>
    /// How many loops (in the haptic thread) should we save information?
    /// Its unit depends on the loop frequency, so it's approximately in ms
    /// </summary>
    [Range(1,200)] // FIXME: this could even go beyond
    public int LogDownsampling = 1;
    /// <summary>
    /// This log is a csv file created by the library.
    ///  Position relative to the main program
    /// </summary>
    public string LogBasename = "output";
    // number to append to log file to prevent overwrites
    private int LogNumber = 0;
    /// <summary>
    /// Useful to know how many times the (fixed)update is called.
    /// If logging is enabled,
    /// </summary>
    public Tick EnableTick = Tick.No;
    /// <summary>
    /// Filter information and debug messages shown in unity
    /// </summary>
    [Header("Debug Settings")]
    [Range(0,3)]
    public int Verbosity = 0;
    // FIXME: still not implemented
    //bool LogPositions = false;
    //bool LogVelocity = false;
    //bool LogForces = false;
    // FIXME: implement this. We should find all Touchable object, find those that are
    // enabled, get their id.
    // The problem is that should happen _before_ the logging starts
    /// <summary>
    /// To log interaction forces, each object must have the
    /// "LogInteractionForces" toggle enabled
    /// </summary>
    // bool LogInteractionForces = false;

    private string GenLogName()
    {
	return LogBasename + "." + LogNumber + ".csv";
    }

    public void ScanDevices() {
	int devices = HapticNativePlugin.count_devices();
	// Get all devices names and print them here
	if (devices < 0) {
	    Debug.LogError(HapticNativePlugin.GetLastErrorMsg());
	    return;
	}
	else if (devices == 0) {
	    Debug.LogError("No devices found!");
	    return;
	}
	else if (devices == 1) {
	    Debug.LogWarning("Only one device found (Probably no real devices attached!)");
	}
	for (int device = -1; device < devices - 1; ++device) {
	    Debug.Log("Found device #" + device + " (" +
		HapticNativePlugin.GetDeviceName(device) + ")");
	CheckCompatibility();
	}
    }

    public void ForceWorkspaceUpdate()
    {
	if (HapticWorkspace != null)
	{
	    UnityHaptics.SetHapticWorkspace(HapticWorkspace);
	}
    }

    private void CheckCompatibility() {
	if (!HapticNativePlugin.IsLibraryCompatible())
	{
	    Debug.LogError("Library version is incompatible! " +
		HapticNativePlugin.GetVersionInfo().ToString());
	}
	else
	{
	    if (Verbosity > 2)
	    {
		Debug.Log("Haptic Library Version (HGPE) is: " +
		    HapticNativePlugin.GetVersionInfo().ToString());
	    }
	}
    }
    private void SetWaitForForces() {
	if (WaitForForces == WaitForces.No) {
	    HapticNativePlugin.disable_wait_for_small_forces();
	    //  HapticNativePlugin.disable_rise_forces();
	    return;
	}
	if (WaitForForces == WaitForces.Small)
	{
	    HapticNativePlugin.enable_wait_for_small_forces();
	}
	else {
	    Debug.LogWarning("Not checked yet (might not work)");
	    HapticNativePlugin.enable_rise_forces();
	}
    }
    private void UpdateToolPositionAndRotation()
    {
	// Debug.Log(UnityHaptics.GetToolPosition());
	transform.SetPositionAndRotation(UseProxyPosition ?
		UnityHaptics.GetToolProxyPosition() :
		UnityHaptics.GetToolPosition(),
		UnityHaptics.GetToolRotation());
    }
    private void Awake()
    {
	CheckCompatibility();
	if (UseSphereRadius)
	{
	    //// This can only render a sphere.
	    //// Warn if attached to something else (maybe they just want to get the input position)
	    if (GetComponent<MeshFilter>().name != "Sphere"
		&& Verbosity > 0) {
		Debug.LogWarning("This script is not attached to a sphere " +
		    "but you are trying to use the sphere radius. You should" +
		    "instead set the radius manually");
	    }
	    // The sphere size in unity is 0.5 units.
	    // We use the Transform scale on the X axis to get the real size
	    Radius = transform.lossyScale.x * 0.5f;
	}

	SetWaitForForces();

	// FIXME: what to do with workspace radius?
	UnityHaptics.SetUnityWorldCoordinates();

	int res = HapticNativePlugin.Initialize(DeviceId, WorkspaceSize, Radius);
	if (res != HapticNativePlugin.SUCCESS)
	{   // success silently, fail loudly
	    Debug.LogError("Could not start haptics");
	    UnityHaptics.LogMessage(res, false);
	    return;
	}

       //  ForceWorkspaceUpdate();

	if (EnableDynamicObjects)
	{
	    HapticNativePlugin.enable_dynamic_objects();
	}
	else
	{
	    HapticNativePlugin.disable_dynamic_objects();
	}

	// FIXME: move to UnityHaptics, enable things we want to log and so on
	if (EnableLog && LogDownsampling != 0) {
	    HapticNativePlugin.SetupLogging(LogDownsampling, DeviceCoordinates);
	}
	// Set initial position
	UpdateToolPositionAndRotation();
    }
    private void SaveLogIfLogging()
    {
	if (HapticNativePlugin.is_logging() !=
	    HapticNativePlugin.SUCCESS)
	{
	    return;
	}

	// Create the file name
	string LogName;
	do
	{
	    LogName = GenLogName();
	    LogNumber += 1;
	}
	while (File.Exists(LogName));

	if (HapticNativePlugin.StopLoggingAndSave(LogName))
	{
	    if (Verbosity > 0) { Debug.Log(LogName); }
	} else
	{
	    // TODO: get error message and print
	    Debug.LogError("Could not save log!");
	}
    }
    // Use this for initialization
    void Start()
    {
	// FIXME: Not working#
	//HapticNativePlugin.setHook(HapticNativePlugin.exampleHook);
	// Update position (if device has been moved between Awake and Start)
	UpdateToolPositionAndRotation();
    }

    private void OnDestroy()
    {
	if ((!HapticNativePlugin.IsInitialized() ||
	     !HapticNativePlugin.IsRunning()))
	{
	    Started = false;
	    return;
	}

	int res = HapticNativePlugin.deinitialize();
	UnityHaptics.LogMessage(res, true);
	SaveLogIfLogging();

	HapticNativePlugin.stop();
	Started = false;
	Running = false;
    }
    // Update is called once per frame
    private void Update()
    {
        //Debug.Log(UnityHaptics.GetToolVelocity());
	LoopNumber += 1;
	if (LoopNumber % 100 == 0)
	{
	    LoopFrequencyHz = HapticNativePlugin.get_loop_frequency();
	    if (Verbosity > 1) { Debug.Log(LoopFrequencyHz); }
	}
	if (EnableTick == Tick.Update) { HapticNativePlugin.tick(); }

	if (! Started || ! Running) {
	    int res = HapticNativePlugin.start();
	    UnityHaptics.LogMessage(res, true);
	    Started = res == HapticNativePlugin.SUCCESS;
	    Running = Started;

	    UpdateToolPositionAndRotation();
	}
    }

    void FixedUpdate ()
    {
	if (!Running) { return; }
	UpdateToolPositionAndRotation();
	if (EnableTick == Tick.FixedUpdate) { HapticNativePlugin.tick(); }
    }
}
