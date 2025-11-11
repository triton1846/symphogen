## TODO
- [x] How should Christian and others report bugs and new feature request?
- [x] Log to application insights
- [x] Sign first package
- [x] Fix issue with summary not shown on approval task
- [ ] Application Insights: Add to application map


## Signing msix files/packages
Copy, change environment and version and run from command line:
```bat
signtool sign /sha1 ff11897861bef1c5be5b758de3da29a1fd2f516e /tr http://timestamp.digicert.com /td sha256 /fd SHA256 "C:\Users\bo\Downloads\Symphogen.Mimer.Client.qa.1.0.0.5308.msix"
```
See also [Preparing packages for deployment](https://symphogenteams.visualstudio.com/Development%20and%20Data%20Engineering/_wiki/wikis/Development-and-Data-Engineering.wiki/502/Preparing-packages-for-deployment)
