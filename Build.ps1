if(Test-Path $PSScriptRoot\Protacon.NetCore.WebApi.TestUtil\artifacts) {
    Remove-Item $PSScriptRoot\Protacon.NetCore.WebApi.TestUtil\artifacts -Force -Recurse
}

dotnet restore
dotnet build

dotnet test $PSScriptRoot\Protacon.NetCore.WebApi.TestUtil.Tests\Protacon.NetCore.WebApi.TestUtil.Tests.csproj

$version = if($env:APPVEYOR_REPO_TAG) {
    "$env:APPVEYOR_REPO_TAG_NAME"
} else {
    "0.0.1-beta$env:APPVEYOR_BUILD_NUMBER"
}

dotnet pack $PSScriptRoot\Protacon.NetCore.WebApi.TestUtil\Protacon.NetCore.WebApi.TestUtil.csproj -c Release -o $PSScriptRoot\Protacon.NetCore.WebApi.TestUtil\artifacts /p:Version=$version