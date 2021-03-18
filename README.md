# MIPDocumentInspector
__MIPDocumentInspector__ retrieves a protected document protection information (the ProtectionDescriptor structure) and shows it in JSON format.<br>
The user can change the document protection information and apply it back to the document.
<br>
MIPDocumentInspector is based on MIP SDK file API.<br><br>

The image shown below shows MIPDocumentInspector:<br>
![alt text](/images/00.%20MIPDocumentInspector.jpg "MIPDocumentInspector").

Protection information is obtained with the following code:
```c#
this.DocumentPath = @"E:\temp\SampleDocumentNoLabel.docx";

var appInfo = new ApplicationInfo()
{
    ApplicationId = clientId,
    ApplicationName = appName,
    ApplicationVersion = appVersion
};
var mip = _mip = new MIPHelper(appInfo);

_authenticationHelper = new AuthenticationHelperMIP(appInfo.ApplicationId, Application.Current.MainWindow);
var task = _authenticationHelper.GetUserIdentityAsync((identity) =>
{
    using (var sec1 = this.GetNamedSection("GetUserIdentityAsyncCallback"))
    {
        this.Identity = identity;

        var fileEngine = mip.CreateFileEngine(_authenticationHelper);

        // List all labels available to the engine created in Action
        var labels = mip.ListLabels();
        this.Labels = labels.ToList();

        var leafLabels = new List<Label>();
        labels.ToList().ForEach(lab =>
        {
            if (lab.Children == null || lab.Children.Count == 0) { leafLabels.Add(lab); }
            else
            {
                leafLabels.AddRange(lab.Children);
            }
        });

        this.LeafLabels = leafLabels.ToList();
        if (File.Exists(this.DocumentPath)) { Commands.GetDescriptor.Execute(null, this); }
    }
});

```
## ADDITIONAL Information
Steps to use the sample:
1. Open MipDocumentInspector.sln 
2. set MipDocumentInspectorFull as the startup project
3. create the appsettings.devXXX.json with settings from your tenant
4. set env=devXXX into the prebuild command to make your appsettings used by the build
build and start


## KEYs
mip sdk document inspector Microsoft Information Protection File API handler

