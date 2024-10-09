function BuildApiImage() {
    Write-Host "Changing directory..."
    Set-Location ../src/Project
    
    Write-Host "Building image..."
    docker build --file ./Project.WebApi/Dockerfile --no-cache --tag project.webapi:v1 .
    
    Set-Location ../../.k8s
}

BuildApiImage