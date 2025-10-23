## TODO
- [ ] How should Christian and others report bugs and new feature request?
- [x] Log to application insights
- [x] Sign first package
- [x] Fix issue with summary not shown on approval task
- [ ] Application Insights: Add to application map


## Signing msix files/packages
Change file location to location of msix file and run from command line:
```bat
signtool sign /sha1 ff11897861bef1c5be5b758de3da29a1fd2f516e /tr http://timestamp.digicert.com /td sha256 /fd SHA256 "C:\Users\bo\Downloads\Symphogen.Mimer.Client.qa.1.0.0.5308.msix"
```
