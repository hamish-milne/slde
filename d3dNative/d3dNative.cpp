#include <d3dcommon.h>
#include <cstdio>

extern "C"
{
	__declspec(dllexport) LPVOID GetBufferPointer(ID3DBlob* blob)
	{
		return blob == nullptr ? nullptr : blob->GetBufferPointer();
	}

	__declspec(dllexport) SIZE_T GetBufferSize(ID3DBlob* blob)
	{
		return blob == nullptr ? 0 : blob->GetBufferSize();
	}

	__declspec(dllexport) void Release(IUnknown* obj)
	{
		if(obj != nullptr) obj->Release();
	}
}