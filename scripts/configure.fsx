#!/usr/bin/env fsharpi

open System
open System.IO
open System.Linq

#r "System.Configuration"
#load "InfraLib/Misc.fs"
#load "InfraLib/Process.fs"
#load "InfraLib/Git.fs"
open FSX.Infrastructure
open Process

let ConfigCommandCheck (commandName: string) =
    Console.Write (sprintf "checking for %s... " commandName)
    if not (Process.CommandWorksInShell commandName) then
        Console.Error.WriteLine "not found"
        Console.Error.WriteLine (sprintf "configuration failed, please install \"%s\"" commandName)
        Environment.Exit 1
    Console.WriteLine "found"

ConfigCommandCheck "make"
ConfigCommandCheck "fsharpc"
ConfigCommandCheck "mono"

// needed by NuGet.Restore.targets & the "update-servers" Makefile target
ConfigCommandCheck "curl"

let oldVersionOfMono =
    // we need this check because Ubuntu 18.04 LTS still brings a very old version of Mono (4.6.2) with no msbuild
    let versionOfMonoWhereTheRuntimeBugWasFixed = "5.4"

    match Misc.GuessPlatform() with
    | Misc.Platform.Windows ->
        // not using Mono anyway
        false
    | Misc.Platform.Mac ->
        // unlikely that anyone uses old Mono versions in Mac, as it's easy to update (TODO: detect anyway)
        false
    | Misc.Platform.Linux ->
        let pkgConfig = "pkg-config"
        ConfigCommandCheck pkgConfig
        let pkgConfigCmd = { Command = pkgConfig
                             Arguments = sprintf "--atleast-version=%s mono" versionOfMonoWhereTheRuntimeBugWasFixed }
        let processResult = Process.Execute(pkgConfigCmd, Echo.OutputOnly)
        processResult.ExitCode <> 0

let buildTool =
    if oldVersionOfMono then
        "xbuild"
    else
        "msbuild"
ConfigCommandCheck buildTool

let prefix = DirectoryInfo(Misc.GatherOrGetDefaultPrefix(Misc.FsxArguments(), false, None))

if not (prefix.Exists) then
    let warning = sprintf "WARNING: prefix doesn't exist: %s" prefix.FullName
    Console.Error.WriteLine warning

let lines =
    let toConfigFileLine (keyValuePair: System.Collections.Generic.KeyValuePair<string,string>) =
        sprintf "%s=%s" keyValuePair.Key keyValuePair.Value

    Map.empty.Add("Prefix", prefix.FullName)
             .Add("BuildTool", buildTool)
    |> Seq.map toConfigFileLine

let path = Path.Combine(__SOURCE_DIRECTORY__, "build.config")
File.WriteAllLines(path, lines |> Array.ofSeq)

let rootDir = DirectoryInfo(Path.Combine(__SOURCE_DIRECTORY__, ".."))
let version = Misc.GetCurrentVersion(rootDir)

let repoInfo = Git.GetRepoInfo()

Console.WriteLine()
Console.WriteLine(sprintf
                      "\tConfiguration summary for gwallet %s %s"
                      (version.ToString()) repoInfo)
Console.WriteLine()
Console.WriteLine(sprintf
                      "\t* Installation prefix: %s"
                      prefix.FullName)
Console.WriteLine()

Console.WriteLine "Configuration succeeded, you can now run `make`"
