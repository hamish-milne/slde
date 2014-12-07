using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SLDE.ShaderAnalyzer.HLSL {

    /// <summary>
    /// Encapsulates functionality for analyzing hlsl shaders through the fxc compiler.
    /// Not thread safe yet
    /// </summary>
    public class Analyzer {
        public CompileOptions options;

        private static Regex lineBreakRegex = new Regex(@"\r\n?|\n");
        private static Regex includeStatementRegex = new Regex(@"^[ \t]*#include ""([^\n""]+)""[ \t]*", RegexOptions.Multiline);

        private Dictionary<string, string> includeCache = new Dictionary<string, string>();
        private string entryFilePath;

        public Analyzer(CompileOptions options = null) {
            this.options = options ?? new CompileOptions();
        }

        /// <summary>
        /// Preprocesses a shader and all files it includes so that it can be fed to the compiler
        /// </summary>
        /// <param name="shader">A string containing the hlsl code for compilation</param>
        /// <param name="path">The path to the shader. Used in line references</param>
        /// <param name="lineOffset">The line where the hlsl code starts at in the source file. Used in line references</param>
        public void UpdateShader(string shader, string path, int lineOffset = 0) {
            options.includePaths.Add(Path.GetDirectoryName(path));

            if (entryFilePath == null) {
                entryFilePath = Path.GetTempFileName();
            }

            lineOffset = lineOffset * 3 + 1; // Multiply by 3 to make the line offset survive through the line break duplication
            string processed = ProcessShader(shader, path, lineOffset); 
            File.WriteAllText(entryFilePath, processed);
            Debug.Print(processed);

            options.includePaths.RemoveAt(options.includePaths.Count - 1);
        }

        /// <summary>
        /// Compiles a function in the preprocessed shader against the specified target
        /// </summary>
        /// <param name="entryPoint">The name of the entry function</param>
        /// <param name="target">The target shader profile</param>
        /// <returns>Compiled and parsed Assembly object</returns>
        public Assembly Compile(string entryPoint, Profile target) {
            return new Assembly(entryFilePath, entryPoint, target, options);
        }

        /// <summary>
        /// Returns all files that have been processed as include files so far
        /// </summary>
        public IEnumerable<string> GetIncludedFiles() {
            return includeCache.Keys;
        }

        /// <summary>
        /// Clears the files that have been processed as include files before.
        /// Call this when you detect that one of them has changed.
        /// </summary>
        public void ClearIncludeCache() {
            foreach (string tempPath in includeCache.Values) {
                File.Delete(tempPath);
            }
            includeCache = new Dictionary<string, string>();
        }

        /// <summary>
        /// Cleans up all temporary files that have been created.
        /// </summary>
        public void Cleanup() {
            ClearIncludeCache();
            if (entryFilePath != null) {
                File.Delete(entryFilePath);
            }
        }

        /// <summary>
        /// Overrides the path from the temp file to the real path through the #line directive
        /// For every included file, it processes that as another shader and changes the path
        /// to reference that instead.
        /// Replaces all line breaks with three to force the compiler to spit out info for each line
        /// </summary>
        private string ProcessShader(string shader, string path, int lineOffset = 1) {
            string header = String.Format("#line {0} \"{1}\"\n", lineOffset, path);
            shader = Analyzer.lineBreakRegex.Replace(header + shader, "\n\n\n");

            MatchEvaluator replEval = match => {
                string filename = ProcessInclude(match.Groups[1].Value);
                return String.Format("#include \"{0}\"", filename);
            };

            return Analyzer.includeStatementRegex.Replace(shader, replEval);
        }

        /// <summary>
        /// Returns the path to the processed shader from the input include path.
        /// If it's not processed already, does so and saves it as a temporary file.
        /// </summary>
        private string ProcessInclude(string path) {
            string fullPath = FindAbsolutePath(path);

            if (includeCache.ContainsKey(fullPath)) {
                return includeCache[fullPath];
            }

            string tempPath = Path.GetTempFileName();
            includeCache[fullPath] = tempPath;

            string inputShader = File.ReadAllText(fullPath);
            string outputShader = ProcessShader(inputShader, fullPath);
            File.WriteAllText(tempPath, outputShader);

            return tempPath;
        }

        /// <summary>
        /// Traverses the include folders and looks for the file at path
        /// </summary>
        private string FindAbsolutePath(string path) {
            if (Path.IsPathRooted(path) && File.Exists(path)) {
                return path;
            }

            foreach (string includePath in options.includePaths) {
                string combined = Path.Combine(includePath, path);
                if (File.Exists(combined)) {
                    return Path.GetFullPath(path);
                }
            }

            throw new FileNotFoundException(path);
        }
    }
}
