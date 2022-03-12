function RunLibraryChecker {
    param (
        [string]$AssemblyPath
    )
    try {
        $assembly = [System.Reflection.Assembly]::LoadFrom($AssemblyPath)
        Write-Output "::group::DefinedTypes"
        Write-Output $assembly.DefinedTypes
        Write-Output "::endgroup::"
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
        Write-Output "::group::python generate.py"
        $names = ($solvers | ForEach-Object Name)
        Write-Output "Run: python generate.py -p $names"
        python generate.py -p $names
        Write-Output "::endgroup::"
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
        Write-Output "::group::Run $($solver.Name)"
        $targetDir = [System.IO.DirectoryInfo](Get-ChildItem "$libraryCheckerProblemsDir" -Recurse -Include $solver.Name)
        if (-not [bool]$targetDir) {
            $failure = $true
            Write-Output "::error::Failed to get solver $($solver.Name)"
        }
        Write-Output "::endgroup::"
    }

    if ($failure) {
        exit 1
    }
}