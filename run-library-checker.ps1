function RunLibraryChecker {
    param (
        [string]$AssemblyPath
    )
    try {
        [System.Reflection.Assembly]::LoadFrom($AssemblyPath)
    }
    catch {
        Write-Output "::error title=::Failed to LoadFrom $AssemblyPath"
        exit 1
    }
}