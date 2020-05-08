.PHONY: build test format

build:
	dotnet build
test: build
	dotnet test -v n --no-build
format:
	dotnet-format
