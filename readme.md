# CAEN ISPECTOR SDK

## Introduction 
 
Ispector use a JSON based REST API for control and configuration of the device. HTTP API allows to perform every operations supported by the web based GUI from a remote host. Ispector use GET/POST HTTP call to both retrieve spectrum/wave data and configure the instruments. 
HTTP protocol, the same used to transport web page, has the big advantage to easy pass firewall and usually it does not require any special configuration on the network. 
Several endpoints are available 

| Endpoint (WEB PAGE) | Operation | Description | 
|--|--|--|
|/set_config.cgi|POST|Set configuration of HV/MCA/PSD|
|/status.cgi|GET|Read the status of the instruments and of all available processing channels (MCA/PSD)|
|/spectrum.cgi|GET|Get spectrum data|
|/wavedump.cgi|GET|Get waveform data|
|/psd.cgi|GET|Get|PSD data (not available in current API version)|
|/get_mca_config.cgi|GET|Readback MCA/PSD configuration|
|/resetspectrum.cgi|GET|Reset spectrum|
|/mca_run.cgi|GET|Start MCA acquisition|
|/mca_stop.cgi|GET|Stop MCA acquisition|
|/fb_settings.cgi|GET|Retrieve fabric configuration|
|/get_sysx.cgi|GET|Retrieve firmware version and installed options 

In order to access to the endpoint perform HTTP GET/POST to address: http://<ispector_ip>/set_config.cgi. Default Ispector IP is: http://192.168.50.2 
Ispector API support multiple user simultaneously connected to the instruments. Meanwhile your are developing, keep your browser open on the Ispector page in order see in live the performed operation. Please mind that configuration parameters are loaded on page load. If you change parameters from API you need to refresh browser page in order to see parameters changing on the GUI


##  Set Configuration 

In order to configure the Ispector the /set_config.cgi endpoint is available. This endpoint is a POST service. You need to perform a RAW post POST operation sending in the body of the POST the configuration JSON. Do not use form-data or x-www-form-urlencoded because they are not supported by ISPECTOR 
There are two different possible configurations you can send to ISPECTOR 
 - HV CONFIGURATION
 - MCA CONFIGURATION 

### HV CONFIGURATION 
	{"command" : "SET_CHANNEL_CONFIG","channel_config" :[{"id" : 0,"HV_STATUS" : true,"HV_VOLTAGE" : 41.5,"MaxV" : 46,"MaxI" : 5, "RAMP" : 20,"TCoeff" : -34,"HV_MODE" : "temperature","HV_PWRON" : true}],"store_flash" : false} 
All parameters are case sensitive 
|PARAMETERS|VALID VALUES|FUNCTION|
|--|--|--|
|HV_STATUS|true/false|Enable/Disable HV|
|HV_VOLTAGE|22..80|HV voltage Please pay attention to do not destroy the sensor setting too high voltage MaxV 22..80 Max output voltage|  
|MaxI|0..9|Trip Current in mA |
|RAMP|1..100|Ramp speed of HV|
|TCoeff|-1000 .. 1000 [mV/°C]|Temperature compensation coefficient|
|MODE|“digital” “temperature”|Enable / Disable temperature compensation on HV|
|HV_PWRON|true/false|Power on/off the HV on instrument boot|

### MCA CONFIGURATION 
	{"command" : "SET_CHANNEL_CONFIG","mca_config" :[{"id" : 0,"trigger_thrs" : 28,"trigger_inib" : 300,"int_pre" : 300,"int_val" : 10,"int_gain" : 80,"pileup_inib" : 30,"pileup_pen" : 30,"baseline_inib" : 24,"baseline_len" : 256,"taget_run" : 0,"taget_value" : 0,"reset_on_apply" : true}],"store_flash" : false}} 

All parameters are case sensitive 

|PARAMETERS|VALID VALUES|FUNCTION|
|--|--|--|
|trigger_thrs|10..1000 [int] (LSB)| Trigger threshold
|trigger_inib|10..1000 [int] (ns)| Trigger inhibit after a trigger events. (set in in order to avoid double triggers)|
|int_pre| 0..1000 [int] (ns)| Charge integrator pre-trigger integration extension|
|int_val|0..100 [float] (us)|Charge integrator integration time|
|int_gain|0..1000 [int]|Charge integrator GAIN|
|pileup_inib|0..100 [float] (us)|Pileup inhibition after a trigger|
|pileup_pen|0..100 [float] (us)|Pileup penalty if a pileup event occurs|
|baseline_inib|0..100 [float] (us)| Baseline inhibition after a trigger|
|baseline_len|1024,512,256, 128,64,32,16|Length is samples of the moving average used to calculate the baseline|
|taget_run|0,1,2 Acquisition run mode 0 – FREE 1 – TIME CONTRAINED (ms) 2 – TOTAL COUNTS ON SPECTRUM taget_value  [int]|Referring to taget_run parameters, this field specify the run limit. For example to run for 10 seconds set taget_run=1 and taget_value=10000 reset_on_apply true/false Reset spectrum when one or more configuration parameters are changed|
 
## Get Instrument Status 
 Get Instrument Status In order to get the status of the inspector perform GET to the following page /status.cgi 

```json
 {
    "command": "GET_SYSTEM_STATUS",
    "Result": "ok",
    "ErrorCode": 0,
    "Reason": "",
    "current_status": {
        "system_status": {
            "temperature": 0,
            "eth_status": 0,
            "eth_ip": "192.168.50.2",
            "last_user_interact": -1,
            "power": "wall",
            "battery": false,
            "battery_life": 0,
            "battery_charge": 0,
            "battery_in_charge": false,
            "remaining_time": 0,
            "battery_voltage": 0,
            "battery_current": 0,
            "battery_temperature": 0,
            "alarm": 0,
            "httpcloud": 0,
            "loracloud": 0
        },
        "channels": [{
            "id": 0,
            "HV_STATUS": true,
            "HV_VOLTAGE": 41.5,
            "HV_MODE": "temperature",
            "COMPL_V": false,
            "COMPL_I": false,
            "Vout": 42.38652,
            "Vref": 1.954313,
            "Iout": 0.3540874,
            "IoutRAW": 0.050250001,
            "Temp": 50.199402,
            "SetPoint": 42.356781,
            "ICR": 1294,
            "OCR": 1254,
            "runtime": 4542,
            "livetime": 4540,
            "sattime": 0,
            "incnt": 5856370,
            "outcnt": 5646006,
            "live": 0.969088,
            "dead": 0.030912,
            "mca_running": 1,
            "mca_status": 0
        }]
    }
}
``` 
    
## Get Spectrum
In order to get the status of the inspector perform GET to the following page /spectrum.cgi 
```json
{
    "command": "GET_SPECTRUM",
    "Result": "ok",
    "ErrorCode": 0,
    "Reason": "",
    "data": [3, 2, 4, 7, .., 1883]
}
``` 
The data field in the JSON contains the spectrum data in a 4096 bin array 

    
## Get Waveform
In order to get the status of the inspector perform GET to the following page /wavedump.cg
```json
{
    "command": "GET_WAVEDUMP",
    "Result": "ok",
    "ErrorCode": 0,
    "Reason": "",
    "data": [
        [3421, 0, 0, 0, 1, 0, 0],
        [3425, 0, 0, 0, 1, 0, 0],
        [3425, 0, 0, 0, 1, 0, 0],
        [3423, 0, 0, 0, 1, 0, 0], …
    ]
}
``` 
The data field in the JSON contains the waveform data.  Each data element is and array of 7 elements: 
 1) Analog data 
 2) Trigger pulse 
 3) Charge Integration Window 
 4) PSD Tail integration Window 
 5) Baseline restorer status 
 6) Pile up rejector discard 
 7) Pile up inhibition 

## Readback MCA configuration
In order to get the status of the inspector perform GET to the following page /get_mca_config.cgi 


## Testing API 
 
Use CURL to POST/GET data 
EXAMPLE: Send configuration for MCA 

		curl -d '{"command" : "SET_CHANNEL_CONFIG","mca_config" :[{"id" : 0,"trigger_thrs" : 100,"trigger_inib" : 100,"int_pre" : 300,"int_val" : 10,"int_gain" : 100,"pileup_inib" : 10,"pileup_pen" : 10,"baseline_inib" : 10,"rebinnig" : 4096,"baseline_len" : 512,"taget_run" : 0,"taget_value" : 0,"reset_on_apply" : true}],"store_flash" : false}}' -H "Content-Type: application/json" -X  POST http://192.168.50.2/set_config.cgi 


 
EXAMPLE: Read system status curl -X POST http://192.168.50.2/status.cgi 
 
 EXAMPLE: Download spectrum curl -X POST http://192.168.50.2/spectrum.cgi 

## C# SDK 
 

An SDK for C# language is available. The SDK use HTTP API communication to interface with the Ispector. 
The SDK requires Newtonsoft JSON module. It will be automatically downloaded from NuGet at compiling time. 
The SDK include a DLL library and a C# example. 
The DLL can be imported in any programming language supporting C# (.NET) dll including VB.NET, Labview, Matlab 

### DOWNLOAD THE SDK: 
Python SDK files can be download from Nuclear Instruments Github https://github.com/NuclearInstruments/IspectorSDK-CSHARP 
or cloned with git:  git clone https://github.com/NuclearInstruments/IspectorSDK-CSHARP.git 
 
The function in C# SDK are the same of Python SDK. Refers to Python SDK for usage guide 


Have a look to example file and pdf in docs to further informations

