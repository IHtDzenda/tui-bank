with import <nixpkgs> {};

mkShell {
  name = "dotnet-env";
  packages = [
    (with dotnetCorePackages; combinePackages [
      sdk_8_0
      sdk_9_0
    ])
  ];
}
