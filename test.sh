# dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet test ./test/Nut.MediatR.Behaviors.Test/Nut.MediatR.Behaviors.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=../../Nut.MediatR.Behaviors.coverage.xml
dotnet test ./test/Nut.MediatR.Behaviors.FluentValidation.Test/Nut.MediatR.Behaviors.FluentValidation.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=../../Nut.MediatR.Behaviors.FluentValidation.coverage.xml

reportgenerator "-reports:./*.coverage.xml" "-targetdir:coveragereport" -reporttypes:Html
