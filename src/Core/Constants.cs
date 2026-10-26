using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedDotNet.CLIPS.Core;

internal static class Constants
{
    internal const string PRETTY_PRINT_INSTANCE_TO_ROUTER = "pretty-print-to-router";

    /// <summary>
    /// Message handler that pretty-prints an instance to a specified router.
    /// Example usage in CLIPS:
    /// (send [gen1] pretty-print-to-router t)
    /// </summary>
    internal const string PRETTY_PRINT_INSTANCE_TO_ROUTER_DEFINITION = @"
        (defmessage-handler USER pretty-print-to-router (?router)
          (bind ?output """")
          
          ;; Get the class name of the instance
          (bind ?class-name (class ?self))
          
          ;; Get the instance name (convert to symbol for use in string)
          (bind ?inst-name (instance-name-to-symbol (instance-name ?self)))
          
          ;; Get all slots defined for this class (including inherited ones)
          (bind ?slots (class-slots ?class-name inherit))
          
          ;; Start building the string: ""gen1 of CAR ""
          (bind ?output (str-cat ?inst-name "" of "" ?class-name "" ""))
          
          ;; Iterate over each slot name and append ""(slotName slotValue)""
          (foreach ?slot ?slots
            (bind ?value (send ?self (sym-cat get- ?slot)))
            (bind ?output (str-cat ?output ""("" ?slot "" "" ?value "") "")))
          
          ;; Print to the specified router
          (printout ?router ?output crlf)
        )";




    internal const string GET_INSTANCE_LIST = "get-instance-list";

    /// <summary>
    /// Function that lists all instances whose classes belong to the specified module.
    /// </summary>
    internal const string GET_INSTANCE_LIST_DEFINITION = @"
        (deffunction get-list-instances (?module-name)
            (return (find-all-instances
            ((?inst OBJECT))
            (eq (defclass-module (class ?inst)) ?module-name)
            ))
        )";
}
