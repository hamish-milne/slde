Shader Language Development Environment
====

SLDE is an IDE tailored towards shader development (and potentially other
low-level, performance-oriented environments). It will include as standard
support for HLSL, GLSL and Cg, providing compiler support through a variety
of different services, including but not limited to:
	- The DirectX HLSL compiler, both linked in and standalone
	- The built-in GLSL compiler (outputs assembly for NVIDIA only)
	- NVIDIA's Cg toolkit
	- AMD's ShaderAnalyzer

For each language, comprehensive syntax highlighting, code completion and
error highlighting will be available, as well as a view of any generated
assembly, mapping each instruction to a line of the input code.

The GUI is a minimalist tabbed layout with the standard IDE feature set:
tabs can be dragged, split into panes and resized at will.

SLDE is written entirely using .NET 3.5 and Windows Forms, and as such
will run on most platforms from a single binary. Some compiler services will
require small native libraries to run, but these are not required for the
rest of the program to function.

Why SLDE?
====

There are a variety of alternatives available. One could use NVIDIA's
NSight extension for Visual Studio, or the aforementioned ShaderAnalyzer,
or proprietary, engine-specific solutions. While these may be appropriate
in certain situations, SLDE aims to provide a complete, cross-platform
solution, allowing shader analysis for any number of different configurations
all in the same interface. In addition, SLDE is entirely free, licensed
under the GPL.

Plugins
====

SLDE is designed with extensibility in mind. Adding new languages, compilers,
UI elements and localizations is very simple, and could potentially allow
further integration with other programs and engines.