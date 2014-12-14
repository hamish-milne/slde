using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SLDE.D3DInterop
{

	public interface IDllSelector
	{
		bool CopyDll(string baseName);
	}

	public class BasicDllSelector : IDllSelector
	{
		public bool CopyDll(string baseName)
		{
			return false;
		}
	}

	public enum D3DErrorCode : uint
	{
		S_OK = 0,
		S_FALSE = 1,
		E_FAIL = 0x80004005,
		E_INVALIDARG = 0x80070057,
		E_OUTOFMEMORY = 0x8007000E,
		E_NOTIMPL = 0x80004001,

		D3DERR_INVALIDCALL =
			((uint)1 << 31) | (0x876 << 16) | (2156),
		D3DERR_WASSTILLDRAWING =
			((uint)1 << 31) | (0x876 << 16) | (540),

		D3D11_ERROR_TOO_MANY_UNIQUE_STATE_OBJECTS =
			((uint)1 << 31) | (0x87c << 16) | (1),
		D3D11_ERROR_FILE_NOT_FOUND =
			((uint)1 << 31) | (0x87c << 16) | (2),
		D3D11_ERROR_TOO_MANY_UNIQUE_VIEW_OBJECTS =
			((uint)1 << 31) | (0x87c << 16) | (3),
		D3D11_ERROR_DEFERRED_CONTEXT_MAP_WITHOUT_INITIAL_DISCARD =
			((uint)1 << 31) | (0x87c << 16) | (4),
	}

	[Flags]
	public enum D3DCompileFlags
	{
		AVOID_FLOW_CONTROL = (1 << 9),
		DEBUG = (1 << 0),
		ENABLE_BACKWARDS_COMPATIBILITY = (1 << 12),
		ENABLE_STRICTNESS = (1 << 11),
		FORCE_PS_SOFTWARE_NO_OPT = (1 << 7),
		FORCE_VS_SOFTWARE_NO_OPT = (1 << 6),
		IEEE_STRICTNESS = (1 << 13),
		NO_PRESHADER = (1 << 8),
		OPTIMIZATION_LEVEL0 = (1 << 14),
		OPTIMIZATION_LEVEL1 = 0,
		OPTIMIZATION_LEVEL2 = ((1 << 14) | (1 << 15)),
		OPTIMIZATION_LEVEL3 = (1 << 15),
		PACK_MATRIX_COLUMN_MAJOR = (1 << 4),
		PACK_MATRIX_ROW_MAJOR = (1 << 3),
		PARTIAL_PRECISION = (1 << 5),
		PREFER_FLOW_CONTROL = (1 << 10),
		RESOURCES_MAY_ALIAS = (1 << 19),
		SKIP_OPTIMIZATION = (1 << 2),
		SKIP_VALIDATION = (1 << 1),
		WARNINGS_ARE_ERRORS = (1 << 18)
	}

	[Flags]
	public enum D3DEffectFlags
	{
		EFFECT_CHILD_EFFECT = (1 << 0),
		EFFECT_ALLOW_SLOW_OPS = (1 << 1)
	}

	[Flags]
	public enum D3DDisasmFlags
	{
		ENABLE_COLOR_CODE = 0x1,
		ENABLE_DEFAULT_VALUE_PRINTS = 0x2,
		ENABLE_INSTRUCTION_NUMBERING = 0x4,
		ENABLE_INSTRUCTION_CYCLE = 0x8,
		DISABLE_DEBUG_INFO = 0x10
	}

	public static class D3DInterop
	{
		public const string InteropDllPath = "d3dInterop";

		[StructLayout(LayoutKind.Sequential)]
		struct D3D_SHADER_MACRO
		{
			[MarshalAs(UnmanagedType.LPStr)] public string Name;
			[MarshalAs(UnmanagedType.LPStr)] public string Definition;

			public D3D_SHADER_MACRO(string name, string definition)
			{
				Name = name;
				Definition = definition;
			}
		}

		[DllImport(InteropDllPath)]
		static extern IntPtr GetBufferPointer(IntPtr blob);

		[DllImport(InteropDllPath)]
		static extern UIntPtr GetBufferSize(IntPtr blob);

		[DllImport(InteropDllPath)]
		static extern D3DErrorCode Compile(
			[MarshalAs(UnmanagedType.LPStr)] string pSrcData,
			UIntPtr SrcDataSize,
			[MarshalAs(UnmanagedType.LPStr)] string pSourceName,
			[MarshalAs(UnmanagedType.LPArray)] D3D_SHADER_MACRO[] pDefines,
			IntPtr pInclude,
			string pEntryPoint,
			[MarshalAs(UnmanagedType.LPStr)] string pTarget,
			D3DCompileFlags Flags1,
			D3DEffectFlags Flags2,
			out IntPtr ppCode,
			out IntPtr ppErrorMsgs);

		[DllImport(InteropDllPath)]
		static extern D3DErrorCode Disassemble(
			IntPtr pSrcData,
			UIntPtr SrcDataSize,
			D3DDisasmFlags Flags,
			[MarshalAs(UnmanagedType.LPStr)] string szComments,
			out IntPtr ppDisassembly);

		static D3DInterop()
		{
			// Copy platform-specific DLL to InteropDLLPath
		}

		public static unsafe void TryCompile(string source)
		{
			IntPtr code, errors;
			var result = Compile(source, new UIntPtr((uint)source.Length), "myFile.hlsl",
				new D3D_SHADER_MACRO[1]{ new D3D_SHADER_MACRO(null, null) },
				new IntPtr(0), "VertexShaderFunction", "vs_1_1", D3DCompileFlags.DEBUG, 0, out code, out errors);
			//Console.WriteLine(GetBufferSize(errors));
			//Console.WriteLine(Marshal.PtrToStringAnsi(GetBufferPointer(errors), (int)GetBufferSize(errors)));

			IntPtr dasm;
			var dresult = Disassemble(GetBufferPointer(code), GetBufferSize(code),
				D3DDisasmFlags.ENABLE_INSTRUCTION_NUMBERING, null, out dasm);
			System.IO.File.WriteAllText("test.asm", Marshal.PtrToStringAnsi(GetBufferPointer(dasm)));
			return;
		}
	}
}
