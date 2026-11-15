cmake_minimum_required(VERSION 3.24)
project(NovoDwarfRml LANGUAGES C CXX)
include(FetchContent)
set(BUILD_SHARED_LIBS OFF CACHE BOOL "" FORCE)
set(FT_DISABLE_ZLIB ON CACHE BOOL "" FORCE)
set(FT_DISABLE_BZIP2 ON CACHE BOOL "" FORCE)
set(FT_DISABLE_PNG ON CACHE BOOL "" FORCE)
set(FT_DISABLE_HARFBUZZ ON CACHE BOOL "" FORCE)
set(FT_DISABLE_BROTLI ON CACHE BOOL "" FORCE)
FetchContent_Declare(freetype
        URL https://codeload.github.com/freetype/freetype/zip/refs/tags/VER-2-13-3
        URL_HASH SHA256=A7165CB405DFCE349AE64D6F59EFB2567565CA8EDBACBEFF2A8D0E4CFA779CF5)
FetchContent_MakeAvailable(freetype)
add_library(Freetype::Freetype ALIAS freetype)
set(FREETYPE_LIBRARY freetype CACHE STRING "" FORCE)
set(FREETYPE_INCLUDE_DIRS "${freetype_SOURCE_DIR}/include" CACHE STRING "" FORCE)
set(RMLUI_SAMPLES OFF CACHE BOOL "" FORCE)
set(RMLUI_LUA_BINDINGS OFF CACHE BOOL "" FORCE)
set(RMLUI_PRECOMPILED_HEADERS ON CACHE BOOL "" FORCE)
FetchContent_Declare(rmlui
        URL https://codeload.github.com/mikke89/RmlUi/zip/refs/tags/6.3
        URL_HASH SHA256=BFB340660E4BCE38D4211CFFC836CFBE1BE676AA36EF9CC48E7DF3855EE74FE3)
FetchContent_MakeAvailable(rmlui)
add_library(NovoDwarf.Rml.Native SHARED Bridge.cpp)
target_compile_features(NovoDwarf.Rml.Native PRIVATE cxx_std_17)
target_link_libraries(NovoDwarf.Rml.Native PRIVATE RmlUi::Core)
set_target_properties(NovoDwarf.Rml.Native PROPERTIES PREFIX "")

if (MINGW)
    target_link_options(NovoDwarf.Rml.Native PRIVATE -static)
endif ()




