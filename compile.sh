#!/bin/bash
dotnet build -c Release
dotnet publish -c Release -r win10-x64 --self-contained
