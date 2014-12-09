using System.Collections.Generic;

namespace SLDE.ShaderAnalyzer {

    public enum InstructionType {
        Arithmetic,
        Sample,
        FlowControl,
        Other
    }

    public enum IdentifierType {
        Function,
        Temporary,
        Input,
        Output,
        Resource,
        Constant,
        Value,
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
        public uint SourceLine { get; private set; }
        public uint IndentLevel { get; private set; }
        public uint CostRank { get; private set; }

        public Instruction(InstructionType type, string displayString, List<Identifier> identifiers = null,
            string sourceFilepath = null, uint sourceLine = 0, uint indentLevel = 0, uint costRank = 0) {
            Type = type;
            DisplayString = displayString;
            Identifiers = identifiers ?? new List<Identifier>();
            SourceFilepath = sourceFilepath;
            SourceLine = sourceLine;
            IndentLevel = indentLevel;
            CostRank = costRank;
        }
    }

    /// <summary>
    /// Part of an Instruction object.
    /// Points to a section of the display string of the Instruction that should show special
    /// behavior, like highlighting, popup description and other functionality.
    /// </summary>
    public class Identifier {
        public IdentifierType Type { get; private set; }
        public uint StartPos { get; private set; }
        public uint EndPos { get; private set; }
        public string Description { get; private set; }

        public Identifier(IdentifierType type, uint startPos, uint endPos, string description = null) {
            Type = type;
            StartPos = startPos;
            EndPos = endPos;
            Description = description;
        }
    }

    /// <summary>
    /// A general notification, either a Hint, Warning or Error
    /// </summary>
    public class Notification {
        public NotificationType Type { get; private set; }
        public uint ID { get; private set; }
        public string Message { get; private set; }

        public Notification(NotificationType type, uint id, string message) {
            Type = type;
            ID = id;
            Message = message;
        }
    }

    /// <summary>
    /// A notification referencing a section of the source code
    /// </summary>
    public class CodeNotification : Notification {
        public uint SourceLine { get; private set; }
        public uint StartPos { get; private set; }
        public uint EndPos { get; private set; }

        public CodeNotification(NotificationType type, uint id, string message, uint sourceLine,
            uint startPos = 0, uint endPos = 0) : base(type, id, message) {
            SourceLine = sourceLine;
            StartPos = startPos;
            EndPos = endPos;
        }
    }

    /// <summary>
    /// A notification about a section of the assembly.
    /// The HLSL compiler does not produce these, but it may be used manually to raise hints and warnings
    /// about expensive branching, looping, dynamic array indexing, etc...
    /// </summary>
    public class AssemblyNotification : Notification {
        public uint InstructionIndex { get; private set; }
        public uint StartPos { get; private set; }
        public uint EndPos { get; private set; }

        public AssemblyNotification(NotificationType type, uint id, string message, uint instructionIndex,
            uint startPos = 0, uint endPos = 0)
            : base(type, id, message) {
            InstructionIndex = instructionIndex;
            StartPos = startPos;
            EndPos = endPos;
        }
    }
}
