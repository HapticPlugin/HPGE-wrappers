using Test

using Revise
push!(LOAD_PATH, ".")
using Haptics

Haptics.get_version_info()
@test Haptics.compatible()

Haptics.get_error_msg.(-100:100)
Haptics.get_error_msg.(-1:28)

Haptics.last_error_msg()

Haptics.tick()

Haptics.count_devices()

# test existing
for dev in -1:(Haptics.count_devices()-2)
    Haptics.get_device_name(dev)
end

# test missing
Haptics.get_device_name(10)

# test valid (mock device)
Haptics.initialize(-1)
# test it twice: error already init
Haptics.initialize(-1)

Haptics.deinitialize()
# Try it twice
Haptics.deinitialize()

# Start without init
Haptics.start()

# Start after init ok
Haptics.initialize(-1)
Haptics.start()
Haptics.deinitialize()
# Deinit twice fail again
Haptics.deinitialize()

Haptics.initialize(-1)
Haptics.start()
# start twice fails
Haptics.start()
Haptics.deinitialize()

# Initialize when running
Haptics.initialize(-1)
Haptics.start()
Haptics.initialize(-1)
Haptics.stop()
Haptics.initialize(-1)
Haptics.start()
Haptics.deinitialize() # found a crash 


# No loop frequency if thread is not initialized
@test Haptics.get_loop_frequency() < 0

# No loop frequency if thread is not running
Haptics.initialize(-1)
@test Haptics.get_loop_frequency() < 0
Haptics.deinitialize()

# Loop frequency if thread is running
Haptics.initialize(-1)
Haptics.start()
# let the thread start
sleep(1.0)
@test Haptics.get_loop_frequency() > 500
Haptics.deinitialize()

# Loop number -1 if not inited
@test Haptics.get_loops() == -1

# Loop number 0 if not running
Haptics.initialize(-1)
# @test Haptics.get_loops() == 0

# NOT WORKING!
# # Loop number strictly growing
# n = Haptics.get_loops()
# @show n
# Haptics.initialize(-1)
# Haptics.start()
# for i in 1:5
#     sleep(1.0)
#     n2 = Haptics.get_loops()
#     @test n2 > n
#     n = n2
# end
# Haptics.stop()
# Haptics.deinitialize()

Haptics.get_log_frame()
Haptics.deinitialize()

Haptics.initialize(-1)
Haptics.start()
sleep(1)
Haptics.get_log_frame()
Haptics.stop()
Haptics.deinitialize()

Haptics.is_initialized()
Haptics.is_running()

# Mock device, can't be pressed
@test all(Haptics.is_tool_button_pressed.(-100:100)) == false


Haptics.initialize(-1)
Haptics.start()
Haptics.is_initialized()
Haptics.is_running()

# Not logging
Haptics.get_log_frame()

Haptics.get_loops()

Haptics.stop()
Haptics.deinitialize()


## those tests require a real device!
@test Haptics.count_devices() == 2


@test Haptics.initialize(0) == 0
@test Haptics.start() == 0
@test Haptics.stop() == 0
@test Haptics.deinitialize() == 0

#=
init
- count
- get device <- fails
=#

for i in 1:10
    @show i
    @test Haptics.count_devices() == 2
    @test Haptics.initialize(0) == 0
    @test Haptics.start() == 0
    @test Haptics.stop() == 0
    @test Haptics.deinitialize() == 0
    @info "Before: $(Haptics.count_devices())"
    @test Haptics.initialize(-1) == 0 ## this is required. Why?
    sleep(1)
    @test Haptics.deinitialize() == 0
    @info "After: $(Haptics.count_devices())"
end

for i in 1:100
    @test Haptics.initialize(-1) == 0
    @test Haptics.deinitialize() == 0
    @test Haptics.count_devices() == 2
end

for i in 1:100
    @show i
    @test Haptics.initialize(0) == 0
    @test Haptics.deinitialize() == 0
    @test Haptics.count_devices() == 2
end
