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

using UnityEngine;
using System.Linq; // Contains
using System.Runtime.InteropServices;

// Helper functions to use the HapticNativePlugin easier form Unity3D.
// This must be the only cs script (except for HapticObject and
// HapticHandler) that depends on unity

static public class UnityHaptics {
    // Generic helpers
    public static Quaternion DoubleToQuat(double [] qd) {
	Quaternion q;
	q.w = (float)qd[0];
	q.x = (float)qd[1];
	q.y = (float)qd[2];
	q.z = (float)qd[3];
	return q;
    }
    public static Vector3 DoubleToVect(double[] qd)
    {
	Vector3 v;
	v.x = (float)qd[0];
	v.y = (float)qd[1];
	v.z = (float)qd[2];
	return v;
    }

    // FIXME: LogMessage Sometimes SegFault
    public static string LogMessage(int error, bool silent = true) {
	string message = HapticNativePlugin.GetErrorMsg(error);
	if (error != 0)
	{
	    Debug.LogError(message);
	} else if (error == 0 && !silent) {
	    Debug.Log(message);
	}
	return message;
    }
    public static void SetUnityWorldCoordinates()
    {
	int res = HapticNativePlugin.set_world_mirror(2, 3, -1);
	LogMessage(res, true);
	// Adjust world for unity. This is required, because it calls the wrapper
	// which is adjusting this ones to tens, the right scale for unity
	// SetWorldScale(new Vector3(1.0f, 1.0f, 1.0f)); // FIXME: re enable
    }

    public static void SetWorldRotationQuaternion(Quaternion q)
    {
	HapticNativePlugin.set_world_rotation_quaternion(q.w, q.x, q.y, q.z);
    }
    public static Quaternion GetWorldRotation()
    {
	return DoubleToQuat(HapticNativePlugin.GetWorldRotation());
    }
    public static Vector3 GetWorldMirror()
    {
	return DoubleToVect(HapticNativePlugin.GetWorldMirror());
    }
    public static Vector3 GetWorldOrder()
    {
	return DoubleToVect(HapticNativePlugin.GetWorldOrder());
    }
    public static Vector3 GetWorldTranslation()
    {
	return DoubleToVect(HapticNativePlugin.GetWorldTranslation());
    }
    public static void SetWorldTranslation(Vector3 t) {
	HapticNativePlugin.set_world_translation(t.x, t.y, t.z);
    }
    public static void SetWorldScale(Vector3 s)
    {
	// Unity's workspace is by default 10 times bigger
	if ((s.x != s.y) || (s.y != s.z))
	{
	    Debug.LogWarning("The scale you are setting is different along axis. " +
			     "This is not supported. Using X only");
	}
	HapticNativePlugin.set_world_scale(s.x * 10.0,
					   s.y * 10.0,
					   s.z * 10.0);
    }
    public static void SetHapticWorkspace(Transform world)
    {
	SetWorldRotationQuaternion(world.rotation);
	SetWorldTranslation(world.position);
	// SetWorldScale(world.lossyScale); // FIXME
    }
    public static Vector3 GetToolProxyPosition()
    {
	return DoubleToVect(HapticNativePlugin.GetToolProxyPosition());
    }
    public static Vector3 GetToolPosition()
    {
	return DoubleToVect(HapticNativePlugin.GetToolPosition());
    }
    public static int SetToolPosition(Vector3 position)
    {
	double [] pos = new double [3] { position.x, position.y, position.z };
	return HapticNativePlugin.SetToolPosition(pos);
    }
    public static Vector3 GetToolVelocity()
    {
	return DoubleToVect(HapticNativePlugin.GetToolVelocity());
    }
    public static Quaternion GetToolRotation()
    {
	return DoubleToQuat(HapticNativePlugin.GetToolRotation());
    }

    public static int CreateMeshObject(Vector3 position, Vector3 scale, Quaternion rotation,
			  Vector3[] vertPos, Vector3[] normals,
			  int vertNum, int[,] tris, int triNum,
			  int uvNum, Vector2[] uv)
    {
	double[] objectPosition = new double[3];
	objectPosition[0] = position.x;
	objectPosition[1] = position.y;
	objectPosition[2] = position.z;

	double[] objectScale = new double[3];
	objectScale[0] = scale.x;
	objectScale[1] = scale.y;
	objectScale[2] = scale.z;

	double[] objectRotation = new double[3];
	objectRotation[0] = rotation.x;
	objectRotation[1] = rotation.y;
	objectRotation[2] = rotation.z;

	double[,] objectVertPos = new double[vertNum, 3];
	for (int i = 0; i < vertNum; i++)
	{
	    objectVertPos[i, 0] = vertPos[i].x;
	    objectVertPos[i, 1] = vertPos[i].y;
	    objectVertPos[i, 2] = vertPos[i].z;
	}

	double[,] objectNormals = new double[vertNum, 3];
	for (int i = 0; i < vertNum; i++)
	{
	    objectNormals[i, 0] = normals[i].x;
	    objectNormals[i, 1] = normals[i].y;
	    objectNormals[i, 2] = normals[i].z;
	}

	double[,] uvs = new double[uvNum, 2];
	for (int i = 0; i < uvNum; i++)
	{
	    uvs[i, 0] = uv[i].x;
	    uvs[i, 1] = uv[i].y;
	}

	// returns the object idx
	return HapticNativePlugin.create_mesh_object(objectPosition, objectScale,
	    objectRotation, objectVertPos,
	    objectNormals, vertNum, tris, triNum,
		uvNum, uvs);
    }
    public static int CreateBox(Vector3 scale, Vector3 position, Quaternion rotation)
    {
	double[] objectScale = new double[3];
	objectScale[0] = (double)scale.x;
	objectScale[1] = (double)scale.y;
	objectScale[2] = (double)scale.z;


	double[] objectPosition = new double[3];
	objectPosition[0] = (double)position.x;
	objectPosition[1] = (double)position.y;
	objectPosition[2] = (double)position.z;


	double[] objectRotation = new double[3];
	objectRotation[0] = (double)rotation.x;
	objectRotation[1] = (double)rotation.y;
	objectRotation[2] = (double)rotation.z;
	return HapticNativePlugin.create_box_object
	(objectScale, objectPosition, objectRotation);
    }

    public static int CreateSphere(double radius,
				   Vector3 position,
				   Quaternion rotation)
    {
	double[] objectPosition = new double[3];
	objectPosition[0] = (double)position.x;
	objectPosition[1] = (double)position.y;
	objectPosition[2] = (double)position.z;


	double[] objectRotation = new double[3];
	objectRotation[0] = (double)rotation.x;
	objectRotation[1] = (double)rotation.y;
	objectRotation[2] = (double)rotation.z;
	return HapticNativePlugin.create_sphere_object
	    (radius, objectPosition, objectRotation);
    }

    public static
    int CreateUnityMesh(Mesh mesh, Transform tf)
    {
	int[,] triangles = new int[(mesh.triangles.Length / 3), 3];

	for (int i = 0; i < mesh.triangles.Length / 3; i++)
	{
	    // Different sort between unity and chai?
	    triangles[i, 0] = mesh.triangles[3 * i + 0];
	    triangles[i, 1] = mesh.triangles[3 * i + 1];
	    triangles[i, 2] = mesh.triangles[3 * i + 2];
	}


	return CreateMeshObject(tf.position, tf.lossyScale,
			tf.rotation,
			mesh.vertices, mesh.normals, mesh.vertices.Length,
			triangles, mesh.triangles.Length / 3,
			mesh.uv.Length, mesh.uv);
    }

    public static
    int CreateUnityShape(Mesh mesh, Transform tf)
    {
	switch (mesh.name)
	{
	    case "Sphere":
		return CreateSphere(mesh.bounds.extents.x * tf.localScale.x,
				    tf.position, tf.rotation);
	    case "Cube":
		return CreateBox(tf.localScale, tf.position, tf.rotation);
	    default:
		// No specific implementation exists
		return -1;
	}
    }

    public static int SetTexture(int ObjectId, Texture2D texture, bool reversed)
    {
	float[] pixels = texture.GetPixels().Select
	    (t => reversed ? 1 - t.grayscale : t.grayscale).ToArray();

	return HapticNativePlugin.SetObjectTexture
	    (ObjectId,
	     (uint)texture.width, (uint)texture.height,
	     pixels, false);
    }

    public static int SetMaterial(int ObjectId, HapticMaterial material)
    {
	return HapticNativePlugin.SetHapticMaterial(ObjectId,
	   material.Stiffness, material.HasSurface,
	   material.StaticFriction, material.DynamicFriction,
	   material.MagneticForce, material.MagneticDistance,
	   material.Viscosity,
	   material.TextureLevel,
	   material.SticksplipStiffness,
	   material.SticksplipForce,
	   material.VibrationFreq,
	   material.VibrationAmplitude);
    }
    public static
    int SetObjectPosition(int ObjectId, Vector3 position)
    {
	double[] pos = new double[] {position.x,
				     position.y,
				     position.z};
	return HapticNativePlugin.set_object_position(ObjectId, pos);
    }

    public static int SetObjectRotation(int ObjectId, Quaternion rotation)
    {
	double[] quat = { rotation.w, rotation.x, rotation.y, rotation.z };

	return HapticNativePlugin.set_object_rotation(ObjectId, quat);
    }
    public static int SetObjectRotationEuler(int ObjectId, Quaternion rotation)
    {
	double[] angle = { rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z };

	return HapticNativePlugin.set_object_rotation_euler(ObjectId, angle);
    }
    public static int SetObjectRotationEuler(int ObjectId, Vector3 rotation)
    {

	double[] angle = { rotation.x, rotation.y, rotation.z };

	return HapticNativePlugin.set_object_rotation_euler(ObjectId, angle);
    }
    public static Quaternion GetObjectRotation(int ObjectId)
    {
	double[] quat = new double[4];

	int ret = HapticNativePlugin.get_object_rotation(ObjectId, quat);
	if (ret == 0) {
	    return new Quaternion((float)quat[0], (float)quat[1], (float)quat[2], (float)quat[3]);
	} else
	{
	    return Quaternion.identity;
	}
    }
    public static int SetObjectScale(int ObjectId, Vector3 scale)
    {
	double[] vect = { scale.x, scale.y, scale.z };
	Debug.LogWarning("Changing object scale is currently not working");

	return HapticNativePlugin.set_object_scale(ObjectId, vect);
    }

    // Extra -- Other nice helpers
    public static bool IsStylus(Collision collision)
    {
	return collision.gameObject.GetComponent<HapticTool>() != null;
    }
    public static bool DeviceMatchesProxy()
    {
	return GetToolProxyPosition() == GetToolPosition();
    }

    /// <summary>
    /// This function takes as its only parameter (Hook) another function, and add it as an hook on each haptic loop.
    /// The 'Hook' parameter must take exactly 2 arguments, the actual position and the actual velocity,
    /// both as UNITY Vector3, and must return the force as a Vector3.
    ///  public float GravityForceIntensity = 1.5f;
    /// A simple example: public Vector3 MyGravity(Vector3 pos, Vector3 vel) {  return new Vector3(0.0f, 0.0f, -GravityForceIntensity); }
    ///
    /// You can change GravityForceIntensity and the rendered force will automatically update :)
    /// </summary>
    /// <param name="Hook"></param>
    public static void SetHook(System.Func<Vector3, Vector3, Vector3> Hook)
    {
        System.Action<System.IntPtr, System.IntPtr, System.IntPtr> real_hook = (p,v,f) => {
            // Prepare data
            double[] ManagedPosition = new double[3];
            Marshal.Copy(p, ManagedPosition, 0, 3);

            double[] ManagedVelocity = new double[3];
            Marshal.Copy(v, ManagedVelocity, 0, 3);

            var pos = new Vector3((float)ManagedPosition[0], (float)ManagedPosition[1], (float)ManagedPosition[2]);
            var vel = new Vector3((float)ManagedVelocity[0], (float)ManagedVelocity[1], (float)ManagedVelocity[2]);

            // Actual invocation
            Vector3 force = Hook(pos, vel);

            // Store output
            double[] o = new double[3];
            o[1] = force.x;
            o[2] = force.y;
            o[0] = -force.z;

            Marshal.Copy(o, 0, f, o.Length);
        };

        HapticNativePlugin.SetHook(real_hook);
    }
}
