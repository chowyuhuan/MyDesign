@echo off
set mcs=%1\Mono\bin\gmcs.bat
REM 测试过，在不将脚本移出Assets（或者移动到WebplayerTemplates下）的情况下，编译出来的DLL与在Assets下大小一致，仅仅是报重复的warning，故不移出脚本直接编译了（前提是编译出的JITDLL.DLL要放在ScriptAssemblies，Unity不会将此文件夹下的DLL编译，如果放到其他文件夹，比如Plugins会报重复编译错误）
set scripts=%~dp0..\..\JITDLL\*.cs
set UnityEngine=%~dp0..\..\..\..\Library\UnityAssemblies\UnityEngine.dll
set UnityEngineUI=%~dp0..\..\..\..\Library\UnityAssemblies\UnityEngine.UI.dll
set AssemblyCSharp=%~dp0..\..\..\..\Library\ScriptAssemblies\Assembly-CSharp.dll
set OutPut=%~dp0%3\JITDLL.dll
if not exist %~dp0..\..\..\..\Logs\CompileDLL (md %~dp0..\..\..\..\Logs\CompileDLL)
if %time:~0,2% leq 9 (set hour=0%time:~1,1%) else (set hour=%time:~0,2%)
set LogFile=%~dp0..\..\..\..\Logs\CompileDLL\%date:~0,4%%date:~5,2%%date:~8,2%_%hour%_%time:~3,2%_%time:~6,2%.log

@echo ------------------------------- 编译指令 ------------------------------------ >> %LogFile%
@echo %mcs% -target:library -define:%2 -reference:%UnityEngine%,%UnityEngineUI%,%AssemblyCSharp% -out:%OutPut% -optimize -unsafe -recurse:%scripts% >> %LogFile%
@echo ------------------------------- 编译日志 ------------------------------------ >> %LogFile%
%mcs% -target:library -define:%2 -reference:%UnityEngine%,%UnityEngineUI%,%AssemblyCSharp% -out:%OutPut% -nowarn:436 -optimize -unsafe  -recurse:%scripts% >> %LogFile% 2>&1
@echo ----------------------------------------------------------------------------- >> %LogFile%
exit /b %ErrorLevel%