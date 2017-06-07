if(Test-Path .\artifacts) {
    Remove-Item .\artifacts -Force -Recurse
}

dotnet restore
dotnet build

dotnet test .\Protacon.NetCore.WebApi.TestUtil.Tests\Protacon.NetCore.WebApi.TestUtil.Tests.csproj

$version = if($env:APPVEYOR_REPO_TAG_NAME) {
    $env:APPVEYOR_REPO_TAG_NAME
} else {
    "0.0.1-beta$env:APPVEYOR_BUILD_NUMBER"
}

dotnet pack .\Protacon.NetCore.WebApi.TestUtil\Protacon.NetCore.WebApi.TestUtil.csproj -c Release -o ..\artifacts /p:Version=$version