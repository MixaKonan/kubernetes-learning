function BuildApiImage() {
    Write-Host "Changing directory..."
    cd ../src/Project
    
    Write-Host "Building image..."
    docker build --file ./Project.WebApi/Dockerfile --no-cache --tag project.webapi:v1 .
    
    cd ../../.k8s
}

BuildApiImage