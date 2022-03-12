function RunLibraryChecker {
    param (
        [string]$AssemblyPath
    )
    try {
        $assembly = [System.Reflection.Assembly]::LoadFrom($AssemblyPath)
        Write-Output $assembly.DefinedTypes
    }
    catch {
        Write-Output "::error::Failed to LoadFrom $AssemblyPath"
        exit 1
    }

    try {
        $solvers = [Kzrnm.Competitive.LibraryChecker.CompetitiveSolvers]::GetSolvers($assembly)
    }
    catch {
        Write-Output "::error::Failed to get solvers"
        exit 1
    }

    $libraryCheckerProblemsDir = "$PSScriptRoot/library-checker-problems"

    try {
        Push-Location $libraryCheckerProblemsDir
        python generate.py -p ($solvers | ForEach-Object Name)
    }
    catch {
        Write-Output "::error::Failed to generate"
        exit 1
    }
    finally {
        Pop-Location
    }

    $failure = $false
    foreach ($solver in $solvers) {
        $targetDir = [System.IO.DirectoryInfo](Get-ChildItem "$libraryCheckerProblemsDir" -Recurse -Include $solver.Name)
        if (-not [bool]$targetDir) {
            $failure = $true
            Write-Output "::error::Failed to get solver $($solver.Name)"
        }
    }

    if ($failure) {
        exit 1
    }
}