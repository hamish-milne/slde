using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SLDE.ShaderAnalyzer.HLSL {

    abstract class OpCodes {
        private static Regex opNameFilter = new Regex(@"^[a-zA-Z_]+", RegexOptions.Compiled);

        public static OpCodesDX9 DX9 = new OpCodesDX9();
//        public static OpCodesDX11 DX11 = new OpCodesDX11();

        protected Dictionary<string, OpCode> opDict;

        public string LookupDescription(string id) {
            return opDict[id].Description;
        }

        /// <summary>
        /// Looks up the operator stored as the first token and formats the instruction into
        /// a more readable format. If the type of the instruction is Ignore, it is not formatted
        /// and null is returned.
        /// </summary>
        /// <exception cref="FormatException">Invalid amount of arguments supplied</exception>
        public string FormatInstruction(string opcode, string[] args, out IOperator iop) {
            OpCode opCode = opDict[opcode];
            iop = (IOperator) opCode;

            if (opCode.Type != InstructionType.Ignore) {
                return opCode.FormatArguments(args, 0);
            } else {
                return null;
            }
        }

        public bool IsValidInstruction(string opcode) {
            return opDict.ContainsKey(opcode);
        }

        protected void AddOpCode(string id, InstructionType type, int returns, string format,
            string altFormat = null, bool indents = false, bool dedents = false, string description = null) {

            opDict.Add(id, new OpCode(id, type, returns, format, altFormat, indents, dedents, description));

            if (type == InstructionType.Arithmetic && returns == 1) {
                // The operator is probably saturable...
                format = "sat(" + format + ")";
                if (altFormat != null) {
                    altFormat = "sat(" + format + ")";
                }
                opDict.Add(id + "_sat", new OpCode(id, type, returns, format, altFormat, indents, dedents, description));
            }
        }

        protected void AddOpCode(string id, InstructionType type, int returns, int arguments,
            int altArguments = -1, bool indents = false, bool dedents = false, string description = null) {
            
            string format = constructDefaultFormatString(id, arguments);
            string altFormat = constructDefaultFormatString(id, altArguments);

            AddOpCode(id, type, returns, format, altFormat, indents, dedents, description);
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

        public interface IOperator {
            string ID { get; }
            InstructionType Type { get; }
            bool Indents { get; }
            bool Dedents { get; }
        }

        protected class OpCode : IOperator {
            public string ID { get; private set; }
            public InstructionType Type { get; private set; }
            public bool Indents { get; private set; }
            public bool Dedents { get; private set; }
            public string Description { get; private set; }

            private int returns;
            private string format;
            private string altFormat;

            public OpCode(string id, InstructionType type, int returns, string format,
                string altFormat = null, bool indents = false, bool dedents = false, string description = null) {
                ID = id;
                Type = type;
                Indents = indents;
                Dedents = dedents;
                Description = description;

                this.returns = returns;
                string sep = (returns > 0) ? "\t= " : "\t  ";
                this.format = sep + format;
                this.altFormat = (altFormat == null) ? null : sep + altFormat;
            }

            public string FormatArguments(string[] tokens, int startAt = 0) {
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

                //                 -----  Arithmetic Instructions -----
                //            id                     type            ret  format/args    
                AddOpCode("abs",         InstructionType.Arithmetic,   1, 1,
                    description: "Computes absolute value.");
                AddOpCode("add",         InstructionType.Arithmetic,   1, "{0} + {1}");
                AddOpCode("cmp",         InstructionType.Arithmetic,   1, "({0} >= 0) ? {1} : {2}");
                AddOpCode("crs",         InstructionType.Arithmetic,   1, 2,
                    description: "Computes a cross product using the right-hand rule.");
                AddOpCode("dp2",         InstructionType.Arithmetic,   1, 2,
                    description: "Computes the two-component dot product of the source registers.");
                AddOpCode("dp2add",      InstructionType.Arithmetic,   1, "dp2({0}, {1}) + {2}");
                AddOpCode("dp3",         InstructionType.Arithmetic,   1, 2,
                    description: "Computes the three-component dot product of the source registers.");
                AddOpCode("dp4",         InstructionType.Arithmetic,   1, 2,
                    description: "Computes the four-component dot product of the source registers.");
                AddOpCode("dst",         InstructionType.Arithmetic,   1, 2,
                    description: "Calculates a distance vector.");
                AddOpCode("dsx",         InstructionType.Arithmetic,   1, 1,
                    description: "Compute the rate of change of the register in the render target's x-direction.");
                AddOpCode("dsy",         InstructionType.Arithmetic,   1, 1,
                    description: "Compute the rate of change of the register in the render target's y-direction.");
                AddOpCode("exp",         InstructionType.Arithmetic,   1, 1,
                    description: "Provides full precision exponential 2^x.");
                AddOpCode("expp",        InstructionType.Arithmetic,   1, 1,
                    description: "Provides partial precision exponential 2^x.");
                AddOpCode("frc",         InstructionType.Arithmetic,   1, 1,
                    description: "Returns the fractional portion of each input component.");
                AddOpCode("lit",         InstructionType.Arithmetic,   1, 1,
                    description: "Provides partial support for lighting by calculating lighting coefficients " +
                                 "from two dot products and an exponent.");
                AddOpCode("log",         InstructionType.Arithmetic,   1, 1,
                    description: "Provides full precision base 2 logarithm.");
                AddOpCode("logp",        InstructionType.Arithmetic,   1, 1,
                    description: "Provides partial precision base 2 logarithm.");
                AddOpCode("lrp",         InstructionType.Arithmetic,   1, 3,
                    description: "Interpolates linearly between the second and third source registers " +
                                 "by a proportion specified in the first source register.");
                AddOpCode("m3x2",        InstructionType.Arithmetic,   1, 2,
                    description: "Multiplies a 3-component vector by a 3x2 matrix starting at the specified register.");
                AddOpCode("m3x3",        InstructionType.Arithmetic,   1, 2,
                    description: "Multiplies a 3-component vector by a 3x3 matrix starting at the specified register.");
                AddOpCode("m3x4",        InstructionType.Arithmetic,   1, 2,
                    description: "Multiplies a 3-component vector by a 3x4 matrix starting at the specified register.");
                AddOpCode("m4x3",        InstructionType.Arithmetic,   1, 2,
                    description: "Multiplies a 4-component vector by a 4x3 matrix starting at the specified register.");
                AddOpCode("m4x4",        InstructionType.Arithmetic,   1, 2,
                    description: "Multiplies a 4-component vector by a 4x4 matrix starting at the specified register.");
                AddOpCode("mad",         InstructionType.Arithmetic,   1, "{0} * {1} + {2}");
                AddOpCode("max",         InstructionType.Arithmetic,   1, 2,
                    description: "Calculates the component-wise maximum of the sources.");
                AddOpCode("min",         InstructionType.Arithmetic,   1, 2,
                    description: "Calculates the component-wise minimum of the sources.");
                AddOpCode("mov",         InstructionType.Arithmetic,   1, "{0}");
                AddOpCode("mova",        InstructionType.Arithmetic,   1, "{0}");
                AddOpCode("mul",         InstructionType.Arithmetic,   1, "{0} * {1}");
                AddOpCode("nrm",         InstructionType.Arithmetic,   1, 1,
                    description: "Normalize a 3D vector.");
                AddOpCode("pow",         InstructionType.Arithmetic,   1, 2,
                    description: "Full precision abs(x)^y.");
                AddOpCode("rcp",         InstructionType.Arithmetic,   1, "1.0 / {0}");
                AddOpCode("rsq",         InstructionType.Arithmetic,   1, "1.0 / sqrt({0})");
                AddOpCode("sge",         InstructionType.Arithmetic,   1, "({0} >= {1}) ? 1.0 : 0.0");
                AddOpCode("sgn",         InstructionType.Arithmetic,   1, 3,
                    description: "Computes the sign of the input. The other two registers hold intermediate results " +
                                 "and are undefined after execution.");
                AddOpCode("slt",         InstructionType.Arithmetic,   1, "({0} < {1}) ? 1.0 : 0.0");
                AddOpCode("sincos",      InstructionType.Arithmetic,   1, 1, altArguments: 3,
                    description: "Computes and outputs the cosine in .x and sine in the .y component, in radians.");


                //                 -----  Flow Control Instructions -----
                //            id                     type             ret  format/args  
                AddOpCode("break",       InstructionType.FlowControl,   0, 0,
                    description: "Break out of the current loop at the nearest <b>endloop</b> or <b>endrep</b>.");
                AddOpCode("break_gt",    InstructionType.FlowControl,   0, "if ({0} > {1}) break");
                AddOpCode("break_lt",    InstructionType.FlowControl,   0, "if ({0} < {1}) break");
                AddOpCode("break_ge",    InstructionType.FlowControl,   0, "if ({0} >= {1}) break");
                AddOpCode("break_le",    InstructionType.FlowControl,   0, "if ({0} <= {1}) break");
                AddOpCode("break_eq",    InstructionType.FlowControl,   0, "if ({0} == {1}) break");
                AddOpCode("break_ne",    InstructionType.FlowControl,   0, "if ({0} != {1}) break");
                AddOpCode("breakp",      InstructionType.FlowControl,   0, 1,
                    description: "Break out of the current loop at the nearest <b>endloop</b> or <b>endrep</b>.\n" +
                                 "Uses one of the components of the predicate register as a condition to determine " +
                                 "whether or not to perform the instruction.");
                AddOpCode("call",        InstructionType.FlowControl,   0, 1,
                    description: "Calls a subroutine marked by where the <b>label</b> l# appears in the program.");
                AddOpCode("callnz",      InstructionType.FlowControl,   0, "if ({1}) call({0})");
                AddOpCode("else",        InstructionType.FlowControl,   0, 0, indents: true, dedents: true);
                AddOpCode("endif",       InstructionType.FlowControl,   0, 0, dedents: true);
                AddOpCode("endloop",     InstructionType.FlowControl,   0, 0, dedents: true);
                AddOpCode("endrep",      InstructionType.FlowControl,   0, 0, dedents: true);
                AddOpCode("if",          InstructionType.FlowControl,   0, "if ({0})", indents: true);
                AddOpCode("if_gt",       InstructionType.FlowControl,   0, "if ({0} > {1})", indents: true);
                AddOpCode("if_lt",       InstructionType.FlowControl,   0, "if ({0} < {1})", indents: true);
                AddOpCode("if_ge",       InstructionType.FlowControl,   0, "if ({0} >= {1})", indents: true);
                AddOpCode("if_le",       InstructionType.FlowControl,   0, "if ({0} <= {1})", indents: true);
                AddOpCode("if_eq",       InstructionType.FlowControl,   0, "if ({0} == {1})", indents: true);
                AddOpCode("if_ne",       InstructionType.FlowControl,   0, "if ({0} != {1})", indents: true);
                AddOpCode("label",       InstructionType.FlowControl,   0, "label {0}", indents: true,
                    description: "Indicates the beginning of a subroutine.");
                AddOpCode("loop",        InstructionType.FlowControl,   0, 2, indents: true,
                    description: "Starts a <b>loop</b>...<b>endloop</b> block.");
                AddOpCode("rep",         InstructionType.FlowControl,   0, 1, indents: true,
                    description: "Starts a <b>rep</b>...<b>endrep</b> block. Parameter specifies the repeat count " +
                                 "in the .x component.");
                AddOpCode("ret",         InstructionType.FlowControl,   0, 0,
                    description: "If within a subroutine, return to the instruction after the call. If not inside a " +
                                 "subroutine, terminate program execution.");
                AddOpCode("texkill",     InstructionType.FlowControl,   0, 1,
                    description: "Cancels rendering of the current pixel if any component of " +
                                 "the register is less than zero.");


                //                 -----  Sampling Instructions -----
                //            id                  type           ret  format/args  
                AddOpCode("texld",       InstructionType.Sample,   0, 2,
                    description: "Sample a texture with a particular sampler, using provided texture coordinates.");
                AddOpCode("texldb",      InstructionType.Sample,   0, 2,
                    description: "Sample a texture with a particular sampler, using provided texture coordinates.\n" +
                                 "The fourth component biases the level-of-detail just before sampling.");
                AddOpCode("texldl",      InstructionType.Sample,   0, 2,
                    description: "Sample a texture with a particular sampler, using provided texture coordinates and mipmap.");
                AddOpCode("texldd",      InstructionType.Sample,   0, 4,
                    description: "Sample a texture with a particular sampler, using provided texture coordinates and gradients.");
                AddOpCode("texldp",      InstructionType.Sample,   0, 4,
                    description: "Projected texture load instruction. This instruction divides the input texture coordinate by " +
                                 "the fourth component just before sampling.");


                //             -----  Uncategorized Instructions -----
                //            id                  type          ret  format/args   
                AddOpCode("nop",         InstructionType.Other,   0, 0,
                    description: "No operation is performed.");
                AddOpCode("setp_gt",     InstructionType.Other,   1, "{0} > {1}");
                AddOpCode("setp_lt",     InstructionType.Other,   1, "{0} < {1}");
                AddOpCode("setp_ge",     InstructionType.Other,   1, "{0} >= {1}");
                AddOpCode("setp_le",     InstructionType.Other,   1, "{0} <= {1}");
                AddOpCode("setp_eq",     InstructionType.Other,   1, "{0} == {1}");
                AddOpCode("setp_ne",     InstructionType.Other,   1, "{0} != {1}");


                //                   -----  Ignored Instructions -----
                // These instructions do not implement any formatting and are skipped when displaying.
                // Between those are operators like saturate, which do not exist on their own, but can
                // still be documented.
                //            id                         type           ret  format/args  
                AddOpCode("sat",                InstructionType.Ignore,   0, -1,
                    description: "Clamps the result of an arithmetic operation to [0.0...1.0] range.");
                AddOpCode("sqrt",               InstructionType.Ignore,   0, -1,
                    description: "Component-wise square root.");
            }  
        }

        public class OpCodesDX11 : OpCodes {
            public OpCodesDX11() {
                throw new NotImplementedException();

                opDict = new Dictionary<string, OpCode>();

                //                 -----  Arithmetic Instructions -----
                //            id                     type            ret  format/args    
                AddOpCode("add",         InstructionType.Arithmetic,   1, "{0} + {1}");
                AddOpCode("dp2",         InstructionType.Arithmetic,   1, 2,
                    description: "Computes the two-component dot product of the source registers.");
                AddOpCode("dp3",         InstructionType.Arithmetic,   1, 2,
                    description: "Computes the three-component dot product of the source registers.");
                AddOpCode("dp4",         InstructionType.Arithmetic,   1, 2,
                    description: "Computes the four-component dot product of the source registers.");
                AddOpCode("exp",         InstructionType.Arithmetic,   1, 1,
                    description: "Provides full precision exponential 2^x.");
                AddOpCode("frc",         InstructionType.Arithmetic,   1, 1,
                    description: "Returns the fractional portion of each input component.");
                AddOpCode("log",         InstructionType.Arithmetic,   1, 1,
                    description: "Provides full precision base 2 logarithm.");
                AddOpCode("mad",         InstructionType.Arithmetic,   1, "{0} * {1} + {2}");
                AddOpCode("max",         InstructionType.Arithmetic,   1, 2,
                    description: "Calculates the component-wise maximum of the sources.");
                AddOpCode("min",         InstructionType.Arithmetic,   1, 2,
                    description: "Calculates the component-wise minimum of the sources.");
                AddOpCode("mov",         InstructionType.Arithmetic,   1, "{0}");
                AddOpCode("mul",         InstructionType.Arithmetic,   1, "{0} * {1}");
                AddOpCode("rcp",         InstructionType.Arithmetic,   1, "1.0 / {0}");
                AddOpCode("rsq",         InstructionType.Arithmetic,   1, "1.0 / sqrt({0})");
                AddOpCode("sincos",      InstructionType.Arithmetic,   2, 1,
                    description: "Component-wise sin(x) in the first output and cos(x) in the second " +
                                 "output, for x in radians.");


                //                 -----  Flow Control Instructions -----
                //            id                     type             ret  format/args  
                AddOpCode("break",       InstructionType.FlowControl,   0, 0,
                    description: "Moves the point of execution to the instruction after the next <b>endloop</b> or <b>endswitch</b>.");
                AddOpCode("call",        InstructionType.FlowControl,   0, 1,
                    description: "Calls a subroutine marked by where the <b>label</b> l# appears in the program.");
                AddOpCode("else",        InstructionType.FlowControl,   0, 0, indents: true, dedents: true);
                AddOpCode("endif",       InstructionType.FlowControl,   0, 0, dedents: true);
                AddOpCode("endloop",     InstructionType.FlowControl,   0, 0, dedents: true);
                AddOpCode("endswitch",   InstructionType.FlowControl,   0, 0, dedents: true);
                AddOpCode("label",       InstructionType.FlowControl,   0, "label {0}", indents: true,
                    description: "Indicates the beginning of a subroutine.");
                AddOpCode("ret",         InstructionType.FlowControl,   0, 0,
                    description: "If within a subroutine, return to the instruction after the call. If not inside a " +
                                 "subroutine, terminate program execution.");


                //                 -----  Sampling Instructions -----
                //            id                  type           ret  format/args  


                //             -----  Uncategorized Instructions -----
                //            id                  type          ret  format/args   
                AddOpCode("nop",         InstructionType.Other,   0, 0,
                    description: "No operation is performed.");


                //               -----  Ignored Instructions -----
                //            id                         type           ret  format/args  
                AddOpCode("sat",                InstructionType.Ignore,   0, -1,
                    description: "Clamps the result of an arithmetic operation to [0.0...1.0] range.");
                AddOpCode("abs",                InstructionType.Ignore,   0, -1,
                    description: "Computes absolute value.");
            }
        }

    }
}
