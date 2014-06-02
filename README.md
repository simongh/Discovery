Discovery
=========

Discovery library for .Net

This project does UPNP style discovery. Included in the samples is a client & server. Both the client & server both support passive & active discovery. Both are multi-threaded.

[![Build status](https://ci.appveyor.com/api/projects/status/0q6ulxammuawn3un)](https://ci.appveyor.com/project/SimonHalsey/discovery)

Client Agent
------
The client agent discovers services. It searches for a particular service type, eg `ge:fridge`. It then waits for any responses. It is also capable of passive discovery, where it listens for service announcements.

Server Agent
-----
The server agent responds to service discovery requests. The server agent can also announce that a service is available & a "bye" message when the service is unavailable.

Service
-----
In this context a service is a description of some service available in some location. The service has a type (`ge:fridge`), a unique name (`uuid:B6562811-9587-493E-B106-7C10AFEAEC16`), a location (`http://foo/bar`) and typically a validity duration that describes how long this service description is valid for.

Usage
----
Typically the server agent would be part of the system hosting the service & the client agent would be part of the client system. This not required though. The discovery response message contains the location of the service, so the system that hosts agent could be anywhere.

For example a registery could contain both client & server agents, responding to discovery requests from clients & whilst keeping a list of active services through service announcements.
