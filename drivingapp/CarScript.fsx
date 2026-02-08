//#load "Car.fs" // way to load a single file
#r "bin/debug/net10.0/drivingapp.dll" // way to load all namespaces/modules from proj
let result = Car.drive 10 8
