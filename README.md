# SCOScriptCodingHelper
A plugin for IV-SDK .NET which makes coding SCO scripts a bit easier.  
It restores a bunch of useful native functions, which could help alot while creating SCO scripts for GTA IV!  

## Requirements
- [IV-SDK .NET](https://github.com/ClonkAndre/IV-SDK-DotNet) (Version 1.8 or above)

## Installation
Just place everything from the downloaded archive inside the "IVSDKDotNet -> plugins" folder!  

## How to use
Check out the [example](https://github.com/IV-SDK-NET-Plugins/SCOScriptCodingHelper/blob/main/SCOScriptCodingHelper/Example/main.c) script on how to create widget groups, add widgets and more!

## Restored Native functions
Here's a list of native functions which this plugin makes functional:
- DEBUG_ON
- DEBUG_OFF
- OPEN_DEBUG_FILE
- CLOSE_DEBUG_FILE
- SAVE_INT_TO_DEBUG_FILE
- SAVE_FLOAT_TO_DEBUG_FILE
- SAVE_NEWLINE_TO_DEBUG_FILE
- SAVE_STRING_TO_DEBUG_FILE
- CREATE_WIDGET_GROUP
- END_WIDGET_GROUP
- ADD_WIDGET_SLIDER
- ADD_WIDGET_FLOAT_SLIDER
- ADD_WIDGET_READ_ONLY
- ADD_WIDGET_FLOAT_READ_ONLY
- ADD_WIDGET_TOGGLE
- ADD_WIDGET_STRING
- DELETE_WIDGET_GROUP
- DELETE_WIDGET
- DOES_WIDGET_GROUP_EXIST
- START_NEW_WIDGET_COMBO
- ADD_TO_WIDGET_COMBO
- FINISH_WIDGET_COMBO
- ADD_TEXT_WIDGET
- GET_CONTENTS_OF_TEXT_WIDGET
- SET_CONTENTS_OF_TEXT_WIDGET
- GET_LATEST_CONSOLE_COMMAND
- RESET_LATEST_CONSOLE_COMMAND

## How to Contribute
Do you have an idea to improve this plugin, or did you happen to run into a bug? Please share your idea or the bug you found in the [issues](https://github.com/IV-SDK-NET-Plugins/SCOScriptCodingHelper/issues) page, or even better: feel free to fork and contribute to this project with a [Pull Request](https://github.com/IV-SDK-NET-Plugins/SCOScriptCodingHelper/pulls).