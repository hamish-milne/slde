#include <d3dcompiler.h>
#define dllexport __declspec(dllexport)

extern "C"
{
	dllexport LPVOID GetBufferPointer(ID3DBlob* blob)
	{
		return blob == nullptr ? nullptr : blob->GetBufferPointer();
	}

	dllexport SIZE_T GetBufferSize(ID3DBlob* blob)
	{
		return blob == nullptr ? 0 : blob->GetBufferSize();
	}

	dllexport void Release(IUnknown* obj)
	{
		if(obj != nullptr) obj->Release();
	}

	dllexport HRESULT WINAPI Compile(LPCVOID pSrcData, SIZE_T SrcDataSize,
		LPCSTR pSourceName, const D3D_SHADER_MACRO *pDefines,
		ID3DInclude *pInclude, LPCSTR pEntryPoint, LPCSTR pTarget,
		UINT Flags1, UINT Flags2, ID3DBlob **ppCode, ID3DBlob **ppErrorMsgs)
	{
		return D3DCompile(pSrcData, SrcDataSize, pSourceName, pDefines, pInclude,
			pEntryPoint, pTarget, Flags1, Flags2, ppCode, ppErrorMsgs);
	}

	dllexport HRESULT WINAPI Disassemble(LPCVOID pSrcData, SIZE_T SrcDataSize,
		UINT Flags, LPCSTR szComments, ID3DBlob **ppDisassembly)
	{
		return D3DDisassemble(pSrcData, SrcDataSize, Flags, szComments, ppDisassembly);
	}
}