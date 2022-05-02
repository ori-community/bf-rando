msbuild /t:Build /restore /p:Configuration=Release /p:OutDir=..\build

rm -R .\release -ErrorAction SilentlyContinue

mkdir .\release\Mods\Assets\OriDeRandomiser\
mkdir .\release\Mods\strings\Randomiser\

cp .\build\OriDeRandomiser.dll .\release\Mods\
cp .\Randomiser\Assets\* .\release\Mods\Assets\OriDeRandomiser\
cp .\Randomiser\strings\* .\release\Mods\strings\Randomiser\
cp .\convertseed.ps1 .\release\

rm .\OriDeRandomiser.zip -ErrorAction SilentlyContinue
powershell Compress-Archive .\release\* .\OriDeRandomiser.zip
