# dotnet tool install -g dotnet-reportgenerator-globaltool
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

dotnet test ./test/Nut.MediatR.ServiceLike.Test/Nut.MediatR.ServiceLike.Test.csproj `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput=..\..\Nut.MediatR.ServiceLike.coverage.xml `
    /p:ExcludeByAttribute=CompilerGenerated

dotnet test ./test/Nut.MediatR.ServiceLike.DependencyInjection.Test/Nut.MediatR.ServiceLike.DependencyInjection.Test.csproj `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput=..\..\Nut.MediatR.ServiceLike.DependencyInjection.coverage.xml `
    /p:Include="[Nut.MediatR.ServiceLike.DependencyInjection*]*" `
    /p:ExcludeByAttribute=CompilerGenerated

reportgenerator "-reports:.\*.coverage.xml" `
    -targetdir:coveragereport `
    -reporttypes:Html
