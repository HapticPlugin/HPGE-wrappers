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

using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine;

static public class HapticNativePlugin
{
    static readonly Version RequiredLibraryVersion = new System.Version(2, 1, 0);
    public const int SUCCESS = 0;

    [DllImport("HPGE")]
    public static extern
	void get_version_info(ref System.Int32 major,
			      ref System.Int32 minor,
			      ref System.Int32 patch);

    [DllImport("HPGE")]
    public static extern
	int get_error_msg(int err, int buffer_lenght, [In, Out] byte[] buffer);

    [DllImport("HPGE")]
    public static extern
	int last_error_msg(int buffer_lenght, [In, Out] byte[] buffer);

    [DllImport("HPGE")]
    public static extern int init_logging(
					  int device_coordinates,
					  int position,
					  int velocity, int force,
					  int interaction_forces, int object_number,
					  int[] objects);
    [DllImport("HPGE")]
    public static extern int start_logging(int sampling_rate);
    [DllImport("HPGE")]
    public static extern int is_logging();

    [DllImport("HPGE")]
    public static extern int stop_logging_and_save(byte[] filename);
    // TODO: not implemtented yet!
    //[DllImport("HPGE")]
    //public static extern int stop_logging_and_get(SavedData data);
    [DllImport("HPGE")]
    public static extern void tick();

    [DllImport("HPGE")]
    public static extern int count_devices();
    [DllImport("HPGE")]
    public static extern int get_device_name(int deviceId, int buffer_size,
					     [In, Out] byte[] deviceName);

    [DllImport("HPGE")]
    public static extern int initialize(int deviceId, double ws, double tool);
    [DllImport("HPGE")]
    public static extern
	int deinitialize();

    [DllImport("HPGE")]
    public static extern
	int start();
    [DllImport("HPGE")]
    public static extern
	int stop();

    // WARNING: interface might change
    [DllImport("HPGE")]
    public static extern double get_loop_frequency();
    // WARNING: Might get removed or merged with get_loop_frequency
    [DllImport("HPGE")]
    public static extern double get_loops();
    [DllImport("HPGE")]
    public static extern long get_log_frame();

    [DllImport("HPGE")]
    public static extern void log_annotate(byte[] note);
    [DllImport("HPGE")]
    public static extern int is_initialized();
    [DllImport("HPGE")]
    public static extern int is_running();

    public delegate void CallbackDelegate(IntPtr position, IntPtr velocity, IntPtr force);

    // TODO: not implemented (the C# code) yet
    [DllImport("HPGE")]
    public extern static void set_hook(int proxy, IntPtr hook);
    [DllImport("HPGE")] // This in theory is working
    public extern static int remove_hook();

    [DllImport("HPGE")]
    public static extern int enable_dynamic_objects();
    [DllImport("HPGE")]
    public static extern int disable_dynamic_objects();

    [DllImport("HPGE")]
    public static extern void enable_wait_for_small_forces();
    [DllImport("HPGE")]
    public static extern void disable_wait_for_small_forces();
    [DllImport("HPGE")]
    public static extern void enable_rise_forces();
    [DllImport("HPGE")]
    public static extern void disable_rise_forces();
    [DllImport("HPGE")]
    public static extern
	void set_world_rotation_eulerXYZ(double x,
					 double y,
					 double z);
    [DllImport("HPGE")]
    public static extern
	void set_world_rotation_quaternion(double w,
					   double x,
					   double y,
					   double z);
    [DllImport("HPGE")]
    public static extern
	void get_world_rotation(double[] array);

    [DllImport("HPGE")]
    public static extern
	int set_world_mirror(int x, int y, int z);
    [DllImport("HPGE")]
    public static extern
	void get_world_mirror(double[] array);
    [DllImport("HPGE")]
    public static extern
	void get_world_order(double[] array);

    [DllImport("HPGE")]
    public static extern
	void set_world_translation(double x, double y, double z);
    [DllImport("HPGE")]
    public static extern
	void get_world_translation(double[] array);

    [DllImport("HPGE")]
    public static extern
	int set_world_scale(double x, double y, double z);
    [DllImport("HPGE")]
    public static extern
	void get_world_scale(double[] array);

    [DllImport("HPGE")]
    static extern
	int get_tool_proxy_position(double[] array);
    [DllImport("HPGE")]
    static extern int get_tool_position(double[] array);
    [DllImport("HPGE")]
    static extern int set_tool_position(double[] array);
    [DllImport("HPGE")]
    static extern int get_tool_velocity(double[] array);
    [DllImport("HPGE")]
    static extern int get_tool_rotation(double[] array); // quaternion

    [DllImport("HPGE")]
    public static extern int is_tool_button_pressed(int buttonId);

    [DllImport("HPGE")]
    public static extern
	int create_mesh_object(double[] objectPos,
			       double[] objectScale, double[] objectRotation,
			       double[,] vertPos, double[,] normals,
			       int vertNum, int[,] tris, int triNum, int uvNum,
			       double[,] uvs);
    [DllImport("HPGE")]
    public static extern
	int create_sphere_object(double radius, double[] objectRotation, double[] objectPos);

    [DllImport("HPGE")]
    public static extern
	int create_box_object(double[] objectrotation,
			      double[] objectRotation,
			      double[] objectPos);

    [DllImport("HPGE")]
    public static extern int add_object_to_world(int objectId);
    [DllImport("HPGE")]
    public static extern int disable_object(int objectId);
    [DllImport("HPGE")]
    public static extern int enable_object(int objectId);
    [DllImport("HPGE")]
    public static extern int object_exists(int objectId);

    // Remember that all strings (byte[] must be nul-terminated)
    [DllImport("HPGE")]
    static extern int set_object_tag(int objectId, byte[] tag);
    [DllImport("HPGE")]
    static extern int set_object_texture(int objectId, uint size_x, uint size_y,
					 float[] pixels, int spherical);
    [DllImport("HPGE")]
    static extern int set_object_material(int objectId,
					  double stiffness, int surface, double staticfriction, double dynamicfriction,
					  double magnetic_max_force, double magnetic_max_distance, double viscosity,
					  double level, double sticksplip_stiffness, double sticksplip_maxforce,
					  double vibration_freq, double vibration_amplitude);

    [DllImport("HPGE")]
    public static extern int enable_position_interpolation(int objectId);
    [DllImport("HPGE")]
    public static extern int disable_position_interpolation(int objectId);

    [DllImport("HPGE")]
    public static extern int enable_rotation_interpolation(int objectId);
    [DllImport("HPGE")]
    public static extern int disable_rotation_interpolation(int objectId);

    // WARNING: overshot might get removed // TODO: decide what to do
    [DllImport("HPGE")]
    public static extern int set_interpolation_period(int objectId, int cycles,
						      double overshot);

    [DllImport("HPGE")]
    public static extern int set_object_position(int objectId, double[] vertPos);
    [DllImport("HPGE")]
    public static extern int set_object_rotation(int objectId, double[] objectRotation);
    [DllImport("HPGE")]
    public static extern int set_object_rotation_euler(int objectId, double[] objectRotation);
    [DllImport("HPGE")]
    public static extern int get_object_rotation(int objectId, double[] objectRotation);
    [DllImport("HPGE")]
    public static extern int set_object_scale(int objectId, double[] scale);


    // -------- HERE the C interface ENDS! ---------


    // ----- HERE there is a pure-C# wrapper -------
    const int StringBufferSize = 150;

    // https://stackoverflow.com/questions/43226928/how-to-pass-function-pointer-from-c-sharp-to-a-c-dll
    public static void GravityHook(IntPtr positionPtr, IntPtr velocityPtr, IntPtr forcePtr)
    {
        double[] ManagedPosition = new double[3];
        Marshal.Copy(positionPtr, ManagedPosition, 0, 3);

        double[] ManagedVelocity = new double[3];
        Marshal.Copy(velocityPtr, ManagedVelocity, 0, 3);

        double[] OutForce = new double[3];
        OutForce[0] = 0.0;
        OutForce[1] = 0.0;
        OutForce[2] = -1.5;
        Marshal.Copy(OutForce, 0, forcePtr, OutForce.Length);
    }

    public static void CenterHook(IntPtr positionPtr, IntPtr velocityPtr, IntPtr forcePtr)
    {
        double[] ManagedPosition = new double[3];
        Marshal.Copy(positionPtr, ManagedPosition, 0, 3);

        double[] ManagedVelocity = new double[3];
        Marshal.Copy(velocityPtr, ManagedVelocity, 0, 3);
        double [] center = { 0, 0, 0 };
        double[] OutForce = new double[3];
        OutForce[0] = center[0] - ManagedPosition[0];
        OutForce[1] = center[1] - ManagedPosition[1];
        OutForce[2] = center[2] - ManagedPosition[2];
        Marshal.Copy(OutForce, 0, forcePtr, OutForce.Length);
    }
    private static CallbackDelegate hook;
    public static void SetHook(Action<IntPtr, IntPtr, IntPtr> new_hook) {
       hook = new CallbackDelegate(new_hook);
       set_hook(1, Marshal.GetFunctionPointerForDelegate(hook));
    }

    public static Version GetVersionInfo() {
	int major, minor, patch;
	major = minor = patch = 0;
	get_version_info(ref major, ref minor, ref patch);
	return new Version(major, minor, patch);
    }

    public static bool IsLibraryCompatible() {
	Version v = GetVersionInfo();

	return (RequiredLibraryVersion.Major == v.Major &&
		RequiredLibraryVersion.Minor <= v.Minor &&
		RequiredLibraryVersion.Build <= v.Build);
    }

    public static
	string GetLastErrorMsg()
    {
	byte[] buff = new byte[StringBufferSize];
	int res = last_error_msg(StringBufferSize, buff);
	if (res == SUCCESS)
	{
	    return Encoding.Default.GetString(buff).Split('\0')[0];
	}
	return "FATAL ERROR: Could not read last error!";
    }

    public static string GetErrorMsg(int err)
    {
	byte[] buff = new byte[StringBufferSize];
	int res = get_error_msg(err, StringBufferSize, buff);
	// If we failed in getting the error message,
	// try getting why we failed. Might fail again
	// but I can't address it again
	if (res != 0)
	{
	    return GetLastErrorMsg();
	}
	return Encoding.Default.GetString(buff).Split('\0')[0];
    }


    public static int SetupLogging(int SamplingRate, bool DeviceCoordinates)
    {
	// TODO: not yet implemented "what to log", so it's not exposed
	int status = init_logging(DeviceCoordinates ? 0 : 1,
	    1, 1, 1, 1, 0, new int[0]);
	if (status == SUCCESS) { status = start_logging(SamplingRate); }
	return status;
    }
    public static bool StopLoggingAndSave(string filename)
    {
	byte[] data = Encoding.ASCII.GetBytes(filename);
	byte[] zdata = new byte[data.Length + 1];
	data.CopyTo(zdata, 0);
        zdata[data.Length] = (byte)0;

        return stop_logging_and_save(zdata) == 0;
    }

    public static void Annotate(string note)
    {
	byte[] data = Encoding.ASCII.GetBytes(note);
	byte[] zdata = new byte[data.Length + 1];
	data.CopyTo(zdata, 0);
        zdata[data.Length] = (byte)0;

        log_annotate(zdata);
    }


    public static string GetDeviceName(int deviceId) {
	byte[] buff = new byte[StringBufferSize];
	int res = get_device_name(deviceId, StringBufferSize, buff);
	if (res == SUCCESS) {
	    return Encoding.Default.GetString(buff).Split('\0')[0];
	} else
	{
	    // If the problem is the buffer size, chances
	    // are there is a failure getting the device name
	    return GetErrorMsg(res);
	}
    }

    public static
    int Initialize(int device = 0, double hapticScale = 1.0, double toolRad = 0.05)
    {
	return initialize(device, hapticScale, toolRad);
    }

    // TODO: deinitialize() has no wrappers. Do we need one?
    // TODO: start() has no wrappers. Do we need one?
    // TODO: stop() has no wrappers. Do we need one?
    // TODO: get_loop_frequency() has no wrappers. Do we need one?
    // FIXME: do we need this?
    // TODO: get_loops() has no wrappers. Do we need one?
    public static
    bool IsInitialized()
    {
	return is_initialized() == SUCCESS;
    }
    public static
    bool IsRunning()
    {
	return is_running() == SUCCESS;
    }

    // FIXME: set_hook() needs a wrapper but I still need to implement it
    // FIXME: remove_hook() needs a wrapper but I still need to implement it

    // TODO: enable_dynamic_objects() has no wrappers. Do we need one?
    // TODO: disable_dynamic_objects() has no wrappers. Do we need one?

    // TODO: enable_wait_for_small_forces() has no wrappers. Do we need one?
    // TODO: disable_wait_for_small_forces() has no wrappers. Do we need one?

    public static double[] GetWorldRotation()
    {
	double[] q = new double[4];
	get_world_rotation(q);
	return q;
    }
    public static double[] GetWorldMirror()
    {
	double[] v = new double[3];
	get_world_mirror(v);
	return v;
    }
    // FIXME: should this be int?
    public static double[] GetWorldOrder()
    {
	double[] v = new double[3];
	get_world_order(v);
	return v;
    }

    // FIXME: set_world_mirror() has no wrappers. Do we need one?

    public static double[] GetWorldTranslation()
    {
	double[] v = new double[3];
	get_world_translation(v);
	return v;
    }

    // FIXME: set_world_translation() has no wrappers. Do we need one?
    // FIXME: set_world_scale() has no wrappers. Do we need one?

    public static double[] GetToolProxyPosition()
    {
	double[] position = new double[3];
	int res = get_tool_proxy_position(position);
	UnityHaptics.LogMessage(res, true);
	return position;
    }
    public static double[] GetToolPosition()
    {
	double[] position = new double[3];
	get_tool_position(position);
	return position;
    }
    public static int SetToolPosition(double[] position)
    {
	return set_tool_position(position);
    }
    public static double[] GetToolVelocity()
    {
	double[] velocity = new double[3];
	get_tool_velocity(velocity);
	return velocity;
    }
    public static double[] GetToolRotation()
    {
	double[] rotation = new double[4];
	get_tool_rotation(rotation);
	return rotation;
    }
    public static bool IsPrimaryButtonPressed()
    {
	return is_tool_button_pressed(0) == 0;
    }
    public static bool IsSecondaryButtonPressed()
    {
	return is_tool_button_pressed(1) == 0;
    }

    // TODO: add_object_to_world() has no wrappers. Do we need one?
    // TODO: disable_object() has no wrappers. Do we need one?
    // TODO: enable_object() has no wrappers. Do we need one?
    // TODO: object_exists() has no wrappers. Do we need one?
    public static bool SetObjectTag(int objectId, string tag)
    {
	byte[] data = Encoding.Default.GetBytes(tag);
	byte[] zdata = new byte[data.Length + 1];
	data.CopyTo(zdata, 0);
	return set_object_tag(objectId, data) == 0;
    }
    public static int SetObjectTexture(int objectId, uint width, uint height,
				    float[] pixels, bool spherical)
    {
	return set_object_texture(objectId, width, height, pixels, spherical ? 1 : 0);
    }

    public static int
    SetHapticMaterial(int objectId, double stiffness, bool surface,
		     double staticfriction, double dynamicfriction,
		     double maxmagnetforce, double maxmagnetdistance,
		     double viscosity, double level, double sticksplip_stiffness,
		     double sticksplip_maxforce,
		     double vibration_freq, double vibration_amplitude)
    {
	return set_object_material(objectId, stiffness, surface ? 1 : 0,
		     staticfriction, dynamicfriction,
		     maxmagnetforce, maxmagnetdistance,
		     viscosity, level, sticksplip_stiffness, sticksplip_maxforce,
		     vibration_freq, vibration_amplitude);
    }

    public static int SetPositionInterpolation(int ObjectId, bool enable) {
	return enable ?
	    enable_position_interpolation(ObjectId) :
	    disable_position_interpolation(ObjectId);
    }
    public static int SetRotationInterpolation(int ObjectId, bool enable)
    {
	return enable ?
	    enable_rotation_interpolation(ObjectId) :
	    disable_rotation_interpolation(ObjectId);
    }
    // TODO: set_interpolation_period() has no wrappers. Do we need one?
}
