#pragma once

#include <stddef.h>
#include <stdint.h>

#ifdef _WIN32
  #ifdef CLIPS_BUILDING
    #define CLIPS_API __declspec(dllexport)
  #else
    #define CLIPS_API __declspec(dllimport)
  #endif
#else
  #define CLIPS_API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* Mirrors constant.h primitive type tags */
enum ClipsPInvokeType
{
  CLIPS_PINVOKE_FLOAT = 0,
  CLIPS_PINVOKE_INTEGER = 1,
  CLIPS_PINVOKE_SYMBOL = 2,
  CLIPS_PINVOKE_STRING = 3,
  CLIPS_PINVOKE_MULTIFIELD = 4,
  CLIPS_PINVOKE_EXTERNAL_ADDRESS = 5,
  CLIPS_PINVOKE_FACT_ADDRESS = 6,
  CLIPS_PINVOKE_INSTANCE_ADDRESS = 7,
  CLIPS_PINVOKE_INSTANCE_NAME = 8,
  CLIPS_PINVOKE_VOID = 9,
  CLIPS_PINVOKE_UNKNOWN = 255
};

typedef struct ClipsPInvokeValue ClipsPInvokeValue;

struct ClipsPInvokeValue
{
  int32_t type;
  int32_t reserved;
  int64_t integerValue;
  double floatValue;
  char *stringValue;          /* owned; free via Clips_FreeValue */
  int64_t factIndex;
  int32_t multifieldLength;
  int32_t reserved2;
  ClipsPInvokeValue *multifieldItems; /* owned; free via Clips_FreeValue */
};

CLIPS_API void *Clips_CreateEnvironment(void);
CLIPS_API int Clips_DestroyEnvironment(void *env);

CLIPS_API int Clips_Eval(void *env, const char *expression, ClipsPInvokeValue *outValue);
CLIPS_API void Clips_FreeValue(ClipsPInvokeValue *value);

CLIPS_API int Clips_Build(void *env, const char *construct);
CLIPS_API int Clips_Load(void *env, const char *filePath);
CLIPS_API void Clips_Reset(void *env);
CLIPS_API long long Clips_Run(void *env, long long limit);
CLIPS_API int Clips_AssertString(void *env, const char *fact);

CLIPS_API int Clips_GetModuleNames(void *env, char ***namesOut, int *countOut);
CLIPS_API int Clips_GetFactPPForms(void *env, const char *moduleName, char ***formsOut, int *countOut);
CLIPS_API int Clips_GetInstancePPForms(void *env, const char *moduleName, char ***formsOut, int *countOut);
CLIPS_API void Clips_FreeStringArray(char **items, int count);

#ifdef __cplusplus
}
#endif
