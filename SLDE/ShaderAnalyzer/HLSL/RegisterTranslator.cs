
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SLDE.ShaderAnalyzer.HLSL {
    public class RegisterTranslator {
        private static Regex identifierParseRegex = new Regex(
            @"^" +
            @"(?<neg>-)?" +
            @"(?<absl>[|])?" +
            @"(?<id>[a-zA-Z][a-zA-Z0-9]*)" +
            @"(?<absk>_abs)?" +
            @"(?:\[(?<index1>[^]]*)])?" +
            @"(?:\[(?<index2>[^]]*)])?" +
            @"(?:\.(?<mask>[xyzw]{1,4}|[rgba]{1,4})\b)?" +
            @"\k<absl>?",
            RegexOptions.Compiled);
        private static Regex registerMatchRegex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*", RegexOptions.Compiled);

        private static char[] trimChars = { '/', ' ', '\t' };
        private static char[] splitChars = { ' ', '\t' };

        private Dictionary<string, ITranslation> registerDict = new Dictionary<string, ITranslation>();
        private Func<string, string> semanticTranslatorFunc;
        private IHeaderParser sectionParser;
        private int blankLineCounter = 0;

        public RegisterTranslator(Func<string, string> translator = null) {
            semanticTranslatorFunc = translator ?? (s => (s));

            // Setup constant translations to semantics
            registerDict.Add("vPos", new TranslationDirect(semanticTranslatorFunc("vPos"), 4));
            registerDict.Add("vFace", new TranslationDirect(semanticTranslatorFunc("vFace"), 1));
            registerDict.Add("oC0", new TranslationDirect(semanticTranslatorFunc("oColor0"), 4));
            registerDict.Add("oC1", new TranslationDirect(semanticTranslatorFunc("oColor1"), 4));
            registerDict.Add("oC2", new TranslationDirect(semanticTranslatorFunc("oColor2"), 4));
            registerDict.Add("oC3", new TranslationDirect(semanticTranslatorFunc("oColor3"), 4));
            registerDict.Add("oC4", new TranslationDirect(semanticTranslatorFunc("oColor4"), 4));
            registerDict.Add("oC5", new TranslationDirect(semanticTranslatorFunc("oColor5"), 4));
            registerDict.Add("oC6", new TranslationDirect(semanticTranslatorFunc("oColor6"), 4));
            registerDict.Add("oC7", new TranslationDirect(semanticTranslatorFunc("oColor7"), 4));
            registerDict.Add("o0", new TranslationDirect(semanticTranslatorFunc("oColor0"), 4));
            registerDict.Add("o1", new TranslationDirect(semanticTranslatorFunc("oColor1"), 4));
            registerDict.Add("o2", new TranslationDirect(semanticTranslatorFunc("oColor2"), 4));
            registerDict.Add("o3", new TranslationDirect(semanticTranslatorFunc("oColor3"), 4));
            registerDict.Add("o4", new TranslationDirect(semanticTranslatorFunc("oColor4"), 4));
            registerDict.Add("o5", new TranslationDirect(semanticTranslatorFunc("oColor5"), 4));
            registerDict.Add("o6", new TranslationDirect(semanticTranslatorFunc("oColor6"), 4));
            registerDict.Add("o7", new TranslationDirect(semanticTranslatorFunc("oColor7"), 4));
            registerDict.Add("oDepth", new TranslationDirect(semanticTranslatorFunc("oDepth"), 1));

        }

        public void ReadHeaderLine(string line) {
            line = line.TrimStart(trimChars);

            if (line.Length != 0) {
                if (sectionParser == null) {
                    switch (line) {
                        case "Parameters:":
                            sectionParser = new Dx9ParameterHeaderParser();
                            break;
                        case "Registers:":
                            sectionParser = new DX9RegistersHeaderParser();
                            break;
                        default:
                            sectionParser = new NullHeaderParser();
                            break;
                    }
                } else {
                    sectionParser.ParseLine(line, registerDict);
                }
                blankLineCounter = 0;
            } else {
                blankLineCounter++;
                if (blankLineCounter == 2) {
                    ClearHeaderSection();
}
            }
        }

        public void ClearHeaderSection() {
            sectionParser = null;
        }

        public void ReadConstantDefinition(string opcode, List<string> args) {
            string type;
            switch (opcode) {
                case "def":
                    type = "float";
                    break;
                case "defi":
                    type = "int";
                    break;
                case "defb":
                    type = "bool";
                    break;
                default:
                    return;
            }

            var trans = new TranslationMasked(type);

            for (int i = 1; i < args.Count; i++) {
                string val = args[i];
                trans.AddTranslation(val, i - 1, 1);
            }

            registerDict.Add(args[0], trans);
        }

        public void ReadDeclaration(string opcode, List<string> args) {
            string semantic;
            if (opcode.StartsWith("dcl_color")) {
                semantic = semanticTranslatorFunc("vColor" + opcode.Substring(9));
            } else if (opcode.StartsWith("dcl_texcoord")) {
                semantic = semanticTranslatorFunc("vTexcoord" + opcode.Substring(12));
            } else {
                return;
            }

            string id = registerMatchRegex.Match(args[0]).Value;

            registerDict.Add(id, new TranslationDirect(semantic));

        }

        public string Translate(string identifier) {
            Match item = identifierParseRegex.Match(identifier);
            if (item.Success) {
                string id = item.Groups["id"].Value;
                if (registerDict.ContainsKey(id)) {
                    var rm = new RegisterMatch();
                    rm.id = id;
                    rm.mask = item.Groups["mask"].Value;

                    if (item.Groups["index1"].Success) {
                        rm.index = Int32.Parse(item.Groups["index1"].Value); // TODO: support for dynamic indexing
                    }
                    if (item.Groups["index2"].Success) {
                        rm.bodyIndex = Int32.Parse(item.Groups["index2"].Value);
                    }
                    if (item.Groups["absl"].Success || item.Groups["absk"].Success) {
                        rm.hasAbs = true;
                    }
                    if (item.Groups["neg"].Success) {
                        rm.hasNeg = true;
                    }

                    return registerDict[id].Translate(rm);
                }
            }

            return identifier;
        }

        private class RegisterMatch {
            public string id;
            public int index = -1;
            public int bodyIndex = -1;
            public string dynamicIndex = null;
            public string mask = null;
            public bool hasAbs;
            public bool hasNeg;

            public bool IsIndexed() {
                return index >= 0;
            }

            public bool IsBodyIndexed() {
                return bodyIndex >= 0;
            }

            public bool IsDynamicIndexed() {
                return !String.IsNullOrEmpty(dynamicIndex);
            }

            public bool IsMasked() {
                return !String.IsNullOrEmpty(mask);
            }

        }

        private interface IHeaderParser {
            void ParseLine(string line, Dictionary<string, ITranslation> registerDict);
        }

        private class NullHeaderParser : IHeaderParser {
            public void ParseLine(string line, Dictionary<string, ITranslation> registerDict) { }
        }

        private abstract class TableHeaderParser : IHeaderParser {
            protected bool pastSeparator;

            public void ParseLine(string line, Dictionary<string, ITranslation> registerDict) {
                if (pastSeparator) {
                    string[] items = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    ParseTableItems(items, registerDict);
                } else if (line.StartsWith("-")) {
                    pastSeparator = true;
                }
            }

            protected abstract void ParseTableItems(string[] items, Dictionary<string, ITranslation> registerDict);
        }


        private class Dx9ParameterHeaderParser : IHeaderParser {
            private static Regex extractNameRegex = new Regex(@"\w+", RegexOptions.Compiled);

            public static Dictionary<string, int> parameterWidth = new Dictionary<string, int>();

            public void ParseLine(string line, Dictionary<string, ITranslation> registerDict) {
                string[] items = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                string name = extractNameRegex.Match(items[1]).Value;

                int width;
                if (!Int32.TryParse(items[0][items[0].Length - 1].ToString(), out width)) {
                    width = 0;
                }

                parameterWidth[name] = width;
            }
        }

        private class DX9RegistersHeaderParser : TableHeaderParser {
            private static Regex registerIdRegex = new Regex(@"^(\w+)(\d+)$", RegexOptions.Compiled);

            protected override void ParseTableItems(string[] items, Dictionary<string, ITranslation> registerDict) {
                string name = items[0];
                string register = items[1];
                int size = Int32.Parse(items[2]);
                int width;
                if (!Dx9ParameterHeaderParser.parameterWidth.TryGetValue(name, out width)) {
                    width = 0;
                }

                if (size == 1) {
                    registerDict.Add(register, new TranslationDirect(name, width));
                } else {
                    // Array / matrix spans over multiple registers, so lets handle that.
                    Match regMatch = registerIdRegex.Match(register);
                    string regId = regMatch.Groups[1].Value;
                    int regOffset = Int32.Parse(regMatch.Groups[2].Value);

                    for (int i = 0; i < size; i++) {
                        string newReg = regId + (i + regOffset);
                        registerDict.Add(newReg, new TranslationDirect(String.Format("{0}[{1}]", name, i), width));
                    }
                }
            }
        }

        private interface ITranslation {
            string Translate(RegisterMatch match);
        }

        private class TranslationDirect : ITranslation {
            private string name;
            private int width;

            public TranslationDirect(string name, int width = 0) {
                this.name = name;
                this.width = width;
            }


            public string Translate(RegisterMatch match) {
                StringBuilder sb = new StringBuilder(name.Length + 11);
                if (match.hasNeg) {
                    sb.Append("-");
                }
                if (match.hasAbs) {
                    sb.Append("abs(");
                }
                sb.Append(name);
                if (match.IsMasked() && match.mask != "xyzw".Substring(0, width)) {
                    sb.Append(".");
                    sb.Append(match.mask);
                }
                if (match.hasAbs) {
                    sb.Append(")");
                }
                return sb.ToString();
            }
        }

        private class TranslationMasked : ITranslation {
            private string type;
            private string[] names;
            private int[] widths;
            private int[] offsets;

            public TranslationMasked(string type) {
                this.type = type;
                names = new string[4];
                widths = new[] {0, 0, 0, 0};
                offsets = new[] {0, 0, 0, 0};
            }

            public void AddTranslation(string name, int position, int width) {
                names[position] = name;
                widths[position] = width;
                for (int i = 0; i < width; i++) {
                    offsets[i + position] = i;
                }
            }

            public string Translate(RegisterMatch match) {
                StringBuilder sb = new StringBuilder();
                if (match.hasNeg) {
                    sb.Append("-");
                }
                if (match.hasAbs) {
                    sb.Append("abs(");
                }

                string mask = match.IsMasked() ? match.mask : "xyzw";
                FormatMasked(sb, mask);

                if (match.hasAbs) {
                    sb.Append(")");
                }
                return sb.ToString();
            }

            public void FormatMasked(StringBuilder sb, string mask) {
                bool multiple = false;
                string currentName = null;
                int width = 0;
                List<char> currentMask = new List<char>(4);

                foreach (char c in mask) {
                    int i = MiscUtilities.ComponentMaskToIndex(c);
                    int offset = offsets[i];
                    width = widths[i - offset];
                    string name = names[i - offset];

                    if (name != currentName) {
                        if (currentName != null) {
                            // We reached a component that points to a different value

                            if (multiple == false) {
                                // The register resolves to multiple values, add an explicit cast
                                multiple = true;
                                sb.Append(type);
                                if (mask.Length > 1) {
                                    sb.Append(mask.Length);
                                }
                                sb.Append("(");
                            } else {
                                sb.Append(", ");
                            }

                            sb.Append(currentName);
                            string newMask = new string(currentMask.ToArray());
                            if (newMask != "xyzw".Substring(0, width)) {
                                sb.Append(".");
                                sb.Append(newMask);
                            }
                        }

                        currentName = name;
                        currentMask = new List<char>(4);
                    }

                    currentMask.Add(MiscUtilities.IndexToComponentMask(offset));
                }

                if (currentName != null) {
                    if (multiple) {
                        sb.Append(", ");
                    }

                    sb.Append(currentName);
                    string newMask = new string(currentMask.ToArray());
                    if (newMask != "xyzw".Substring(0, width)) {
                        sb.Append(".");
                        sb.Append(newMask);
                    }
                }

                if (multiple) {
                    sb.Append(")");
                }
            }
        }

        private class TranslationBuffer : ITranslation {

            public string Translate(RegisterMatch match) {
                throw new NotImplementedException();
            }
        }
    }
}
