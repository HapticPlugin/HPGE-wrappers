module Haptics

const library = "libHPGE.so"

# lib = Libdl.dlopen("./libHPGE.so") # Open the library explicitly.
# sym = Libdl.dlsym(lib, :my_fcn)   # Get a symbol for the function to call.
# ccall(sym, ...) # Use the pointer `sym` instead of the (symbol, library) tuple (remaining arguments are the same).
# Libdl.dlclose(lib) # Close the library explicitly.

const requiredversion = v"2.3.6"

const string_buffer_length = Cint(100)

function get_version_info()
    major = Ref{Cint}(0); minor = Ref{Cint}(0); patch = Ref{Cint}(0);
    ccall((:get_version_info, "libHPGE.so"), Cvoid,
          (Ref{Cint}, Ref{Cint}, Ref{Cint}),
          major, minor, patch)
    VersionNumber(major[], minor[], patch[])
end

compatible() = get_version_info() == requiredversion

function get_error_msg(error::Integer)
    message = Vector{UInt8}(undef, string_buffer_length)
    res = ccall((:get_error_msg, "libHPGE.so"), Cint,
                (Cint, Cint, Cstring), error, string_buffer_length, pointer(message))
    if res != 0
        return "Could not get error message"
    end
    unsafe_string(pointer(message))
end

function last_error_msg()
    message = Vector{UInt8}(undef, string_buffer_length)
    res = ccall((:last_error_msg, "libHPGE.so"), Cint,
                (Cint, Cstring), string_buffer_length, pointer(message))
    if res != 0
        return "Could not get error message"
    end
    unsafe_string(pointer(message))
end


# int init_logging(
#     int device_coordinates,
#     int position,
#     int velocity, int force,
#     int interaction_forces, int object_number,
#     int[] objects);

#     public static extern int start_logging(int sampling_rate);
#     public static extern int is_logging();
#     public static extern int stop_logging_and_save(byte[] filename);


tick() = ccall((:tick, "libHPGE.so"), Cvoid, ())

count_devices()::Int = ccall((:count_devices, "libHPGE.so"), Cint, ())


function get_device_name(deviceid::Integer)
    devicename = Vector{UInt8}(undef, string_buffer_length)
    res = ccall((:get_device_name, "libHPGE.so"), Cint,
                (Cint, Cint, Cstring),
                deviceid, string_buffer_length, pointer(devicename))
    res != 0 && throw(get_error_msg(res))
    unsafe_string(pointer(devicename))
end

function initialize(deviceid::Integer; ws::Float64 = 10.0, tool::Float64 = 0.05)
    res = ccall((:initialize, "libHPGE.so"), Cint,
                (Cint, Cdouble, Cdouble), deviceid, ws, tool)
    # res != 0 && throw(get_error_msg(res))
    # true
end

function deinitialize()
    res = ccall((:deinitialize, "libHPGE.so"), Cint, ())
    # res != 0 && throw(get_error_msg(res))
    # true
end

function start()
    res = ccall((:start, "libHPGE.so"), Cint, ())
    # res != 0 && throw(get_error_msg(res))
    # true
end

function stop()
    res = ccall((:stop, "libHPGE.so"), Cint, ())
    # res != 0 && throw(get_error_msg(res))
    # true
end

get_loop_frequency() = ccall((:get_loop_frequency, "libHPGE.so"), Cdouble, ())
get_loops() = ccall((:get_loops, "libHPGE.so"), Cint, ())
get_log_frame() = ccall((:get_log_frame, "libHPGE.so"), Clong, ())
is_initialized() = ccall((:is_initialized, "libHPGE.so"), Cint, ()) == 0
is_running() = ccall((:is_running, "libHPGE.so"), Cint, ()) == 0
is_tool_button_pressed(button::Integer) =
    ccall((:is_tool_button_pressed, "libHPGE.so"), Cint, (Cint,), button) == 0

# log_annotate(byte[] note)
# set_hook, remove_hook

# public static extern int enable_dynamic_objects();
# public static extern int disable_dynamic_objects();
# public static extern void enable_wait_for_small_forces();
# public static extern void disable_wait_for_small_forces();
# public static extern void enable_rise_forces();
# public static extern void disable_rise_forces();

end
