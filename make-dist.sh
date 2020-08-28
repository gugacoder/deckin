#!/bin/bash

target=ProcessaApp
version=v0.1.0

build ()
{
  runtime=$1
  extension=$2
  
  mkdir -p Dist
  
  output=Dist/${target}-${version}-${runtime}.${extension}
  folder=Director/bin/Release/netcoreapp3.1/${runtime}/publish
  
  echo
  echo [STAGE]BUILDING...
  echo
  
  #dotnet publish -c Release -r ${runtime} /p:PublishSingleFile=true /p:PublishTrimmed=true Director/Director.csproj
  dotnet publish --self-contained -c Release -r ${runtime} Director/Director.csproj
  echo ${version}-${runtime} > ${folder}/REVISION.txt
  
  echo
  echo [STAGE]PACKING ${output}...
  echo
  
  ln -s ${folder} ${target}
  
  if [ $extension == "tar.gz" ]; then
    tar -czvf ${output} ${target}/*
  else
    zip ${output} ${target}/*
  fi
  
  rm ${target}
  
  echo
  echo [OK]PACKAGE BUILT:
  echo "  ${output}"
  echo
}

build linux-x64 tar.gz
build osx-x64 tar.gz
build win-x64 zip

echo
echo [OK]ALL DONE!
find dist | grep x64
echo