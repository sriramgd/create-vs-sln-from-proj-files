using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        const string visualStudioProgID = "VisualStudio.Solution";
        static Type solutionObjectType = System.Type.GetTypeFromProgID(visualStudioProgID, true);
        static object obj = System.Activator.CreateInstance(solutionObjectType, true);
        static EnvDTE80.Solution2 solutionObject = (EnvDTE80.Solution2)obj;
        static Dictionary<string, string> allCsProjAndSlnFiles = new Dictionary<string, string>();

        static void LogToConsole(string mesg, bool isError)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(mesg ?? string.Empty);
            Console.ResetColor();
        }

        static void CreateSolution(string fullcsprojPath, string fullslnPath)
        {
            try
            {
                var slnName = Path.GetFileNameWithoutExtension(fullslnPath);
                var slnPath = Path.GetDirectoryName(fullslnPath);
                solutionObject.Create(slnPath, slnName);
                solutionObject.AddFromFile(fullcsprojPath);
                solutionObject.SaveAs(fullslnPath);
                LogToConsole("SAVED " + fullslnPath + " OK", false);
            }
            catch (Exception e)
            {
                LogToConsole(Environment.NewLine + "FAILED " + fullcsprojPath, true);
                LogToConsole(e.Message + Environment.NewLine, true);
            }
        }


        //Populate dictionary with cs and vb project files, and their proposed solution names.
        //The proposed solution name is the same as the project file, only with different (.sln) extension.
        //So A.csproj will have A.sln as its proposed solution name.
        //Skip any solutions with the same name which already exist.
        //So we dont overwrite solutions. To re-generate, we need to skip the file.
        static void PopulateAllCsProjAndProposedSlnFiles(string rootDir)
        {           
            Directory.EnumerateFiles(rootDir).Where(f => f.Trim().ToUpper().EndsWith(".CSPROJ") || f.Trim().ToUpper().EndsWith(".VBPROJ")).
                ToList().
                ForEach(c =>
                {
                    if (!allCsProjAndSlnFiles.ContainsKey(c))
                    {
                        var slnFile = Path.Combine(Path.GetDirectoryName(c), 
                                          Path.GetFileNameWithoutExtension(c) + ".sln");
                        if (!File.Exists(slnFile))
                        {
                            allCsProjAndSlnFiles.Add(c, slnFile);
                        }
                        else
                        {
                            LogToConsole("SKIPPED " + slnFile + ", already exists. To overwrite manually delete this solution file", false);
                        }
                    }
                });
            Directory.GetDirectories(rootDir).ToList().ForEach(d => PopulateAllCsProjAndProposedSlnFiles(d));
        }

        static void CreateSolutionFiles() {
            allCsProjAndSlnFiles.ToList().ForEach(c => CreateSolution(c.Key, c.Value));
        }

        static void Usage()
        {
            Console.WriteLine("Usage: CreateSln <rootDir>");
            Console.Write(", where rootDir is the directory for which .sln files need to be created for project (.csproj and .vbproj) files. ");
            Console.Write("rootDir is searched recursively for project files, and solution file created with the same name. ");
            Console.WriteLine("If a solution file with the same name already exists for a project, nothing is generated for that project.");
        }

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Usage();
                return -1;
            }
            string rootDir = args[0];
            if (!Directory.Exists(rootDir))
            {
                Usage();
                return -2;
            }
            PopulateAllCsProjAndProposedSlnFiles(rootDir);
            CreateSolutionFiles();
            return 0;
        }
    }
}
