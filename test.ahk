Numpad0::
	WinGetActiveTitle, Title
	WinGet, PID, PID, %Title%
	Run, ControllerMenu.exe %PID%