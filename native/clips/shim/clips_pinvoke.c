#include "clips_pinvoke.h"

#include "clips.h"

#include <stdlib.h>
#include <string.h>

static char *Clips_StrDup(const char *s)
{
  size_t n;
  char *copy;

  if (s == NULL)
    {
     return NULL;
    }

  n = strlen(s);
  copy = (char *) malloc(n + 1);
  if (copy == NULL)
    {
     return NULL;
    }

  memcpy(copy, s, n + 1);
  return copy;
}

static void Clips_ClearValue(ClipsPInvokeValue *value)
{
  if (value == NULL)
    {
     return;
    }

  memset(value, 0, sizeof(*value));
  value->type = CLIPS_PINVOKE_VOID;
}

/* Match save-facts / save-instances: instance addresses as [name], escaped strings. */
typedef struct ClipsSavePrintFlags
{
  bool preserveEscapedCharacters;
  bool addressesToStrings;
  bool instanceAddressesToNames;
} ClipsSavePrintFlags;

static ClipsSavePrintFlags Clips_BeginSaveStylePrint(Environment *theEnv)
{
  ClipsSavePrintFlags previous;

  previous.preserveEscapedCharacters = PrintUtilityData(theEnv)->PreserveEscapedCharacters;
  previous.addressesToStrings = PrintUtilityData(theEnv)->AddressesToStrings;
  previous.instanceAddressesToNames = PrintUtilityData(theEnv)->InstanceAddressesToNames;

  PrintUtilityData(theEnv)->PreserveEscapedCharacters = true;
  PrintUtilityData(theEnv)->AddressesToStrings = true;
  PrintUtilityData(theEnv)->InstanceAddressesToNames = true;

  return previous;
}

static void Clips_EndSaveStylePrint(Environment *theEnv, ClipsSavePrintFlags previous)
{
  PrintUtilityData(theEnv)->PreserveEscapedCharacters = previous.preserveEscapedCharacters;
  PrintUtilityData(theEnv)->AddressesToStrings = previous.addressesToStrings;
  PrintUtilityData(theEnv)->InstanceAddressesToNames = previous.instanceAddressesToNames;
}

static int Clips_FillFromCLIPSValue(Environment *theEnv, CLIPSValue *cv, ClipsPInvokeValue *out)
{
  unsigned short type;
  size_t i;

  Clips_ClearValue(out);

  if (cv == NULL || cv->header == NULL)
    {
     out->type = CLIPS_PINVOKE_VOID;
     return 1;
    }

  type = cv->header->type;
  out->type = type;

  switch (type)
    {
     case FLOAT_TYPE:
       out->floatValue = cv->floatValue->contents;
       break;

     case INTEGER_TYPE:
       out->integerValue = cv->integerValue->contents;
       break;

     case SYMBOL_TYPE:
     case STRING_TYPE:
     case INSTANCE_NAME_TYPE:
       out->stringValue = Clips_StrDup(cv->lexemeValue->contents);
       break;

     case MULTIFIELD_TYPE:
       out->multifieldLength = (int) cv->multifieldValue->length;
       if (out->multifieldLength > 0)
         {
          out->multifieldItems = (ClipsPInvokeValue *) calloc((size_t) out->multifieldLength, sizeof(ClipsPInvokeValue));
          if (out->multifieldItems == NULL)
            {
             return 0;
            }

          for (i = 0; i < cv->multifieldValue->length; i++)
            {
             if (! Clips_FillFromCLIPSValue(theEnv, &cv->multifieldValue->contents[i], &out->multifieldItems[i]))
               {
                return 0;
               }
            }
         }
       break;

     case FACT_ADDRESS_TYPE:
       out->factIndex = FactIndex(cv->factValue);
       break;

     case INSTANCE_ADDRESS_TYPE:
       /* Instance pretty name via InstanceName if available; store empty string for now */
       out->stringValue = Clips_StrDup("");
       break;

     case EXTERNAL_ADDRESS_TYPE:
       out->type = CLIPS_PINVOKE_EXTERNAL_ADDRESS;
       break;

     case VOID_TYPE:
       out->type = CLIPS_PINVOKE_VOID;
       break;

     default:
       out->type = CLIPS_PINVOKE_UNKNOWN;
       break;
    }

  return 1;
}

void *Clips_CreateEnvironment(void)
{
  return (void *) CreateEnvironment();
}

int Clips_DestroyEnvironment(void *env)
{
  if (env == NULL)
    {
     return 0;
    }

  return DestroyEnvironment((Environment *) env) ? 1 : 0;
}

int Clips_Eval(void *env, const char *expression, ClipsPInvokeValue *outValue)
{
  Environment *theEnv;
  CLIPSValue cv;
  EvalError err;

  if (env == NULL || expression == NULL || outValue == NULL)
    {
     return 0;
    }

  theEnv = (Environment *) env;
  Clips_ClearValue(outValue);

  err = Eval(theEnv, expression, &cv);
  if (err != EE_NO_ERROR)
    {
     return 0;
    }

  if (! Clips_FillFromCLIPSValue(theEnv, &cv, outValue))
    {
     Clips_FreeValue(outValue);
     return 0;
    }

  return 1;
}

void Clips_FreeValue(ClipsPInvokeValue *value)
{
  int i;

  if (value == NULL)
    {
     return;
    }

  if (value->stringValue != NULL)
    {
     free(value->stringValue);
     value->stringValue = NULL;
    }

  if (value->multifieldItems != NULL)
    {
     for (i = 0; i < value->multifieldLength; i++)
       {
        Clips_FreeValue(&value->multifieldItems[i]);
       }
     free(value->multifieldItems);
     value->multifieldItems = NULL;
    }

  value->multifieldLength = 0;
}

int Clips_Build(void *env, const char *construct)
{
  if (env == NULL || construct == NULL)
    {
     return 0;
    }

  return Build((Environment *) env, construct) == BE_NO_ERROR ? 1 : 0;
}

int Clips_Load(void *env, const char *filePath)
{
  if (env == NULL || filePath == NULL)
    {
     return 0;
    }

  return Load((Environment *) env, filePath) == LE_NO_ERROR ? 1 : 0;
}

void Clips_Reset(void *env)
{
  if (env != NULL)
    {
     Reset((Environment *) env);
    }
}

long long Clips_Run(void *env, long long limit)
{
  if (env == NULL)
    {
     return 0;
    }

  return Run((Environment *) env, limit);
}

int Clips_AssertString(void *env, const char *fact)
{
  if (env == NULL || fact == NULL)
    {
     return 0;
    }

  return AssertString((Environment *) env, fact) != NULL ? 1 : 0;
}

int Clips_GetModuleNames(void *env, char ***namesOut, int *countOut)
{
  Environment *theEnv;
  Defmodule *module;
  int count = 0;
  int i = 0;
  char **names;

  if (env == NULL || namesOut == NULL || countOut == NULL)
    {
     return 0;
    }

  theEnv = (Environment *) env;
  *namesOut = NULL;
  *countOut = 0;

  for (module = GetNextDefmodule(theEnv, NULL); module != NULL; module = GetNextDefmodule(theEnv, module))
    {
     count++;
    }

  if (count == 0)
    {
     return 1;
    }

  names = (char **) calloc((size_t) count, sizeof(char *));
  if (names == NULL)
    {
     return 0;
    }

  for (module = GetNextDefmodule(theEnv, NULL); module != NULL; module = GetNextDefmodule(theEnv, module))
    {
     names[i++] = Clips_StrDup(DefmoduleName(module));
    }

  *namesOut = names;
  *countOut = count;
  return 1;
}

int Clips_GetFactPPForms(void *env, const char *moduleName, char ***formsOut, int *countOut)
{
  Environment *theEnv;
  Defmodule *module;
  Defmodule *previous;
  Fact *fact;
  int count = 0;
  int i = 0;
  char **forms;
  StringBuilder *sb;

  if (env == NULL || moduleName == NULL || formsOut == NULL || countOut == NULL)
    {
     return 0;
    }

  theEnv = (Environment *) env;
  *formsOut = NULL;
  *countOut = 0;

  module = FindDefmodule(theEnv, moduleName);
  if (module == NULL)
    {
     return 0;
    }

  previous = SetCurrentModule(theEnv, module);

  for (fact = GetNextFactInScope(theEnv, NULL); fact != NULL; fact = GetNextFactInScope(theEnv, fact))
    {
     count++;
    }

  if (count == 0)
    {
     SetCurrentModule(theEnv, previous);
     return 1;
    }

  forms = (char **) calloc((size_t) count, sizeof(char *));
  if (forms == NULL)
    {
     SetCurrentModule(theEnv, previous);
     return 0;
    }

  sb = CreateStringBuilder(theEnv, 256);
  if (sb == NULL)
    {
     free(forms);
     SetCurrentModule(theEnv, previous);
     return 0;
    }

  {
   ClipsSavePrintFlags printFlags = Clips_BeginSaveStylePrint(theEnv);

   for (fact = GetNextFactInScope(theEnv, NULL); fact != NULL; fact = GetNextFactInScope(theEnv, fact))
     {
      SBReset(sb);
      FactPPForm(fact, sb, false);
      forms[i++] = Clips_StrDup(sb->contents);
     }

   Clips_EndSaveStylePrint(theEnv, printFlags);
  }

  SBDispose(sb);
  SetCurrentModule(theEnv, previous);

  *formsOut = forms;
  *countOut = count;
  return 1;
}

int Clips_GetInstancePPForms(void *env, const char *moduleName, char ***formsOut, int *countOut)
{
  Environment *theEnv;
  Defmodule *module;
  Defmodule *previous;
  Instance *instance;
  int count = 0;
  int i = 0;
  char **forms;
  StringBuilder *sb;

  if (env == NULL || moduleName == NULL || formsOut == NULL || countOut == NULL)
    {
     return 0;
    }

  theEnv = (Environment *) env;
  *formsOut = NULL;
  *countOut = 0;

  module = FindDefmodule(theEnv, moduleName);
  if (module == NULL)
    {
     return 0;
    }

  previous = SetCurrentModule(theEnv, module);

  for (instance = GetNextInstanceInScope(theEnv, NULL); instance != NULL; instance = GetNextInstanceInScope(theEnv, instance))
    {
     count++;
    }

  if (count == 0)
    {
     SetCurrentModule(theEnv, previous);
     return 1;
    }

  forms = (char **) calloc((size_t) count, sizeof(char *));
  if (forms == NULL)
    {
     SetCurrentModule(theEnv, previous);
     return 0;
    }

  sb = CreateStringBuilder(theEnv, 256);
  if (sb == NULL)
    {
     free(forms);
     SetCurrentModule(theEnv, previous);
     return 0;
    }

  {
   ClipsSavePrintFlags printFlags = Clips_BeginSaveStylePrint(theEnv);

   for (instance = GetNextInstanceInScope(theEnv, NULL); instance != NULL; instance = GetNextInstanceInScope(theEnv, instance))
     {
      SBReset(sb);
      InstancePPForm(instance, sb);
      forms[i++] = Clips_StrDup(sb->contents);
     }

   Clips_EndSaveStylePrint(theEnv, printFlags);
  }

  SBDispose(sb);
  SetCurrentModule(theEnv, previous);

  *formsOut = forms;
  *countOut = count;
  return 1;
}

void Clips_FreeStringArray(char **items, int count)
{
  int i;

  if (items == NULL)
    {
     return;
    }

  for (i = 0; i < count; i++)
    {
     free(items[i]);
    }

  free(items);
}
