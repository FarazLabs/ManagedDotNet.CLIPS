cmake_minimum_required(VERSION 3.16)
project(clips C)

# CLIPS 7.0.x core (6.4 sources remain under CLIPSCLRWrapper for reference)
set(CLIPS_SRC_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../clipsrules-code-r982-branches-70x/core")
set(CLIPS_SHIM_DIR "${CMAKE_CURRENT_SOURCE_DIR}/shim")

file(GLOB CLIPS_C_SOURCES "${CLIPS_SRC_DIR}/*.c")
# Shared library does not need the console entry point
list(FILTER CLIPS_C_SOURCES EXCLUDE REGEX ".*/main\\.c$")
list(APPEND CLIPS_C_SOURCES "${CLIPS_SHIM_DIR}/clips_pinvoke.c")

add_library(clips SHARED ${CLIPS_C_SOURCES})

target_include_directories(clips
  PUBLIC
    "${CLIPS_SHIM_DIR}"
    "${CLIPS_SRC_DIR}"
)

target_compile_definitions(clips PRIVATE
  CLIPS_BUILDING
  ALLOW_ENVIRONMENT_GLOBALS=0
  DEFFUNCTION_CONSTRUCT=1
  OBJECT_SYSTEM=1
  DEFTABLE_CONSTRUCT=1
  BLOAD_AND_BSAVE=1
  BLOAD_INSTANCES=1
  BSAVE_INSTANCES=1
  RUN_TIME=0
  BLOAD_ONLY=0
)

if(WIN32)
  target_compile_definitions(clips PRIVATE WIN_MVC=1)
  set_target_properties(clips PROPERTIES OUTPUT_NAME "clips")
else()
  target_compile_definitions(clips PRIVATE LINUX=1)
  set_target_properties(clips PROPERTIES OUTPUT_NAME "clips")
endif()

if(MSVC)
  target_compile_options(clips PRIVATE /W3)
else()
  target_compile_options(clips PRIVATE -Wall -Wno-unused-parameter)
endif()

# Stage RID-specific outputs for the managed project / NuGet pack
if(WIN32)
  set(_CLIPS_RID_DIR "${CMAKE_BINARY_DIR}/native/win-x64")
  set(_CLIPS_OUT_NAME "clips.dll")
else()
  set(_CLIPS_RID_DIR "${CMAKE_BINARY_DIR}/native/linux-x64")
  set(_CLIPS_OUT_NAME "libclips.so")
endif()

add_custom_command(TARGET clips POST_BUILD
  COMMAND ${CMAKE_COMMAND} -E make_directory "${_CLIPS_RID_DIR}"
  COMMAND ${CMAKE_COMMAND} -E copy_if_different
          "$<TARGET_FILE:clips>"
          "${_CLIPS_RID_DIR}/${_CLIPS_OUT_NAME}"
  COMMENT "Copy clips native library to ${_CLIPS_RID_DIR}"
)

install(TARGETS clips
  RUNTIME DESTINATION bin
  LIBRARY DESTINATION lib
  ARCHIVE DESTINATION lib
)
