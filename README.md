# Grpc-Demo

The proposal of this project is to serve as a demo on implementing gRPC communication and another technologies, like RavenDb, MediatR, etc.

In this project we have Client and Server in the same solution for demo purposes only. In a real-world application, Server and Client could stay in separated solutions and git repositories.

## Technologies and Features

* ASP.Net Core 3.1
* gRPC
* Protobuf
* HTTP/2
* MediatR
* FluentResults
* gRPC Interceptors (request logger and error handling)
* ASP.Net Core Authentication and Authorization
* RavenDb Embedded - in a real-world application you should use a [cloud](https://cloud.ravendb.net/) or a on-premisses version

## How to execute?

1. Start the **GrpcDemo.Api** application (this is the gRPC server);
2. Right-click on the **GrpcDemo.Client** application (this is a gRPC console client), select 'Debug' in the context menu and click on 'Start New Instance';
3. Otherwise, you can use the BloomRPC to call the RPC services loading the Protobuf files that exists in the 'Protos' directory;

## MediatR Notes

The MediatR was created by the same AutoMapper author, Jimmy Bogard.

Simple mediator implementation in .NET

In-process messaging with no dependencies.

Supports request/response, commands, queries, notifications and events, synchronous and async with intelligent dispatching via C# generic variance.

[See more about MediatR here](https://github.com/jbogard/MediatR)

[MediatR com ASP.Net Core - my personal PT-BR blog](https://www.wellingtonjhn.com/posts/mediatr-com-asp.net-core/)

[Fail-fast Validations com Pipeline Behavior no MediatR e ASP.Net Core - my personal PT-BR blog](https://www.wellingtonjhn.com/posts/fail-fast-validations-com-pipeline-behavior-no-mediatr-e-asp.net-core/)


## RavenDB Notes

The RavenDb is a NoSQL (document) database written in .Net and uses Lucene as search engine. Is a direct concurrent for MongoDb.

In this project, the RavenDb Management Studio will be loaded automatically when the application starts, to disable this behavior change the following configuration on 'appsettings.json':

> "OpenRavenDbStudioInBrowser": false

[See more about RavenDb Cloud here](https://cloud.ravendb.net/)

## BloomRPC Notes

The BloomRPC is a GUI Client for test your GRPC services.

I recommend that you use the BloomRPC to test your RPC services. Just load the Protobuf file (*.proto), enter the service address Url, call the RPC methods with the required inputs, and be happy! :)

[See more about BloomRPC here](https://github.com/uw-labs/bloomrpc)

## References

* [grpc.io](https://grpc.io/)
* [Protocol Buffers](https://developers.google.com/protocol-buffers)
* [HTTP/2](https://developers.google.com/web/fundamentals/performance/http2)
* [Microsoft - Introduction to gRPC on .NET Core](https://docs.microsoft.com/en-us/aspnet/core/grpc)
* [Microsoft - Compare gRPC services with HTTP APIs](https://docs.microsoft.com/en-us/aspnet/core/grpc/comparison)
* [.Net gRPC Samples](https://github.com/grpc/grpc-dotnet/tree/master/examples)
* [Introducing gRPC HTTP API (Rest + Swagger support)](http://james.newtonking.com/archive/2020/03/31/introducing-grpc-http-api)
* [gRPC Web](https://github.com/grpc/grpc-web)
* [Awesome gRPC](https://github.com/grpc-ecosystem/awesome-grpc)
