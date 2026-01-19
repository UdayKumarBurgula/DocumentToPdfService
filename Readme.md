mkdir DocumentToPdfService
cd DocumentToPdfService

dotnet new sln -n DocumentToPdfService

dotnet new classlib -n DocumentToPdf.Core
dotnet new webapi   -n DocumentToPdf.Api

dotnet sln add DocumentToPdf.Core/DocumentToPdf.Core.csproj
dotnet sln add DocumentToPdf.Api/DocumentToPdf.Api.csproj

dotnet add DocumentToPdf.Api/DocumentToPdf.Api.csproj reference DocumentToPdf.Core/DocumentToPdf.Core.csproj

dotnet add DocumentToPdf.Core package Aspose.Words

DocumentToPdfService\DocumentToPdf.Api>dotnet add package Swashbuckle.AspNetCore --version 6.5.0


DocumentToPdfService>curl -X POST http://localhost:5147/api/convert/to-pdf  -F "file=@sample.docx" --output out.pdf
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed
100   232    0    17  100   215     59    757 --:--:-- --:--:-- --:--:--   816

DocumentToPdfService>


-------------------------------------------------------------
Why this design is good
----------------------------------------------
Design Choice	        Benefit
---------------------------------------
Interface	            Easy to mock and test
Singleton	            Efficient
Stream-based	        Memory friendly
Switch routing	        Extensible
MemoryStream output	    API friendly
Async signature	        ASP.NET compatible

The converter is registered as Singleton because it is stateless and thread-safe, so one shared instance is optimal. 
The ConvertToPdfAsync method accepts a stream to avoid unnecessary memory usage, 
detects file type from the filename, routes to the correct Aspose converter, 
converts the document entirely in memory using MemoryStream, and returns the PDF as a byte array. 
The async signature aligns with ASP.NET’s asynchronous request pipeline.

-------------------------------------------------------------------

git init
dotnet new gitignore
git status
git add .
git commit -m "Initial .NET 8 microservice with Aspose PDF conversion"
git status
DocumentToPdfService>git remote add origin https://github.com/UdayKumarBurgula/DocumentToPdfService.git
DocumentToPdfService>git branch -M main
DocumentToPdfService>git push -u origin main --force

-------------------------------------------------------------
-------------------------------------------------------------
Aim:
1) Host DocumentToPdf.Core library as nugget package - local feed (best for learning) 
2) Test from webapi as docker or k8s container

------------------------
Part A — Make DocumentToPdf.Core a NuGet package (local feed)
-------------------------------------------------------------

1. Host DocumentToPdf.Core as local nugget package
    a. Add configuration to DocumentToPdf.Core/DocumentToPdf.Core.csproj
    <!-- NuGet packing -->
    <PackageId>DocumentToPdf.Core</PackageId>
  
2. Create a local folder for nugget packages
   command: mkdir -p .nuget-local

3. Pack the DocumentToPdf.Core project
   command: dotnet pack DocumentToPdf.Core/DocumentToPdf.Core.csproj -c Release -o ./.nuget-local
   output:  ./.nuget-local/DocumentToPdf.Core.1.0.0.nupkg

4. add NuGet.config (OR) re-check the below entry.
   <add key="local" value="./.nuget-local" />
   
------------------------
Part B — Consume the local NuGet package from your WebAPI
----------------------------------------------------------
1. Reference local DocumentToPdf.Core in DocumentToPdf.Api
    a) In DocumentToPdf.Api.csproj, remove:
       <ProjectReference Include="..\DocumentToPdf.Core\DocumentToPdf.Core.csproj" />
    b) Add the package reference instead
       dotnet add DocumentToPdf.Api package DocumentToPdf.Core --version 1.0.0
    c) restore and build
        dotnet restore
        dotnet build
    
2. When you change library code.
    1. Update Core csproj version:
        <Version>1.0.1</Version>
    2. Pack again 
        dotnet pack DocumentToPdf.Core/DocumentToPdf.Core.csproj -c Release -o ./.nuget-local
    3. Update API package:
        dotnet add DocumentToPdf.Api package DocumentToPdf.Core --version 1.0.1

---------------------
Part C — Run WebAPI as Docker container
--------------------------------------------------

1. Create Dockerfile in DocumentToPdf.Api folder
   FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
   WORKDIR /app
   EXPOSE 80

   FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
   WORKDIR /src
   COPY ["DocumentToPdf.Api/DocumentToPdf.Api.csproj", "DocumentToPdf.Api/"]
   RUN dotnet restore "DocumentToPdf.Api/DocumentToPdf.Api.csproj"
   COPY . .
   WORKDIR "/src/DocumentToPdf.Api"
   RUN dotnet build "DocumentToPdf.Api.csproj" -c Release -o /app/build

   FROM build AS publish
   RUN dotnet publish "DocumentToPdf.Api.csproj" -c Release -o /app/publish

   FROM base AS final
   WORKDIR /app
   COPY --from=publish /app/publish .
   ENTRYPOINT ["dotnet", "DocumentToPdf.Api.dll"]

2. Build image from solution root
     docker build -t doc2pdf:local -f DocumentToPdf.Api/Dockerfile .

3. Run container
	 docker run --rm -p 8080:8080 doc2pdf:local



-------------------------------------------------------------
-------------------------------------------------------------
winget install Kubernetes.kubectl
winget install Kubernetes.minikube
kubectl version --client
minikube version

minikube start --driver=docker

Why Minikube is needed for your learning
--------------------------
Without Minikube:
Docker = containers only
Minikube = real Kubernetes cluster
So:
Docker teaches containers
Minikube teaches orchestration

How your PDF service fits
---------------------------
Your flow becomes:
NuGet package → WebAPI → Docker image → Minikube → Kubernetes Service

Minikube provides a local single-node Kubernetes cluster that allows me to test containerized microservices 
without needing a cloud Kubernetes environment.

Next step after install
-------------------------------
minikube image load doc2pdf:local
kubectl apply -f k8s.yaml
kubectl get pods
kubectl get svc

DocumentToPdfService>kubectl port-forward svc/doc2pdf-svc 8081:80
Forwarding from 127.0.0.1:8081 -> 8080
Forwarding from [::1]:8081 -> 8080
Handling connection for 8081
Handling connection for 8081