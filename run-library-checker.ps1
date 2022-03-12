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

    $failure = $false
    foreach ($solver in $solvers) {
        $targetDir = [System.IO.DirectoryInfo](Get-ChildItem "$PSScriptRoot/library-checker-problems" -Recurse -Include ($solver.Name+"2"))
        if (-not [bool]$targetDir) {
            $failure = $true
            Write-Output "::error::Failed to get solver $($solver.Name)"
        }
    }

    if ($failure) {
        exit 1
    }
}