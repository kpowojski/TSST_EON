@echo off

echo Podaj ile komponentow potrzebujesz:

echo ClientNode
set/p "ClientNode=>>"
echo NetworkNode
set/p "NetworkNode=>>"


for /L %%A in (1,1,%ClientNode%) do (
	START ClientNode\ClientNode\bin\Debug\ClientNode.exe
	)

for /L %%A in (1,1,%NetworkNode%) do (
	START NetworkNode\NetworkNode\bin\Debug\NetworkNode.exe
	)

START NetworkCloud\NetworkCloud\bin\Debug\NetworkCloud.exe
START NetworkManager\NetworkManager\bin\Debug\NetworkManager.exe
START NetworkModelViewer\NetworkModelViewer\bin\Debug\NetworkModelViewer.exe


end
