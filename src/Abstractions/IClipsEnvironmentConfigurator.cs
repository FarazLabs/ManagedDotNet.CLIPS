using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedDotNet.Clips.Api;
using ManagedDotNet.Clips.Core;


namespace ManagedDotNet.Clips.Abstractions
{
    /// <summary>
    /// Responsible for configuring CLIPS environments.
    /// The environment returned is reset and only then is ready to be used and can be safely inserted into a pool.
    /// </summary>
    public interface IClipsEnvironmentConfigurator
    {
        /// <summary>
        /// configures the pre-initialized Managed CLIPS environment.
        /// The environment is automatically reset before and after each use to ensure a clean state.<br />
        /// You can load rules files, define functions, and configure other environment settings here.
        /// </summary>
        void Configure(IClipsEnvironment env);
    }
}
