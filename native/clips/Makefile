# Build libclips.so (Linux) without CMake.
# Usage: make -C native/clips

CLIPS_SRC := ../CLIPSCLRWrapper/Source/CLIPS
SHIM_SRC  := shim
OUT_DIR   := build/native/linux-x64
TARGET    := $(OUT_DIR)/libclips.so

CFLAGS := -O2 -fPIC -shared -Wall -Wno-unused-parameter \
	-I$(CLIPS_SRC) -I$(SHIM_SRC) \
	-DLINUX=1 \
	-DCLIPS_BUILDING \
	-DALLOW_ENVIRONMENT_GLOBALS=0 \
	-DDEFFUNCTION_CONSTRUCT=1 \
	-DOBJECT_SYSTEM=1 \
	-DBLOAD_AND_BSAVE=1 \
	-DBLOAD_INSTANCES=1 \
	-DBSAVE_INSTANCES=1 \
	-DRUN_TIME=0 \
	-DBLOAD_ONLY=0

SOURCES := $(wildcard $(CLIPS_SRC)/*.c) $(SHIM_SRC)/clips_pinvoke.c

.PHONY: all clean

all: $(TARGET)

$(TARGET): $(SOURCES) | $(OUT_DIR)
	$(CC) $(CFLAGS) -o $@ $(SOURCES)

$(OUT_DIR):
	mkdir -p $@

clean:
	rm -rf build
