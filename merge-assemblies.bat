@echo off
echo Merging assemblies...
"C:\Program Files (x86)\Microsoft\ILMerge\ILMerge.exe" /internalize /ndebug /copyattrs /targetplatform:4.0,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /out:"Dist\SignalR.MongoRabbit.dll" "SignalR.MongoRabbit\bin\Release\SignalR.MongoRabbit.dll" "SignalR.MongoRabbit\bin\Release\MongoDB.Driver.Core.dll" "SignalR.MongoRabbit\bin\Release\MongoDB.Driver.dll" "SignalR.MongoRabbit\bin\Release\MongoDB.Bson.dll"
echo Done
