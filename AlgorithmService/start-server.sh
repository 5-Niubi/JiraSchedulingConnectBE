kill -9 $(lsof -ti:5289)
rm -r bin
dotnet run --project AlgorithmService.csproj