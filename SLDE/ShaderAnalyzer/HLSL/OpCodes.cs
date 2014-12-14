using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SLDE.ShaderAnalyzer.HLSL {

    abstract class OpCodes {
        private static Regex opNameFilter = new Regex(@"^[a-zA-Z_]+", RegexOptions.Compiled);

        public static OpCodesDX9 DX9 = new OpCodesDX9();
        public static OpCodesDX11 DX11 = new OpCodesDX11();

        protected Dictionary<string, OpCode> opDict;

        public string LookupDescription(string id) {
            return opDict[id].Description;
        }

        /// <summary>
        /// Looks up the operation stored as the first token and formats the instruction into
        /// a more readable format. If the type of the instruction is Ignore, it is not formatted
        /// and null is returned.
        /// </summary>
        /// <exception cref="ArgumentException">Invalid operation</exception>
        /// <exception cref="KeyNotFoundException">Operation is not defined</exception>
        /// <exception cref="FormatException">Invalid amount of arguments supplied</exception>
        public string FormatInstruction(string[] tokens, out IOperation iop) {
            if (tokens == null || tokens.Length == 0) {
                throw new ArgumentException("At least one token needs to be supplied.");
            }
            Match match = opNameFilter.Match(tokens[0]);
            if (!match.Success) {
                throw new ArgumentException("Invalid instruction token. Might be a predicate, which are not supported.");
            }
            string id = match.Groups[0].Value;

            OpCode opCode = opDict[id];
            iop = (IOperation) opCode;

            if (opCode.Type != InstructionType.Ignore) {
                return opCode.formatArguments(tokens, 1);
            } else {
                return null;
            }
        }

        protected void AddOpCode(string id, InstructionType type, int cost, int returns, string format,
            string altFormat = null, bool indents = false, bool dedents = false, string description = null) {

            opDict.Add(id, new OpCode(id, type, cost, returns, format, altFormat, indents, dedents, description));

            if (type == InstructionType.Arithmetic && returns == 1) {
                // The operation is probably saturable...
                format = "sat(" + format + ")";
                if (altFormat != null) {
                    altFormat = "sat(" + format + ")";
                }
                opDict.Add(id + "_sat", new OpCode(id, type, cost, returns, format, altFormat, indents, dedents, description));
            }
        }

        protected void AddOpCode(string id, InstructionType type, int cost, int returns, int arguments,
            int altArguments = -1, bool indents = false, bool dedents = false, string description = null) {
            
            string format = constructDefaultFormatString(id, arguments);
            string altFormat = constructDefaultFormatString(id, altArguments);

            AddOpCode(id, type, cost, returns, format, altFormat, indents, dedents, description);
        }

        private string constructDefaultFormatString(string id, int arguments) {
            if (arguments > 0) {
                string[] args = new string[arguments];
                for (int i = 0; i < arguments; i++) {
                    args[i] = "{" + i + "}";
                }
                return id + "(" + String.Join(", ", args) + ")";
            } else if (arguments == 0) {
                return id;
            } else {
                return null;
            }
        }

        public interface IOperation {
            string ID { get; }
            InstructionType Type { get; }
            int Cost { get; }
            bool Indents { get; }
            bool Dedents { get; }
        }

        protected class OpCode : IOperation {
            public string ID { get; private set; }
            public InstructionType Type { get; private set; }
            public int Cost { get; private set; }
            public bool Indents { get; private set; }
            public bool Dedents { get; private set; }
            public string Description { get; private set; }

            private int returns;
            private string format;
            private string altFormat;

            public OpCode(string id, InstructionType type, int cost, int returns, string format,
                string altFormat = null, bool indents = false, bool dedents = false, string description = null) {
                ID = id;
                Type = type;
                Cost = cost;
                Indents = indents;
                Dedents = dedents;
                Description = description;

                this.returns = returns;
                string sep = (returns > 0) ? "\t= " : "\t  ";
                this.format = sep + format;
                this.altFormat = (altFormat == null) ? null : sep + altFormat;
            }

            public string formatArguments(string[] tokens, int startAt = 0) {
                string returnString = String.Join(", ", tokens, startAt, returns);
                startAt += returns;

                object[] args = new object[tokens.Length - startAt];
                Array.Copy(tokens, startAt, args, 0, args.Length);

                string bodyString;
                try {
                    bodyString = String.Format(format, args);
                } catch (FormatException) {
                    if (altFormat != null) {
                        bodyString = String.Format(altFormat, args);
                    } else {
                        throw;
                    }
                }

                return returnString + bodyString;
            }
        }

        public class OpCodesDX9 : OpCodes {
            public OpCodesDX9() {
                opDict = new Dictionary<string, OpCode>();

                //                        -----  Arithmetic Instructions -----
                //            id                     type           cost  ret  format/args    
                AddOpCode("abs",         InstructionType.Arithmetic,   1,   1, 1,
                    description: "<b>abs</b>: Computes absolute value.");
                AddOpCode("add",         InstructionType.Arithmetic,   1,   1, "{0} + {1}");
                AddOpCode("cmp",         InstructionType.Arithmetic,   1,   1, "({0} >= 0) ? {1} : {2}");
                AddOpCode("crs",         InstructionType.Arithmetic,   2,   1, 2,
                    description: "<b>cross</b>: Computes a cross product using the right-hand rule.");
                AddOpCode("dp2",         InstructionType.Arithmetic,   1,   1, 2,
                    description: "<b>dot</b>: Computes the two-component dot product of the source registers.");
                AddOpCode("dp2add",      InstructionType.Arithmetic,   2,   1, "dp2({0}, {1}) + {2}");
                AddOpCode("dp3",         InstructionType.Arithmetic,   1,   1, 2,
                    description: "<b>dot</b>: Computes the three-component dot product of the source registers.");
                AddOpCode("dp4",         InstructionType.Arithmetic,   1,   1, 2,
                    description: "<b>dot</b>: Computes the four-component dot product of the source registers.");
                AddOpCode("dsx",         InstructionType.Arithmetic,   2,   1, 1,
                    description: "<b>ddx</b>: Compute the rate of change of the register in the render target's x-direction.");
                AddOpCode("dsy",         InstructionType.Arithmetic,   2,   1, 1,
                    description: "<b>ddy</b>: Compute the rate of change of the register in the render target's y-direction.");
                AddOpCode("exp",         InstructionType.Arithmetic,   1,   1, 1,
                    description: "<b>exp</b>:Provides full precision exponential 2^x.");
                AddOpCode("frc",         InstructionType.Arithmetic,   1,   1, 1,
                    description: "<b>frac</b>: Returns the fractional portion of each input component.");
                AddOpCode("log",         InstructionType.Arithmetic,   1,   1, 1,
                    description: "<b>log</b>: Provides full precision base 2 logarithm.");
                AddOpCode("lrp",         InstructionType.Arithmetic,   2,   1, 3,
                    description: "<b>lerp</b>: Interpolates linearly between the second and third source registers " +
                    "by a proportion specified in the first source register.");
                AddOpCode("m3x2",        InstructionType.Arithmetic,   2,   1, 2,
                    description: "<b>mul</b>: Multiplies a 3-component vector by a 3x2 matrix starting at the specified register.");
                AddOpCode("m3x3",        InstructionType.Arithmetic,   3,   1, 2,
                    description: "<b>mul</b>: Multiplies a 3-component vector by a 3x3 matrix starting at the specified register.");
                AddOpCode("m3x4",        InstructionType.Arithmetic,   4,   1, 2,
                    description: "<b>mul</b>: Multiplies a 3-component vector by a 3x4 matrix starting at the specified register.");
                AddOpCode("m4x3",        InstructionType.Arithmetic,   3,   1, 2,
                    description: "<b>mul</b>: Multiplies a 4-component vector by a 4x3 matrix starting at the specified register.");
                AddOpCode("m4x4",        InstructionType.Arithmetic,   4,   1, 2,
                    description: "<b>mul</b>: Multiplies a 4-component vector by a 4x4 matrix starting at the specified register.");
                AddOpCode("mad",         InstructionType.Arithmetic,   1,   1, "{0} * {1} + {2}");
                AddOpCode("max",         InstructionType.Arithmetic,   1,   1, 2,
                    description: "<b>max</b>: Calculates the component-wise maximum of the sources.");
                AddOpCode("min",         InstructionType.Arithmetic,   1,   1, 2,
                    description: "<b>min</b>: Calculates the component-wise minimum of the sources.");
                AddOpCode("mov",         InstructionType.Arithmetic,   1,   1, 1,
                    description: "Move data between registers.");
                AddOpCode("mul",         InstructionType.Arithmetic,   1,   1, "{0} * {1}");
                AddOpCode("nrm",         InstructionType.Arithmetic,   3,   1, 1,
                    description: "<b>normalize</b>: Normalize a 3D vector.");
                AddOpCode("pow",         InstructionType.Arithmetic,   3,   1, 2,
                    description: "<b>pow</b>: Full precision abs(x)^y.");
                AddOpCode("rcp",         InstructionType.Arithmetic,   1,   1, "1.0 / {0}");
                AddOpCode("rsq",         InstructionType.Arithmetic,   1,   1, 1,
                    description: "<b>rsqrt</b>: Computes the reciprocal square root (positive only) of the source scalar.");
                AddOpCode("sincos",      InstructionType.Arithmetic,   8,   1, 1, altArguments: 3,
                    description: "<b>sincos</b>: Computes and outputs the cosine in .x and sine in the .y component, in radians.");


                //                       -----  Flow Control Instructions -----
                //            id                     type            cost  ret  format/args  
                AddOpCode("break",       InstructionType.FlowControl,   1,   0, 0,
                    description: "<b>break</b>: Break out of the current loop at the nearest <b>endloop</b> or <b>endrep</b>.");
                AddOpCode("break_gt",    InstructionType.FlowControl,   3,   0, "if ({0} > {1}) break");
                AddOpCode("break_lt",    InstructionType.FlowControl,   3,   0, "if ({0} < {1}) break");
                AddOpCode("break_ge",    InstructionType.FlowControl,   3,   0, "if ({0} >= {1}) break");
                AddOpCode("break_le",    InstructionType.FlowControl,   3,   0, "if ({0} <= {1}) break");
                AddOpCode("break_eq",    InstructionType.FlowControl,   3,   0, "if ({0} == {1}) break");
                AddOpCode("break_ne",    InstructionType.FlowControl,   3,   0, "if ({0} != {1}) break");
                AddOpCode("breakp",      InstructionType.FlowControl,   3,   0, 1,
                    description: "Break out of the current loop at the nearest <b>endloop</b> or <b>endrep</b>.\n" +
                    "Uses one of the components of the predicate register as a condition to determine whether or not " +
                    "to perform the instruction.");
                AddOpCode("call",        InstructionType.FlowControl,   2,   0, 1,
                    description: "Calls a subroutine marked by where the <b>label</b> l# appears in the program.");
                AddOpCode("callnz",      InstructionType.FlowControl,   3,   0, "if ({1}) call({0})");
                AddOpCode("else",        InstructionType.FlowControl,   1,   0, 0, indents: true, dedents: true);
                AddOpCode("endif",       InstructionType.FlowControl,   1,   0, 0, dedents: true);
                AddOpCode("endloop",     InstructionType.FlowControl,   2,   0, 0, dedents: true);
                AddOpCode("endrep",      InstructionType.FlowControl,   2,   0, 0, dedents: true);
                AddOpCode("if",          InstructionType.FlowControl,   3,   0, "if ({0})", indents: true);
                AddOpCode("if_gt",       InstructionType.FlowControl,   3,   0, "if ({0} > {1})", indents: true);
                AddOpCode("if_lt",       InstructionType.FlowControl,   3,   0, "if ({0} < {1})", indents: true);
                AddOpCode("if_ge",       InstructionType.FlowControl,   3,   0, "if ({0} >= {1})", indents: true);
                AddOpCode("if_le",       InstructionType.FlowControl,   3,   0, "if ({0} <= {1})", indents: true);
                AddOpCode("if_eq",       InstructionType.FlowControl,   3,   0, "if ({0} == {1})", indents: true);
                AddOpCode("if_ne",       InstructionType.FlowControl,   3,   0, "if ({0} != {1})", indents: true);
                AddOpCode("label",       InstructionType.FlowControl,   0,   0, "label {0}", indents: true,
                    description: "Indicates the beginning of a subroutine.");
                AddOpCode("loop",        InstructionType.FlowControl,   3,   0, 2, indents: true,
                    description: "Starts a <b>loop</b>...<b>endloop</b> block.");
                AddOpCode("rep",         InstructionType.FlowControl,   3,   0, 1, indents: true,
                    description: "Starts a <b>rep</b>...<b>endrep</b> block. Parameter specifies the repeat count " +
                    "in the .x component.");
                AddOpCode("ret",         InstructionType.FlowControl,   1,   0, 0,
                    description: "If within a subroutine, return to the instruction after the call. If not inside a " +
                    "subroutine, terminate program execution.");
                AddOpCode("texkill",     InstructionType.FlowControl,   2,   0, 1,
                    description: "<b>clip</b>: Cancels rendering of the current pixel if any component of " +
                    "the register is less than zero.");


                //                         -----  Sampling Instructions -----
                //            id                  type          cost  ret  format/args  
                AddOpCode("texld",       InstructionType.Sample,   1,   0, 2,
                    description: "Sample a texture with a particular sampler, using provided texture coordinates.");
                AddOpCode("texldb",      InstructionType.Sample,   6,   0, 2,
                    description: "Sample a texture with a particular sampler, using provided texture coordinates.\n" +
                    "The fourth component biases the level-of-detail just before sampling.");
                AddOpCode("texldl",      InstructionType.Sample,   2,   0, 2,
                    description: "Sample a texture with a particular sampler, using provided texture coordinates and mipmap.");
                AddOpCode("texldd",      InstructionType.Sample,   3,   0, 4,
                    description: "Sample a texture with a particular sampler, using provided texture coordinates and gradients.");
                AddOpCode("texldp",      InstructionType.Sample,   3,   0, 4,
                    description: "Projected texture load instruction. This instruction divides the input texture coordinate by " +
                    "the fourth component just before sampling.");


                //                      -----  Uncategorized Instructions -----
                //            id                  type         cost  ret  format/args   
                AddOpCode("nop",         InstructionType.Other,   1,   0, 0,
                    description: "No operation is performed.");
                AddOpCode("setp_gt",     InstructionType.Other,   1,   1, "{0} > {1}");
                AddOpCode("setp_lt",     InstructionType.Other,   1,   1, "{0} < {1}");
                AddOpCode("setp_ge",     InstructionType.Other,   1,   1, "{0} >= {1}");
                AddOpCode("setp_le",     InstructionType.Other,   1,   1, "{0} <= {1}");
                AddOpCode("setp_eq",     InstructionType.Other,   1,   1, "{0} == {1}");
                AddOpCode("setp_ne",     InstructionType.Other,   1,   1, "{0} != {1}");


                //                         -----  Ignored Instructions -----
                // These instructions do not implement any formatting and are skipped when displaying.
                // Between those are operations like saturate, which do not exist on their own, but can
                // still be documented and also declarations and definitions that only serve as a guide
                // to the hardware... their meaning is already apparent from the shader header.
                //            id                         type          cost  ret  format/args  
                AddOpCode("sat",                InstructionType.Ignore,   0,   0, -1,
                    description: "<b>saturate</b>: Clamps the result of an arithmetic operation to [0.0...1.0] range.");
                AddOpCode("dcl_2d",             InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_cube",           InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_position",       InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_blendweight",    InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_blendindices",   InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_normal",         InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_psize",          InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_texcoord",       InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_tangent",        InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_binormal",       InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_tessfactor",     InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_color",          InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_fog",            InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_depth",          InstructionType.Ignore,   0,   0, -1);
                AddOpCode("dcl_sample",         InstructionType.Ignore,   0,   0, -1);
                AddOpCode("def",                InstructionType.Ignore,   0,   0, -1);
                AddOpCode("defb",               InstructionType.Ignore,   0,   0, -1);
                AddOpCode("defi",               InstructionType.Ignore,   0,   0, -1);
            }  
        }

        public class OpCodesDX11 : OpCodes {
        }

    }
}
