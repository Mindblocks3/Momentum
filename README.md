![Mirror Logo](https://i.imgur.com/ikP9eYs.png)

[![Discord](https://img.shields.io/discord/343440455738064897.svg)](https://discordapp.com/invite/N9QVxbM)
[![release](https://img.shields.io/github/release/MirrorNG/Momentum.svg)](https://github.com/MirrorNG/Momentum/releases/latest)

[![Build](https://github.com/MirrorNG/Momentum/workflows/CI/badge.svg)](https://github.com/MirrorNG/Momentum/actions?query=workflow%3ACI)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=MirrorNG_Momentum&metric=alert_status)](https://sonarcloud.io/dashboard?id=MirrorNG_Momentum)
[![SonarCloud Coverage](https://sonarcloud.io/api/project_badges/measure?project=MirrorNG_Momentum&metric=coverage)](https://sonarcloud.io/component_measures?id=MirrorNG_Momentum&metric=coverage)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=MirrorNG_Momentum&metric=ncloc)](https://sonarcloud.io/dashboard?id=MirrorNG_Momentum)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=MirrorNG_Momentum&metric=sqale_index)](https://sonarcloud.io/dashboard?id=MirrorNG_Momentum)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=MirrorNG_Momentum&metric=code_smells)](https://sonarcloud.io/dashboard?id=MirrorNG_Momentum)


Momentum is a plugin for MirrorNG for synchronizing movement. 
It uses snapshots and delta compression to achieve low bandwidth and low latency

## Installation
The preferred installation method is Unity Package manager.

If you are using unity 2019.3 or later: 

1) Open your project in unity
2) Install [MirrorNG](https://github.com/MirrorNG/MirrorNG)
3) Click on Windows -> Package Manager
4) Click on the plus sign on the left and click on "Add package from git URL..."
5) enter https://github.com/MirrorNG/Momentum.git?path=/Assets/Momentum
6) Unity will download and install MirrorNG Momentum

Alternatively you can download it from [Download Mirror](https://github.com/MirrorNG/Momentum/releases).  You will need to install some dependencies yourself such as cecil.

## Usage

1) In Unity create a NetworkManager gameobject from the GameObject -> Networking -> NetworkManager.
2) Then remove the NetworkManagerHud (the default transport), and add a NetworkMomentumHud.

## Contributing

There are several ways to contribute to this project:

* Pull requests for bug fixes and features are always appreciated.
* Pull requests to improve the documentation is also welcome
* Make tutorials on how to use this
* Test it and open issues
* Review existing pull requests
* Donations

When contributing code, please keep these things in mind:

* [KISS](https://en.wikipedia.org/wiki/KISS_principle) principle. Everything needs to be **as simple as possible**. 
* An API is like a joke,  if you have to explain it is not a good one.  Do not require people to read the documentation if you can avoid it.
* Follow [C# code conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).
* Follow [SOLID principles](https://en.wikipedia.org/wiki/SOLID) as much as possible. 
* Keep your pull requests small and obvious,  if a PR can be split into several small ones, do so.

