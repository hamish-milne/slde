

using System.Collections;
using System.Collections.Generic;

namespace SLDE.ShaderAnalyzer {
    public interface IAssembly {

        /// <summary>
        /// Returns if the compiler was able to produce an assembly.
        /// The returned value of these functions:
        ///     GetRawCompilerOutput()
        ///     GetInstructions()
        ///     GetCodeNotifications()
        ///     GetAssemblyNotifucations()
        /// is empty if CompiledSuccessfully() returns false
        /// </summary>
        bool CompiledSuccessfully();

        string GetRawCompilerOutput();

        string GetRawCompilerErrors();

        IEnumerable<Instruction> GetInstructions();

        IEnumerable<Notification> GetNotifications();

        IEnumerable<CodeNotification> GetCodeNotifications();

        IEnumerable<AssemblyNotification> GetAssemblyNotifications();

    }
}
