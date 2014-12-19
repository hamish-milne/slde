using System.Collections.Generic;
using SLDE.ShaderAnalyzer.HLSL;

namespace SLDE.ShaderAnalyzer {

    public enum InstructionType {
        Arithmetic,
        Sample,
        FlowControl,
        Other,
        Ignore,
        Invalid
    }

    public enum IdentifierType {
        Function,
        Temporary,
        Input,
        Output,
        Resource,
        Label,
        Constant,
        Literal,
        Other
    }

    public enum NotificationType {
        Hint,
        Warning,
        Error
    }

    /// <summary>
    /// Describes a single instruction in the assembly.
    /// </summary>
    public class Instruction {
        public InstructionType Type { get; private set; }
        public string DisplayString { get; private set; }
        public IEnumerable<Identifier> Identifiers { get; private set; }
        public string SourceFilepath { get; private set; }
        public int SourceLine { get; private set; }

        public Instruction(InstructionType type, string displayString, List<Identifier> identifiers = null,
            string sourceFilepath = null, int sourceLine = 0) {
            Type = type;
            DisplayString = displayString;
            Identifiers = identifiers ?? new List<Identifier>();
            SourceFilepath = sourceFilepath;
            SourceLine = sourceLine;
        }
    }

    /// <summary>
    /// Part of an Instruction object.
    /// Points to a section of the display string of the Instruction that should show special
    /// behavior, like highlighting, popup description and other functionality.
    /// </summary>
    public class Identifier {
        public IdentifierType Type { get; private set; }
        public int Offset { get; private set; }
        public int Length { get; private set; }

        public Identifier(IdentifierType type, int offset, int length) {
            Type = type;
            Offset = offset;
            Length = length;
        }
    }

    /// <summary>
    /// A general notification, either a Hint, Warning or Error
    /// </summary>
    public class Notification {
        public NotificationType Type { get; private set; }
        public string Message { get; private set; }

        public Notification(NotificationType type, string message) {
            Type = type;
            Message = message;
        }
    }

    /// <summary>
    /// A notification referencing a section of the source code
    /// </summary>
    public class CodeNotification : Notification {
        public int SourceLine { get; private set; }
        public int Offset { get; private set; }
        public int Length { get; private set; }

        public CodeNotification(NotificationType type, string message, int sourceLine,
            int offset = 0, int length = 0) : base(type, message) {
            SourceLine = sourceLine;
            Offset = offset;
            Length = length;
        }
    }

    /// <summary>
    /// A notification about a section of the assembly.
    /// The HLSL compiler does not produce these, but it may be used manually to raise hints and warnings
    /// about expensive branching, looping, dynamic array indexing, etc...
    /// </summary>
    public class AssemblyNotification : Notification {
        public int InstructionIndex { get; private set; }
        public int Offset { get; private set; }
        public int Length { get; private set; }

        public AssemblyNotification(NotificationType type, string message, int instructionIndex,
            int offset = 0, int length = -1) : base(type, message) {
            InstructionIndex = instructionIndex;
            Offset = offset;
            Length = length;
        }
    }
}
