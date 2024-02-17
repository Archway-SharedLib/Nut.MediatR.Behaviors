# dotnet tool install -g dotnet-reportgenerator-globaltool

Param([switch]$noReport = $false)

dotnet test ./test/Nut.MediatR.Behaviors.Test/Nut.MediatR.Behaviors.Test.csproj `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput=..\..\Nut.MediatR.Behaviors.coverage.xml `
    /p:ExcludeByAttribute=CompilerGenerated

dotnet test ./test/Nut.MediatR.Behaviors.FluentValidation.Test/Nut.MediatR.Behaviors.FluentValidation.Test.csproj `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput=..\..\Nut.MediatR.Behaviors.FluentValidation.coverage.xml `
    /p:ExcludeByAttribute=CompilerGenerated

if(!$noReport) {
    dotnet tool run reportgenerator "-reports:./*.coverage.xml" `
        -targetdir:coveragereport `
        -reporttypes:Html
}
