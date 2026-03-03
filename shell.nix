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
  # huskyDotNET = pkgs.buildDotnetGlobalTool {
  #   pname = "Husky";
  #   executables = [ "husky" ];
  #   # dotnet-runtime = currentSdk;
  #   dotnet-runtime = pkgs.dotnet-sdk_8;
  #   version = "0.8.0";
  #   nugetHash = "sha256-WBCOF5aEVjQZRzOwrS4W9TZ6+lSFUEL/Ej+f6RPy+KQ=";
  # };
in
pkgs.mkShell {
  nativeBuildInputs = with pkgs; [
    roslyn-ls
    currentSdk
    dotnet-ef
    dotnet-aspnetcore_10
    reportgenerator
    nodejs_latest
    # huskyDotNET
    husky
    openssl
    icu
    zlib
  ];

  shellHook = ''
    export DOTNET_ROOT="${currentSdk}/share/dotnet";
    export MSBuildSDKsPath="${currentSdk}/share/dotnet/sdk/${currentSdk.version}/Sdks";
    export LD_LIBRARY_PATH="${pkgs.icu}/lib:${pkgs.openssl}/lib:${pkgs.zlib}/lib:$LD_LIBRARY_PATH";
    export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
    alias project-tree="tree -I 'obj|bin|TestResults|coveragereport'"
  '';
}
