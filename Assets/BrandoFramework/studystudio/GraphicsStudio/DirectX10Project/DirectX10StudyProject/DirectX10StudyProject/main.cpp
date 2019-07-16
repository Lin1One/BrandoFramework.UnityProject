//////////////////////////////////////////////////////////////////////////////// 
// Filename: main.cpp 
//////////////////////////////////////////////////////////////////////////////// 
#include "systemclass.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, PSTR pScmdline, int iCmdshow)
{
	SystemClass* System;
	bool result;

	// ����system����
	System = new SystemClass;
	if (!System)
	{
		return 0;
	}

	//��ʼ��������system����
	result = System->Initialize();
	if (result)
	{
		System->Run();
	}

	// �رղ��ͷ�system����
	System->Shutdown();

	delete System;
	System = 0;
	return 0;
}
