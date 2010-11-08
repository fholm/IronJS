# ![IronJS](http://ironjs.com/logo.png)

IronJS is a ECMAScript 3.0 implementation built on top of the [Dynamic Language Runtime](http://dlr.codeplex.com/) from [Microsoft](http://www.microsoft.com/) which allows you to embed a javascript runtime into yor .NET applications. The current state of the code is at best alpha quality. Use at your own risk.

## License

IronJS is released under the [Apache License Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

## Requirements

What you need to use IronJS

* .NET 4.0 (Src/IronJS.CLR4.sln)
* .NET 3.5 (Src/IronJS.CLR2.sln)
* Mono 2.8 (Src/IronJS.Mono28.sln)

## Getting the source

* ~$ git clone git@github.com:fholm/IronJS.git IronJS
* ~$ cd IronJS\Src\Dependencies
* ~$ git clone git@github.com:fholm/FSKit.git
* ~$ cd FSKit
* ~$ git checkout 0.0.5
