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


