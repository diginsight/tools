echo on
set env=dev1
IF NOT DEFINED UpdateConfig set UpdateConfig=true

rem dir

echo  IF /I %UpdateConfig%==True copy /Y/V "%ProjectDir%appsettings.%env%.json" "%ProjectDir%appsettings.json"
IF /I %UpdateConfig%==True copy /Y/V "%ProjectDir%appsettings.%env%.json" "%ProjectDir%appsettings.json"


