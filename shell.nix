{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  nativeBuildInputs = with pkgs; [
    roslyn-ls
    dotnet-sdk_10
  ];
}
