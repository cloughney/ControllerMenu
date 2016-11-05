Down::
	WinGetActiveTitle, Title
	WinGet, PID, PID, %Title%
	Run, ControllerMenu\bin\Debug\ControllerMenu.exe %PID%