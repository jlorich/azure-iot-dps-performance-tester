# DPS Performance Tester

This application will test performance to each call to Azure IoT DPS to help locate latency issues. This is built with .NET 5.0 and runs on any platform.

## Usage

Simply fill in your ScopeId, Registration Id, and derived device Key in Program.cs and call `dotnet run`

## Output

Output should appear as follows, containing response time of each http request as well as the overall assignment.

```
--------------------------------
Initial registration call for 5.4d1b65a2b3dcc259.b36aef30-af46-4412-b2ea-eef7849b106a completed in 00:00:00.20
Registration Status check for 5.4d1b65a2b3dcc259.b36aef30-af46-4412-b2ea-eef7849b106a completed in 00:00:00.05
Assignment made to jl-demo-device-streams-iot.azure-devices.net.
Final assignment completed in 00:00:01.25

--------------------------------
Initial registration call for 5.4d1b65a2b3dcc259.e31e5bf0-33d1-439a-975e-5f86f102cd09 completed in 00:00:00.20
Registration Status check for 5.4d1b65a2b3dcc259.e31e5bf0-33d1-439a-975e-5f86f102cd09 completed in 00:00:00.05
Assignment made to jl-demo-device-streams-iot.azure-devices.net.
Final assignment completed in 00:00:01.25

--------------------------------
Initial registration call for 5.4d1b65a2b3dcc259.976cf554-ea93-4ff9-a053-041edbc69dda completed in 00:00:00.19
Registration Status check for 5.4d1b65a2b3dcc259.976cf554-ea93-4ff9-a053-041edbc69dda completed in 00:00:00.05
Assignment made to jl-demo-device-streams-iot.azure-devices.net.
Final assignment completed in 00:00:01.25

--------------------------------
Initial registration call for 5.4d1b65a2b3dcc259.261ed94c-e6ad-4893-86b5-0f231aa89756 completed in 00:00:00.22
Registration Status check for 5.4d1b65a2b3dcc259.261ed94c-e6ad-4893-86b5-0f231aa89756 completed in 00:00:00.04
Assignment made to jl-demo-device-streams-iot.azure-devices.net.
Final assignment completed in 00:00:01.27
```

