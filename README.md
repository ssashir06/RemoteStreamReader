# RemoteStreamReader

This C# library named RemoteStreamReader is a Stream implementation that supplies file reading feature through HTTP connection.

This git repository contains the library of this project and also a sample of Asp.Net MVC to read 7z archive binary via browser that supports File API.

## Sample page
http://remotewebstream.azurewebsites.net/FileReadDemo/SevenZipFileList


how to use:

1. At the time the page is opened, you have pre-ticket number in the textbox. Press the start button to establish connection.
2. Specify your 7z file on your PC. Press the open button to read its binary. In the server side, SevenZipSharp library reads this file via this RemoteStreamReader Stream. Browser shows a archived file list as a result.
