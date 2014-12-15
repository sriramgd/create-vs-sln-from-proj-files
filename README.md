create-vs-sln-from-proj-files
=============================

Create one visual studio solution (.sln) file per project file recursively in a directory.


Install
=======
Open solution in Visual Studio and build. The executable should be available in \bin\debug directory
OR
In a command prompt:
    cd <path-to-git-repos>\create-vs-sln-from-proj-files
    msbuild

Usage
=====
The binary CreateSln.exe should be created in the bin\debug directory.
In command prompt: 
    CreateSln <rootDir>

where rootDir is the directory for which .sln files need to be created for project (.csproj and .vbproj) files.
rootDir is searched recursively for project files, and solution file created with the same name.
If a solution file with the same name already exists for a project, nothing is generated for that project.

