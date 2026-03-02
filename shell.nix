{ pkgs ? import <nixpkgs> {} }:

let
  currentSdk = pkgs.dotnet-sdk_10;
  reportgenerator = pkgs.buildDotnetGlobalTool {
    pname = "dotnet-reportgenerator-globaltool";
    version = "5.5.2";
    nugetHash = "sha256-5THxbApvqbX08HQ6YtNhtqwmURe67+njmCNLy8RAaJM=";
    dotnet-runtime = currentSdk;
    executables = [ "reportgenerator" ];
  };
in
pkgs.mkShell {
  nativeBuildInputs = with pkgs; [
    roslyn-ls
    currentSdk
    dotnet-ef
    dotnet-aspnetcore_10
    reportgenerator
  ];
}
