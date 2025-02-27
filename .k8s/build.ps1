function BuildApiImage() {
    Write-Host "Building image..."
    docker build --file ./Project.WebApi/Dockerfile --no-cache --tag project.webapi:v1 .
    
    Set-Location ../../.k8s
}

BuildApiImage