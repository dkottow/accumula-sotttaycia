
# About The Project
Web Service to insert rows into SQL database. Implemented as an Azure Function.

# Getting Started
## Prerequisites
1. Dot.Net 3.1
2. Azure CLI / Azure Functions
## Installation
1. Clone the repo
2. Compile library with `cd src/libaccumula` and `dotnet build`
3. Compile and run tests with `cd src/test`...
4. Log into your Azure tenant with `az login`
5. Publish function with `cd src/azfunc` and `func azure functionapp publish accumula-sottaycia`
