// Fill out your copyright notice in the Description page of Project Settings.

using System.IO;
using UnrealBuildTool;

public class BoostLibrary : ModuleRules
{
	
	string GetPlatformString ()
	{
		return (Target.Platform == UnrealTargetPlatform.Win64) ? "x64" : "x32";
	}

	string GetRootPath()
	{
		return Path.GetFullPath(Path.Combine(ModuleDirectory, "../../"));
	}
	
	private string ModulePath
	{
		get { return ModuleDirectory; }
	}

	private string BinariesPath
	{
		get { return Path.GetFullPath(Path.Combine(ModulePath, "../Binaries/")); }
	}

	public void LoadBoostNuget()
	{
		const string LibraryPath = "../packages/boost.1.79.0/lib/native/include";//Theoretically "NuGet packages" are inside 'packages' dir

		PublicIncludePaths.Add(LibraryPath);
			
		string PlatformString = GetPlatformString();
		string PackagesDir = GetRootPath() + "packages";
		string boostVersionInDir = "1_79";
		string visualStudioCompiler = "vc142";
		foreach (var d in System.IO.Directory.GetDirectories(PackagesDir))
		{
			var dirName = new System.IO.DirectoryInfo(d).Name;
			if (dirName == "boost.1.79.0")
			{
				continue;
			}
			string[] splittedString = dirName.Split('-');
			PublicAdditionalLibraries.Add(System.IO.Path.Combine(PackagesDir, dirName + "/lib/native", "lib" + splittedString[0] + "-" + visualStudioCompiler + "-mt-" + PlatformString + "-" + boostVersionInDir + ".lib"));
			PublicAdditionalLibraries.Add(System.IO.Path.Combine(PackagesDir, dirName + "/lib/native", "lib" + splittedString[0] + "-" + visualStudioCompiler + "-mt-gd-" + PlatformString + "-" + boostVersionInDir + ".lib"));
			// PublicAdditionalLibraries.Add(System.IO.Path.Combine(PackagesDir, dirName + "/lib/native", "lib" + splittedString[0] + "-" + visualStudioCompiler + "-mt-sgd-" + PlatformString + "-" + boostVersionInDir + ".lib"));
			// PublicAdditionalLibraries.Add(System.IO.Path.Combine(PackagesDir, dirName + "/lib/native", "lib" + splittedString[0] + "-" + visualStudioCompiler + "-mt-s-" + PlatformString + "-" + boostVersionInDir + ".lib"));
		}

		PublicDefinitions.Add(string.Format("WITH_BOOST_BINDING={0}", 1));
		
	}

	public void LoadBoost()
	{
		PublicAdditionalLibraries.Add(Path.Combine(ModulePath, "lib/libboost_chrono-vc141-mt-1_64.lib"));
		PublicAdditionalLibraries.Add(Path.Combine(ModulePath, "lib/libboost_date_time-vc141-mt-1_64.lib"));
		PublicAdditionalLibraries.Add(Path.Combine(ModulePath, "lib/libboost_filesystem-vc141-mt-1_64.lib"));
		PublicAdditionalLibraries.Add(Path.Combine(ModulePath, "lib/libboost_iostreams-vc141-mt-1_64.lib"));
		PublicAdditionalLibraries.Add(Path.Combine(ModulePath, "lib/libboost_system-vc141-mt-1_64.lib"));
		PublicAdditionalLibraries.Add(Path.Combine(ModulePath, "lib/libboost_thread-vc141-mt-1_64.lib"));
		PublicIncludePaths.Add(Path.Combine(ModulePath, "include/boost-1_64"));
		
		// Not sure if needed
		PublicDefinitions.Add("_CRT_SECURE_NO_WARNINGS=1");
		PublicDefinitions.Add("BOOST_DISABLE_ABI_HEADERS=1");
		
		
		// Needed configurations in order to run Boost
		bUseRTTI = true;
		bEnableExceptions = true;
		
		PublicDefinitions.Add(string.Format("WITH_PCL_BINDING={0}", 1));
		PublicDefinitions.Add(string.Format("WITH_BOOST_BINDING={0}", 1));
	}
	
	public BoostLibrary(ReadOnlyTargetRules Target) : base(Target)
	{
		Type = ModuleType.External;

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			LoadBoost();
		}
	}

}

