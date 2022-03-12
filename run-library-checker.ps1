$libraryCheckerProblemsDir = "$PSScriptRoot/library-checker-problems"
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

    $failedSolvers = [System.Collections.Generic.List[string]]::new()
    foreach ($solver in $solvers) {
        Write-Output "::group::Run $($solver.Name)"
        try {
            RunCheckers -solver $solver
        }
        catch {
            $message = $_.Exception.Message
            $failedSolvers.Add($solver.Name)
            Write-Output "::error::$message"
        }
        Write-Output "::endgroup::"
    }

    if ($failedSolvers.Count -gt 0) {
        Write-Output "::error::Failed solvers: $($failedSolvers -join ', ')"
        exit 1
    }
}

function RunCheckers {
    param (
        [Kzrnm.Competitive.LibraryChecker.ICompetitiveSolver]$solver
    )
    $targetDir = [System.IO.DirectoryInfo](Get-ChildItem "$libraryCheckerProblemsDir" -Recurse -Include $solver.Name)
    Write-Output "targetDir: $targetDir"
    if (-not $targetDir) {
        throw "Failed to get solver $($solver.Name)"
    }
    mkdir "$targetDir/got"
    foreach ($inputFile in (Get-ChildItem "$targetDir/in/*.in")) {
        $fileName = $inputFile.BaseName
        $inStream = [System.IO.FileStream]::new($inputFile.FullName, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read)
        $outStream = [System.IO.FileStream]::new("$targetDir/got/$fileName.got", [System.IO.FileMode]::Create, [System.IO.FileAccess]::ReadWrite)
        try {
            Write-Output "Run: $($solver.Name) $fileName"
            [Kzrnm.Competitive.LibraryChecker.CompetitiveSolvers]::RunSolverWithTimeout($solver, $inStream, $outStream, 0.00002)
        }
        catch {
            throw "timeout $($solver.Name) $fileName"
        }
        finally {
            $inStream.Dispose()
            $outStream.Dispose()
        }

        try {
            Push-Location $libraryCheckerProblemsDir
            Write-Output "Check: $($solver.Name) $fileName"
            . "$targetDir/checker" "$targetDir/in/$fileName.in" "$targetDir/out/$fileName.out" "$targetDir/got/$fileName.got"
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to check $($solver.Name) $fileName"
            }
        }
        catch {
            throw "Failed to check $($solver.Name) $fileName"
        }
        finally {
            Pop-Location
        }
    }
}