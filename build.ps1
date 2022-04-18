msbuild /t:Build /restore /p:Configuration=Release /p:OutDir=..\build

rm -R .\release -ErrorAction SilentlyContinue

mkdir .\release\Mods\Assets\OriDeRandomiser\

cp .\build\OriDeRandomiser.dll .\release\Mods\
cp .\build\OriDeUILib.dll .\release\Mods\
cp .\Randomiser\Assets\LocationData.json .\release\Mods\Assets\OriDeRandomiser\
cp .\convertseed.ps1 .\release\

rm .\OriDeRandomiser.zip -ErrorAction SilentlyContinue
powershell Compress-Archive .\release\* .\OriDeRandomiser.zip
