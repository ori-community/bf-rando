msbuild /t:Build /restore /p:Configuration=Release /p:OutDir=..\build

rm -R .\release -ErrorAction SilentlyContinue

mkdir .\release\
mkdir .\release\assets\
mkdir .\release\strings\

cp .\build\OriDeRandomiser.dll .\release\
cp .\build\Archipelago.MultiClient.Net.dll .\release\
cp .\build\websocket-sharp.dll .\release\
cp .\build\Newtonsoft.json.dll .\release\
cp .\Randomiser\Assets\* .\release\assets\ -Recurse
cp .\Randomiser\strings\* .\release\strings\
cp .\convertseed.ps1 .\release\
cp .\mod.json .\release\

rm .\OriDeRandomiser.zip -ErrorAction SilentlyContinue
powershell Compress-Archive .\release\* .\OriDeRandomiser.zip
