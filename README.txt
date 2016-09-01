HyperIoC Project
=================

Overview
--------

The project makes use of the VS shared project to enable windows and universal shared code. Both the HyperIoC.Windows
and HyperIoC.Universal are empty. These both reference the HyperIoC shared project. 

The NuGet packages ate built from the HyperIoC.Windows and HyperIoC.Universal projects. The nuspec files reside in the
parent directory. 


Tests
-----

The tests use MSTest. In each test there is a #if statement at the top tp load the correct version of MSTest for each platform.

I have tried NUnit but although it seems to work, both R# and Test Explorer do not work consistently well (at time of writing).

To avoid frustration, the tests make use in a few places of #if blocks. The main place this occurrs is around exception testing.

MSTest is not consistent in this area. One platform prefers Assert.ThrowsException<> and the other uses the ExpectedException attribute.

Neither have a shared process.

Also due to the inconsistences again, standard DataRow tests are supported by the less elegant:

var data = []{ 1,2,3} - whatever your values are.

This will be reviewed in time when hopefully a framework that works well for all platforms and test runners becomes available.

