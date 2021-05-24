default: start

start:
	dotenv -- dotnet run --no-launch-profile -v d -p ./Nov.Caps.Int.D365/Nov.Caps.Int.D365.Server/Nov.Caps.Int.D365.Server.csproj

build:
	dotnet build ./Nov.Caps.Int.D365

clean:
	dotnet clean ./Nov.Caps.Int.D365
