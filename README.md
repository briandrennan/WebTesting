# Web Testing

This repo provides examples of how to perform two kinds of testing:

- End-to-end tests of an ASP.NET Core Web API and hand-coded client
- Behavior testing using SpecFlow.NET

The first is useful from the perspective of ensuring a client and server can communicate with one another correctly. The latter is useful for things like acceptance testing. The database layer is built using EFCore, which is also used to construct copies of the database in tests. The Respawn package is used to clean the database between test runs to ensure tests do not step on each other. The entire system uses the xUnit test framework.
