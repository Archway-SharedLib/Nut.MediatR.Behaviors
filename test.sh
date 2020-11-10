# dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet test ./test/Nut.MediatR.Behaviors.Test/Nut.MediatR.Behaviors.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=../../Nut.MediatR.Behaviors.coverage.xml
reportgenerator "-reports:./Nut.MediatR.Behaviors.coverage.xml" "-targetdir:coveragereport" -reporttypes:Html
